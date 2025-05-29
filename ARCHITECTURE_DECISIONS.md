# 🏛️ Decisões de Arquitetura e Design do Projeto Flow Wise

Este documento serve como um guia de alto nível para as principais decisões de arquitetura e design tomadas no desenvolvimento do Projeto Flow Wise. Nosso objetivo é construir um sistema de fluxo de caixa diário que seja **alto desempenho, escalável, resiliente e seguro**, e as escolhas arquiteturais refletem diretamente esses pilares.

Ele visa fornecer o "porquê" por trás das nossas abordagens, complementando os detalhes de "como" fazer que são encontrados em outros documentos e no próprio código.

## Sumário

1.  [Visão Geral da Arquitetura](#1-visão-geral-da-arquitetura)
2.  [Pilares Fundamentais e Objetivos](#2-pilares-fundamentais-e-objetivos)
3.  [Padrões e Decisões de Design Chave](#3-padrões-e-decisões-de-design-chave)
    * [3.1. Microsserviços](#31-microsserviços)
    * [3.2. Domain-Driven Design (DDD)](#32-domain-driven-design-ddd)
    * [3.3. CQRS (Command Query Responsibility Segregation)](#33-cqrs-command-query-responsibility-segregation)
    * [3.4. Event Sourcing](#34-event-sourcing)
    * [3.5. Comunicação Assíncrona (RabbitMQ)](#35-comunicação-assíncrona-rabbitmq)
    * [3.6. Persistência de Dados (PostgreSQL)](#36-persistência-de-dados-postgresql)
    * [3.7. Cache e Resiliência (Redis)](#37-cache-e-resiliência-redis)
    * [3.8. Observabilidade (OpenTelemetry)](#38-observabilidade-opentelemetry)
4.  [Boas Práticas de Engenharia e Qualidade](#4-boas-práticas-de-engenharia-e-qualidade)
5.  [Estratégia de Deploy e Infraestrutura](#5-estratégia-de-deploy-e-infraestrutura)

---

## 1. Visão Geral da Arquitetura

O Flow Wise é projetado com uma arquitetura de **Microsserviços**, utilizando uma abordagem **Cloud-Native** e uma forte aderência a padrões como **Domain-Driven Design (DDD), CQRS e Event Sourcing**. Cada microsserviço (ex: Lançamentos, Consolidação) possui responsabilidades bem definidas e é construído com C#/.NET 8+, utilizando tecnologias como PostgreSQL, RabbitMQ e Redis.

## 2. Pilares Fundamentais e Objetivos

Nossas decisões arquiteturais são guiadas pelos seguintes objetivos, que são cruciais para o sucesso do Flow Wise:

* **Alto Desempenho e Escalabilidade:** Capacidade de suportar 50 requisições por segundo, com baixa latência e expansão horizontal.
* **Resiliência e Tolerância a Falhas:** Minimizar a perda de dados (máximo 5% em dias de pico) e manter a funcionalidade mesmo em caso de falhas parciais.
* **Segurança:** Proteção robusta de dados financeiros, com foco em *Secure by Design*.
* **Manutenibilidade e Evolução:** Código limpo, bem-organizado e fácil de entender/modificar para futuras funcionalidades.
* **Observabilidade:** Capacidade de monitorar e diagnosticar o sistema em tempo real.

## 3. Padrões e Decisões de Design Chave

### 3.1. Microsserviços

**Porquê:** Escolhemos a arquitetura de microsserviços para obter **flexibilidade, escalabilidade independente, resiliência e manutenibilidade**. Ao dividir o sistema em serviços menores e coesos, podemos desenvolver, implantar e escalar componentes de forma autônoma, reduzindo o acoplamento e o risco de falhas em cascata. Isso também permite a utilização de tecnologias diferentes (poliglota, embora no MVP seja C#) para cada serviço se necessário.

### 3.2. Domain-Driven Design (DDD)

**Porquê:** O DDD é fundamental para lidar com a complexidade do domínio financeiro. Ele nos permite criar um modelo de negócio rico e expressivo, com uma **linguagem ubíqua** clara, garantindo que o software reflita precisamente o negócio. A utilização de **Agregados, Entidades, Value Objects e Domain Events** ajuda a encapsular a lógica de negócio, proteger invariantes e modelar a consistência transacional dentro de Bounded Contexts.

### 3.3. CQRS (Command Query Responsibility Segregation)

**Porquê:** A separação explícita entre operações de **escrita (Comandos)** e **leitura (Consultas)** resolve o problema de otimizar a performance de sistemas com diferentes padrões de acesso. O Serviço de Lançamentos, por exemplo, foca em comandos, enquanto o Serviço de Consolidação mantém um **modelo de leitura (Read Model)** otimizado para consultas rápidas, garantindo que operações de escrita não bloqueiem as de leitura e vice-versa. Isso permite escalabilidade independente para cargas de trabalho distintas.

### 3.4. Event Sourcing

**Porquê:** Adotamos Event Sourcing (especialmente no Serviço de Lançamentos) para ter uma **fonte da verdade imutável** e completa de todas as mudanças de estado do domínio. Isso proporciona **auditabilidade** total, a capacidade de **reconstruir o estado** do sistema a qualquer ponto no tempo, e serve como base para a **projeção de dados** para os Read Models do CQRS. A persistência de eventos em vez de estados reflete de perto a natureza transacional do fluxo de caixa.

### 3.5. Comunicação Assíncrona (RabbitMQ)

**Porquê:** A comunicação entre microsserviços é predominantemente **assíncrona** via **RabbitMQ**. Isso promove o **baixo acoplamento** entre os serviços, aumenta a **resiliência** (serviços podem estar offline e processar mensagens depois) e permite a **escalabilidade independente**. A **consistência eventual** é um pilar dessa escolha, onde os dados eventualmente se propagam e se tornam consistentes em todo o sistema.

### 3.6. Persistência de Dados (PostgreSQL)

**Porquê:** Escolhemos o **PostgreSQL** como o banco de dados exclusivo devido à sua robustez, capacidade transacional, extensibilidade e ampla adoção. A decisão crucial é que **cada microsserviço possui seu próprio banco de dados (ou esquema isolado)**, garantindo o isolamento de dados e a independência dos microsserviços. Isso reforça a autonomia e evita que problemas em um serviço afetem a persistência de outro.

### 3.7. Cache e Resiliência (Redis)

**Porquê:** O **Redis** é utilizado para cenários que exigem alta performance e resiliência. Ele serve como:
* **Cache:** Para otimizar leituras frequentes em modelos de leitura.
* **Mecanismos de Resiliência:** Pode ser usado para implementar padrões como *Circuit Breaker* (evitar sobrecarga de serviços falhos) e *Rate Limiting* (controlar o número de requisições), aumentando a tolerância a falhas.

### 3.8. Observabilidade (OpenTelemetry)

**Porquê:** A observabilidade é vital em um ambiente de microsserviços distribuídos. Adotamos o **OpenTelemetry** como padrão para coletar **traces, logs e métricas**. Isso nos permite:
* **Rastreabilidade (Correlation ID):** Propagar um `Correlation ID` por toda a cadeia de requisições, facilitando a depuração em ambientes complexos.
* **Monitoramento:** Ter insights sobre o desempenho e o comportamento do sistema.
* **Integração:** Ser *plug-and-play* com diversas ferramentas de mercado (Datadog, Elastic, Dynatrace), garantindo flexibilidade na escolha de soluções de monitoramento.

## 4. Boas Práticas de Engenharia e Qualidade

Nossa arquitetura é sustentada por rigorosas boas práticas de engenharia, que garantem a saúde do codebase:

* **Princípios SOLID:** Aplicados consistentemente para promover designs de código flexíveis, coesos e de baixo acoplamento.
* **Design Patterns:** Utilização estratégica de padrões de design comprovados (ex: Repository, Strategy, etc.) para resolver problemas comuns de forma eficiente.
* **Clean Architecture:** Organização das camadas de código (Domain, Application, Infrastructure, API) com dependências bem definidas, promovendo isolamento e testabilidade.
* **Test-Driven Development (TDD):** Encorajado como prática para guiar o desenvolvimento, garantindo alta cobertura de testes e qualidade desde o início.
* **Documentação no Código (XML Comments):** Uso mandatório de comentários XML para documentar o "o quê" e o "porquê" do código, especialmente em lógica de negócio complexa, com referências a requisitos.

Para detalhes sobre essas práticas, consulte as [Diretrizes de Codificação](/standards/CODING_GUIDELINES.md) e [Diretrizes de Testes](/standards/TESTING_GUIDELINES.md).

## 5. Estratégia de Deploy e Infraestrutura

A solução Flow Wise é projetada para ambientes de nuvem, priorizando automação e escalabilidade.

* **Conteinerização (Docker/Kubernetes):** Todos os microsserviços são conteinerizados com Docker e preparados para orquestração em Kubernetes, permitindo portabilidade e escalabilidade horizontal.
* **Infraestrutura como Código (IaC - Terraform):** Utilizamos Terraform para gerenciar a infraestrutura em nuvem, garantindo repetibilidade, consistência e capacidade de **multi-cloud**.
* **Pipelines de CI/CD:** Automação completa de build, teste e deploy via GitHub Actions.

Para mais detalhes sobre a infraestrutura, consulte o documento [Infraestrutura do Projeto](/standards/INFRASTRUCTURE.md).