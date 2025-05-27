# 🚀 Flow Wise: Serviço de Consolidação

Este repositório contém o código-fonte do **Serviço de Consolidação** do Projeto Flow Wise. Este microsserviço é fundamental para prover uma visão agregada e em tempo quase real do fluxo de caixa diário, otimizada para consultas e relatórios.

## ✨ Visão Geral do Serviço

O **Serviço de Consolidação** atua como o **lado de consulta (leitura)** do padrão CQRS no Flow Wise. Ele não processa diretamente o registro de lançamentos, mas reage a eventos de domínio publicados pelo Serviço de Lançamentos para construir e manter uma projeção de dados (Query Model) que é otimizada para a leitura rápida do saldo diário consolidado.

### Responsabilidades Chave:

* **Consumo de Eventos:** Ouvir e processar eventos de lançamentos (ex: `LancamentoRegistradoEvent`) publicados no RabbitMQ.
* **Projeção de Dados:** Manter uma projeção atualizada do saldo de caixa diário consolidado em seu próprio banco de dados otimizado para leitura.
* **Consulta de Saldo Consolidado:** Expor uma API para que Gerentes Financeiros e sistemas de BI possam consultar o saldo diário e gerar relatórios de forma performática.
* **Resiliência:** Garantir que a consolidação de dados ocorra de forma assíncrona, sem impactar a capacidade de registro de lançamentos.

### Contexto no C4 Model:

* No [Diagrama de Contêineres](/docs/diagrams/C4-Container.jpg), este serviço é o `Microsserviço: Serviço de Consolidação`.
* Ele interage com o `RabbitMQ` para consumir eventos e com o `DB Consolidados (Query Model)` para persistir e consultar suas projeções.

## 📦 Estrutura do Projeto

Este microsserviço segue uma arquitetura em camadas para separar responsabilidades:

```

FlowWise.Services.Consolidacao/
├── FlowWise.Services.Consolidacao.Api/             \# Projeto API REST (endpoints, controladores, DTOs de consulta)
├── FlowWise.Services.Consolidacao.Application/     \# Camada de Aplicação (queries, handlers, orquestração de projeções)
├── FlowWise.Services.Consolidacao.Domain/          \# Camada de Domínio (representação das projeções, regras de leitura)
├── FlowWise.Services.Consolidacao.Infrastructure/  \# Camada de Infraestrutura (implementações de persistência com EF Core, consumo de eventos do RabbitMQ)
└── FlowWise.Services.Consolidacao.Tests/           \# Projetos de Testes (Unitários, Integração)

````

## ▶️ Como Rodar Localmente

Para rodar este serviço localmente, você precisará ter o [Docker Desktop](https://www.docker.com/products/docker-desktop) e o [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalados.

1.  **Configure o Ambiente:**
    * Siga as instruções de [configuração do ambiente no guia GET_STARTED.md](/standards/GET_STARTED.md), especialmente a parte de [Configurar Variáveis de Ambiente (Credenciais) para o Serviço de Consolidação](#2-configurar-variáveis-de-ambiente-credenciais).
    * Certifique-se de que as dependências locais (PostgreSQL, RabbitMQ, Redis) estejam rodando via Docker Compose: `docker-compose up -d postgres rabbitmq redis`.
2.  **Aplique as Migrações do Banco de Dados:**
    * Navegue até a pasta de infraestrutura do serviço: `cd src/FlowWise.Services.Consolidacao/FlowWise.Services.Consolidacao.Infrastructure`
    * Execute a migração: `dotnet ef database update --project ..\FlowWise.Services.Consolidacao.Api/FlowWise.Services.Consolidacao.Api.csproj`
3.  **Execute o Serviço:**
    * Navegue até a pasta do projeto API: `cd src/FlowWise.Services.Consolidacao/FlowWise.Services.Consolidacao.Api`
    * Restaure as dependências e execute: `dotnet restore && dotnet run`

A API estará disponível em `http://localhost:5001` (verifique `launchSettings.json` para a porta exata). Você pode acessar a documentação Swagger em `http://localhost:5001/swagger`.

## 📚 APIs (Endpoints)

A documentação interativa da API deste serviço está disponível via Swagger UI quando o serviço está rodando localmente.

* **Endpoint Principal:** `http://localhost:5001/api/consolidados`
* **Swagger UI:** `http://localhost:5001/swagger`

### Principais Endpoints:

* `GET /api/consolidados?data={YYYY-MM-DD}`: Consulta o saldo consolidado para uma data específica (D-1).
* `GET /api/consolidados/relatorio?startDate={YYYY-MM-DD}&endDate={YYYY-MM-DD}`: Gera um relatório de fluxo de caixa para um período.

## 📥 Eventos Consumidos

Este serviço consome os seguintes eventos de domínio do RabbitMQ:

* `LancamentoRegistradoEvent`: Para processar e incluir novos lançamentos na consolidação.
* `LancamentoAtualizadoEvent`: Para reprocessar lançamentos alterados e atualizar a consolidação.
* `LancamentoExcluidoEvent`: Para ajustar a consolidação em caso de exclusão de lançamentos.

## 🧪 Executando Testes

Para executar os testes unitários e de integração específicos deste serviço:

```bash
cd src/FlowWise.Services.Consolidacao/FlowWise.Services.Consolidacao.Tests
dotnet test
````

Para rodar todos os testes do repositório e gerar o relatório de cobertura, use o script na raiz: `..\..\coverage-report.sh`.

## 🤝 Contribuindo

Consulte o [guia](/CONTRIBUTING.md) principal de [URL inválido removido] para detalhes sobre nosso fluxo de trabalho, padrões de commits e diretrizes de codificação.