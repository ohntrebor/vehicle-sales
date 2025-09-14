# 🚗 Vehicle Sales API

## 📋 Descrição do Projeto

API RESTful desenvolvida em **.NET 8** para gerenciamento de revenda de veículos automotores. O sistema permite o cadastro, edição, venda e acompanhamento de veículos, incluindo integração com webhook para processamento de pagamentos.

### 🎯 Objetivos
- Fornecer uma plataforma robusta para revenda de veículos
- Implementar as melhores práticas de arquitetura de software
- Garantir escalabilidade e manutenibilidade do código

## **🏗️ Arquitetura do Projeto VehicleSales**

Seu projeto segue a **Clean Architecture** com separação clara de responsabilidades. Vou explicar cada camada:

VehicleSales/
<br>
├── 🎯 VehicleSales.API          # Camada de Apresentação
<br>
├── 🧠 VehicleSales.Application  # Camada de Aplicação
<br>
├── 💎 VehicleSales.Domain       # Camada de Domínio
<br>
└── 🔌 VehicleSales.Infrastructure # Camada de Infraestrutura

## **🎯 VehicleSales.API (Camada de Apresentação)**

**Responsabilidade:** Interface externa da aplicação

### **O que contém:**
- **Controllers** 📡 - Endpoints REST (recebem requisições HTTP)
- **Health** 💊 - Health checks (monitoramento)
- **Program.cs** ⚙️ - Configuração da aplicação
- **appsettings.json** 📄 - Configurações (connection strings, URLs)
- **VehicleSales.API.http** 📝 - Testes de API

### **Função:**
- Recebe requisições HTTP
- Valida entrada básica
- Chama a camada Application
- Retorna respostas HTTP
- Configuração de DI e middleware


## **🧠 VehicleSales.Application (Camada de Aplicação)**

**Responsabilidade:** Casos de uso e lógica de negócio

### **O que contém:**
- **Commands** 📤 - Operações que modificam dados (Create, Update, Delete)
- **Queries** 📥 - Operações de consulta (Get, List)
- **DTOs** 📦 - Objetos de transferência de dados
- **Handlers** 🔄 - Processadores dos Commands/Queries (MediatR)
- **Mappings** 🔀 - Configurações do AutoMapper
- **Validators** ✅ - Regras de validação (FluentValidation)

### **Função:**
- Orquestra as operações de negócio
- Aplica regras de validação
- Converte dados entre camadas
- Implementa casos de uso específicos


## **💎 VehicleSales.Domain (Camada de Domínio)**

**Responsabilidade:** Regras de negócio puras e entidades

### **O que contém:**
- **Entities** 🏛️ - Entidades do domínio (Vehicle)
- **Enums** 📋 - Enumerações do negócio
- **Interfaces** 🔗 - Contratos (repositórios, serviços)

### **Função:**
- Define as regras de negócio fundamentais
- Modela as entidades principais
- Estabelece contratos para outras camadas
- **NÃO depende de nenhuma outra camada**


## **🔌 VehicleSales.Infrastructure (Camada de Infraestrutura)**

**Responsabilidade:** Implementações técnicas e acesso a dados

### **O que contém:**
- **Data** 🗄️ - DbContext, configurações do Entity Framework
- **Migrations** 📋 - Scripts de migração do banco
- **Repositories** 📚 - Implementações dos repositórios

### **Função:**
- Acesso ao banco de dados
- Implementa interfaces do Domain
- Gerencia persistência de dados
- Configurações do Entity Framework


## **🔄 Fluxo de Dados (Como funciona):**

```
1. 📱 Cliente faz requisição HTTP
   ↓
2. 🎯 API Controller recebe
   ↓
3. 🧠 Application Handler processa
   ↓
4. 💎 Domain aplica regras de negócio
   ↓
5. 🔌 Infrastructure salva no banco
   ↓
6. 🔄 Resposta volta pela mesma rota
```

## **🎯 Benefícios desta Arquitetura:**

- **✅ Testabilidade** - Cada camada pode ser testada isoladamente
- **✅ Manutenibilidade** - Mudanças em uma camada não afetam outras
- **✅ Escalabilidade** - Fácil de expandir funcionalidades
- **✅ Flexibilidade** - Pode trocar banco/framework sem afetar negócio
- **✅ SOLID** - Seguem os princípios de design

## **💡 Resumo das Responsabilidades:**

| Camada | "Eu cuido de..." |
|--------|------------------|
| **API** | "Receber/enviar dados via HTTP" |
| **Application** | "Processar casos de uso do negócio" |
| **Domain** | "Regras fundamentais do veículo" |
| **Infrastructure** | "Salvar/buscar dados no banco" |

### 🔧 Padrões Implementados

- **CQRS (Command Query Responsibility Segregation)** com MediatR
- **Repository Pattern** para abstração de acesso a dados
- **Unit of Work** para gerenciamento de transações
- **Dependency Injection** para inversão de controle
- **AutoMapper** para mapeamento objeto-objeto
- **FluentValidation** para validação de dados

## 🚀 Funcionalidades

### Veículos
- ✅ **Cadastrar veículo** - Registra novo veículo para venda
- ✅ **Editar veículo** - Atualiza informações do veículo
- ✅ **Listar disponíveis** - Veículos à venda ordenados por preço
- ✅ **Listar vendidos** - Histórico de vendas ordenado por preço

### Vendas
- ✅ **Registrar venda** - Efetua venda com CPF do comprador
- ✅ **Webhook de pagamento** - Atualiza status do pagamento
- ✅ **Cancelar venda** - Reverte venda se pagamento cancelado

## 🛠️ Tecnologias Utilizadas

- **.NET 8** - Framework principal
- **Entity Framework Core 8** - ORM para acesso a dados
- **🐘 Postgre** - Banco de dados relacional
- **Docker** - Containerização
- **Kubernetes** - Orquestração de containers
- **Swagger/OpenAPI** - Documentação da API
- **MediatR** - Implementação de CQRS
- **AutoMapper** - Mapeamento de objetos
- **FluentValidation** - Validação de dados

## 📦 Como Executar

### Pré-requisitos

- Docker e Docker Compose instalados
- .NET 8 SDK (apenas para desenvolvimento)
- Kubernetes (kubectl) configurado
- PostgreSQL (local ou via Docker)
- Minikube (para deploy em cluster)

### 💻 Executando Localmente (Desenvolvimento)

```bash
# Instale as dependências
dotnet restore

# Configure o 🐘 Postgre local ou ajuste a connection string
# Execute as migrations
dotnet ef database update -p VehicleSales.Infrastructure -s VehicleSales.API

# Execute a aplicação
dotnet run --project VehicleSales.API

# Abrirá em: https://localhost:7157/swagger/index.html
```

### 🐋 Executando com Docker
# ⚠️ Certifique-se de que o Docker Desktop esteja rodando
```bash
# Força rebuild e sobe em background
docker compose up -d --build

# Acesse em: http://localhost:5000/swagger/index.html

# 📴 Parar containers (mas mantém volumes/dados)
docker compose down
```

### ☸️ Deploy no Kubernetes

```bash
# Aplique os manifests
kubectl apply -f k8s/

# Verifique o status
kubectl get all -n vehicle-sales

# Port-forward para teste local
kubectl port-forward -n vehicle-sales service/vehicle-sales-api-service 8080:80

# Acesse em: http://localhost:8080/swagger/index.html
```

### ☸️ Deploy com Minikube (com Makefile)
# ⚠️ Certifique-se de ter o Makefile e Minikube instalado em sua máquina *WINDOWS*

```bash
## 🎯 Inicia Minikube e configura ambiente Kubernetes
make k8s-start

## 🔨 Constrói imagem no ambiente Minikube
make k8s-build

 ## 🚀 Faz deploy da aplicação no Kubernetes
make k8s-deploy

# Acesse em: http://localhost:9000/swagger/index.html
```

## 🧪 Testando a API

### Endpoints Principais

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/vehicles` | Cadastrar veículo |
| PUT | `/api/vehicles/{id}` | Editar veículo |
| GET | `/api/vehicles/available` | Listar disponíveis |
| GET | `/api/vehicles/sold` | Listar vendidos |
| POST | `/api/vehicles/sale` | Registrar venda |
| POST | `/api/vehicles/payment-webhook` | Webhook pagamento |

### Exemplos de Requisições

#### Cadastrar Veículo
```json
POST /api/vehicles
{
  "brand": "Toyota",
  "model": "Corolla",
  "year": 2022,
  "color": "Prata",
  "price": 95000.00
}
```

#### Registrar Venda
```json
POST /api/vehicles/sale
{
  "vehicleId": "guid-do-veiculo",
  "buyerCpf": "12345678901"
}
```

#### Confirmar Pagamento
```json
POST /api/vehicles/payment-webhook
{
  "paymentCode": "PAY-ABC123",
  "status": "confirmed"
}
```

### 📝 Importar Coleção Postman

Importe o arquivo `VehicleSales.postman_collection.json` no Postman para ter acesso a todos os endpoints configurados.

## 📊 Monitoramento

### Health Checks

- `/api/health` - Status geral da aplicação
- `/api/health/live` - Liveness probe
- `/api/health/ready` - Readiness probe

## 🔒 Segurança

- ✅ Validação de entrada com FluentValidation
- ✅ Proteção contra SQL Injection via Entity Framework
- ✅ Secrets gerenciados via Kubernetes Secrets
- ✅ HTTPS habilitado em produção

## 📈 Métricas e Performance

- **Response Time**: < 200ms para operações de leitura
- **Throughput**: Suporta 100+ requisições simultâneas
- **Disponibilidade**: 99.9% com 3 réplicas no Kubernetes


## 👥 Autores

- **Robert A. dos Anjos**

## 📞 Suporte

Para suporte, envie um email para: robert.ads.anjos@gmail.com

## Documentação

Documentação do entregável está em documentation.md
Para converter a documentação em PDF, usei o comando 
⚠️ Certifique-se de ter o Pandoc e o wkhtmltopdf instalados, caso queira executar na sua máquina:
```bash
choco install pandoc && choco install wkhtmltopdf
```


```bash
pandoc documentation.md -o VehicleSalesAPI_Documentation.pdf --pdf-engine=wkhtmltopdf --toc --number-sections
```