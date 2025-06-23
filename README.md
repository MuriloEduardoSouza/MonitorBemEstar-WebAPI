## 📊 Monitor de Bem-Estar Digital
Este projeto consiste em uma API desenvolvida em ASP.NET Core, que permite ao usuário registrar dados sobre sua produtividade diária, incluindo:

 ⏳ Horas de uso do celular

 🎯 Tipo de atividade realizada

 😊 Nível de humor no dia

A API oferece também filtros inteligentes para que o usuário visualize sua produtividade tanto na semana atual quanto no mês atual.

# 🚀 Tecnologias utilizadas
✅ ASP.NET Core MVC

✅ C#

✅ Entity Framework Core (com migrations)

✅ PostgreSQL (banco de dados relacional)

✅ REST API

# 🏗️ Funcionalidades
📄 Registro diário de:

Horas de uso do celular

Tipo de atividade

Humor no dia

Data do registro

# 🔍 Filtros por:

Semana atual

Mês atual

 👤 Controle por usuário (cada usuário tem seus próprios dados registrados)

 🔐 Autenticação de usuários

# 🔧 Instalação e execução
# ✔️ Pré-requisitos:
.NET SDK (versão 7.0 ou superior)

PostgreSQL instalado e rodando

Visual Studio ou Visual Studio Code

Git instalado


# 📡 Endpoints principais
Método	Endpoint	Descrição
POST	/api/registrodiario	Criar novo registro diário
GET	/api/registrodiario	Listar registros do usuário
GET	/api/registrodiario/semana	Filtro da semana atual
GET	/api/registrodiario/mes	Filtro do mês atual
PATCH	/api/registrodiario/{id}	Atualizar um registro
DELETE	/api/registrodiario/{id}	Remover um registro

# 🛠️ Futuras melhorias
Dashboard com gráficos

Front-end integrado (em desenvolvimento)

Notificações de produtividade

🤝 Contribuição
Contribuições são bem-vindas! Sinta-se livre para abrir Issues ou Pull Requests.
