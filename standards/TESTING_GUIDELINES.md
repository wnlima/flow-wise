# 🧪 Diretrizes de Testes e Cobertura de Código para o Projeto Flow Wise

Este documento descreve a estratégia de testes automatizados e as diretrizes de cobertura de código para o Projeto Flow Wise. A qualidade é um pilar fundamental da nossa solução, e a automação de testes é essencial para garantir a robustez, a confiabilidade e a capacidade de evolução do sistema.

## 🎯 Filosofia de Testes

Nossa filosofia de testes é baseada na **Pirâmide de Testes**, onde a maior parte dos testes é composta por testes rápidos e baratos (testes unitários), complementados por testes de integração e, em menor proporção, por testes de aceitação/end-to-end. Este modelo nos ajuda a obter feedback rápido e construir confiança incrementalmente.

* **Rapidez:** Testes devem ser executados rapidamente para fornecer feedback contínuo aos desenvolvedores.
* **Confiabilidade:** Testes devem ser determinísticos e confiáveis, não falhando intermitentemente (flaky tests).
* **Manutenibilidade:** Testes devem ser fáceis de escrever, ler, entender e manter, evoluindo junto com o código de produção.
* **Cobertura:** Testes devem cobrir a lógica de negócio crítica, os fluxos mais importantes e as diferentes camadas da aplicação.

## 📊 Tipos de Testes e Ferramentas

Utilizamos diferentes tipos de testes para garantir a qualidade em todas as camadas da aplicação, com ferramentas específicas para cada propósito.

### 1. Testes Unitários

* **Propósito:** Validar a menor unidade de código isoladamente (ex: uma classe, um método, um *value object*). Testes unitários **não devem ter dependências externas** como banco de dados, sistemas de arquivos, APIs externas ou message brokers.
* **Foco:** Lógica de negócio pura, algoritmos, validações internas de domínio, comportamento de classes individuais.
* **Ferramentas C#/.NET:**
    * **Framework de Teste:** `xUnit` (preferencial), `NUnit` ou `MSTest`.
    * **Mocks/Fakes:** `Moq` ou `NSubstitute` (recomendado) para criar dublês de teste e isolar dependências.
* **Localização:** Projetos de teste dedicados para cada microsserviço (ex: `src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Tests.UnitTests/`).
* **Cobertura de Código Alvo:**
    * **Mínimo de 80% de cobertura de linha para a lógica de negócio crítica** nas camadas de Domínio (`.Domain/`) e Aplicação (`.Application/`).
    * Para *Value Objects* e entidades com lógica complexa, buscar cobertura próxima a 100%.

### 2. Testes de Integração

* **Propósito:** Validar a interação entre diferentes componentes internos do sistema (ex: serviço com banco de dados, serviço com message broker, camada de aplicação com camada de infraestrutura) ou com sistemas externos simulados/reais.
* **Foco:** Fluxos de dados, comunicação entre camadas e serviços, integração com a infraestrutura (persistência, cache, mensageria).
* **Ferramentas C#/.NET:**
    * O próprio framework de teste (`xUnit`) com a capacidade de inicializar partes da aplicação real.
    * Para testes de APIs ASP.NET Core, utilizar `Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory` para testar a API em memória.
    * Para interações com RabbitMQ/Redis/PostgreSQL, pode-se usar bibliotecas como `Testcontainers` ou inicializar contêineres Docker via `docker-compose` em tempo de teste para simular o ambiente real de forma controlada.
* **Localização:** Projetos de teste dedicados para cada microsserviço (ex: `src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Tests.IntegrationTests/`).

### 3. Testes de Aceitação / End-to-End (E2E)

* **Propósito:** Validar o sistema completo de ponta a ponta, simulando cenários de usuário real através das interfaces externas (APIs). Para o MVP, este tipo de teste será focado na validação via API entre os microsserviços.
* **Foco:** Fluxos completos de negócio que envolvem múltiplas APIs/serviços e suas interações, garantindo que o sistema como um todo atende aos requisitos do usuário.
* **Ferramentas:** Pode-se usar **clientes HTTP automatizados** (ex: `System.Net.Http.HttpClient`, `RestSharp`) dentro de projetos de teste dedicados. Ferramentas de BDD (Behavior-Driven Development) como **SpecFlow** podem ser consideradas para descrever cenários em linguagem natural.

### 4. Testes de Performance e Carga

* **Propósito:** Validar o atendimento aos Requisitos Não-Funcionais (NFRs) de performance e escalabilidade, como os "50 requisições por segundo para o serviço de consolidação com no máximo 5% de perda em dias de pico".
* **Foco:** Vazão (throughput), latência, tempo de resposta, uso de recursos (CPU, memória) sob diferentes níveis de carga.
* **Ferramentas:** `JMeter`, `K6`, `Locust`, `Azure Load Testing`, `AWS Load Testing` ou similares.
* **Ambiente de Execução:** Estes testes serão executados em ambientes dedicados de QA/Homologação, que simulem de perto o ambiente de produção, e não no ambiente de desenvolvimento local.

### 5. Testes de Segurança

* **Propósito:** Identificar vulnerabilidades e garantir a conformidade com as diretrizes de segurança do projeto.
* **Foco:** Prevenção de ameaças comuns (OWASP Top 10), testes de autenticação, autorização, criptografia, manipulação de dados sensíveis.
* **Ferramentas:**
    * **SAST (Static Application Security Testing):** Ferramentas que analisam o código-fonte (ex: SonarQube, Snyk Code) para identificar padrões de código vulneráveis.
    * **DAST (Dynamic Application Security Testing):** Ferramentas que testam a aplicação em execução (ex: OWASP ZAP, Burp Suite) para encontrar vulnerabilidades em tempo de execução.
    * **SCA (Software Composition Analysis):** Ferramentas para identificar vulnerabilidades em dependências de terceiros (pacotes NuGet).
    * ***Pentests* (Testes de Penetração):** Realizados por equipes de segurança internas ou parceiros especializados, geralmente em fases mais avançadas do projeto ou em produção.

## 📈 Cobertura de Código

A cobertura de código é uma métrica importante que nos ajuda a identificar áreas do código que não estão sendo testadas e a manter a qualidade.

* **Política de Cobertura:**
    * **Objetivo Mínimo:** Todos os projetos de domínio (`.Domain/`) e aplicação (`.Application/`) devem aspirar a um **mínimo de 70% de cobertura de linha** por testes unitários e de integração.
    * Para a **lógica de negócio crítica e invariantes de domínio**, a meta é um **mínimo de 80-90% de cobertura de linha**.
    * **Obrigatoriedade em CI:** A verificação de cobertura será parte integrante do nosso pipeline de Integração Contínua (CI). *Pull Requests* (PRs) que baixem a cobertura abaixo do limite estabelecido poderão ser bloqueados ou sinalizados para revisão manual até que a cobertura seja restaurada ou uma justificativa técnica seja aprovada.
* **Ferramentas C#/.NET para Cobertura:**
    * **`coverlet.collector`:** Um coletor de cobertura de código que se integra nativamente ao `dotnet test`. É a ferramenta recomendada para coletar métricas de cobertura.
        * Para instalá-lo em seu projeto de testes (ex: `FlowWise.Services.Lancamentos.Tests.UnitTests.csproj`):
            ```bash
            dotnet add [SEU_PROJETO_DE_TESTE.csproj] package coverlet.collector
            ```
    * **Relatórios de Cobertura:**
        * Após a execução dos testes com `coverlet`, relatórios podem ser gerados em diferentes formatos (Cobertura XML, LCOV, OpenCover) para serem consumidos por ferramentas de visualização e análise.
        * **Exemplo de Execução dos Testes com Cobertura:**
            ```bash
            dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
            ```
            Isso gerará um arquivo `.coverage` ou `coverage.xml` na pasta de resultados (`./TestResults`).

## 🚀 Executando Testes e Verificando Cobertura

### 1. Rodar Todos os Testes

Para executar todos os testes unitários e de integração no repositório, a partir da raiz do projeto:

```bash
dotnet test src/
```

### 2. Rodar Testes de um Projeto Específico

Para rodar testes de um projeto de teste específico (ex: testes unitários do serviço de Lançamentos), a partir da raiz do repositório:

```bash
dotnet test src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Tests.UnitTests/
```
Ou para testes de integração:
```bash
dotnet test src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Tests.IntegrationTests/
```

### 3. Rodar Testes e Gerar Relatório de Cobertura HTML Localmente

Para facilitar a análise da cobertura de código localmente com um relatório HTML amigável, utilize o *script* shell fornecido:

1.  Certifique-se de que o **ReportGenerator** esteja instalado como uma ferramenta global do .NET (se não estiver, execute: `dotnet tool install --global ReportGenerator`).
2.  Abra seu terminal na **raiz do repositório**.
3.  Execute o script:
    ```bash
    ./coverage-report.sh
    ```
4.  Após a execução, o relatório HTML detalhado estará disponível na pasta `./CoverageReport` na raiz do seu repositório. Abra o arquivo `index.htm` nesta pasta em seu navegador para visualizar.

## 🔄 Integração Contínua (CI)

Os testes automatizados (unitários e de integração) e a verificação de cobertura de código são partes integrantes do nosso pipeline de CI/CD (GitHub Actions). Qualquer *Pull Request* deve passar por esses *checks* para ser *mergeado* na *branch* `main`, garantindo que novas contribuições não introduzam regressões e mantenham os padrões de qualidade.