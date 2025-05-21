# 🚀 Inicie Aqui! Guia de Setup do Ambiente de Desenvolvimento

Bem-vindo(a) ao guia de início rápido do Projeto Flow Wise! Este documento o(a) guiará pela configuração do seu ambiente de desenvolvimento local, permitindo que você comece a contribuir rapidamente.

## 📋 Pré-requisitos

Certifique-se de que as seguintes ferramentas e softwares estejam instalados em sua máquina:

1.  **Git:** Para clonar o repositório e gerenciar o controle de versão.
    * [Download Git](https://git-scm.com/downloads)
2.  **Docker Desktop:** Essencial para rodar nossas dependências (PostgreSQL, RabbitMQ, Redis) e para conteinerizar os microsserviços.
    * [Download Docker Desktop](https://www.docker.com/products/docker-desktop)
    * **Importante:** Certifique-se de que o Docker Desktop esteja **executando** antes de tentar subir os serviços.
3.  **.NET 8 SDK:** O *framework* principal para o desenvolvimento das aplicações C#.
    * [Download .NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
4.  **Editor de Código / IDE:**
    * **Visual Studio Code (Recomendado):** Leve e extensível, com muitas extensões úteis para C# e Docker.
        * [Download VS Code](https://code.visualstudio.com/download)
        * **Extensões Recomendadas:**
            * C# (Microsoft)
            * Docker
            * GitLens
            * EditorConfig for VS Code
    * **Visual Studio 2022 (Alternativa):** IDE completa para desenvolvimento .NET.
        * [Download Visual Studio](https://visualstudio.microsoft.com/downloads/)
5.  **Cliente HTTP (Opcional, mas Recomendado):** Para testar as APIs localmente.
    * **Postman:** [Download Postman](https://www.postman.com/downloads/)
    * **Insomnia:** [Download Insomnia](https://insomnia.rest/download)

## ⬇️ Configuração do Ambiente

Siga os passos abaixo para configurar seu ambiente:

### 1. Clonar o Repositório

Abra seu terminal ou prompt de comando e execute:

```bash
git clone [https://github.com/wnlima/flow-wise.git](https://github.com/wnlima/flow-wise.git)
cd flow-wise
````

### 2. Configurar Variáveis de Ambiente (Credenciais)

Para rodar os microsserviços localmente, você precisará configurar as strings de conexão e outras credenciais. Recomendamos usar o **.NET User Secrets** para isso, garantindo que suas credenciais não sejam versionadas no código.

#### Para o `FlowWise.Services.Lancamentos.Api`:

1.  Navegue até a pasta do projeto API:
    ```bash
    cd src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Api
    ```
2.  Inicialize os User Secrets (se ainda não o fez):
    ```bash
    dotnet user-secrets init
    ```
3.  Defina os segredos necessários para o serviço de Lançamentos:

    ```bash
    dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=flowwise_lancamentos_db;Username=flowwise_user;Password=flowwise_password"
    dotnet user-secrets set "RabbitMQ:Host" "localhost"
    dotnet user-secrets set "RabbitMQ:Username" "flowwise_user"
    dotnet user-secrets set "RabbitMQ:Password" "flowwise_password"
    dotnet user-secrets set "Redis:Connection" "localhost:6379"
    ```

#### Para o `FlowWise.Services.Consolidacao.Api`:

1.  Navegue até a pasta do projeto API:
    ```bash
    cd src/FlowWise.Services.Consolidacao/FlowWise.Services.Consolidacao.Api
    ```
2.  Inicialize os User Secrets (se ainda não o fez):
    ```bash
    dotnet user-secrets init
    ```
3.  Defina os segredos necessários para o serviço de Consolidação:

    ```bash
    dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=flowwise_consolidacao_db;Username=flowwise_user;Password=flowwise_password"
    dotnet user-secrets set "RabbitMQ:Host" "localhost"
    dotnet user-secrets set "RabbitMQ:Username" "flowwise_user"
    dotnet user-secrets set "RabbitMQ:Password" "flowwise_password"
    dotnet user-secrets set "Redis:Connection" "localhost:6379"
    ```

### 3\. Subir Dependências Locais com Docker Compose

As dependências de infraestrutura (PostgreSQL, RabbitMQ, Redis) são orquestradas via Docker Compose.

1.  Na raiz do repositório, execute o comando para subir os serviços em segundo plano:
    ```bash
    docker-compose up -d postgres rabbitmq redis
    ```
3.  Verifique se os contêineres subiram corretamente:
    ```bash
    docker-compose ps
    ```
    Você deve ver `healthy` ou `Up` para `postgres`, `rabbitmq` e `redis`.

### 4\. Rodar os Microsserviços

Você pode rodar os microsserviços diretamente da sua IDE ou via terminal:

#### Opção A: Via Terminal (para cada serviço)

Abra um novo terminal (ou aba no terminal) para cada microsserviço que deseja rodar.

1.  **Serviço de Lançamentos:**

    ```bash
    cd src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Api
    dotnet restore
    dotnet run
    ```

    A API estará disponível em `http://localhost:5000` (ou a porta configurada). O Swagger UI em `http://localhost:5000/swagger`.

2.  **Serviço de Consolidação:**

    ```bash
    cd src/FlowWise.Services.Consolidacao/FlowWise.Services.Consolidacao.Api
    dotnet restore
    dotnet run
    ```

    A API estará disponível em `http://localhost:5001` (ou a porta configurada). O Swagger UI em `http://localhost:5001/swagger`.

#### Opção B: Via Visual Studio (Se usar)

1.  Abra a *solution* principal (`.sln`) no Visual Studio.
2.  Você pode configurar múltiplos projetos de inicialização para rodar ambos os serviços simultaneamente, ou iniciá-los individualmente.
3.  As portas serão definidas no `launchSettings.json` de cada projeto API.

### 5. Executar Migrações de Banco de Dados (PostgreSQL)

Cada microsserviço com persistência de dados no PostgreSQL (Lançamentos e Consolidação) precisará ter suas migrações aplicadas. **É essencial que cada microsserviço tenha um `DbContext` configurado para o seu banco de dados específico (`flowwise_lancamentos_db` e `flowwise_consolidacao_db` respectivamente).**

#### 5.2. Para criar novos migrations:

1.  Certifique-se de que o serviço `postgres` esteja `healthy` via `docker-compose ps`.
2.  Navegue até a raiz do repositório:
3.  Execute o comando para criar a migration no projeto:
    ```bash
    dotnet ef migrations add sua_migration \
        --context LancamentosDbContext \
        --output-dir "Migrations" \
        --project "src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Infrastructure" \
        --startup-project "src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Infrastructure"
    ```
4.  Execute o comando para criar a migration no projeto:
    ```bash
    dotnet ef migrations add sua_migration \
        --context ConsolidacaoDbContext \
        --output-dir "Migrations" \
        --project "src/FlowWise.Services.Consolidacao/FlowWise.Services.Consolidacao.Infrastructure" \
        --startup-project "src/FlowWise.Services.Consolidacao/FlowWise.Services.Consolidacao.Infrastructure"
    ```
#### 5.2. Para o Banco de Dados de Lançamentos:

1.  Certifique-se de que o serviço `postgres` esteja `healthy` via `docker-compose ps`.
2.  Navegue até a raiz do repositório:
3.  Execute o comando para aplicar as migrações no banco de dados `flowwise_lancamentos_db`:
    ```bash
    dotnet ef database update \
        --context LancamentosDbContext \
        --project "src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Infrastructure" \
        --startup-project "src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Infrastructure"
    ```
4.  Execute o comando para aplicar as migrações no banco de dados `flowwise_lancamentos_db`:
    ```bash
    dotnet ef database update \
        --context ConsolidacaoDbContext \
        --project "src/FlowWise.Services.Consolidacao/FlowWise.Services.Consolidacao.Infrastructure" \
        --startup-project "src/FlowWise.Services.Consolidacao/FlowWise.Services.Consolidacao.Infrastructure"
    ```


### 6\. Testar as APIs

Com os serviços rodando, você pode usar seu cliente HTTP preferido (Postman, Insomnia) para testar os endpoints expostos:

  * **Serviço de Lançamentos API:** `http://localhost:5000/swagger`
  * **Serviço de Consolidação API:** `http://localhost:5001/swagger`

### 7\. Executando Testes Automatizados

Para rodar todos os testes automatizados do projeto:

```bash
dotnet test src/
```