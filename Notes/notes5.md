# Acessar banco postgresql da vm

Instalar o pgadmin4 no windows.

Descobrir o IP da sua VM Ubuntu
hostname -I

Habilitar acesso remoto no PostgreSQL
2.1 Editar postgresql.conf
Abra o arquivo:
sudo nano /etc/postgresql/*/main/postgresql.conf

Procure a linha:
#listen_addresses = 'localhost'

Troque para:
listen_addresses = '*'
Isso permite conexões externas.
Salve com CTRL+O, saia com CTRL+X.

Editar regras do pg_hba.conf
Agora:
sudo nano /etc/postgresql/*/main/pg_hba.conf

Adicione no final:
host    all    all    0.0.0.0/0    md5
Ou, por segurança, restrinja ao IP da sua máquina Windows:
host    all    all    <IP_WINDOWS>/32   md5

Reiniciar o PostgreSQL
sudo systemctl restart postgresql

Liberar porta no firewall da VM
Se o Ubuntu usa UFW:
sudo ufw allow 5432/tcp
sudo ufw reload

Se o firewall estiver desativado, você verá:
Status: inactive

Configurar o pgAdmin 4 (no Windows)
Abra o pgAdmin 4
Clique em Add New Server
Na aba General, dê um nome (ex: PostgreSQL VM)
Na aba Connection, preencha:

Configuração            Valor
Host name/address       IP da VM(ex: 172.184.215.180)
Port                    5432
Maintenance Database    postgres
Username                postgres
Password                senha do usuário postgres


Marque Save Password se quiser.
Clique Save.
Se tudo estiver OK → Conectado! 🚀

## String de conexão para fazer migration fora da vm

"ConnectionStrings": {
    "DefaultConnection": 
    "Host=172.184.215.180;Database=moviesdb;Username=postgres;Password=19861989"
  }