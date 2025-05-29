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
    * **Visual Studio Code:** Leve e extensível, com muitas extensões úteis para C# e Docker.
        * [Download VS Code](https://code.visualstudio.com/download)
        * **Extensões Recomendadas:**
            * C# (Microsoft)
            * Docker
            * GitLens
            * EditorConfig for VS Code
5.  **Cliente HTTP (Opcional, mas Recomendado):** Para testar as APIs localmente.
    * **Postman:** [Download Postman](https://www.postman.com/downloads/)
    * **Insomnia:** [Download Insomnia](https://insomnia.rest/download)

## ⬇️ Configuração do Ambiente

Siga os passos abaixo para configurar seu ambiente:

### 1. Clonar o Repositório

Abra seu terminal ou prompt de comando e execute:

```bash
git clone https://github.com/wnlima/flow-wise.git
cd flow-wise
````

### 2\. Configurar User Secrets (Credenciais)

Para rodar os microsserviços localmente, você precisará configurar as strings de conexão e outras credenciais sensíveis. É **mandatório** o uso do **.NET User Secrets** para isso, garantindo que suas credenciais **não sejam versionadas** no código.

#### Para o `FlowWise.Services.Lancamentos.Api`:

1.  Navegue até a pasta do projeto API:

    ```bash
    cd src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Api
    ```

2.  Inicialize os User Secrets (se ainda não o fez):

    ```bash
    dotnet user-secrets init
    ```

3.  Defina os segredos necessários para o serviço de Lançamentos. **Substitua os valores de `flowwise_user` e `flowwise_password` por senhas fortes que você controlará localmente:**

    ```bash
    dotnet user-secrets set "ConnectionStrings:PostgreSQL" "Host=localhost;Port=5432;Database=flowwise_lancamentos_db;Username=flowwise_user;Password=flowwise_password"
    dotnet user-secrets set "RabbitMQ:Host" "localhost"
    dotnet user-secrets set "RabbitMQ:Username" "guest" # Usuário padrão do RabbitMQ para desenvolvimento
    dotnet user-secrets set "RabbitMQ:Password" "guest" # Senha padrão do RabbitMQ para desenvolvimento
    dotnet user-secrets set "Redis:Configuration" "localhost:6379"
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

3.  Defina os segredos necessários para o serviço de Consolidação. **Substitua os valores de `flowwise_user` e `flowwise_password` por senhas fortes que você controlará localmente:**

    ```bash
    dotnet user-secrets set "ConnectionStrings:PostgreSQL" "Host=localhost;Port=5432;Database=flowwise_consolidacao_db;Username=flowwise_user;Password=flowwise_password"
    dotnet user-secrets set "RabbitMQ:Host" "localhost"
    dotnet user-secrets set "RabbitMQ:Username" "flowwise_user"
    dotnet user-secrets set "RabbitMQ:Password" "flowwise_password"
    dotnet user-secrets set "Redis:Configuration" "localhost:6379"
    ```

    *Para mais informações sobre User Secrets e gerenciamento de segredos em .NET, consulte a [documentação oficial da Microsoft](https://learn.microsoft.com/pt-br/aspnet/core/security/app-secrets%3Fview%3Daspnetcore-8.0%26tabs%3Dwindows).*

### 3\. Subir Dependências Locais com Docker Compose

As dependências de infraestrutura (PostgreSQL, RabbitMQ, Redis) são orquestradas via Docker Compose.

1.  Na raiz do repositório (`flow-wise/`), execute o comando para subir os serviços em segundo plano:

    ```bash
    docker compose up -d postgres rabbitmq redis
    ```

    *Este comando iniciará apenas os serviços `postgres`, `rabbitmq` e `redis` definidos no seu `docker-compose.yml`, além de suas dependências.*

2.  Verifique se os contêineres subiram corretamente e estão em estado `healthy`:

    ```bash
    docker compose ps
    ```

    Você deve ver `healthy` ou `Up` para `flowwise-postgres`, `flowwise-rabbitmq` e `flowwise-redis`. Aguarde até que o `flowwise-postgres` esteja `healthy` antes de prosseguir com as migrações.

### 4\. Executar Migrações de Banco de Dados (PostgreSQL)

Após as dependências de banco de dados estarem em execução e `healthy`, aplique as migrações do Entity Framework Core para criar/atualizar os schemas dos bancos de dados de cada microsserviço.

1.  **Para o Banco de Dados de Lançamentos (`flowwise_lancamentos_db`):**
    Navegue até a raiz do repositório (`flow-wise/`) e execute:

    ```bash
    dotnet ef database update --project src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Infrastructure/FlowWise.Services.Lancamentos.Infrastructure.csproj --startup-project src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Api/FlowWise.Services.Lancamentos.Api.csproj --context LancamentosDbContext
    ```

2.  **Para o Banco de Dados de Consolidação (`flowwise_consolidacao_db`):**
    Navegue até a raiz do repositório (`flow-wise/`) e execute:

    ```bash
    dotnet ef database update --project src/FlowWise.Services.Consolidacao/FlowWise.Services.Consolidacao.Infrastructure/FlowWise.Services.Consolidacao.Infrastructure.csproj --startup-project src/FlowWise.Services.Consolidacao/FlowWise.Services.Consolidacao.Api/FlowWise.Services.Consolidacao.Api.csproj --context ConsolidacaoDbContext
    ```

    *Estes comandos aplicarão todas as migrações pendentes, criando as tabelas necessárias no PostgreSQL.*

    **Nota:** Se precisar **criar novas migrações** no futuro (após alterações no modelo de domínio), use o comando `dotnet ef migrations add [NomeDaSuaMigracao]` com os parâmetros `--project`, `--startup-project` e `--context` apropriados, como no exemplo abaixo (não execute isso agora, apenas para referência):

    ```bash
    # Exemplo para criar uma nova migração no serviço de Lançamentos
    dotnet ef migrations add SuaNovaMigracaoLancamentos --project src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Infrastructure/FlowWise.Services.Lancamentos.Infrastructure.csproj --startup-project src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Api/FlowWise.Services.Lancamentos.Api.csproj --context LancamentosDbContext
    ```

### 5\. Rodar os Microsserviços

Após as dependências e bancos de dados estarem configurados, você pode iniciar os microsserviços.

#### Opção A: Via Terminal (para cada serviço)

Abra um novo terminal (ou aba no terminal) para cada microsserviço que deseja rodar. Certifique-se de estar na raiz do repositório (`flow-wise/`) antes de executar os comandos.

1.  **Serviço de Lançamentos:**

    ```bash
    dotnet run --project src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Api/
    ```

    A API estará disponível em `http://localhost:5000`. O Swagger UI em `http://localhost:5000/swagger`.

2.  **Serviço de Consolidação:**

    ```bash
    dotnet run --project src/FlowWise.Services.Consolidacao/FlowWise.Services.Consolidacao.Api/
    ```

    A API estará disponível em `http://localhost:5001`. O Swagger UI em `http://localhost:5001/swagger`.

#### Opção B: Via Visual Studio (Se usar)

1.  Abra a *solution* principal (`FlowWise.sln`) localizada na pasta `src/` no Visual Studio.
2.  No Solution Explorer, clique com o botão direito na Solution `FlowWise.sln` -\> "Set Startup Projects...".
3.  Selecione "Multiple startup projects" e marque `FlowWise.Services.Lancamentos.Api` e `FlowWise.Services.Consolidacao.Api` para "Start".
4.  Pressione `F5` ou clique no botão "Start" para iniciar ambos os serviços simultaneamente.
5.  As portas serão definidas no `Properties/launchSettings.json` de cada projeto API.

### 6\. Testar as APIs

Com os serviços rodando, você pode usar seu cliente HTTP preferido (Postman, Insomnia) para testar os endpoints expostos:

  * **Serviço de Lançamentos API:** `http://localhost:5000/swagger`
  * **Serviço de Consolidação API:** `http://localhost:5001/swagger`

### 7\. Executando Testes Automatizados

Para rodar todos os testes automatizados do projeto:

1.  Navegue para a raiz do repositório (`flow-wise/`).
2.  Execute o comando:
    ```bash
    dotnet test src/
    ```
    *Este comando executará todos os testes unitários e de integração definidos nos projetos de teste.*

-----

## 🛑 Parando o Projeto

Para parar os serviços e limpar os recursos do Docker:

1.  **Pare os Microsserviços:**
    Se você os iniciou via terminal, simplesmente feche os terminais ou pressione `Ctrl+C` em cada um. Se usou o Visual Studio, pare a depuração.

2.  **Pare e Remova os Contêineres Docker:**
    Na raiz do repositório (`flow-wise/`), execute:

    ```bash
    docker compose down
    ```

    *Este comando irá parar e remover os contêineres, redes e volumes criados pelo `docker-compose up`.*