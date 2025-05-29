# 🤝 Como Contribuir com Código para o Flow Wise

Bem-vindo(a) ao guia de contribuição de código do Projeto Flow Wise! Agradecemos o seu interesse em colaborar conosco para construir um sistema de fluxo de caixa diário robusto, escalável e bem documentado.

Este documento detalha o fluxo de trabalho para contribuições de código, as boas práticas esperadas e os padrões de qualidade que regem o desenvolvimento no Flow Wise.

## Sumário

1.  [Processo de Contribuição de Código](#1-processo-de-contribuição-de-código)
    * [1.1. Configuração do Ambiente](#11-configuração-do-ambiente)
    * [1.2. Fluxo de Trabalho do Git](#12-fluxo-de-trabalho-do-git)
    * [1.3. Boas Práticas de Desenvolvimento](#13-boas-práticas-de-desenvolvimento)
    * [1.4. Testes](#14-testes)
    * [1.5. Documentação no Código (XML Comments)](#15-documentação-no-código-xml-comments)
    * [1.6. Observabilidade](#16-observabilidade)
    * [1.7. Padrões de Commit Messages](#17-padrões-de-commit-messages)
    * [1.8. Criando um Pull Request (PR)](#18-criando-um-pull-request-pr)
2.  [Revisão de Código](#2-revisão-de-código)
3.  [Reconhecimento da Contribuição](#3-reconhecimento-da-contribuição)

---

## 1. Processo de Contribuição de Código

Para contribuir com código para o Flow Wise, siga o fluxo de trabalho detalhado abaixo.

### 1.1. Configuração do Ambiente

Antes de iniciar o desenvolvimento, certifique-se de que seu ambiente esteja devidamente configurado. Consulte o guia [🚀 Inicie Aqui! (GET\_STARTED.md)](GET_STARTED.md) para instruções detalhadas sobre a instalação de pré-requisitos, configuração de secrets e execução local das dependências e serviços.

### 1.2. Fluxo de Trabalho do Git

Utilizamos um fluxo de trabalho baseado em *feature branches* e *pull requests* para gerenciar o desenvolvimento e garantir a estabilidade da branch `main`.

1.  **Faça um *fork* do Repositório:** Clique no botão "Fork" no GitHub para criar uma cópia do repositório em sua conta pessoal.
2.  **Clone o seu *fork*:**
    ```bash
    git clone https://github.com/SEU-USUARIO/flow-wise.git
    cd flow-wise
    ```
3.  **Adicione o repositório original como 'upstream' remoto:**
    Isso permitirá que você sincronize facilmente seu *fork* com as últimas mudanças do projeto principal.
    ```bash
    git remote add upstream https://github.com/wnlima/flow-wise.git
    ```
4.  **Mantenha sua branch `main` atualizada:**
    Sempre antes de começar a trabalhar em uma nova funcionalidade ou correção, atualize sua branch `main` local com as últimas mudanças do repositório original:
    ```bash
    git checkout main
    git pull upstream main
    ```
5.  **Crie uma nova *feature branch*:**
    Crie uma branch com um nome descritivo para sua funcionalidade ou correção. Use o formato `feature/[nome-da-feature]` para novas funcionalidades e `bugfix/[nome-da-correcao]` para correções de bugs (ex: `feature/adicionar-lancamento-recorrente`, `bugfix/correcao-saldo-negativo`).
    ```bash
    git checkout -b feature/sua-feature-ou-bugfix
    ```
6.  **Desenvolva suas mudanças.**
7.  **Faça commits atômicos e descritivos.** (Veja [Padrões de Commit Messages](#17-padrões-de-commit-messages))
8.  **Faça *push* da sua *feature branch* para o seu *fork*:**
    ```bash
    git push origin feature/sua-feature-ou-bugfix
    ```
9.  **Abra um Pull Request (PR)** (Veja [Criando um Pull Request (PR)](#18-criando-um-pull-request-pr)).

### 1.3. Boas Práticas de Desenvolvimento

Nós aderimos a um conjunto rigoroso de boas práticas para garantir a qualidade, manutenibilidade e escalabilidade do código do Flow Wise.

* **SOLID Principles:** Aplicação rigorosa dos princípios SOLID para garantir código flexível e de fácil manutenção.
* **Design Patterns:** Utilização de padrões de design apropriados (ex: Repository, Unit of Work, Mediator, Strategy) para resolver problemas comuns de forma eficaz.
* **Clean Architecture:** Organização do código em camadas claras com dependências bem definidas (Domain -> Application -> Infrastructure -> API), promovendo a separação de preocupações e a testabilidade.
* **Domain-Driven Design (DDD):** Foco no domínio de negócio, com entidades, *value objects*, agregados e eventos de domínio bem definidos para modelar a complexidade do negócio.
* **CQRS (Command Query Responsibility Segregation):** Separação clara entre comandos (operações de escrita) e queries (operações de leitura), tipicamente implementada com MediatR, otimizando o desempenho e a complexidade do modelo.
* **Event Sourcing:** Implementação de Event Sourcing (quando aplicável ao módulo) para persistência de eventos e reconstituição do estado, provendo auditabilidade e resiliência.
* **Consistência:** Mantenha a consistência com o estilo de código e as convenções já existentes no projeto.
* **Separação de Preocupações:** Cada componente deve ter uma única responsabilidade bem definida.
* **Tratamento de Erros:** Implemente um tratamento de erros robusto e consistente, retornando respostas padronizadas de API (ex: `ApiResponse` do `FlowWise.Common`).

Consulte as [🧑‍💻 Diretrizes de Codificação (CODING\_GUIDELINES.md)](CODING_GUIDELINES.md) para mais detalhes sobre os padrões de código específicos.

### 1.4. Testes

A qualidade do código é fundamental, e os testes são uma parte essencial do nosso processo de desenvolvimento.

* **Test-Driven Development (TDD):** Encorajamos fortemente a prática de TDD para novas funcionalidades e correções de bugs, escrevendo testes antes da implementação do código de produção.
* **Tipos de Testes:**
    * **Testes Unitários:** Concentram-se em unidades isoladas de código (métodos, classes, *value objects*, entidades de domínio). Devem ser rápidos, focar na lógica de negócio e não depender de infraestrutura externa.
    * **Testes de Integração:** Verificam a interação entre componentes (ex: aplicação e banco de dados, ou entre microsserviços via barramento de eventos como RabbitMQ).
* **Cobertura de Testes:** Buscamos alta cobertura de testes. Todas as novas funcionalidades e correções de bugs devem ser acompanhadas por testes relevantes que validem o comportamento esperado.
* **Executando Testes:** Para rodar todos os testes automatizados do projeto na raiz do repositório:
    ```bash
    dotnet test src/
    ```
    Para mais opções de execução e detalhes sobre a estratégia de testes, consulte o [🧪 Diretrizes de Testes (TESTING\_GUIDELINES.md)](TESTING_GUIDELINES.md).

### 1.5. Documentação no Código (XML Comments)

A documentação no código é um pilar da manutenibilidade do Flow Wise. Todo o código C# (classes, interfaces, enums, métodos, propriedades, construtores) DEVE incluir documentação XML.

* **Padrão Mandatório:** Utilize as tags XML de documentação do .NET (ex: `<summary>`, `<remarks>`, `<param>`, `<returns>`, `<example>`) conforme aplicável a cada elemento.
* **Clareza e Utilidade:** Comentários devem ser claros, concisos e agregar valor, explicando o "porquê" e o "o quê" de forma sucinta, especialmente para lógica de negócio complexa, decisões de design ou aspectos não óbvios do código. Evite comentários redundantes ou que apenas parafraseiam o nome do método/variável.
* **Referências a Requisitos:** A tag `<remarks>` DEVE ser usada para referenciar explicitamente os requisitos atendidos ou relacionados (Funcionais, Histórias de Usuário, Regras de Negócio, Não Funcionais, Decisões de Arquitetura, regras de observabilidade, etc.).

### 1.6. Observabilidade

A implementação de observabilidade é um requisito não funcional crítico para o Flow Wise, facilitando o monitoramento e a depuração em ambientes de produção.

* **Correlation ID:** Deve ser propagado de forma consistente através de todos os comandos e queries (via `BaseCommand`/`BaseQuery`) e usado para enriquecer todos os logs.
* **OpenTelemetry:** Garanta que seu código utilize o OpenTelemetry para gerar traces, logs e métricas. Isso é crucial para a interoperabilidade "plug-and-play" com ferramentas de mercado como Datadog, Elastic e Dynatrace.
* **Logs:** Utilize uma biblioteca de logging configurada (como Serilog) com *sinks* para saída estruturada, facilitando a ingestão por sistemas de log centralizados. Registre logs informativos, de debug, warning e erro apropriadamente.
* **Métricas:** Adicione métricas relevantes (contadores, *gauges*, *histograms*) para monitorar o desempenho, a latência e o comportamento da sua funcionalidade.

### 1.7. Padrões de Commit Messages

Adotamos o padrão [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) para garantir commits significativos, padronizados e que facilitem a geração automática de changelogs.

Exemplos de tipos de commit:

* `feat`: Uma nova funcionalidade (ex: `feat: adicionar endpoint de cadastro de lancamento`)
* `fix`: Uma correção de bug (ex: `fix: corrigir calculo de saldo diario`)
* `docs`: Alterações apenas na documentação (ex: `docs: atualizar guia de contribuicao`)
* `chore`: Mudanças de manutenção que não afetam a funcionalidade (ex: `chore: atualizar dependencias do nuget`)
* `refactor`: Refatoração de código sem mudança de comportamento externo (ex: `refactor: refatorar classe Lancamento para DDD`)
* `test`: Adição ou modificação de testes (ex: `test: adicionar testes unitarios para CategoriaLancamento`)
* `perf`: Melhoria de performance (ex: `perf: otimizar consulta de saldo diario`)
* `build`: Mudanças no sistema de build ou dependências externas (ex: `build: configurar pipeline de CI/CD`)
* `ci`: Mudanças nos arquivos de configuração de CI/CD (ex: `ci: adicionar linting ao pipeline`)

### 1.8. Criando um Pull Request (PR)

Quando suas mudanças estiverem prontas para revisão e você as tiver testado localmente, siga estes passos para criar um Pull Request:

1.  **Sincronize sua branch:**
    Certifique-se de que sua *feature branch* esteja atualizada com as últimas mudanças da branch `main` do repositório original para evitar conflitos.
    ```bash
    git checkout feature/sua-feature-ou-bugfix
    git pull origin feature/sua-feature-ou-bugfix
    git pull upstream main # Para integrar as últimas mudanças da main
    ```
    Resolva quaisquer conflitos de merge que possam surgir.

2.  **Faça *push* final:**
    ```bash
    git push origin feature/sua-feature-ou-bugfix
    ```

3.  **Abra um Pull Request:** Vá para a página do repositório Flow Wise no GitHub e você verá um botão para criar um novo Pull Request da sua *feature branch* para a branch `main` do repositório original.

4.  **Descrição do PR:** Preencha a descrição do Pull Request com as seguintes informações:
    * **Título Claro:** Um título conciso que resuma as mudanças, preferencialmente seguindo o padrão Conventional Commits.
    * **Descrição Detalhada:** Explique o "o quê" (quais mudanças foram feitas) e o "porquê" (a razão para as mudanças, qual problema resolve ou qual funcionalidade adiciona).
    * **Testes:** Mencione como as mudanças foram testadas (ex: "Testes unitários e de integração foram executados e passaram.", "Testado manualmente via Swagger para casos X, Y, Z.").
    * **Checklist (Opcional, mas Recomendado):** Você pode incluir um checklist simples para garantir que todos os pontos de qualidade foram abordados (ex: `[ ] Testes escritos`, `[ ] Documentação atualizada`, `[ ] Padrões de código seguidos`, `[ ] XML Comments adicionados`).

## 2. Revisão de Código

Todos os Pull Requests passarão por uma revisão de código por membros da equipe principal. O objetivo da revisão é:

* Garantir a qualidade, segurança e manutenibilidade do código.
* Compartilhar conhecimento e disseminar as boas práticas.
* Identificar bugs ou oportunidades de melhoria.
* Garantir a adesão às diretrizes arquiteturais e de código do projeto.

Esteja aberto(a) a feedback e discussões construtivas. O processo de revisão visa aprimorar o código e o conhecimento de todos.

## 3. Reconhecimento da Contribuição

Todas as contribuições são valorizadas! Se seu Pull Request for aceito e *mergeado*, seu nome será reconhecido nas notas de lançamento ou na seção de contribuidores do projeto (se houver).

Agradecemos novamente por sua colaboração e por ajudar a construir o Flow Wise!