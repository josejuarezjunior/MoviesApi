📌 Visão geral da arquitetura

Você montou isso:

Internet
   ↓
Cloudflare (DNS + Tunnel)
   ↓
cloudflared (rodando no seu PC)
   ↓
Nginx (porta 80)
   ↓
API .NET (Kestrel na porta 5000)


🔹 Você não tem IP público
🔹 O Cloudflare Tunnel cria um “túnel reverso”
🔹 Ninguém acessa sua máquina diretamente

🧱 ETAPA 1 — Preparar a API .NET (simples, em memória)
1. Criar a API
dotnet new webapi -n ApiMovies
cd ApiMovies

2. Criar um controller simples (dados em memória)

Exemplo:

[ApiController]
[Route("movies")]
public class MoviesController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new[]
        {
            new { Id = 1, Title = "Matrix" },
            new { Id = 2, Title = "Interstellar" }
        });
    }
}

3. Garantir que a API escuta localmente

No Program.cs, deixe assim (ou similar):

app.MapControllers();

4. Testar localmente
dotnet run


Testar:

curl http://localhost:5000/movies


✔️ Se isso funciona, a API está OK.

🧱 ETAPA 2 — Publicar a API
1. Publicar em modo Release
dotnet publish -c Release -o /var/www/api-movies


Resultado:

/var/www/api-movies/
 ├ ApiMovies.dll
 ├ appsettings.json
 └ ...

2. Executar manualmente para testar
cd /var/www/api-movies
dotnet ApiMovies.dll


Testar:

curl http://localhost:5000/movies


✔️ Continua funcionando → pode parar a API (Ctrl + C)

🧱 ETAPA 3 — Ajustar permissões da pasta
sudo chown -R www-data:www-data /var/www/api-movies
sudo chmod -R 755 /var/www/api-movies


📌 Isso garante que:

Nginx pode acessar

Serviço pode rodar sem erro de permissão

🧱 ETAPA 4 — Instalar e configurar o Nginx
1. Instalar
sudo apt update
sudo apt install nginx

2. Criar site da API
sudo nano /etc/nginx/sites-available/api-movies


Conteúdo base:

server {
    listen 80;
    server_name api-movies.seudominio.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}

3. Ativar o site
sudo ln -s /etc/nginx/sites-available/api-movies /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx

4. Testar Nginx
curl http://localhost/movies


✔️ Se retorna JSON → Nginx está OK

🧱 ETAPA 5 — Comprar domínio no Cloudflare

✔️ Comprar domínio
✔️ Deixar DNS gerenciado pelo Cloudflare
✔️ Não precisa criar A record

🧱 ETAPA 6 — Criar o Cloudflare Tunnel
1. Instalar o cloudflared
curl -fsSL https://pkg.cloudflare.com/install.sh | sudo bash
sudo apt install cloudflared

2. Login
cloudflared tunnel login


✔️ Isso cria:

~/.cloudflared/cert.pem

3. Criar o tunnel
cloudflared tunnel create api-movies


Guarde:

Tunnel ID

Token

🧱 ETAPA 7 — Criar o Public Hostname (no Cloudflare)

No painel Cloudflare:

Tunnel → api-movies

Public Hostname:

Subdomain: api-movies

Domain: seudominio.com

Type: HTTP

URL: http://localhost:80

📌 Aqui o Cloudflare:

api-movies.seudominio.com → túnel → nginx

🧱 ETAPA 8 — Instalar tunnel como serviço
sudo cloudflared service install SEU_TOKEN_AQUI


Depois:

sudo systemctl status cloudflared


✔️ Status: active (running)
✔️ Tunnel: Healthy

🧱 ETAPA 9 — Subir a API definitivamente

Você pode subir de 3 formas:

Opção A — Manual (teste)
dotnet /var/www/api-movies/ApiMovies.dll

Opção B — systemd (recomendado)

Criar serviço depois (quando quiser algo “real”)

🧱 ETAPA 10 — Testes finais
1. Do servidor
curl http://localhost:5000/movies
curl http://localhost/movies

2. De qualquer lugar
https://api-movies.seudominio.com/movies


✔️ Funciona no celular
✔️ Funciona fora da rede
✔️ Sem IP público

🚨 Coisas IMPORTANTES pra lembrar
❌ ping não funciona com Cloudflare Tunnel

Normal. Ignore.

✔️ Cloudflare Tunnel é HTTP/HTTPS only
✔️ Nginx é obrigatório para organizar rotas
🧠 Próximo nível (quando voltar nesse projeto)

Criar service systemd para a API

Adicionar Postgres / Supabase

HTTPS automático (Cloudflare já cuida)

Versionamento /v1/movies

Autenticação (JWT)
