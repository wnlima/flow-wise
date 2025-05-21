# 🧑‍💻 Diretrizes de Codificação C#/.NET para o Projeto Flow Wise

Este documento estabelece as diretrizes e padrões de codificação para o desenvolvimento de todos os microsserviços do Projeto Flow Wise em C#/.NET 8+. Aderir a estas diretrizes é crucial para garantir a consistência, manutenibilidade, escalabilidade e segurança do nosso codebase.

## 🎯 Princípios Fundamentais

Nossas diretrizes de codificação são baseadas nos seguintes princípios:

* **Clareza e Legibilidade:** O código deve ser fácil de ler e entender por qualquer desenvolvedor da equipe.
* **Simplicidade:** Prefira soluções simples e diretas em vez de complexas e super-engenheiradas.
* **Manutenibilidade:** O código deve ser fácil de modificar, depurar e estender no futuro.
* **Testabilidade:** O design do código deve facilitar a escrita de testes automatizados (unitários, integração).
* **Segurança:** Segurança deve ser uma preocupação desde o design até a implementação (Secure by Design).
* **Performance:** Otimize o código onde o desempenho for crítico, sem comprometer a legibilidade ou a manutenibilidade excessivamente.

## 🧱 Padrões de Design e Arquitetura

O Flow Wise adota padrões arquiteturais e de design específicos para garantir a robustez e a escalabilidade dos microsserviços:

* **Domain-Driven Design (DDD):**
    * Foco na modelagem de um domínio de negócio rico, com a linguagem ubíqua em mente.
    * Uso de **Agregados, Entidades, Value Objects e Domain Events** para encapsular e proteger a lógica de negócio.
    * Separação clara entre o domínio, aplicação, infraestrutura e apresentação.
* **CQRS (Command Query Responsibility Segregation):**
    * Separação explícita entre operações de **Comando (escrita)** e **Consulta (leitura)**.
    * No serviço de Lançamentos, o foco é em Comandos. No serviço de Consolidação, o foco é em Consultas e projeções.
    * Uso de **MediatR** ou similar para despachar comandos e consultas, promovendo um design mais limpo e testável.
* **Event Sourcing:**
    * Para os domínios onde a auditabilidade e a reconstrução do estado são críticas (ex: Lançamentos), as mudanças de estado serão armazenadas como uma sequência imutável de eventos. Isso complementa o CQRS.
* **Princípios SOLID:**
    * **Single Responsibility Principle (SRP):** Cada classe/módulo deve ter uma única razão para mudar.
    * **Open/Closed Principle (OCP):** Entidades de software devem ser abertas para extensão, mas fechadas para modificação.
    * **Liskov Substitution Principle (LSP):** Objetos de uma superclasse devem ser substituíveis por objetos de suas subclasses sem afetar a corretude do programa.
    * **Interface Segregation Principle (ISP):** Clientes não devem ser forçados a depender de interfaces que não utilizam.
    * **Dependency Inversion Principle (DIP):** Módulos de alto nível não devem depender de módulos de baixo nível; ambos devem depender de abstrações.

## 🗃️ Estrutura do Projeto (Monorepo)

Cada microsserviço no diretório `src/` seguirá uma estrutura de projetos modular para separar responsabilidades e facilitar o desenvolvimento:

```
src/
└── FlowWise.Services.NomeDoServico/
    ├── FlowWise.Services.NomeDoServico.Api/             # Camada de Apresentação/Comandos (API REST)
    ├── FlowWise.Services.NomeDoServico.Application/     # Camada de Aplicação (orquestra o domínio, DTOs, Handlers)
    ├── FlowWise.Services.NomeDoServico.Domain/          # Camada de Domínio (Entidades, Value Objects, Agregados, Domain Events)
    ├── FlowWise.Services.NomeDoServico.Infrastructure/  # Camada de Infraestrutura (Persistência, Integrações Externas)
    └── FlowWise.Services.NomeDoServico.Tests/           # Projetos de Testes (Unitários, Integração)
```

## 📝 Convenções de Nomenclatura e Estilo

* **Nomenclatura:**
    * **PascalCase:** Para classes, interfaces, enums, propriedades públicas, métodos públicos e *namespaces*.
    * **camelCase:** Para variáveis locais, parâmetros de método e campos privados.
    * **Prefixo 'I' para Interfaces:** (ex: `ILancamentoRepository`).
* **Espaços em Branco:** 4 espaços para indentação (não tabs).
* **Linhas em Branco:** Use linhas em branco para separar blocos lógicos de código e melhorar a legibilidade.
* **Chaves (`{}`):** Iniciar na mesma linha da declaração (estilo Allman/K&R pode ser aceitável, mas padronizar).
* **Uso de `var`:** Prefira `var` quando o tipo for óbvio pela inicialização.
* **Null-Conditional Operator (`?.`):** Use para acesso seguro a membros e métodos (ex: `myObject?.MyProperty`).
* **Null-Coalescing Operator (`??`):** Use para atribuir um valor padrão se uma expressão for nula (ex: `result ?? defaultValue`).
* **Strings Interpoladas:** Prefira strings interpoladas (`$""`) em vez de `string.Format()` ou concatenação.
* **Uso de LINQ:** Prefira a sintaxe de método de LINQ para consultas, a menos que a sintaxe de query seja mais legível para um caso específico.

## 🛠️ Boas Práticas de Desenvolvimento

* **Injeção de Dependência (DI):** Use o contêiner de DI nativo do .NET Core ou um de terceiros (ex: Autofac, Lamar) para gerenciar dependências.
* **Tratamento de Exceções:**
    * Evite "engolir" exceções. Capture exceções específicas e registre-as.
    * Utilize middlewares para tratamento global de exceções em APIs.
    * Nunca retorne exceções brutas para o cliente; mapeie-as para respostas de erro significativas (ex: HTTP 400, 404, 500).
* **Logging:**
    * Utilize um framework de logging (ex: Serilog, NLog) e injete `ILogger<T>` nas classes.
    * Configure logs estruturados (JSON) para facilitar a análise em sistemas de observabilidade.
    * Evite registrar dados sensíveis em logs.
* **Validações:** Implemente validações rigorosas em todas as entradas (APIs, comandos), tanto a nível de domínio quanto de aplicação, utilizando bibliotecas como FluentValidation.
* **Assincronicidade (`async`/`await`):** Use `async` e `await` para operações I/O-bound para melhorar a escalabilidade e a responsividade. Sempre que possível, utilize `ConfigureAwait(false)`.
* **Imutabilidade:** Onde apropriado, use tipos imutáveis (ex: *record types*, `init` setters) para Value Objects e DTOs para reduzir a complexidade e bugs.

## 🔒 Gerenciamento de Dados Sensíveis e Segurança

A segurança é primordial no Flow Wise, especialmente com dados financeiros.

* **Dados Sensíveis no Código:** **Jamais** armazene credenciais, chaves de API, tokens ou quaisquer dados sensíveis diretamente no código-fonte ou em arquivos versionados (ex: `appsettings.json`).
* **Desenvolvimento Local (User Secrets):** Para o ambiente de desenvolvimento local e POC, utilize o **.NET User Secrets** para gerenciar dados sensíveis, conforme descrito no [GET_STARTED.md](/standards/GET_STARTED.md).
* **Ambientes de Produção (Secrets Management):** Em ambientes de QA, Homologação e Produção, é **mandatório** o uso de soluções de *Secrets Management* da nuvem (ex: Azure Key Vault, AWS Secrets Manager, HashiCorp Vault). O acesso a esses segredos deve ser feito via Identity/Role-Based Access Control (RBAC) e não via chaves estáticas.
* **Validação e Sanitização de Entradas:** Todas as entradas de usuário e dados de sistemas externos devem ser rigorosamente validadas e sanitizadas para prevenir ataques como SQL Injection, XSS (Cross-Site Scripting) e outros ataques de injeção.
* **Autenticação e Autorização:**
    * Utilize os mecanismos de autenticação e autorização do .NET Core (JWT, IdentityServer, ASP.NET Core Identity, integração com SSO corporativo no pós-MVP).
    * Implemente **Autorização Baseada em Papéis (RBAC)** ou *Claims-Based Authorization* para controlar o acesso a recursos e funcionalidades específicas.
* **Criptografia:**
    * Garanta que todos os dados sensíveis sejam criptografados **em trânsito** (usando HTTPS/TLS 1.2+ para APIs e comunicação entre serviços) e **em repouso** (criptografia de banco de dados e armazenamento).
* **Logging Seguro:** Evite registrar dados sensíveis (senhas, PII, números de cartão de crédito) em logs. Utilize técnicas de mascaramento ou anonimização quando necessário.
* **Dependências Seguras:** Mantenha as dependências (pacotes NuGet) atualizadas para as últimas versões, mitigando vulnerabilidades conhecidas. Utilize ferramentas de Análise de Composição de Software (SCA) para verificar vulnerabilidades em dependências.
* **Tratamento de Erros e Exceções:** Não exponha mensagens de erro detalhadas ou *stack traces* em ambientes de produção que possam revelar informações sensíveis sobre a infraestrutura ou o código.

## 🧪 Estratégia de Testes

Consulte o documento **Diretrizes de Testes (/standards/TESTING_GUIDELINES.md)** para informações detalhadas sobre nossa abordagem de testes, incluindo testes unitários, de integração, de performance e de segurança, bem como a política de cobertura de código.

## 🤝 Colaboração e Revisão de Código

* Todo código deve passar por revisão por pares via Pull Requests (PRs).
* Seja construtivo(a) ao dar feedback e aberto(a) a recebê-lo.
* Siga os *Padrões de Commits Semânticos* descrito no CONTRIBUTING.md para manter o histórico de *commits* limpo e significativo.