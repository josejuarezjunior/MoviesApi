## ALTERANDO A API

Sim, é forte recomendação parar o serviço/processo atual antes de substituir os arquivos e rodar a nova versão.

Se você não fizer isso, corre risco de:

Arquivos travados e não serem substituídos

Ter duas instâncias da API rodando (porta 5000 já ocupada)

Nginx começar a devolver erro 502/504 porque não consegue se conectar à API

🔹 Cenário atual

Você provavelmente tem uma instância rodando via terminal ou systemd.

Se rodou via terminal (ex: dotnet /var/www/api-movies/ApiMovies.dll):


Pare pressionando Ctrl + C no terminal onde está rodando

Ou mate o processo:

ps aux | grep ApiMovies.dll
kill -9 <PID>


Se rodou como serviço systemd:

### Vamos ver como identificar se sua API está rodando e como checar o serviço. Vou detalhar tudo, tanto para quem usa systemd quanto para quem roda manualmente.

🔹 1️⃣ Se você criou a API como serviço systemd
Listar todos os serviços ativos
systemctl list-units --type=service


Isso mostra todos os serviços ativos no servidor

Procure algo como api-movies.service (ou o nome que você deu)

Ver status de um serviço específico
sudo systemctl status api-movies.service


Exemplo de saída:

● api-movies.service - API Movies .NET
   Loaded: loaded (/etc/systemd/system/api-movies.service; enabled; vendor preset: enabled)
   Active: active (running) since Sat 2026-02-08 14:00:00 UTC; 2h 5min ago
 Main PID: 12345 (dotnet)
    Tasks: 5 (limit: 4915)
   Memory: 50.0M
   CGroup: /system.slice/api-movies.service
           └─12345 /usr/bin/dotnet /var/www/api-movies/ApiMovies.dll


✅ Active: active (running) significa que a API está rodando.

🔹 2️⃣ Se você rodou a API manualmente

Liste os processos do .NET:

ps aux | grep ApiMovies.dll


Exemplo de saída:

usuario   12345  0.5  1.2 123456 12345 ?    Ssl  14:00   0:05 dotnet /var/www/api-movies/ApiMovies.dll
usuario   12346  0.0  0.0  12345  1234 pts/0 S+   16:00   0:00 grep --color=auto ApiMovies.dll


O processo 12345 é a API rodando

Se não aparecer nada além do grep, significa que nenhuma instância está rodando

###########################
Antes de atualizar a API você deve parar o serviço atual, para evitar conflitos ou arquivos travados. Depois, você pode iniciar a nova versão.

Vou te passar o passo a passo completo, tanto para quem usa systemd (recomendado) quanto para quem roda manualmente.

🔹 1️⃣ Se a API roda como serviço systemd (recomendado)
Parar o serviço
sudo systemctl stop api-movies.service


Isso para a API que está rodando

Nenhum cliente conseguirá acessar enquanto estiver parada

Verificar se parou
sudo systemctl status api-movies.service


Deve mostrar Active: inactive (dead) ou stopped

Iniciar o serviço novamente

Depois de atualizar os arquivos:

sudo systemctl start api-movies.service

Verificar se está rodando
sudo systemctl status api-movies.service


Deve mostrar Active: active (running)

Se não estiver, veja os logs:

journalctl -u api-movies.service -f

🔹 2️⃣ Se a API roda manualmente (via terminal)
Parar a API

Se está rodando no terminal, pressione Ctrl + C

Ou encontre o processo e mate:

ps aux | grep ApiMovies.dll
kill -9 <PID>

Iniciar a API novamente
cd /var/www/api-movies
dotnet ApiMovies.dll


Teste:

curl http://localhost:5000/movies

🔹 Dica prática

Para deploy “profissional”, sempre use systemd.

Isso garante que:

A API sobe automaticamente após reboot

Você pode controlar com start/stop/restart/status

Logs ficam centralizados (journalctl)
##########################

🔹 3️⃣ Checar se está respondendo (teste rápido)

Mesmo sem ver processos, você pode testar se a API está viva:

curl http://localhost:5000/movies


Se retornar JSON, a API está rodando

Se der erro de conexão, não está rodando

🔹 Resumo

Systemd: systemctl status api-movies.service ✅

Manual: ps aux | grep ApiMovies.dll

Teste real: curl http://localhost:5000/movies

## PARAR O SERVIÇO
sudo systemctl stop api-movies.service


Confirme que parou:

sudo systemctl status api-movies.service
# ou
ps aux | grep ApiMovies.dll

🔹 Próximo passo

Depois que a instância atual estiver parada:

Execute o publish:
sudo dotnet publish -c Release -o /var/www/api-movies


Inicie a nova versão:
Manual:
dotnet /var/www/api-movies/ApiMovies.dll


Com systemd (recomendado):
sudo systemctl start api-movies.service

Ver o status:
sudo systemctl status api-movies.service ✅


Teste via Nginx e Cloudflare:

curl http://localhost/movies
curl https://api-movies.seudominio.com/movies


✅ Isso garante que não haverá conflito e que os arquivos serão substituídos corretamente.

## Servidor Azure
Fazendo o publish localmente:
dotnet publish -c Release -o publish

Testando a conexão com o servidor azure:
ssh -i C:\Projects\chaves\JbuntuServerVm_key.pem azureuser@172.184.215.180

Enviado a pasta de publish para o servidor via SCP:
Com chave ssh pública:
scp -i C:\Projects\chaves\JbuntuServerVm_key.pem -r C:\Users\josejuarez.junior\Documents\projects\web-api\ApiMoviesLab\publish azureuser@172.184.215.180:/home/azureuser/web-api

Sem chave, apenas com a senha:
scp -r "C:/Users/josejuarez.junior/Documents/projects/web-api/ApiMoviesLab/publish" azureuser@172.184.215.180:/home/azureuser/web-api
Ele vai pedir:
azureuser@172.184.215.180's password:

Acessando a VM, quando só a senha é necessária:
ssh usuario@IP_PUBLICO_DA_VM
ssh azureuser@172.184.215.180
