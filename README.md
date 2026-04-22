# RPA Worker Service + Web API

## Visão Geral

Este projeto consiste em um sistema automatizado para coleta e disponibilização de dados externos, composto por dois serviços principais:

- Worker (RPA): responsável por coletar dados periodicamente de uma fonte externa
- Web API: responsável por expor os dados coletados via endpoints REST

A solução foi desenvolvida utilizando .NET 8, com foco em separação de responsabilidades, resiliência e facilidade de manutenção.


## Arquitetura

A solução foi estruturada seguindo princípios de arquitetura em camadas, separando claramente responsabilidades entre domínio, aplicação e infraestrutura.

### Estrutura de Projetos

RpaWorkerService
├── Application
├── Domain
├── Infra.Data
├── RpaWorkerService.Web (API)
├── RpaWorkerService.Worker
└── docker-compose.yml

### Descrição das Camadas

#### Domain

Camada central da aplicação, responsável por definir:

- Entidades do sistema (CollectedData)
- Interfaces (contratos), como:
  - ICollectedDataRepository
  - IDataCollectorService

Essa camada não possui dependência de outras.


#### Application

Responsável pelos casos de uso da aplicação:

- CollectDataUseCase
- GetCollectedDataUseCase
- GetCollectedDataByIdUseCase

Essa camada orquestra a lógica de negócio utilizando as interfaces definidas no Domain.


#### Infra.Data

Responsável pelas implementações concretas:

- Persistência com Entity Framework Core e SQLite
- Repositórios (CollectedDataRepository)
- Integração com serviço externo (CurrencyCollectorService)
- Configuração de injeção de dependência


#### API (RpaWorkerService.Web)

Responsável por:

- Expor endpoints REST
- Configuração do Swagger
- Inicialização do banco de dados

Principais endpoints:

GET /api/CollectedData
GET /api/CollectedData/{id}

#### Worker (RpaWorkerService.Worker)

Responsável por:

- Execução em background
- Coleta periódica de dados
- Persistência dos dados coletados

O Worker executa em intervalos configuráveis.


## Fluxo da Aplicação

1. O Worker é iniciado
2. A cada intervalo configurado:
   - Consome dados de uma API externa
   - Converte os dados para o modelo interno
   - Persiste no banco de dados
3. A API expõe os dados armazenados via endpoints HTTP


## Fonte de Dados

A aplicação consome dados da API pública de cotações:

https://api.frankfurter.dev/v2/rates?base=BRL&quotes=USD,EUR,GBP

Os dados coletados incluem:

- Moeda base (BRL)
- Moeda de comparação (USD, EUR, GBP)
- Valor da cotação


## Persistência

- Banco utilizado: SQLite
- Arquivo compartilhado entre containers
- Volume Docker utilizado para persistência

Para evitar duplicidade, a aplicação verifica registros existentes antes de inserir novos dados.


## Execução Local

### Pré-requisitos

- .NET 8 SDK
- Docker Desktop

### Rodando sem Docker

1. Executar a API:

Você pode utilizar o comando abaixo para rodar a API diretamente:

dotnet run --project RpaWorkerService.Web

dotnet run --project RpaWorkerService.Worker

ou você pode rodar simultaneamente ambos projetos utilizando o visual studio, definindo ambos como projetos de inicialização. 

## Execução com Docker

### Build e execução

Na raiz da solução:

docker compose up --build


## Configuração

### Variáveis importantes

- ConnectionStrings__DefaultConnection
- CurrencySettings__SourceUrl
- WorkerSettings__IntervalInMinutes
- ASPNETCORE_ENVIRONMENT


## Decisões Arquiteturais

### Separação de responsabilidades

A solução foi dividida em múltiplos projetos para garantir:

- baixo acoplamento
- alta coesão
- facilidade de manutenção


### Uso de Worker Service

O Worker foi implementado como serviço em background para simular um robô (RPA) que executa tarefas periódicas.


### Uso de SQLite

SQLite foi escolhido por:

- simplicidade de configuração
- ausência de dependências externas
- facilidade de uso em ambiente Docker


### Docker e Docker Compose

A solução foi containerizada com:

- um container para a API
- um container para o Worker
- volume compartilhado para o banco SQLite


## Possíveis Melhorias

- Implementação de paginação nos endpoints
- Uso de DTOs para desacoplamento da entidade
- Uso de migrations ao invés de EnsureCreated
- Integração com banco relacional (PostgreSQL/SQL Server)
- Monitoramento e observabilidade (logs estruturados)
- testes unitários e de integração
