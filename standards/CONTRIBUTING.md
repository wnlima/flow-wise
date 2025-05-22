# Como Contribuir para o Projeto Flow Wise

Bem-vindo(a) ao repositório do Projeto Flow Wise! Valorizamos imensamente suas contribuições e estamos entusiasmados em tê-lo(a) a bordo. Este guia tem como objetivo fornecer todas as informações necessárias para que você possa contribuir de forma eficaz e alinhada com nossos padrões e fluxo de trabalho.

## 🌟 Princípios de Colaboração

* **Qualidade:** Nosso objetivo é entregar código de alta qualidade, testado e de fácil manutenção.
* **Comunicação:** Mantenha a comunicação clara e proativa sobre seu trabalho e quaisquer desafios.
* **Padrões:** Siga as diretrizes e padrões de codificação e arquitetura definidos para o projeto.
* **Foco no Valor:** Concentre-se em entregar funcionalidades que adicionem valor real ao negócio.

## ▶️ Primeiros Passos

Se você é novo(a) no projeto ou no repositório, comece lendo nosso guia de início rápido: **[Inicie Aqui! (/standards/GET_STARTED.md)](/standards/GET_STARTED.md)**. Ele aborda a configuração do ambiente, pré-requisitos e como rodar o projeto localmente.

## 🌿 Fluxo de Trabalho (Git/GitHub Flow)

Adotamos um fluxo de trabalho baseado no **GitHub Flow**, que é simples e focado na entrega contínua.

1.  **Crie uma Issue:** Para qualquer nova funcionalidade, correção de bug ou melhoria, abra uma `Issue` no GitHub. Isso ajuda a rastrear o trabalho e discutir os detalhes.
2.  **Crie um Branch:** Crie um *branch* novo a partir da *branch* `main`. Use um nome descritivo para o *branch* no formato `tipo/numero-da-issue-descricao` (ex: `feat/123-registrar-lancamento`, `fix/456-bug-relatorio`).
3.  **Desenvolva e Faça Commits:** Desenvolva sua funcionalidade ou correção. Faça *commits* pequenos, atômicos e que sigam os [Padrões de Commits Semânticos](#📝-padrões-de-commits-semânticos).
4.  **Testes Locais:** Certifique-se de que todos os testes automatizados (unitários e de integração) passem localmente antes de enviar seu código.
5.  **Atualize seu Branch:** Antes de abrir um *Pull Request (PR)*, faça um `git pull --rebase origin main` para garantir que seu *branch* esteja atualizado com as últimas mudanças da `main`. Resolva quaisquer conflitos.
6.  **Abra um Pull Request (PR):** Abra um PR para a *branch* `main`. Consulte as [Diretrizes para Pull Requests](#📥-diretrizes-para-pull-requests) abaixo.
7.  **Revisão de Código:** Seu PR será revisado por pelo menos um colega de equipe (requer 1 aprovação). Esteja aberto(a) a feedback e faça as alterações solicitadas.
8.  **Merge:** Após a aprovação e a passagem de todos os *checks* de CI/CD, seu PR será *mergeado* na *branch* `main`.

## 📝 Padrões de Commits Semânticos

Utilizamos [Commits Semânticos](https://www.conventionalcommits.org/en/v1.0.0/) para manter um histórico de *commits* limpo, legível e para automatizar a geração de *release notes*. Cada mensagem de *commit* deve seguir o formato:

```

\<tipo\>(\<escopo opcional\>): \<descrição\>

[corpo opcional]

[rodapé opcional]

```

### Tipos Comuns:

* **`feat`**: Uma nova funcionalidade (correlaciona-se com `MINOR` no versionamento semântico).
* **`fix`**: Uma correção de bug (correlaciona-se com `PATCH` no versionamento semântico).
* **`docs`**: Alterações apenas na documentação.
* **`style`**: Mudanças que não afetam o significado do código (espaços em branco, formatação, ponto e vírgula ausente, etc.).
* **`refactor`**: Uma mudança de código que não corrige um bug nem adiciona uma funcionalidade.
* **`perf`**: Uma mudança de código que melhora o desempenho.
* **`test`**: Adição de testes ausentes ou correção de testes existentes.
* **`build`**: Alterações que afetam o sistema de *build* ou dependências externas (npm, nuget, docker, etc.).
* **`ci`**: Alterações nos arquivos e *scripts* de CI/CD.
* **`chore`**: Outras mudanças que não modificam o código-fonte ou os arquivos de teste.

### Escopo (Opcional):

O escopo é um nome curto que descreve o local da alteração (ex: `lancamentos-api`, `consolidacao-svc`, `docs`, `infra`).

### Descrição:

A descrição deve ser concisa (máximo 50-70 caracteres), imperativa (ex: "adiciona", "corrige", "refatora") e começar com letra minúscula.

### Exemplos:

* `feat(lancamentos-api): adiciona endpoint para registrar lancamento de debito`
* `fix(consolidacao-svc): corrige calculo do saldo diario para lancamentos noturnos`
* `docs: atualiza guia de contribuicao`
* `chore(deps): atualiza nuget package X para versao 1.2.0`
* `ci: configura pipeline de build para novo servico`

## 📥 Diretrizes para Pull Requests

Ao abrir um *Pull Request*, por favor, siga estas diretrizes:

* **Título:** Use o mesmo formato de [Commit Semântico](#📝-padrões-de-commits-semânticos) para o título do PR (ex: `feat(lancamentos): implementa registro de lancamentos`).
* **Descrição:**
    * Forneça um resumo claro das mudanças.
    * Explique o *porquê* das mudanças.
    * Liste os requisitos ou *issues* que o PR resolve (ex: `Closes #123`).
    * Descreva como testar as mudanças.
* **Revisores:** Atribua pelo menos um revisor da equipe.
* **Checks de CI/CD:** Garanta que todos os *checks* do GitHub Actions (build, testes, análise estática) passem com sucesso antes de solicitar a revisão ou o *merge*.

## ✍️ Padrões de Codificação e Design

Nossa base de código segue as [Diretrizes de Codificação C#/.NET](/standards/CODING_GUIDELINES.md) e aplica padrões de arquitetura como DDD, CQRS e princípios SOLID.

## 🧪 Estratégia de Testes

Garantimos a qualidade do nosso código através de uma estratégia robusta de testes. Para mais detalhes sobre como escrever e rodar testes, e nossa política de cobertura, consulte as **Diretrizes de Testes (/standards/TESTING_GUIDELINES.md)**.

## 🔒 Gerenciamento de Segredos e Segurança

A segurança é primordial. Para informações sobre gerenciamento de dados sensíveis e práticas de desenvolvimento seguro, consulte a seção relevante em nossas [Diretrizes de Codificação](/standards/CODING_GUIDELINES.md).