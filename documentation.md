# 🚗 Vehicle Resale API
## Clean Architecture & Kubernetes Implementation

---

**API RESTful em .NET 8 implementando Clean Architecture e princípios SOLID para gerenciamento de veículos, com infraestrutura completa em Docker e Kubernetes.**

---

## 🔗 Links Principais

### 📂 Repositório GitHub
**https://github.com/ohntrebor/vehicle-resale**

### 🎥 Vídeo Demonstrativo
**https://youtu.be/ehMrxDCCR5k (15 min)**

---

## 🏗️ Arquitetura

### 🎯 Clean Architecture
- **Domain:** Entidades, Value Objects, Interfaces
- **Application:** Use Cases, DTOs, Validações
- **Infrastructure:** EF Core, Repositórios, Serviços
- **Presentation:** Controllers, API, Middlewares

### ⚖️ Princípios SOLID
- **S:** Single Responsibility Principle
- **O:** Open/Closed Principle
- **L:** Liskov Substitution Principle
- **I:** Interface Segregation Principle
- **D:** Dependency Inversion Principle

---

## 🚀 Execução Local

### 🐳 Docker Compose (Recomendado)
```bash
git clone https://github.com/ohntrebor/vehicle-resale
cd vehicle-resale
docker compose up -d --build
```
**Acesso:** http://localhost:5000/swagger


## 📁 Estrutura do Repositório

```
vehicle-resale/
├── README.md                    # Documentação do projeto
├── Dockerfile                   # Build da aplicação
├── docker-compose.yml           # Orquestração local
├── Makefile                     # Automação de comandos
├── VehicleResale.API/          # Controllers & Config
├── VehicleResale.Application/  # Use Cases & DTOs
├── VehicleResale.Domain/       # Entidades & Interfaces
├── VehicleResale.Infrastructure/ # EF Core & Repositories
├── k8s/                         # Manifestos Kubernetes
│   ├── namespace.yaml              # Namespace
│   ├── configmap.yaml              # Configurações
│   ├── secret.yaml                 # Dados sensíveis
│   ├── api-deployment.yaml         # API Deployment
│   ├── api-service.yaml            # API Service
│   ├── postgres-deployment.yaml    # DB Deployment
│   ├── postgres-service.yaml       # DB Service
│   └── postgres-pvc.yaml           # Storage persistente
└── tests/                       # Testes automatizados
```

---

## 🛠️ Tecnologias Utilizadas

- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM
- **PostgreSQL** - Banco de dados
- **Docker** - Containerização
- **Kubernetes** - Orquestração
- **Swagger** - Documentação da API

---

## 📊 Endpoints da API

### 🚗 Veículos
- `GET /api/vehicles` - Listar veículos
- `GET /api/vehicles/{id}` - Obter veículo por ID
- `POST /api/vehicles` - Criar novo veículo
- `PUT /api/vehicles/{id}` - Atualizar veículo
- `DELETE /api/vehicles/{id}` - Remover veículo

### ❤️ Health Check
- `GET /health` - Status da aplicação
- `GET /health/live` - Liveness probe
- `GET /health/ready` - Readiness probe

---

## 🎬 Demonstração em Vídeo

**Link do YouTube:** https://www.youtube.com/watch?v=ehMrxDCCR5k

O vídeo demonstra:
- ✅ Execução local com Docker Compose
- ✅ Deploy no Kubernetes com Minikube
- ✅ Funcionalidades da API
- ✅ Clean Architecture implementada
- ✅ Infraestrutura funcionando

---

## 🏆 Características da Solução

### ✅ Clean Architecture
- Separação clara de responsabilidades
- Independência de frameworks
- Testabilidade facilitada
- Manutenibilidade aprimorada

### ✅ Princípios SOLID
- Código bem estruturado
- Baixo acoplamento
- Alta coesão
- Facilidade de extensão

### ✅ Containerização Completa
- Dockerfile otimizado
- Docker Compose para ambiente local
- Manifestos Kubernetes completos
- Alta disponibilidade

### ✅ Pronto para Produção
- Health checks implementados
- Configurações externalizadas
- Logs estruturados
- Monitoramento preparado

---

**🚀 Solução completa implementando as melhores práticas de Arquitetura no desenvolvimento e DevOps**