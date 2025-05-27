# 🚀 Flow Wise: Serviço de Lançamentos

Este repositório contém o código-fonte do **Serviço de Lançamentos** do Projeto Flow Wise. Este microsserviço é um componente central para a gestão de fluxo de caixa, responsável por todo o ciclo de vida dos lançamentos financeiros (débitos e créditos).

## ✨ Visão Geral do Serviço

O **Serviço de Lançamentos** é a fonte da verdade para todas as operações transacionais de débito e crédito no Flow Wise. Ele implementa o **lado de comando (escrita)** do padrão CQRS para garantir a integridade e a rastreabilidade das informações financeiras. Após cada operação de sucesso, eventos de domínio são publicados para o Message Broker, permitindo que outros serviços (como o Serviço de Consolidação) reajam a essas mudanças.

### Responsabilidades Chave:

* **Registro de Lançamentos:** Permitir a criação de novos lançamentos de débito e crédito.
* **Consulta de Lançamentos:** Oferecer a capacidade de consultar lançamentos individuais ou por período.
* **Edição e Exclusão de Lançamentos:** Gerenciar a modificação e remoção de lançamentos, seguindo as regras de negócio de auditoria e imutabilidade histórica.
* **Publicação de Eventos de Domínio:** Publicar eventos (ex: `LancamentoRegistradoEvent`) no RabbitMQ para comunicar alterações de estado para outros serviços.

### Contexto no C4 Model:

* No [Diagrama de Contêineres](/docs/diagrams/C4-Container.jpg), este serviço é o `Microsserviço: Serviço de Lançamentos`.
* Ele interage com o `DB Lançamentos` para persistência e com o `RabbitMQ` para publicação de eventos.

## 📦 Estrutura do Projeto

Este microsserviço segue uma arquitetura em camadas para separar responsabilidades:

```
FlowWise.Services.Lancamentos/
├── FlowWise.Services.Lancamentos.Api/             \# Projeto API REST (endpoints, controladores, DTOs de entrada)
├── FlowWise.Services.Lancamentos.Application/     \# Camada de Aplicação (comandos, queries, handlers, orquestração de domínio)
├── FlowWise.Services.Lancamentos.Domain/          \# Camada de Domínio (Entidades, Value Objects, Agregados, Domain Events, regras de negócio)
├── FlowWise.Services.Lancamentos.Infrastructure/  \# Camada de Infraestrutura (implementações de persistência com EF Core, integração com RabbitMQ)
└── FlowWise.Services.Lancamentos.Tests/           \# Projetos de Testes (Unitários, Integração)
````

## ▶️ Como Rodar Localmente

Para rodar este serviço localmente, você precisará ter o [Docker Desktop](https://www.docker.com/products/docker-desktop) e o [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalados.

1.  **Configure o Ambiente:**
    * Siga as instruções de [configuração do ambiente no guia GET_STARTED.md](/standards/GET_STARTED.md), especialmente a parte de [Configurar Variáveis de Ambiente (Credenciais) para o Serviço de Lançamentos](#2-configurar-variáveis-de-ambiente-credenciais).
    * Certifique-se de que as dependências locais (PostgreSQL, RabbitMQ, Redis) estejam rodando via Docker Compose: `docker-compose up -d postgres rabbitmq redis`.
2.  **Aplique as Migrações do Banco de Dados:**
    * Navegue até a pasta de infraestrutura do serviço: `cd src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Infrastructure`
    * Execute a migração: `dotnet ef database update --project ..\FlowWise.Services.Lancamentos.Api/FlowWise.Services.Lancamentos.Api.csproj`
3.  **Execute o Serviço:**
    * Navegue até a pasta do projeto API: `cd src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Api`
    * Restaure as dependências e execute: `dotnet restore && dotnet run`

A API estará disponível em `http://localhost:5000` (verifique `launchSettings.json` para a porta exata). Você pode acessar a documentação Swagger em `http://localhost:5000/swagger`.

## 📚 APIs (Endpoints)

A documentação interativa da API deste serviço está disponível via Swagger UI quando o serviço está rodando localmente.

* **Endpoint Principal:** `http://localhost:5000/api/lancamentos`
* **Swagger UI:** `http://localhost:5000/swagger`

### Principais Endpoints:

* `POST /api/lancamentos`: Registra um novo lançamento (débito ou crédito).
* `GET /api/lancamentos/{id}`: Consulta um lançamento específico pelo ID.
* `GET /api/lancamentos?data={YYYY-MM-DD}`: Lista lançamentos por data.
* `PUT /api/lancamentos/{id}`: Edita um lançamento existente.
* `DELETE /api/lancamentos/{id}`: Exclui um lançamento.

## 📬 Eventos Publicados

Este serviço publica os seguintes eventos de domínio no RabbitMQ:

* `LancamentoRegistradoEvent`: Publicado após o registro bem-sucedido de um novo lançamento.
* `LancamentoAtualizadoEvent`: Publicado após a atualização de um lançamento existente.
* `LancamentoExcluidoEvent`: Publicado após a exclusão de um lançamento.

Outros serviços, como o Serviço de Consolidação, consomem esses eventos para manter seus dados atualizados.

## 🧪 Executando Testes

Para executar os testes unitários e de integração específicos deste serviço:

```bash
cd src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Tests
dotnet test
````

Para rodar todos os testes do repositório e gerar o relatório de cobertura, use o script na raiz: `..\..\coverage-report.sh`.

## 🤝 Contribuindo

Consulte o [guia](/CONTRIBUTING.md) principal de [URL inválido removido] para detalhes sobre nosso fluxo de trabalho, padrões de commits e diretrizes de codificação.