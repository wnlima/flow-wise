# 📊 Flow Wise: Serviço de Consolidação

Este repositório contém o código-fonte do **Serviço de Consolidação** do Projeto Flow Wise. Este microsserviço é responsável por processar os eventos de lançamentos financeiros e construir um modelo de leitura otimizado para consultas rápidas de saldos diários e relatórios de fluxo de caixa.

## ✨ Visão Geral do Serviço

O **Serviço de Consolidação** atua como o **lado de consulta (leitura)** do padrão CQRS. Ele não recebe comandos diretamente, mas reage a eventos de domínio publicados pelo Serviço de Lançamentos (e, potencialmente, outros serviços no futuro). Ao consumir esses eventos, ele mantém uma projeção de dados (Read Model) que é otimizada para consultas de alta performance, sem a complexidade do modelo transacional de escrita.

### Responsabilidades Chave:

* **Consumo de Eventos:** Inscrever-se e consumir eventos de domínio do RabbitMQ (ex: `LancamentoRegistradoEvent`, `LancamentoAtualizadoEvent`, `LancamentoExcluidoEvent`).
* **Projeção de Dados (Read Model):** Processar os eventos recebidos para manter um estado consolidado e otimizado (ex: `SaldoDiario`).
* **Consulta de Saldo Diário:** Oferecer a capacidade de consultar o saldo consolidado para uma data específica.
* **Geração de Relatórios:** Gerar relatórios agregados de fluxo de caixa com base nos dados consolidados.

### Contexto no C4 Model:

* No [Diagrama de Contêineres](/docs/diagrams/C4-Container.jpg), este serviço é o `Microsserviço: Serviço de Consolidação`.
* Ele interage com o `DB Consolidação` para persistência do Read Model e consome eventos do `RabbitMQ`.

## 📦 Estrutura do Projeto

Este microsserviço segue uma arquitetura em camadas para separar responsabilidades:

```

FlowWise.Services.Consolidacao/
├── FlowWise.Services.Consolidacao.Api/             \# Projeto API REST (endpoints, controladores, DTOs de entrada)
├── FlowWise.Services.Consolidacao.Application/     \# Camada de Aplicação (Queries, Query Handlers, Event Consumers)
├── FlowWise.Services.Consolidacao.Domain/          \# Camada de Domínio (Entidades do Read Model, regras de consistência de leitura)
├── FlowWise.Services.Consolidacao.Infrastructure/  \# Camada de Infraestrutura (implementações de persistência com EF Core, configuração do MassTransit)
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

* **Endpoint Principal:** `http://localhost:5001/api/consolidacoes`
* **Swagger UI:** `http://localhost:5001/swagger`

### Principais Endpoints:

* `GET /api/consolidacoes/saldo-diario?data={YYYY-MM-DD}`: Consulta o saldo consolidado para uma data específica.
* `GET /api/consolidacoes/relatorio-fluxo-caixa?dataInicio={YYYY-MM-DD}&dataFim={YYYY-MM-DD}`: Gera um relatório de fluxo de caixa para um período.

## 📥 Eventos Consumidos

Este serviço consome os seguintes eventos do RabbitMQ, publicados pelo Serviço de Lançamentos, para construir e manter seu modelo de leitura consolidado:

* `LancamentoRegistradoEvent`
* `LancamentoAtualizadoEvent`
* `LancamentoExcluidoEvent`

Ao consumir esses eventos, o Serviço de Consolidação atualiza o `SaldoDiario` e outros dados de projeção para refletir as mudanças no fluxo de caixa.

## 🧪 Executando Testes

Para executar os testes unitários e de integração específicos deste serviço:

```bash
cd src/FlowWise.Services.Consolidacao/FlowWise.Services.Consolidacao.Tests
dotnet test
````

Para rodar todos os testes do repositório e gerar o relatório de cobertura, use o script na raiz: `..\..\coverage-report.sh`.