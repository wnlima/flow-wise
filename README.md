# Projeto Flow Wise: Gerenciamento Inteligente de Fluxo de Caixa

![C4 Model - Contexto](/docs/diagrams/C4-Context.jpg)

*Diagrama de Contexto do Projeto Flow Wise - Visão Geral do Sistema*

## 🚀 Visão Geral do Projeto

O **Projeto Flow Wise** é uma iniciativa estratégica para modernizar e otimizar o controle de fluxo de caixa diário da organização. Desenvolvido com uma arquitetura de **Microsserviços** e utilizando as tecnologias **C#/.NET 8+**, o Flow Wise visa centralizar, automatizar e prover insights acionáveis sobre o fluxo de caixa. Nosso objetivo é transformar a gestão financeira de um processo reativo para uma operação proativa, automatizada e com valor estratégico, preparando a empresa para futuras capacidades de análise preditiva via Inteligência Artificial.

Este repositório contém o código-fonte do **Produto Mínimo Viável (MVP)** do Flow Wise, implementado como uma prova de conceito (POC) para validar a arquitetura proposta e as principais funcionalidades.

## ✨ Destaques da Arquitetura e Tecnologia

* **Arquitetura:** Microsserviços com Domain-Driven Design (DDD), CQRS e Event Sourcing.
* **Linguagem & Framework:** C# / .NET 8+.
* **Bancos de Dados:** PostgreSQL.
* **Comunicação Assíncrona:** RabbitMQ como Message Broker.
* **Cache & Resiliência:** Redis para otimização de performance e suporte à resiliência.
* **Orquestração:** Conteinerização com Docker e preparação para implantação em Kubernetes.
* **Infraestrutura como Código (IaC):** Terraform para gestão de infraestrutura (Pós-MVP).
* **Segurança:** Foco em Secure by Design, com preparação para integração SSO e Pentests robustos.
* **Observabilidade:** Preparado para integração 'plug-and-play' com ferramentas de monitoramento líderes de mercado.

Para um detalhamento completo da visão estratégica, requisitos, decisões arquiteturais e padrões, consulte a [Documentação Estratégica Completa](/docs/Flow%20Wise%20-%20Visão%20Estratégica%20e%20Arquitetura%20da%20Solução.pdf).


## 📦 Estrutura do Repositório

Este é um *monorepo* que organiza os diferentes microsserviços e a documentação do projeto.

```
.
├── .github/                      \# Configurações do GitHub Actions
│   └── workflows/                \# Pipelines de CI/CD
├── docs/                         \# Documentação de alto nível e decisões arquiteturais
│   ├── 01-vision-strategy/       \# Visão geral, requisitos funcionais e não-funcionais
│   ├── 02-arquitetura-software/  \# Detalhes da arquitetura (C4 Containers, ADRs)
│   ├── diagrams/                 \# Arquivos Draw.io e imagens dos diagramas C4
│   └── Flow Wise - Visão Estratégica e Arquitetura da Solução.pdf \# Cópia do documento principal em PDF
├── infra/                        \# Scripts de Infraestrutura como Código (Terraform)
├── standards/                    \# Padrões organizacionais e diretrizes de contribuição
│   ├── CONTRIBUTING.md           \# Guia de contribuição (incluindo commits semânticos e Git Flow)
│   ├── GET\_STARTED.md            \# Guia de início rápido para novos desenvolvedores
│   ├── INFRASTRUCTURE.md         \# Detalhes e configuração da infraestrutura
│   ├── CODING\_GUIDELINES.md      \# Padrões de codificação C\#/.NET
│   └── TESTING\_GUIDELINES.md     \# Diretrizes de testes e cobertura de código
├── src/                          \# Código-fonte dos microsserviços
│   ├── FlowWise.Services.Lancamentos/  \# Microsserviço de gestão de lançamentos
│   │   ├── FlowWise.Services.Lancamentos.Api/ \# Projeto da API
│   │   ├── FlowWise.Services.Lancamentos.Domain/ \# Projeto de Domínio (DDD)
│   │   ├── FlowWise.Services.Lancamentos.Infrastructure/ \# Projeto de Infraestrutura/Dados
│   │   └── FlowWise.Services.Lancamentos.Tests/ \# Projetos de Testes
│   ├── FlowWise.Services.Consolidacao/ \# Microsserviço de consolidação de fluxo de caixa
│   │   ├── FlowWise.Services.Consolidacao.Api/
│   │   ├── FlowWise.Services.Consolidacao.Domain/
│   │   ├── FlowWise.Services.Consolidacao.Infrastructure/
│   │   └── FlowWise.Services.Consolidacao.Tests/
│   └── FlowWise.Common/          \# Biblioteca de classes comuns/compartilhadas
└── docker-compose.yml            \# Arquivo para orquestração local de serviços

````

## 📚 Documentação Estratégica Completa

A documentação completa do Projeto Flow Wise, incluindo a visão estratégica, requisitos de negócio e não-funcionais detalhados, decisões arquiteturais e padrões organizacionais, está disponível:

* **PDF:** Uma cópia estática da versão mais recente pode ser encontrada em [`docs/Flow Wise - Visão Estratégica e Arquitetura da Solução.pdf`](/docs/Flow%20Wise%20-%20Visão%20Estratégica%20e%20Arquitetura%20da%20Solução.pdf).

## ▶️ Primeiros Passos para Desenvolvedores (Inicie Aqui!)

Para novos desenvolvedores ou contribuidores, recomendamos começar lendo o guia **[Inicie Aqui!](standards/GET_STARTED.md)** para configurar seu ambiente e entender o fluxo de trabalho.

## 🤝 Como Contribuir

Valorizamos as contribuições e incentivamos a colaboração. Por favor, consulte o nosso guia detalhado **[Como Contribuir](standards/CONTRIBUTING.md)** para entender nosso fluxo de trabalho de desenvolvimento, padrões de *commits semânticos*, diretrizes de codificação e processo de *Pull Request*.

## 🐳 Rodando o Projeto Localmente (POC)

Este projeto utiliza Docker para facilitar o desenvolvimento local, isolando dependências e garantindo um ambiente consistente.

### Pré-requisitos

* [Docker Desktop](https://www.docker.com/products/docker-desktop) instalado e em execução.
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado.

### Passos

1.  **Clone o Repositório:**
    ```bash
    git clone [https://github.com/wnlima/flow-wise.git](https://github.com/wnlima/flow-wise.git)
    cd flow-wise
    ```
2.  **Construa e Suba os Contêineres de Suporte:**
    O arquivo `docker-compose.yml` já contém as configurações para o PostgreSQL, RabbitMQ e Redis.
    ```bash
    docker-compose up -d postgres rabbitmq redis
    ```
3.  **Construa e Execute os Microsserviços:**
    Navegue até a pasta de cada microsserviço (ex: `src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Api/`) e use os comandos .NET para rodar. Ou, para rodar todos:
    ```bash
    # Para rodar o serviço de Lançamentos
    cd src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Api
    dotnet run
    # Abra um novo terminal para rodar o serviço de Consolidação
    cd src/FlowWise.Services.Consolidacao/FlowWise.Services.Consolidacao.Api
    dotnet run
    ```
    *Para uma experiência Docker completa para os microsserviços, consulte o `docker-compose.yml` e o `GET_STARTED.md`.*

### Acesso às APIs (POC)

Com os serviços rodando localmente, você pode acessar as APIs via Swagger:

* **Serviço de Lançamentos API:** `http://localhost:5000/swagger` (ou a porta configurada localmente)
* **Serviço de Consolidação API:** `http://localhost:5001/swagger` (ou a porta configurada localmente)

## 🧪 Executando Testes

Para executar os testes unitários e de integração de todos os serviços:

```bash
dotnet test src/
````

Para mais detalhes sobre a estratégia de testes e cobertura de código, consulte o **[Guia de Testes](/standards/TESTING_GUIDELINES.md)**.

## 🔒 Gerenciamento de Dados Sensíveis

**ATENÇÃO:** No ambiente de desenvolvimento local e para o POC, credenciais podem ser gerenciadas via `User Secrets` do .NET para fins de conveniência. **Em ambientes de produção, é mandatório o uso de soluções de *Secrets Management* da nuvem** (ex: Azure Key Vault, AWS Secrets Manager, HashiCorp Vault), conforme detalhado nas [Diretrizes de Segurança](/standards/CODING_GUIDELINES.md) (será abordado em `standards/CODING_GUIDELINES.md`).

## ⚙️ Pipelines de CI/CD (GitHub Actions)

Este repositório utiliza GitHub Actions para automatizar os pipelines de Integração Contínua (CI). As configurações estão em `.github/workflows/`. O pipeline realizará:

  * Build dos projetos.
  * Execução de testes.
  * Análise estática de código.
  * Build de imagens Docker.
  * (Em futuras fases) Deployment automatizado.

-----

## 📞 Contato

Para dúvidas, sugestões ou suporte, entre em contato com [Willian Lima][Willian Lima](https://www.linkedin.com/in/w-lima)

[![Perfil do LinkedIn](https://media.licdn.com/dms/image/v2/D4D03AQGRObzA0_NRkg/profile-displayphoto-shrink_200_200/profile-displayphoto-shrink_200_200/0/1703104875697?e=1751500800&v=beta&t=jWwem7-YUYxBoktc3ayzIMLMdT4RlMQcsh-WlFW0pTM)](https://www.linkedin.com/in/w-lima)