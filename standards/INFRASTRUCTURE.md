# ⚙️ Configuração e Gerenciamento da Infraestrutura do Projeto Flow Wise

Este documento descreve a estratégia e os padrões para a configuração e o gerenciamento da infraestrutura do Projeto Flow Wise, com foco em Infraestrutura como Código (IaC), conteinerização, orquestração e a visão de preparação para múltiplos ambientes de nuvem.

## 🚀 Visão Geral da Estratégia de Infraestrutura

A infraestrutura do Flow Wise será tratada como código, garantindo automação, reprodutibilidade, consistência e auditabilidade. A abordagem modular e o uso de ferramentas agnósticas à nuvem permitirão flexibilidade e resiliência em nosso ambiente operacional.

## 🐳 Conteinerização e Orquestração (Docker e Kubernetes)

Todos os microsserviços do Flow Wise serão conteinerizados, o que nos proporciona portabilidade, isolamento de dependências e um ambiente de execução consistente desde o desenvolvimento local até a produção.

* **Docker:** Cada microsserviço (Serviço de Lançamentos, Serviço de Consolidação) será empacotado como uma imagem Docker, incluindo todas as suas dependências.
* **Kubernetes (K8s):** Será a plataforma de orquestração de contêineres padrão para implantações em ambientes de QA, Homologação e Produção. O Kubernetes garante:
    * **Escalabilidade Horizontal:** Ajuste dinâmico do número de instâncias de microsserviços em resposta à demanda.
    * **Auto-Healing:** Recuperação automática de contêineres falhos.
    * **Gerenciamento de Recursos:** Otimização do uso de CPU, memória e rede.
    * **Descoberta de Serviços:** Facilita a comunicação entre os microsserviços.

## Terraform: Infraestrutura como Código (IaC) e Multi-Cloud Ready

O Terraform será a ferramenta primária para o provisionamento e o gerenciamento de todos os recursos de infraestrutura na nuvem.

* **Automação e Reprodutibilidade:** Toda a infraestrutura será definida em arquivos de código (`.tf`), permitindo a criação e recriação de ambientes de forma automatizada e consistente.
* **Versionamento:** Os arquivos Terraform serão versionados no repositório Git, integrando o gerenciamento da infraestrutura ao fluxo de desenvolvimento e permitindo auditoria de alterações.
* **Visão Multi-Cloud Ready:**
    * Os módulos Terraform serão projetados para serem **agnósticos ao provedor de nuvem** na medida do possível, utilizando abstrações quando necessário, para permitir a implantação em diferentes plataformas (ex: Azure Kubernetes Service - AKS, Amazon Elastic Kubernetes Service - EKS, Google Kubernetes Engine - GKE) com adaptações mínimas.
    * Esta abordagem reduz o acoplamento a um único provedor de nuvem e oferece flexibilidade estratégica para o futuro da organização.
    * **No diretório `infra/`**, teremos subdiretórios para cada provedor de nuvem suportado (ex: `infra/azure/`, `infra/aws/`), contendo os arquivos `.tf` específicos.

## 🌳 Estrutura do Diretório `infra/`

O diretório `infra/` na raiz do repositório organizará todos os scripts de IaC:

```
infra/
├── modules/                        # Módulos Terraform reutilizáveis (ex: k8s-cluster, postgres-db, rabbitmq)
│   ├── k8s-cluster/
│   ├── postgres-db/
│   └── rabbitmq/
├── environments/                   # Configurações específicas por ambiente
│   ├── dev/
│   │   ├── main.tf
│   │   └── variables.tf
│   ├── qa/
│   └── prod/
├── azure/                          # Scripts específicos para Azure (se for o provedor inicial)
│   ├── main.tf
│   └── variables.tf
└── aws/                            # Scripts específicos para AWS (se for o provedor secundário)
    ├── main.tf
    └── variables.tf
```

## 🗄️ Serviços de Infraestrutura Chave

Os seguintes serviços serão provisionados e gerenciados via IaC:

1.  **Orquestrador de Contêineres:** Cluster Kubernetes (AKS, EKS, GKE).
2.  **Bancos de Dados PostgreSQL:**
    * Instâncias gerenciadas na nuvem para cada microsserviço (ex: Azure Database for PostgreSQL, AWS RDS for PostgreSQL).
    * Cada microsserviço terá seu próprio banco de dados logicamente separado, mesmo que residam na mesma instância ou cluster gerenciado para otimização de custos em ambientes de desenvolvimento.
3.  **Message Broker (RabbitMQ):**
    * Instância gerenciada na nuvem ou implantada via contêineres em Kubernetes.
4.  **Cache/Data Store (Redis):**
    * Instância gerenciada na nuvem (ex: Azure Cache for Redis, AWS ElastiCache for Redis).
5.  **API Gateway / Load Balancer:**
    * Componente para expor as APIs dos microsserviços externamente, gerenciar roteamento, segurança e *rate limiting*. Pode ser um Ingress Controller no Kubernetes ou um serviço de API Gateway da nuvem.
6.  **Sistema de Monitoramento e Logs:**
    * Integração com as ferramentas corporativas de observabilidade (Datadog, Elastic, Dynatrace) através de agentes e exportadores configurados via IaC.

## 🤝 Colaboração com o Time de Infraestrutura

O desenvolvimento e a manutenção dos scripts de IaC serão realizados em estreita colaboração com o **Time de Infraestrutura** da organização. Este time é fundamental para:

* Garantir a conformidade com as políticas de segurança e governança da nuvem.
* Otimizar os custos de infraestrutura.
* Definir as melhores práticas para a implantação e operação em larga escala.
* Apoiar a criação de módulos Terraform reutilizáveis e a manutenção da arquitetura de nuvem.

## 🚧 Próximos Passos (IaC)

* Definição e criação dos primeiros módulos Terraform para os serviços básicos (Kubernetes cluster, PostgreSQL).
* Implementação dos pipelines de CI/CD para o *deployment* da infraestrutura.
* Validação de custos e dimensionamento em cada ambiente.