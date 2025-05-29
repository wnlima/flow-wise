# Projeto Flow Wise: Gerenciamento Inteligente de Fluxo de Caixa

![C4 Model - Contexto](/docs/diagrams/C4-Context.jpg)

*Diagrama de Contexto do Projeto Flow Wise - Visão Geral do Sistema*

## 🚀 Visão Geral do Projeto

O **Projeto Flow Wise** é uma iniciativa estratégica para modernizar e otimizar o controle de fluxo de caixa diário da organização. Desenvolvido com uma arquitetura de **Microsserviços** e utilizando as tecnologias **C#/.NET 8+**, o Flow Wise visa centralizar, automatizar e prover insights acionáveis sobre o fluxo de caixa. Nosso objetivo é transformar a gestão financeira de um processo reativo para uma operação proativa, automatizada e com valor estratégico, preparando a empresa para futuras capacidades de análise preditiva via Inteligência Artificial.

Este repositório contém o código-fonte do **Produto Mínimo Viável (MVP)** do Flow Wise, implementado como uma prova de conceito (POC) para validar a arquitetura proposta e as principais funcionalidades.

## ✨ Destaques da Arquitetura e Tecnologia

* **Arquitetura:** Microsserviços com Domain-Driven Design (DDD), CQRS e Event Sourcing.
* **Linguagem & Framework:** C# / .NET 8+.
* **Bancos de Dados:** PostgreSQL (exclusivamente).
* **Comunicação Assíncrona:** RabbitMQ como Message Broker.
* **Cache & Resiliência:** Redis para otimização de performance e suporte à resiliência.
* **Orquestração:** Conteinerização com Docker e preparação para implantação em Kubernetes.
* **Infraestrutura como Código (IaC):** Terraform para gestão de infraestrutura.
* **Segurança:** Foco em Secure by Design, autenticação JWT e preparação para integração SSO.
* **Observabilidade:** OpenTelemetry como padrão para traces, logs e métricas, visando integração facilitada com ferramentas de mercado (Datadog, Elastic, Dynatrace).

## 📖 Sumário da Documentação

Para facilitar a navegação e o entendimento do projeto, consulte os seguintes documentos essenciais:

* **🏛️ Decisões Arquiteturais Chave (ARCHITECTURE\_DECISIONS.md):** Um resumo conciso das principais escolhas de arquitetura e padrões de design do Flow Wise, explicando o *porquê* de cada decisão.
    * [🏛️ ARQUITETURA\_DECISIONS.md](ARCHITECTURE_DECISIONS.md)
* **Visão Estratégica Completa (PDF):** Aprofunde-se no documento estratégico principal que detalha o escopo, requisitos de negócio e a arquitetura geral do Flow Wise.
    * [📄 Flow Wise - Visão Estratégica e Arquitetura da Solução.pdf](docs/Flow%20Wise%20-%20Visão%20Estratégica%20e%20Arquitetura%20da%20Solução.pdf)
* **Guias Essenciais para Desenvolvedores:**
    * [🚀 Inicie Aqui! (GET\_STARTED.md)](standards/GET_STARTED.md): Guia passo a passo para configurar seu ambiente de desenvolvimento e executar o projeto localmente.
    * [🤝 Como Contribuir (CONTRIBUTING.md)](standards/CONTRIBUTING.md): Entenda nosso fluxo de trabalho, padrões de commit e o processo para Pull Requests.
    * [🧑‍💻 Diretrizes de Codificação (CODING\_GUIDELINES.md)](standards/CODING_GUIDELINES.md): Conheça os padrões de código C#/.NET, boas práticas e princípios de design aplicados no projeto.
    * [🧪 Diretrizes de Testes (TESTING\_GUIDELINES.md)](standards/TESTING_GUIDELINES.md): Saiba mais sobre nossa estratégia de testes, os tipos de testes (unitários, integração), e as políticas de cobertura de código.
* **Detalhes da Infraestrutura:**
    * [⚙️ Infraestrutura do Projeto (INFRASTRUCTURE.md)](standards/INFRASTRUCTURE.md): Documentação sobre a configuração de Docker, Kubernetes, Infraestrutura como Código com Terraform e a estratégia multi-cloud.
* **Documentação dos Microsserviços:**
    * [💸 Serviço de Lançamentos (README.md)](src/FlowWise.Services.Lancamentos/README.md): Documentação detalhada sobre o microsserviço responsável pelo registro, consulta, edição e exclusão de lançamentos financeiros.
    * [📊 Serviço de Consolidação (README.md)](src/FlowWise.Services.Consolidacao/README.md): Documentação específica do microsserviço que processa e consolida os dados para relatórios de fluxo de caixa.

## 🏛️ Estrutura do Repositório

Este é um *monorepo* que organiza os diferentes microsserviços, bibliotecas compartilhadas e a documentação do projeto, conforme a estrutura abaixo:

```

.
├── .github/                      \# Configurações do GitHub Actions
│   └── workflows/                \# Pipelines de CI/CD (ci-pipeline.yml)
├── docs/                         \# Documentação de alto nível, diagramas e decisões arquiteturais
│   ├── diagrams/                 \# Arquivos Draw.io e imagens dos diagramas C4 (Contexto, Contêineres)
│   └── Flow Wise - Visão Estratégica e Arquitetura da Solução.pdf \# Documento principal em PDF
├── infra/                        \# Scripts de Infraestrutura como Código (Terraform) e configuração de serviços (PostgreSQL init)
├── standards/                    \# Padrões organizacionais e diretrizes (CONTRIBUTING.md, GET\_STARTED.md, etc.)
├── src/                          \# Código-fonte dos microsserviços e bibliotecas
│   ├── FlowWise.sln              \# Arquivo da Solução .NET principal
│   ├── FlowWise.Core/            \# Biblioteca Core (IoC, Shared Kernel, Middlewares, Behaviors, Configurações Base)
│   ├── FlowWise.Common/          \# Biblioteca com DTOs, Exceções e Value Objects comuns
│   ├── FlowWise.Services.Lancamentos/  \# Microsserviço de gestão de lançamentos
│   │   ├── FlowWise.Services.Lancamentos.Api/           \# Camada de API (Controllers, Requests, Responses)
│   │   ├── FlowWise.Services.Lancamentos.Application/   \# Camada de Aplicação (Commands, Queries, Handlers, Validators, Events)
│   │   ├── FlowWise.Services.Lancamentos.Domain/        \# Camada de Domínio (Entities, Value Objects, Domain Events, Interfaces de Repositório)
│   │   ├── FlowWise.Services.Lancamentos.Infrastructure/\# Camada de Infraestrutura (Persistence com EF Core, Migrations, Event Publishers)
│   │   ├── FlowWise.Services.Lancamentos.Tests.UnitTests/    \# Testes unitários para o serviço de lançamentos
│   │   └── FlowWise.Services.Lancamentos.Tests.IntegrationTests/ \# Testes de integração para o serviço de lançamentos
│   └── FlowWise.Services.Consolidacao/ \# Microsserviço de consolidação de fluxo de caixa
│       ├── FlowWise.Services.Consolidacao.Api/          \# Camada de API (Controllers, Requests, Responses)
│       ├── FlowWise.Services.Consolidacao.Application/  \# Camada de Aplicação (Event Consumers, Queries, Handlers)
│       ├── FlowWise.Services.Consolidacao.Domain/       \# Camada de Domínio (Entities, Interfaces de Repositório)
│       ├── FlowWise.Services.Consolidacao.Infrastructure/\# Camada de Infraestrutura (Persistence com EF Core, Migrations)
│       ├── FlowWise.Services.Consolidacao.Tests.UnitTests/   \# Testes unitários para o serviço de consolidação
│       └── FlowWise.Services.Consolidacao.Tests.IntegrationTests/ \# Testes de integração para o serviço de consolidação
├── README.md                     \# Este arquivo
├── coverage-report.sh            \# Script para geração de relatório de cobertura de testes
└── docker-compose.yml            \# Arquivo para orquestração local de serviços (PostgreSQL, RabbitMQ, Redis)

````

## ▶️ Rodando o Projeto Localmente (POC)

Para instruções detalhadas sobre como configurar seu ambiente de desenvolvimento e executar os microsserviços e suas dependências (PostgreSQL, RabbitMQ, Redis) localmente usando Docker, consulte o guia:

* **[🚀 Inicie Aqui! (GET\_STARTED.md)](standards/GET_STARTED.md)**

### Acesso às APIs (Swagger)

Com os serviços rodando localmente:

* **Serviço de Lançamentos API:** `http://localhost:5000/swagger` (ou a porta configurada em `launchSettings.json`)
* **Serviço de Consolidação API:** `http://localhost:5001/swagger` (ou a porta configurada em `launchSettings.json`)

## 🧪 Executando Testes

Para executar os testes unitários e de integração de todos os serviços e gerar relatórios de cobertura:

* **[🧪 Diretrizes de Testes (TESTING\_GUIDELINES.md)](standards/TESTING_GUIDELINES.md)**

Resumidamente, na raiz do projeto:
```bash
dotnet test src/
./coverage-report.sh
````

## 🔒 Gerenciamento de Dados Sensíveis

**ATENÇÃO:** No ambiente de desenvolvimento local, utilize o **User Secrets** do .NET. Para ambientes de Produção, é **mandatório** o uso de soluções de *Secrets Management* da nuvem (ex: Azure Key Vault, AWS Secrets Manager). Consulte as [🧑‍💻 Diretrizes de Codificação](standards/CODING_GUIDELINES.md) para mais detalhes.

## ⚙️ Pipelines de CI/CD (GitHub Actions)

Este repositório utiliza GitHub Actions para Integração Contínua (CI). O pipeline (`.github/workflows/ci-pipeline.yml`) automatiza:

  * Build dos projetos.
  * Execução de testes e verificação de cobertura mínima.
  * (Futuras etapas) Análise estática, build de imagens Docker e deployment.

-----

## 📞 Contato

Para dúvidas, sugestões ou suporte, entre em contato com [Willian Lima](https://www.linkedin.com/in/w-lima).

![Perfil do LinkedIn](https://media.licdn.com/dms/image/v2/D4D03AQGRObzA0_NRkg/profile-displayphoto-shrink_200_200/profile-displayphoto-shrink_200_200/0/1703104875697?e=1751500800&v=beta&t=jWwem7-YUYxBoktc3ayzIMLMdT4RlMQcsh-WlFW0pTM)