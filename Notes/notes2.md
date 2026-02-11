Quando eu desligar o pc onde o servidor web está instalado, o que devo fazer ao ligar novamente?

Excelente pergunta — essa é a pergunta certa quando a gente sai do “funciona agora” e entra no “isso é um servidor de verdade” 👌
Vou ser bem direto e prático.

🟢 Situação IDEAL (o que você quer atingir)

Quando você desliga e liga o PC:

✅ Nginx sobe sozinho
✅ Cloudflare Tunnel sobe sozinho
✅ API .NET sobe sozinha
✅ Você não digita comando nenhum
✅ https://api-movies.seudominio.com/movies já funciona

Se isso ainda não está assim, é porque falta 1 peça.

🔍 O que já acontece automaticamente hoje
✔️ Nginx

O Nginx já é um serviço do sistema:

sudo systemctl status nginx


Ele sobe automaticamente ao ligar o PC.
Você não precisa fazer nada.

✔️ Cloudflare Tunnel

Como você executou:

sudo cloudflared service install SEU_TOKEN


Isso criou um serviço systemd.

Verifique:

sudo systemctl status cloudflared


Ele também sobe automaticamente após reboot.

🔴 O que NÃO sobe sozinho (hoje)

👉 Sua API .NET

Se você precisa rodar:

dotnet ApiMovies.dll


manual → não é servidor ainda, é app em execução.

🧱 A peça que falta: serviço systemd para a API

Isso é o que transforma sua API em algo profissional.

✅ SOLUÇÃO DEFINITIVA (recomendada)
1️⃣ Criar um serviço systemd para a API
sudo nano /etc/systemd/system/api-movies.service


Cole isso (ajuste o nome se necessário):

[Unit]
Description=API Movies .NET
After=network.target

[Service]
WorkingDirectory=/var/www/api-movies
ExecStart=/usr/bin/dotnet /var/www/api-movies/ApiMovies.dll
Restart=always
RestartSec=10
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000

[Install]
WantedBy=multi-user.target

2️⃣ Ativar o serviço
sudo systemctl daemon-reexec
sudo systemctl daemon-reload
sudo systemctl enable api-movies
sudo systemctl start api-movies

3️⃣ Verificar se está rodando
sudo systemctl status api-movies


Testar:

curl http://localhost:5000/movies
curl http://localhost/movies


E por fim:

https://api-movies.seudominio.com/movies

🔁 O que fazer APÓS reboot (resposta curta)

👉 Nada.

Mas se quiser conferir:

sudo systemctl status nginx
sudo systemctl status cloudflared
sudo systemctl status api-movies


Se todos estiverem active (running) → tudo certo ✅

🧠 Checklist mental (guarde isso)

Quando ligar o PC:

Linux iniciou

Nginx subiu sozinho

Cloudflare Tunnel subiu sozinho

API .NET subiu sozinha

Domínio responde

🎯 Servidor pronto

⚠️ Se algo não funcionar após reboot
API não responde
journalctl -u api-movies -n 50

Tunnel caiu
sudo systemctl restart cloudflared

🏁 Conclusão

Agora você sabe a diferença entre:

❌ “rodar uma API”
✅ “operar um servidor”

Se quiser, no próximo passo posso:

revisar seu serviço systemd

ensinar backup + restore

adicionar HTTPS local

colocar logs estruturados

ou preparar isso pra produção real (Postgres, segurança, etc)

É só dizer 🚀