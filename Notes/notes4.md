# Criando VM no azure

## Criar a VM no azure.

-- Criei uma VM do UbuntuServer 24.04 em 11/Fev/2026

## Conectar na VM:
ssh -i sua-chave.pem azureuser@IP_DA_VM
ssh -i C:\Projects\chaves\JbuntuServerVm_key.pem azureuser@172.184.215.180

## Atualize o Sistema:
sudo apt update
sudo apt upgrade -y


## Adicionar repositório oficial da Microsoft
sudo apt install -y wget apt-transport-https software-properties-common


Baixe o pacote da Microsoft:
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb

Instale:
sudo dpkg -i packages-microsoft-prod.deb

Atualize novamente:
sudo apt update

## Instalar o .NET 8

SDK:
sudo apt install -y dotnet-sdk-8.0

Ou se preferer, apenas o runtime:
sudo apt install -y aspnetcore-runtime-8.0

Verificar se instalou corretamente
dotnet --version

Deve retornar algo como:
8.0.xxx

# Subir a API para o servidor:
Fazer o publish na máquina local:
dotnet publish -c Release -o publish

Testando a conexão com o servidor azure:
ssh -i C:\Projects\chaves\JbuntuServerVm_key.pem azureuser@172.184.215.180

Enviado a pasta de publish para o servidor via SCP:
scp -i C:\Projects\chaves\JbuntuServerVm_key.pem -r C:\Users\josejuarez.junior\Documents\projects\web-api\ApiMoviesLab\publish azureuser@172.184.215.180:/home/azureuser/web-api

Executar a API na VM:
dotnet /home/azureuser/web-api/ApiMoviesLab.dll

Assim que você roda o comando, aparece algo como:

azureuser@UbuntuServerVm:/$ dotnet /home/azureuser/web-api/ApiMovies.dll
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /

CONFIGURAÇÃO PARA TESTE APENAS:
###########################################################################
Executando assim só está visível dentro da VM: 
dotnet ApiMovies.dll

Executando assim está visível fora da VM:
dotnet ApiMovies.dll --urls "http://0.0.0.0:5000"

Para acessar no navegador, a porta 5000 do TCP/IP deve estar listada na VM,
em configurações de rede > Regras de porta de entrada, porém apenas para testes.
Em produção, deve estar aberta somente a porta 80.
###########################################################################

## Instalar o nginx

sudo apt update
sudo apt install nginx -y

sudo systemctl start nginx
sudo systemctl enable nginx

Para testar no navegador se está funcionando o nginx (Para funcionar, a porta 80 deve estar liberada no azure):
http://SEU_IP_PUBLICO
http://172.184.215.180/

Testando no teminal do windows:  Test-NetConnection 172.184.215.180 -Port 5000
Testando o nginx na própria VM: curl http://localhost

Criar configuração para sua API
Vamos criar um arquivo de site na VM:
sudo nano /etc/nginx/sites-available/apimovies

Coloque exatamente isso (ajustado corretamente):

server {
    listen 80;
    server_name 172.184.215.180;

    location / {
        proxy_pass         http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
}

Use server_name _; para aceitar qualquer IP/dominio.

Salve e saia:
CTRL + O
ENTER
CTRL + X

## Ativar o site
sudo ln -sf /etc/nginx/sites-available/apimovies /etc/nginx/sites-enabled/apimovies
Que faz o seguinte:
Ele cria um link simbólico (atalho)
sites-available  → onde ficam as configurações
sites-enabled    → quais estão ativas
O Nginx só carrega os arquivos que estão dentro de: 
/etc/nginx/sites-enabled/
Então o que esse comando faz é:
 Criar um “atalho” dentro de sites-enabled apontando para o arquivo real que está em sites-available.
 Quebrando o comando
ln

Comando para criar links.
-s

Cria um link simbólico (atalho), não uma cópia.
-f

Força a substituição se já existir.

Estrutura:
ln -s origem destino

Então:

origem  → /etc/nginx/sites-available/apimovies
destino → /etc/nginx/sites-enabled/apimovies

Por que o Nginx funciona assim?

Isso permite:
Ter vários arquivos de configuração prontos
Ativar/desativar facilmente
Não precisar copiar arquivos

Para desativar um site:
sudo rm /etc/nginx/sites-enabled/apimovies


## Remover o default (se ainda existir):
sudo rm -f /etc/nginx/sites-enabled/default
Esse comando remove o site padrão do Nginx
O que é esse "default"?
Quando você instala o Nginx, ele cria um servidor padrão que responde na porta 80 com aquela página:
"Welcome to nginx!"
Esse arquivo fica em:
/etc/nginx/sites-available/default
E ele é ativado porque existe um link simbólico em:
/etc/nginx/sites-enabled/default
Então o que o comando faz na prática?
Ele remove o link simbólico do site padrão.

Ou seja:
Nginx deixa de usar a configuração padrão
E passa a usar apenas a sua:
apimovies


## Testar configuração
sudo nginx -t

Tem que aparecer:
syntax is ok
test is successful

Reiniciar Nginx
sudo systemctl restart nginx

Agora teste no navegador
http://SEU_IP/Movies
http://172.184.215.180/Movies  (Lembre-se de que a API deve estar rodando)

## Instalar PostgreSQL na VM Ubuntu

No terminal da VM:
sudo apt update
sudo apt install -y postgresql postgresql-contrib
sudo systemctl start postgresql
sudo systemctl enable postgresql

Criar usuário e banco:
sudo -i -u postgres
psql

Dentro do psql, rode:
-- Criar banco de dados
CREATE DATABASE moviesdb;

-- Criar usuário
CREATE USER juarez WITH PASSWORD 'sua_senha';

-- Dar privilégios ao usuário
GRANT ALL PRIVILEGES ON DATABASE moviesdb TO juarez;

\q
exit

Testar conexão
No terminal da VM:
psql -U juarez -d moviesdb -h localhost

## Configurar sua Web API .NET 8
📌 Instalar pacotes EF Core para PostgreSQL
No projeto:
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0

📌 Configurar DbContext
Data/AppDbContext.cs:
```cs
using Microsoft.EntityFrameworkCore;
using SeuProjeto.Models;

namespace SeuProjeto.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Movie> Movies { get; set; }
    }
}
```
📌 Connection string no appsettings.json (Versão onde o banco está na mesma vm que a web api)
```json
 "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=moviesdb;Username=postgres;Password=19861989"
  }
```
Para testar na máquina local, use:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=IP_PUBLICO_DA_VM;Port=5432;Database=moviesdb;Username=juarez;Password=sua_senha"
}
```

📌 Program.cs
```cs
using Microsoft.EntityFrameworkCore;
using SeuProjeto.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();
```

Criar migrations e atualizar banco
No projeto: 
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Pronto! Agora o PostgreSQL está rodando na VM e a Web API está conectada.

## Criando um serviço systemd na VM, para a API rodar de forma

✔ automática
✔ segura
✔ em background
✔ com restart automático
✔ integrada aos logs do sistema
Ou seja: muito melhor do que rodar dotnet api.dll manualmente.

🚀 Como transformar sua API .NET em um serviço systemd
1. Escolha uma pasta final para o publish
Por exemplo:
/home/azureuser/web-api

Sua DLL vai estar lá, tipo:
/home/azureuser/web-api/api.dll

📝 2. Criar o arquivo de serviço do systemd
Você vai criar um arquivo:
/etc/systemd/system/api.service
Crie com:
sudo nano /etc/systemd/system/movies-api.service

Conteúdo do arquivo:
```txt
[Unit]
Description=API Movies .NET 8
After=network.target

[Service]
WorkingDirectory=/home/azureuser/web-api
ExecStart=/usr/bin/dotnet /home/azureuser/web-api/api-movies.dll
Restart=always
# Usuario que irá rodar a API
User=azureuser
# Grupo do usuario
Group=azureuser
# Variáveis de ambiente (opcional)
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```


Explicando as partes importantes:
ExecStart: comando para iniciar sua aplicação.
Restart=always: reinicia automaticamente se a API falhar.
User e Group: define com qual usuário o serviço roda (nunca use root se não precisar).
WantedBy=multi-user.target: faz com que o serviço inicie automaticamente no boot

## Ativar e iniciar o serviço
Depois de salvar o arquivo:

Recarregar o systemd para reconhecer o novo serviço
sudo systemctl daemon-reload

Habilitar para iniciar no boot
sudo systemctl enable movies-api.service

Iniciar imediatamente
sudo systemctl start movies-api.service

Checar status
sudo systemctl status movies-api.service

Se tudo estiver certo, você verá algo como:
Active: active (running)

Para reiniciar manualmente:
sudo systemctl restart movies-api.service

Para parar:
sudo systemctl stop movies-api.service

✅ Benefícios de rodar a API como serviço

Inicia automaticamente quando a VM liga.
Reinicia se der erro.
Você consegue controlar facilmente com systemctl.
Mais seguro: roda com usuário específico, não com root.


Testar na VM:
curl http://localhost:5000/api/movies

Testar em outra máquina (com o nginx configurado), no terminal:
curl http://IP_PUBLICO/endpoint

Testar em outra máquina (com o nginx configurado), direto no navegador:
http://IP_PUBLICO/endpoint
