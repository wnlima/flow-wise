# 🚀 Flow Wise: Serviço de Lançamentos

Este repositório contém o código-fonte do **Serviço de Lançamentos** do Projeto Flow Wise. Este microsserviço é um componente central para a gestão de fluxo de caixa, responsável por todo o ciclo de vida dos lançamentos financeiros (débitos e créditos) com foco em integridade e rastreabilidade.

## ✨ Visão Geral do Serviço

O **Serviço de Lançamentos** atua como a **fonte da verdade transacional** para todas as movimentações financeiras de débito e crédito no Flow Wise. Ele é um excelente exemplo da aplicação do **Domain-Driven Design (DDD)** e implementa o **lado de comando (escrita)** do padrão **CQRS (Command Query Responsibility Segregation)**.

Após cada operação de negócio bem-sucedida (criação, atualização, exclusão), eventos de domínio ricos em contexto são publicados de forma transacional para o **RabbitMQ (Message Broker)**. Isso permite que outros serviços (como o Serviço de Consolidação) reajam a essas mudanças de estado de forma assíncrona, garantindo a consistência eventual e a escalabilidade da solução.

### Responsabilidades Chave:

* **Registro de Lançamentos:** Permitir a criação de novos lançamentos de débito e crédito, validando regras de negócio complexas.
* **Consulta de Lançamentos:** Oferecer a capacidade de consultar lançamentos individuais ou por critérios específicos (ex: por período).
* **Edição e Exclusão de Lançamentos:** Gerenciar a modificação e remoção de lançamentos, seguindo as regras de negócio de auditoria, imutabilidade histórica e compensação.
* **Publicação de Eventos de Domínio:** Disparar eventos ricos em dados (`LancamentoRegistradoEvent`, `LancamentoAtualizadoEvent`, `LancamentoExcluidoEvent`) no RabbitMQ para comunicar alterações de estado para o ecossistema Flow Wise.

### Contexto no C4 Model:

* No [Diagrama de Contêineres](/docs/diagrams/C4-Container.jpg), este serviço é representado pelo `Microsserviço: Serviço de Lançamentos`.
* Ele interage diretamente com o `DB Lançamentos` (PostgreSQL) para persistência dos dados transacionais e com o `RabbitMQ` para publicação de eventos de domínio.

## 📦 Estrutura do Projeto

Este microsserviço segue uma arquitetura em camadas bem definida, aderindo aos princípios da **Clean Architecture** para separar responsabilidades e facilitar o desenvolvimento, teste e manutenção.

```
FlowWise.Services.Lancamentos/
├── FlowWise.Services.Lancamentos.Api/             # Camada de Apresentação/Comandos (Endpoints RESTful, Modelos de Requisição/Resposta, Middlewares)
├── FlowWise.Services.Lancamentos.Application/     # Camada de Aplicação (Comandos, Queries, Handlers, Validações, DTOs, Orquestração de Domínio)
├── FlowWise.Services.Lancamentos.Domain/          # Camada de Domínio (Agregados, Entidades, Value Objects, Domain Events, Interfaces de Repositório, Regras de Negócio Core)
├── FlowWise.Services.Lancamentos.Infrastructure/  # Camada de Infraestrutura (Implementações de Repositórios com EF Core, Configurações de Persistência, Integração com RabbitMQ)
└── FlowWise.Services.Lancamentos.Tests/           # Projetos de Testes (Unitários, Integração)
    ├── FlowWise.Services.Lancamentos.Tests.UnitTests/
    └── FlowWise.Services.Lancamentos.Tests.IntegrationTests/
```

## ▶️ Como Rodar Localmente

Para configurar e rodar o **Serviço de Lançamentos** em seu ambiente de desenvolvimento local, siga os passos abaixo. Certifique-se de ter o [Docker Desktop](https://www.docker.com/products/docker-desktop) e o [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalados.

1.  **Configuração do Ambiente Inicial:**
    * Primeiramente, configure seu ambiente e as dependências locais (PostgreSQL, RabbitMQ, Redis) seguindo o guia principal: [🚀 Inicie Aqui! (GET\_STARTED.md)](/standards/GET_STARTED.md). Este guia inclui a clonagem do repositório, a configuração dos `User Secrets` para o **Serviço de Lançamentos** e a inicialização dos contêineres Docker essenciais.
    * Certifique-se de que os contêineres `postgres`, `rabbitmq` e `redis` estejam rodando e `healthy` através do comando `docker compose ps` na raiz do repositório.

2.  **Aplicar Migrações do Banco de Dados:**
    * Após o PostgreSQL estar em execução, aplique as migrações do Entity Framework Core para criar o schema do banco de dados `flowwise_lancamentos_db`. A partir da raiz do repositório (`flow-wise/`), execute:
        ```bash
        dotnet ef database update \
            --project src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Infrastructure/FlowWise.Services.Lancamentos.Infrastructure.csproj \
            --startup-project src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Api/FlowWise.Services.Lancamentos.Api.csproj \
            --context LancamentosDbContext
        ```
        *Este comando garante que seu banco de dados local esteja com as tabelas mais recentes.*

3.  **Executar o Serviço:**
    * A partir da raiz do repositório (`flow-wise/`), execute o projeto API:
        ```bash
        dotnet run --project src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Api/
        ```
    * O serviço estará disponível em `http://localhost:5000` por padrão (verifique `src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Api/Properties/launchSettings.json` para a porta exata).

## 📚 APIs (Endpoints RESTful)

O **Serviço de Lançamentos** expõe uma API RESTful para gerenciar as operações de fluxo de caixa. A documentação interativa completa está disponível via **Swagger UI** quando o serviço está rodando localmente em `http://localhost:5000/swagger`.

Todas as respostas da API seguem um padrão consistente utilizando `ApiResponse` ou `ApiResponseWithData<TData>`, incluindo `Success`, `Message`, `Errors` (para validações) e `CorrelationId` para rastreabilidade.

### Endpoints Principais:

#### 1. `POST /api/lancamentos`
* **Descrição:** Registra um novo lançamento financeiro (débito ou crédito) no sistema.
* **Corpo da Requisição (`application/json`):**
    ```json
    {
      "valor": 150.75,
      "tipo": "Credito",
      "data": "2024-05-29T10:00:00Z",
      "descricao": "Venda de produto X",
      "categoria": "Vendas",
      "observacoes": "Pagamento recebido via Pix."
    }
    ```
* **Respostas Possíveis:**
    * `201 Created` (Sucesso): Retorna o ID do lançamento criado.
        ```json
        {
          "data":"a1b2c3d4-e5f6-7890-1234-567890abcdef",
          "success": true,
          "message": "Lançamento registrado com sucesso.",
          "correlationId": "some-guid-string"
        }
        ```
    * `400 Bad Request` (Erro de Validação): Se os dados da requisição forem inválidos.
    * `500 Internal Server Error` (Erro Inesperado).

#### 2. `GET /api/lancamentos/{id}`
* **Descrição:** Retorna os detalhes de um lançamento financeiro específico pelo seu identificador único.
* **Parâmetros de Rota:** `id` (GUID) - O ID do lançamento a ser consultado.
* **Respostas Possíveis:**
    * `200 OK` (Sucesso): Retorna os dados do lançamento.
        ```json
        {
          "data": {
            "id": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
            "valor": 150.75,
            "data": "2024-05-29T10:00:00Z",
            "descricao": "Venda de produto X",
            "tipo": "Credito",
            "categoria": "Vendas",
            "observacoes": "Pagamento recebido via Pix."
          },
          "success": true,
          "message": "Operação realizada com sucesso.",
          "correlationId": "some-guid-string"
        }
        ```
    * `404 Not Found`: Se nenhum lançamento for encontrado com o ID fornecido.
    * `500 Internal Server Error`.

#### 3. `GET /api/lancamentos`
* **Descrição:** Lista todos os lançamentos financeiros, com opção de filtro por data.
* **Parâmetros de Query (Opcional):**
    * `data` (string, formato `YYYY-MM-DD`): Filtra lançamentos para a data especificada.
* **Exemplos de Requisição:**
    * `GET /api/lancamentos`
    * `GET /api/lancamentos?data=2024-05-29`
* **Respostas Possíveis:**
    * `200 OK` (Sucesso): Retorna uma lista de lançamentos.
        ```json
        {
          "data": [
            {
              "id": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
              "valor": 150.75,
              "data": "2024-05-29T10:00:00Z",
              "descricao": "Venda de produto X",
              "tipo": "Credito",
              "categoria": "Vendas",
              "observacoes": "Pagamento recebido via Pix."
            },
            {
              "id": "b2c3d4e5-f6a7-8901-2345-67890abcdef1",
              "valor": 50.00,
              "data": "2024-05-29T11:30:00Z",
              "descricao": "Compra de material de escritório",
              "tipo": "Debito",
              "categoria": "Material",
              "observacoes": null
            }
          ],
          "success": true,
          "message": "Operação realizada com sucesso.",
          "correlationId": "some-guid-string"
        }
        ```
    * `400 Bad Request`: Se o formato da data for inválido.
    * `500 Internal Server Error`.

#### 4. `PUT /api/lancamentos/{id}`
* **Descrição:** Atualiza um lançamento financeiro existente. A atualização pode afetar o valor, tipo, descrição, categoria e observações.
* **Parâmetros de Rota:** `id` (GUID) - O ID do lançamento a ser atualizado.
* **Corpo da Requisição (`application/json`):**
    ```json
    {
      "valor": 160.00,
      "tipo": "Credito",
      "data": "2024-05-29T10:00:00Z",
      "descricao": "Venda de produto X (valor ajustado)",
      "categoria": "Vendas",
      "observacoes": "Valor renegociado conforme contrato."
    }
    ```
* **Respostas Possíveis:**
    * `200 OK` (Sucesso): Indica que o lançamento foi atualizado.
        ```json
        {
          "success": true,
          "message": "Lançamento atualizado com sucesso.",
          "correlationId": "some-guid-string"
        }
        ```
    * `400 Bad Request`: Se os dados da requisição forem inválidos ou houver regras de negócio violadas.
    * `404 Not Found`: Se o lançamento com o ID fornecido não existir.
    * `500 Internal Server Error`.

#### 5. `DELETE /api/lancamentos/{id}`
* **Descrição:** Exclui um lançamento financeiro do sistema.
* **Parâmetros de Rota:** `id` (GUID) - O ID do lançamento a ser excluído.
* **Respostas Possíveis:**
    * `204 No Content` (Sucesso): Indica que o lançamento foi excluído. A resposta `ApiResponse` será retornada mesmo com `204 No Content` para consistência.
        ```json
        {
          "success": true,
          "message": "Lançamento excluído com sucesso.",
          "correlationId": "some-guid-string"
        }
        ```
    * `404 Not Found`: Se o lançamento com o ID fornecido não existir.
    * `500 Internal Server Error`.

## 📬 Eventos de Domínio Publicados

Este serviço é um publicador de eventos essencial para o ecossistema Flow Wise, emitindo eventos de domínio ricos em dados via RabbitMQ para garantir a consistência eventual e permitir que outros microsserviços reajam às mudanças no fluxo de caixa. Todos os eventos herdam de `EventBase` e incluem um `CorrelationId` para rastreabilidade.

### Lista de Eventos:

* **`LancamentoRegistradoEvent`**
    * **Descrição:** Publicado imediatamente após um novo lançamento de caixa ser registrado com sucesso no sistema.
    * **Dados Contidos:** `LancamentoId` (Guid), `Valor` (decimal), `Tipo` (string), `Data` (DateTime), `CorrelationId` (string).
    * **Propósito:** Notificar outros serviços sobre a criação de um novo lançamento, permitindo que eles atualizem seus próprios modelos de leitura ou executem lógicas dependentes.

* **`LancamentoAtualizadoEvent`**
    * **Descrição:** Publicado quando um lançamento de caixa existente é modificado. Este evento é especialmente rico, pois inclui um *snapshot* do lançamento **antes** e **depois** da atualização.
    * **Dados Contidos:** `LancamentoId` (Guid), `LancamentoAntigo` (LancamentoSnapshot), `LancamentoNovo` (LancamentoSnapshot), `CorrelationId` (string).
    * **Propósito:** Essencial para a lógica de **compensação** e atualização do Read Model em sistemas eventuais. O `LancamentoAntigo` é crucial para reverter o impacto da versão anterior e aplicar o impacto da nova versão.

* **`LancamentoExcluidoEvent`**
    * **Descrição:** Publicado quando um lançamento de caixa é excluído do sistema.
    * **Dados Contidos:** `LancamentoId` (Guid), `Valor` (decimal), `Tipo` (string), `Data` (DateTime), `CorrelationId` (string).
    * **Propósito:** Utilizado por outros serviços (como o Serviço de Consolidação) para reverter ou ajustar seus próprios modelos de leitura, removendo o impacto do lançamento excluído.

## 🧪 Executando Testes

Para garantir a qualidade e a estabilidade deste microsserviço, é fundamental executar os testes automatizados regularmente.

* **Executar todos os testes deste serviço (unitários e de integração):**
    A partir da raiz do repositório (`flow-wise/`):
    ```bash
    dotnet test src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Tests/
    ```
    *Este comando executará os testes em ambos os projetos `FlowWise.Services.Lancamentos.Tests.UnitTests` e `FlowWise.Services.Lancamentos.Tests.IntegrationTests`.*

* **Executar testes de um projeto específico:**
    * Testes Unitários:
        ```bash
        dotnet test src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Tests.UnitTests/
        ```
    * Testes de Integração:
        ```bash
        dotnet test src/FlowWise.Services.Lancamentos/FlowWise.Services.Lancamentos.Tests.IntegrationTests/
        ```

* Para rodar todos os testes do repositório e gerar o relatório de cobertura de código, utilize o script na raiz do projeto: [coverage-report.sh](/coverage-report.sh).

* Para diretrizes mais detalhadas sobre a estratégia de testes e cobertura de código, consulte o documento: [🧪 Diretrizes de Testes (TESTING\_GUIDELINES.md)](/standards/TESTING_GUIDELINES.md).

## 🤝 Contribuindo

Sua contribuição é muito bem-vinda! Para entender o processo de contribuição de código, incluindo o fluxo de trabalho do Git, padrões de *commit messages* e as diretrizes de codificação e revisão de código, consulte o guia principal:

* **[🤝 Como Contribuir com Código para o Flow Wise (CONTRIBUTING\_GUIDELINES.md)](/standards/CONTRIBUTING.md)**