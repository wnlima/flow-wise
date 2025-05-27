# 🧪 Diretrizes de Testes e Cobertura de Código para o Projeto Flow Wise

Este documento descreve a estratégia de testes automatizados e as diretrizes de cobertura de código para o Projeto Flow Wise. A qualidade é um pilar fundamental da nossa solução, e a automação de testes é essencial para garantir a robustez, a confiabilidade e a capacidade de evolução do sistema.

## 🎯 Filosofia de Testes

Nossa filosofia de testes é baseada na **Pirâmide de Testes**, onde a maior parte dos testes é composta por testes rápidos e baratos (testes unitários), complementados por testes de integração e, em menor proporção, por testes de aceitação/end-to-end.

* **Rapidez:** Testes devem ser executados rapidamente para fornecer feedback contínuo.
* **Confiabilidade:** Testes devem ser determinísticos e confiáveis, não falhando intermitentemente.
* **Manutenibilidade:** Testes devem ser fáceis de escrever, ler e manter.
* **Cobertura:** Testes devem cobrir a lógica de negócio crítica e os fluxos mais importantes.

## 📊 Tipos de Testes e Ferramentas

Utilizamos diferentes tipos de testes para garantir a qualidade em todas as camadas da aplicação.

### 1. Testes Unitários

* **Propósito:** Validar a menor unidade de código isoladamente (ex: uma classe, um método). Não devem ter dependências externas (banco de dados, sistemas externos).
* **Foco:** Lógica de negócio, algoritmos, validações.
* **Ferramentas C#/.NET:**
    * **Framework de Teste:** `xUnit` (preferencial), `NUnit` ou `MSTest`.
    * **Mocks/Fakes:** `Moq` / `NSubsitute` (recomendado) para isolar dependências.
* **Localização:** Projetos de teste específicos (ex: `FlowWise.Services.Lancamentos.Tests/UnitTests/`).
* **Cobertura de Código Alvo:** **Mínimo de 25% de cobertura de linha para a lógica de negócio crítica** na camada de Domínio e Aplicação.

### 2. Testes de Integração

* **Propósito:** Validar a interação entre diferentes componentes internos do sistema (ex: serviço com banco de dados, serviço com message broker) ou com sistemas externos simulados.
* **Foco:** Fluxos de dados, comunicação entre camadas e serviços, integração com infraestrutura.
* **Ferramentas C#/.NET:**
    * O próprio framework de teste (`xUnit`) com a capacidade de inicializar partes da aplicação real ou usar **testes de integração baseados em `WebApplicationFactory`** para APIs ASP.NET Core.
    * Para RabbitMQ/Redis, pode-se usar instâncias em contêineres Docker (via `Testcontainers` ou `docker-compose` em tempo de teste) para simular o ambiente real.
* **Localização:** Projetos de teste específicos (ex: `FlowWise.Services.Lancamentos.Tests/IntegrationTests/`).

### 3. Testes de Aceitação / End-to-End (E2E)

* **Propósito:** Validar o sistema completo de ponta a ponta, simulando cenários de usuário real. Para o MVP, este tipo de teste será mais focado na validação via API.
* **Foco:** Fluxos completos de negócio através de múltiplas APIs/serviços.
* **Ferramentas:** Pode-se usar **clientes HTTP automatizados** (ex: RestSharp, HttpClient) dentro de projetos de teste dedicados ou ferramentas como **SpecFlow** para BDD.

### 4. Testes de Performance e Carga

* **Propósito:** Validar o atendimento aos Requisitos Não-Funcionais (NFRs) de performance e escalabilidade, como os 50 requisições/segundo para o serviço de consolidação.
* **Foco:** Vazão, latência, uso de recursos sob carga.
* **Ferramentas:** `JMeter`, `K6`, `Azure Load Testing` ou similares. Estes testes serão executados em ambientes dedicados (QA/Homologação), não no ambiente de desenvolvimento local.

### 5. Testes de Segurança

* **Propósito:** Identificar vulnerabilidades e garantir a conformidade com as diretrizes de segurança.
* **Foco:** Prevenção de OWASP Top 10, autenticação, autorização, criptografia.
* **Ferramentas:** SAST (Static Application Security Testing - ex: SonarQube), DAST (Dynamic Application Security Testing), e *pentests* (realizados pelo time de segurança ou parceiros, pós-MVP).

## 📈 Cobertura de Código

A cobertura de código é uma métrica importante que nos ajuda a identificar áreas do código que não estão sendo testadas.

* **Política de Cobertura:**
    * **Objetivo Mínimo:** Todos os projetos de domínio (`.Domain/`) e aplicação (`.Application/`) devem aspirar a um **mínimo de 25% de cobertura de linha** por testes unitários.
    * **Obrigatoriedade em CI:** A verificação de cobertura será parte do nosso pipeline de Integração Contínua (CI). PRs que baixem a cobertura abaixo do limite estabelecido poderão ser bloqueados até que a cobertura seja restaurada ou justificada.
* **Ferramentas C#/.NET para Cobertura:**
    * **`coverlet.collector`:** Um coletor de cobertura de código que se integra ao `dotnet test`. É a ferramenta recomendada para coletar métricas de cobertura.
        * Para instalá-lo em seu projeto de testes:
            ```bash
            dotnet add package coverlet.collector
            ```
    * **Relatórios de Cobertura:**
        * Após a execução dos testes com `coverlet`, relatórios podem ser gerados em diferentes formatos (Cobertura XML, LCOV, OpenCover) para serem consumidos por ferramentas de visualização (ex: ReportGenerator, SonarQube).
        * **Exemplo de Execução com Cobertura:**
            ```bash
            dotnet test --collect:"XPlat Code Coverage"
            ```
            Isso gerará um arquivo `.coverage` ou `coverage.xml` na pasta de resultados.

## 🚀 Executando Testes e Verificando Cobertura

### 1. Rodar Todos os Testes

Para executar todos os testes unitários e de integração no repositório:

```bash
dotnet test src/
```

### 2. Rodar Testes de um Projeto Específico

Para rodar testes de um projeto específico (ex: testes de Lançamentos):

```bash
dotnet test src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Tests/
```

### 3. Rodar Testes e Gerar Relatório de Cobertura

Para facilitar a análise da cobertura de código localmente, disponibilizamos um *script* shell que automatiza a execução dos testes e a geração do relatório HTML.

#### **Executando o Script de Cobertura Local**

1.  Certifique-se de que o **ReportGenerator** esteja instalado globalmente (se não estiver, execute: `dotnet tool install --global ReportGenerator`).
2.  Abra seu terminal na raiz do repositório.
3.  Execute o script:
    ```bash
    ./coverage-report.sh
    ```
4.  Após a execução, o relatório HTML estará disponível na pasta `./CoverageReport` na raiz do seu repositório. Abra o arquivo `index.htm` nesta pasta em seu navegador para visualizar.

## 🔄 Integração Contínua (CI)

Os testes automatizados e a verificação de cobertura de código são partes integrantes do nosso pipeline de CI/CD (GitHub Actions). Qualquer *Pull Request* deve passar por esses *checks* para ser *mergeado* na *branch* `main`.