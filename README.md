# 🚗 Vehicle Sales API

## 📋 Descrição do Projeto

API RESTful desenvolvida em **.NET 8** para gerenciamento de vendas de veículos automotores. O sistema permite registrar vendas, processar pagamentos via webhook e consultar catálogo de veículos através de integração com serviço externo. Focado em operações de venda e processamento de transações.

### 🎯 Objetivos
- Fornecer uma plataforma robusta para processamento de vendas de veículos
- Integrar com sistema externo de catálogo de veículos
- Implementar processamento de pagamentos via webhook
- Garantir escalabilidade e manutenibilidade do código com Clean Architecture

## 🏗️ Arquitetura do Projeto VehicleSales

O projeto segue a **Clean Architecture** com separação clara de responsabilidades:

```
VehicleSales/
├── 🎯 VehicleSales.API          # Camada de Apresentação
├── 🧠 VehicleSales.Application  # Camada de Aplicação  
├── 💎 VehicleSales.Domain       # Camada de Domínio
└── 🔌 VehicleSales.Infrastructure # Camada de Infraestrutura
```

### 🎯 VehicleSales.API (Camada de Apresentação)
**Responsabilidade:** Interface externa da aplicação

- **Controllers** 📡 - Endpoints REST (SalesController, CatalogController)
- **Health** 💊 - Health checks para monitoramento
- **Program.cs** ⚙️ - Configuração da aplicação e DI
- **appsettings.json** 📄 - Configurações (MongoDB Atlas, URLs de serviços)

### 🧠 VehicleSales.Application (Camada de Aplicação)
**Responsabilidade:** Casos de uso e orquestração de negócio

- **UseCases** 📤 - Lógica de negócio (vendas, pagamentos)
- **DTOs** 📦 - Objetos de transferência de dados
- **Interfaces** 🔗 - Contratos para serviços externos
- **Controllers** 🔄 - Orquestradores de casos de uso
- **Services** 🌐 - Integração com APIs externas

### 💎 VehicleSales.Domain (Camada de Domínio)
**Responsabilidade:** Regras de negócio puras

- **Entities** 🏛️ - Sale (Venda)
- **Enums** 📋 - PaymentStatus (Status de pagamento)
- **Gateways** 🔗 - Contratos para persistência

### 🔌 VehicleSales.Infrastructure (Camada de Infraestrutura)
**Responsabilidade:** Implementações técnicas

- **Data** 🗄️ - Configurações MongoDB
- **Gateways** 📚 - Implementações de persistência
- **External** 🌐 - Integrações com serviços externos

## 🔄 Fluxo de Integração

O sistema integra-se com o **Vehicle Catalog API** para consulta de veículos:

```
1. 📱 Cliente consulta catálogo → CatalogController
2. 🌐 Proxy para Vehicle Catalog API
3. 📊 Retorna dados de veículos disponíveis
4. 🛒 Cliente registra venda → SalesController  
5. 💾 Salva no MongoDB Atlas
6. 💳 Processa webhook de pagamento
7. 🔄 Notifica Vehicle Catalog sobre venda
```

## 🚀 Funcionalidades

### 📊 Consulta de Catálogo
- ✅ **Listar veículos** - Todos os veículos do catálogo externo
- ✅ **Buscar por filtros** - Marca, modelo, preço, ano, cor, disponibilidade
- ✅ **Buscar por ID** - Detalhes de veículo específico

### 💰 Gestão de Vendas
- ✅ **Registrar venda** - Cria nova venda com código de pagamento único
- ✅ **Consultar venda** - Busca venda por ID
- ✅ **Listar vendas** - Histórico completo de vendas
- ✅ **Webhook de pagamento** - Atualiza status via gateway de pagamento
- ✅ **Notificação de venda** - Informa Vehicle Catalog sobre venda confirmada

## 🛠️ Tecnologias Utilizadas

- **.NET 8** - Framework principal
- **MongoDB Atlas** - Banco de dados em nuvem
- **Docker** - Containerização
- **Kubernetes** - Orquestração de containers
- **Swagger/OpenAPI** - Documentação da API
- **HttpClient** - Integração com Vehicle Catalog API
- **Health Checks** - Monitoramento de saúde

## 📦 Como Executar

### Pré-requisitos

- Docker e Docker Compose instalados
- .NET 8 SDK (desenvolvimento)
- Kubernetes (kubectl) configurado
- Minikube (para deploy local)
- MongoDB Atlas configurado
- Vehicle Catalog API rodando

### 💻 Executando Localmente (Desenvolvimento)

```bash
# Instale as dependências
dotnet restore

# Configure connection strings no appsettings.json
# MongoDB Atlas: mongodb+srv://...
# Vehicle Catalog API: http://localhost:5000/api

# Execute a aplicação
dotnet run --project VehicleSales.API

# Abrirá em: https://localhost:7157/swagger/index.html
```

### 🐋 Executando com Docker

```bash
# Força rebuild e sobe em background
docker compose up -d --build

# Vehicle Sales API: http://localhost:5001/swagger/index.html
# Vehicle Catalog API: http://localhost:5000/swagger/index.html

# Parar containers
docker compose down
```

### ☸️ Deploy no Kubernetes

```bash
# Aplique os manifests
kubectl apply -f k8s/

# Verifique o status
kubectl get all -n vehicle-sales

# Port-forward para teste local
kubectl port-forward -n vehicle-sales service/vehicle-sales-api-service 9000:80

# Acesse em: http://localhost:9000/swagger/index.html
```

### ☸️ Deploy com Minikube (Automatizado)

```bash
# Setup completo com um comando
make k8s-full-deploy

# Acessos:
# Vehicle Sales API: http://localhost:9000/swagger/index.html
# Vehicle Catalog API: http://localhost:5000/swagger/index.html

# Para parar port-forwards
make k8s-stop
```

## 🧪 Testando a API

### 📊 Endpoints de Catálogo

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/catalog/vehicles` | Listar todos os veículos |
| GET | `/api/catalog/vehicles/search` | Buscar com filtros |
| GET | `/api/catalog/vehicles/{id}` | Buscar veículo por ID |

### 💰 Endpoints de Vendas

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/sales` | Registrar nova venda |
| GET | `/api/sales/{id}` | Buscar venda por ID |
| GET | `/api/sales` | Listar todas as vendas |
| POST | `/api/sales/payment-webhook` | Webhook de pagamento |

### Exemplos de Requisições

#### Buscar Veículos com Filtros
```json
GET /api/catalog/vehicles/search?brand=Toyota&minPrice=50000&maxPrice=100000&isAvailable=true
```

#### Registrar Venda
```json
POST /api/sales
{
  "vehicleId": "550e8400-e29b-41d4-a716-446655440000",
  "buyerCpf": "12345678901"
}
```

#### Resposta da Venda
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "vehicleId": "550e8400-e29b-41d4-a716-446655440000",
  "buyerCpf": "12345678901",
  "paymentCode": "PAY-ABC123",
  "paymentStatus": "Pending",
  "saleDate": "2024-09-15T10:30:00Z",
  "totalAmount": 95000.00
}
```

#### Webhook de Pagamento
```json
POST /api/sales/payment-webhook
{
  "paymentCode": "PAY-ABC123",
  "status": "confirmed"
}
```

### 📝 Importar Coleção Postman

Importe o arquivo `VehicleSales.postman_collection.json` no Postman para ter acesso a todos os endpoints configurados.

## 🔗 Integração com Vehicle Catalog

O sistema integra-se com o **Vehicle Catalog API** para:

- **Consultar veículos disponíveis** - Proxy transparente para o catálogo
- **Notificar vendas** - Informa quando veículo é vendido
- **Verificar disponibilidade** - Valida se veículo está disponível para venda

### Configuração da Integração

```json
{
  "ExternalServices": {
    "VehicleCatalogApi": "http://vehicle-catalog-service:80/api"
  }
}
```

## 📊 Monitoramento

### Health Checks

- `/health` - Status geral (inclui MongoDB e Vehicle Catalog API)
- `/health/live` - Liveness probe para Kubernetes
- `/health/ready` - Readiness probe para Kubernetes

### Verificações Incluídas

- ✅ **MongoDB Atlas** - Conectividade com banco de dados
- ✅ **Vehicle Catalog API** - Disponibilidade do serviço externo
- ✅ **Memória** - Uso de recursos do sistema

## 🔒 Segurança

- ✅ **Connection Strings** - Secrets gerenciados via Kubernetes
- ✅ **Validação de Entrada** - DTOs com validação rigorosa
- ✅ **HTTPS** - Habilitado em produção
- ✅ **CORS** - Configurado para ambientes apropriados

## 📈 Performance e Escalabilidade

- **Response Time**: < 500ms para consultas de catálogo
- **Throughput**: Suporta 50+ vendas simultâneas
- **Disponibilidade**: 99.9% com múltiplas réplicas
- **Auto-scaling**: Configurado no Kubernetes baseado em CPU

## 🗄️ Estrutura de Dados

### Sale (Venda)
```csharp
{
  "Id": "Guid",
  "VehicleId": "Guid", 
  "BuyerCpf": "string",
  "PaymentCode": "string",
  "PaymentStatus": "Pending|Paid|Cancelled|Failed",
  "SaleDate": "DateTime",
  "TotalAmount": "decimal"
}
```

### PaymentStatus (Enum)
- **Pending** - Aguardando pagamento
- **Paid** - Pago e confirmado
- **Cancelled** - Cancelado
- **Failed** - Falha no pagamento

## 🚀 Roadmap

- [ ] **Autenticação JWT** - Controle de acesso
- [ ] **Relatórios** - Dashboard de vendas
- [ ] **Notificações** - Email/SMS para compradores
- [ ] **Histórico de Pagamentos** - Auditoria completa
- [ ] **Cache Redis** - Performance de consultas

## 👥 Autor

**Robert A. dos Anjos**
- Email: robert.ads.anjos@gmail.com
- GitHub: @ohntrebor

## 📞 Suporte

Para suporte técnico, envie um email para: robert.ads.anjos@gmail.com

## 📋 Documentação Técnica

Para documentação técnica detalhada, consulte o arquivo `documentation.md`.

### Gerar PDF da Documentação

```bash
# Instalar dependências (Windows)
choco install pandoc && choco install wkhtmltopdf

# Gerar PDF
pandoc documentation.md -o VehicleSalesAPI_Documentation.pdf --pdf-engine=wkhtmltopdf --toc --number-sections
```

---

*Sistema de vendas de veículos desenvolvido com foco em arquitetura limpa, escalabilidade e integração com serviços externos.*