# 🧑‍💻 Diretrizes de Codificação C#/.NET para o Projeto Flow Wise

Este documento estabelece as diretrizes e padrões de codificação para o desenvolvimento de todos os microsserviços do Projeto Flow Wise em C#/.NET 8+. Aderir a estas diretrizes é crucial para garantir a consistência, manutenibilidade, escalabilidade e segurança do nosso codebase.

## 🎯 Princípios Fundamentais

Nossas diretrizes de codificação são baseadas nos seguintes princípios:

* **Clareza e Legibilidade:** O código deve ser fácil de ler, entender e depurar por qualquer desenvolvedor da equipe.
* **Simplicidade:** Prefira soluções simples e diretas em vez de abordagens excessivamente complexas ou super-engenheiradas.
* **Manutenibilidade:** O código deve ser fácil de modificar, depurar e estender no futuro, minimizando o impacto de alterações.
* **Testabilidade:** O design do código deve facilitar a escrita e execução de testes automatizados (unitários, integração), promovendo a validação contínua.
* **Segurança:** A segurança deve ser uma preocupação inerente desde a fase de design (Secure by Design) até a implementação e operação.
* **Performance:** Otimize o código onde o desempenho for criticamente importante, mas sempre buscando um equilíbrio com a legibilidade e manutenibilidade.

## 🧱 Padrões de Design e Arquitetura

O Flow Wise adota padrões arquiteturais e de design específicos para garantir a robustez, escalabilidade e alinhamento com o domínio de negócio dos microsserviços:

* **Domain-Driven Design (DDD):**
    * Foco na modelagem de um domínio de negócio rico, com a **linguagem ubíqua** bem definida e refletida no código.
    * Uso de **Agregados, Entidades, Value Objects e Domain Events** para encapsular e proteger a lógica de negócio, garantir a consistência e a rastreabilidade das operações.
    * Separação clara entre as camadas de domínio, aplicação, infraestrutura e apresentação para isolar preocupações.
* **CQRS (Command Query Responsibility Segregation):**
    * Separação explícita entre operações de **Comando (escrita)** e **Consulta (leitura)**.
    * No serviço de Lançamentos, o foco é predominantemente em Comandos e no registro de eventos. No serviço de Consolidação, o foco é em Consultas e na projeção de dados para modelos de leitura eficientes.
    * Uso de **MediatR** ou padrão similar para despachar comandos e consultas, promovendo um design mais limpo, desacoplado e testável.
* **Event Sourcing:**
    * Para os domínios onde a auditabilidade completa e a capacidade de reconstrução do estado são críticas (ex: Lançamentos), as mudanças de estado serão armazenadas como uma sequência imutável de eventos de domínio. Isso complementa o CQRS, fornecendo uma fonte de verdade para o domínio e para a geração de projeções.
* **Princípios SOLID:**
    * **Single Responsibility Principle (SRP):** Cada classe, módulo ou componente deve ter uma única razão para mudar.
    * **Open/Closed Principle (OCP):** Entidades de software (classes, módulos, funções, etc.) devem ser abertas para extensão, mas fechadas para modificação.
    * **Liskov Substitution Principle (LSP):** Objetos de uma superclasse devem ser substituíveis por objetos de suas subclasses sem afetar a corretude do programa.
    * **Interface Segregation Principle (ISP):** Clientes não devem ser forçados a depender de interfaces que não utilizam.
    * **Dependency Inversion Principle (DIP):** Módulos de alto nível não devem depender de módulos de baixo nível; ambos devem depender de abstrações.

## 🗃️ Estrutura do Projeto (Monorepo)

Cada microsserviço no diretório `src/` seguirá uma estrutura de projetos modular para separar responsabilidades e facilitar o desenvolvimento e a compreensão:

```
src/
└── FlowWise.Services.NomeDoServico/
    ├── FlowWise.Services.NomeDoServico.Api/             # Camada de Apresentação/Comandos (API RESTful)
    ├── FlowWise.Services.NomeDoServico.Application/     # Camada de Aplicação (Orquestra o domínio, DTOs, Command/Query Handlers, Validações)
    ├── FlowWise.Services.NomeDoServico.Domain/          # Camada de Domínio (Entidades, Value Objects, Agregados, Domain Events, Interfaces de Repositório)
    ├── FlowWise.Services.NomeDoServico.Infrastructure/  # Camada de Infraestrutura (Implementações de Repositórios, Persistência com EF Core, Integrações Externas)
    └── FlowWise.Services.NomeDoServico.Tests/           # Projetos de Testes (Unitários, Integração para cada camada ou funcionalidade)
```

## 📝 Convenções de Nomenclatura e Estilo

A consistência é chave para um codebase limpo e compreensível.

* **Nomenclatura:**
    * **PascalCase:** Para classes, interfaces, enums, propriedades públicas, métodos públicos e *namespaces*.
    * **camelCase:** Para variáveis locais, parâmetros de método e campos privados.
    * **Prefixo 'I' para Interfaces:** Todas as interfaces devem começar com o prefixo 'I' (ex: `ILancamentoRepository`, `IDomainEventPublisher`).
* **Espaços em Branco:** Use 4 espaços para indentação (não tabs).
* **Linhas em Branco:** Use linhas em branco para separar blocos lógicos de código, métodos e propriedades para melhorar a legibilidade.
* **Chaves (`{}`):** As chaves de abertura (`{`) devem estar na mesma linha da declaração que as precede (estilo K&R/Egyptian Brace Style), separadas por um espaço.
    ```csharp
    public class Exemplo
    {
        public void MetodoExemplo()
        {
            // código
        }
    }
    ```
* **Uso de `var`:** Prefira `var` quando o tipo da variável for óbvio pela inicialização, melhorando a concisão.
    ```csharp
    var lancamento = new Lancamento(...); // Tipo óbvio
    List<string> nomes = new List<string>(); // Tipo não óbvio, especifique
    ```
* **Null-Conditional Operator (`?.`):** Use para acesso seguro a membros e métodos de objetos que podem ser nulos, evitando `NullReferenceException`.
    ```csharp
    string nome = usuario?.Endereco?.Rua;
    ```
* **Null-Coalescing Operator (`??`):** Use para atribuir um valor padrão se uma expressão for nula.
    ```csharp
    string descricao = lancamento.Descricao ?? "Sem descrição";
    ```
* **Strings Interpoladas:** Prefira strings interpoladas (`$""`) em vez de `string.Format()` ou concatenação de strings, pois são mais legíveis e eficientes.
    ```csharp
    logger.LogInformation($"Lancamento {lancamentoId} criado com sucesso.");
    ```
* **Uso de LINQ:** Prefira a sintaxe de método de LINQ para consultas, a menos que a sintaxe de query seja significativamente mais legível para um caso específico e complexo.

## 🛠️ Boas Práticas de Desenvolvimento

Estas práticas garantem um código de alta qualidade e alinhado com a robustez do Flow Wise.

* **Injeção de Dependência (DI):** Utilize o contêiner de DI nativo do .NET Core para gerenciar dependências e promover o baixo acoplamento. Injete interfaces, não implementações concretas.
* **Tratamento de Exceções:**
    * Evite "engolir" exceções silenciosamente. Capture exceções específicas e registre-as com informações relevantes.
    * Utilize middlewares para tratamento global de exceções em APIs, mapeando-as para respostas HTTP padronizadas e significativas (ex: HTTP 400 Bad Request para validações, 404 Not Found para recursos inexistentes, 500 Internal Server Error para erros inesperados).
    * Nunca exponha detalhes internos de exceções ou *stack traces* diretamente ao cliente em ambientes de produção.
* **Logging:**
    * Utilize um framework de logging configurado (como Serilog) e injete `ILogger<T>` nas classes onde o logging é necessário.
    * Configure logs **estruturados** (JSON) para facilitar a análise e a busca em sistemas de observabilidade (ex: Datadog, Elastic).
    * Registre logs em níveis apropriados (Information, Warning, Error, Debug, Trace).
    * **Evite registrar dados sensíveis** (senhas, PII, credenciais) em logs. Utilize mascaramento ou anonimização quando necessário.
* **Validações:**
    * Implemente validações rigorosas em todas as entradas, tanto a nível de API/Application (usando bibliotecas como FluentValidation para Commands e Queries) quanto a nível de Domínio (garantindo invariantes de entidades e *value objects*).
    * As validações devem ser claras e retornar mensagens de erro significativas.
* **Assincronicidade (`async`/`await`):**
    * Utilize `async` e `await` para todas as operações I/O-bound (banco de dados, chamadas HTTP, acesso a filas/cache) para melhorar a escalabilidade e a responsividade da aplicação.
    * Sempre que possível e apropriado (especialmente em bibliotecas que não interagem diretamente com contextos de UI/HTTP), utilize `.ConfigureAwait(false)` para evitar *deadlocks* e melhorar o desempenho.
* **Imutabilidade:**
    * Onde apropriado, use tipos imutáveis (ex: *record types* no C# 9+, propriedades com `init` setters) para Value Objects e DTOs. Isso reduz a complexidade, melhora a segurança de thread e ajuda a prevenir efeitos colaterais indesejados.
* **Extensões de Método:** Use extensões de método de forma consciente para adicionar funcionalidade a tipos existentes, mas evite abusar para não poluir os *namespaces*.

## 🔒 Gerenciamento de Dados Sensíveis e Segurança

A segurança é primordial no Flow Wise, especialmente ao lidar com dados financeiros e infraestrutura de produção.

* **Dados Sensíveis no Código:** **Jamais** armazene credenciais, chaves de API, tokens, strings de conexão, ou quaisquer dados sensíveis diretamente no código-fonte, em arquivos de configuração versionados (ex: `appsettings.json`) ou em variáveis de ambiente em texto puro em ambientes de produção.
* **Desenvolvimento Local (User Secrets):** Para o ambiente de desenvolvimento local e POC, utilize o **.NET User Secrets** para gerenciar dados sensíveis de forma segura, conforme descrito no [GET\_STARTED.md](GET_STARTED.md).
* **Ambientes de Produção (Secrets Management):** Em ambientes de QA, Homologação e Produção, é **mandatório** o uso de soluções de *Secrets Management* da nuvem (ex: Azure Key Vault, AWS Secrets Manager, HashiCorp Vault). O acesso a esses segredos deve ser feito via Identity/Role-Based Access Control (RBAC) e não via chaves estáticas.
* **Validação e Sanitização de Entradas:** Todas as entradas de usuário e dados provenientes de sistemas externos devem ser rigorosamente validadas e sanitizadas para prevenir ataques comuns como SQL Injection, XSS (Cross-Site Scripting), Command Injection e outros ataques de injeção.
* **Autenticação e Autorização:**
    * Utilize os mecanismos de autenticação e autorização fornecidos pelo .NET Core (JWT para APIs, integração com SSO corporativo no pós-MVP).
    * Implemente **Autorização Baseada em Papéis (RBAC)** ou *Claims-Based Authorization* para controlar o acesso granular a recursos e funcionalidades específicas.
    * Aplique os atributos de autorização (`[Authorize]`, `[AllowAnonymous]`) e políticas corretamente.
* **Criptografia:**
    * Garanta que todos os dados sensíveis sejam criptografados **em trânsito** (usando HTTPS/TLS 1.2+ para APIs e comunicação entre serviços, e TLS para RabbitMQ e PostgreSQL) e **em repouso** (criptografia de banco de dados e armazenamento).
* **Logging Seguro:** Reforçando o ponto de logging, **nunca** registre dados sensíveis (senhas, PII, números de cartão de crédito, informações de saúde) em logs. Utilize técnicas de mascaramento, truncamento ou anonimização quando o logging for essencial e contiver tais dados.
* **Dependências Seguras:** Mantenha as dependências (pacotes NuGet) atualizadas para as últimas versões estáveis, mitigando vulnerabilidades conhecidas. Utilize ferramentas de Análise de Composição de Software (SCA) e SAST (Static Application Security Testing) para verificar vulnerabilidades em dependências e no código.
* **Tratamento de Erros e Exceções (Segurança):** Não exponha mensagens de erro detalhadas ou *stack traces* em ambientes de produção que possam revelar informações sensíveis sobre a infraestrutura, o código ou a lógica de negócio interna. Mensagens de erro para o cliente devem ser genéricas e seguras.

## 🧪 Estratégia de Testes

Para informações detalhadas sobre nossa abordagem de testes, incluindo testes unitários, de integração, de performance e de segurança, bem como a política de cobertura de código, consulte o documento específico:

* **[🧪 Diretrizes de Testes (TESTING\_GUIDELINES.md)](TESTING_GUIDELINES.md)**

## 🤝 Colaboração e Revisão de Código

Para entender o processo de contribuição de código, incluindo o fluxo de trabalho do Git, padrões de *commit messages* e o processo de Pull Request (PR) e revisão de código, consulte o documento:

* **[🤝 Como Contribuir com Código para o Flow Wise (CONTRIBUTING\_GUIDELINES.md)](CONTRIBUTING.md)**