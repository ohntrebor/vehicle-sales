.PHONY: help build run test clean restore migration-add migration-remove migration-update docker-build docker-run docker-stop docker-logs docker-clean k8s-start k8s-deploy k8s-delete k8s-status k8s-port-forward k8s-logs k8s-clean dev-setup

## 📋 Mostra esta ajuda com todos os comandos disponíveis
help:
	@echo "🚀 Vehicle Sales API - Comandos Disponiveis:"
	@echo ""
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

# ========================================
# 🔧 COMANDOS DE DESENVOLVIMENTO LOCAL
# ========================================

## 📦 Restaura pacotes NuGet do projeto
restore: 
	@echo "📦 Restaurando pacotes NuGet..."
	dotnet restore

## 🔨 Compila a aplicação .NET
build: 
	@echo "🔨 Compilando aplicacao..."
	dotnet build

## ▶️  Executa a aplicação localmente
run: 
	@echo "▶️ Iniciando aplicacao local..."
	@echo "🌐 API disponível em: http://localhost:5000"
	dotnet run --project VehicleSales.API

## 🧪 Executa todos os testes do projeto
test: 
	@echo "🧪 Executando todos os testes..."
	dotnet test --verbosity normal

## 🚀 Executa apenas testes unitários
test-unit:
	@echo "🚀 Executando testes unitarios..."
	dotnet test --verbosity normal --filter "Category=Unit"

## 🗄️ Executa apenas testes de integração (banco em memória)
test-integration:
	@echo "🗄️ Executando testes de integracao..."
	dotnet test --verbosity normal --filter "Category=Integration"

## 🧹 Remove arquivos de build e temporários
clean:
	@echo "🧹 Limpando arquivos temporarios..."
	dotnet clean
	rm -rf */bin */obj

## 🛠️  Configuração inicial completa para desenvolvimento
dev-setup: restore build 
	@echo "✅ Ambiente de desenvolvimento configurado!"

# ========================================
# 🗄️ COMANDOS DE BANCO DE DADOS
# ========================================

## ➕ Adiciona nova migration (uso: make migration-add NAME=NomeDaMigracao)
migration-add: 
	@echo "➕ Adicionando migration: $(NAME)"
	dotnet ef migrations add $(NAME) -p VehicleSales.Infrastructure -s VehicleSales.API -o Data/Migrations

## ➖ Remove última migration
migration-remove: 
	@echo "➖ Removendo ultima migration..."
	dotnet ef migrations remove -p VehicleSales.Infrastructure -s VehicleSales.API

## 🔄 Atualiza banco de dados com migrations pendentes
migration-update: 
	@echo "🔄 Atualizando banco de dados..."
	dotnet ef database update -p VehicleSales.Infrastructure -s VehicleSales.API

## 📊 Mostra status das migrations
migration-status: 
	@echo "📊 Status das migrations:"
	dotnet ef migrations list -p VehicleSales.Infrastructure -s VehicleSales.API


# ========================================
# 🐳 COMANDOS DOCKER COMPOSE
# ========================================

## 🔨 Constrói imagem Docker da aplicação
docker-build: 
	@echo "🔨 Construindo imagem Docker..."
	docker build -t vehicle-sales-api:latest .

## 🚀 Inicia todos os serviços com Docker Compose
docker-run: 
	@echo "🚀 Iniciando servicos com Docker Compose..."
	docker-compose up -d
	@echo "🌐 API disponivel em: http://localhost:5000"
	@echo "🗄️ Banco de dados disponivel na porta: 1433"

 ## ⏹️  Para todos os serviços do Docker Compose
docker-stop:
	@echo "⏹️ Parando servicos do Docker Compose..."
	docker-compose down

## 📋 Mostra logs dos containers em tempo real
docker-logs: 
	@echo "📋 Logs dos containers:"
	docker-compose logs -f

## 🔄 Reinicia todos os serviços Docker
docker-restart: docker-stop docker-run

## 🧹 Remove containers, volumes e imagens não utilizados
docker-clean: 
	@echo "🧹 Limpando recursos Docker..."
	docker-compose down -v --remove-orphans
	docker system prune -f

 ## 🗄️ Inicia apenas o banco de dados
docker-db-only:
	@echo "🗄️ Iniciando apenas SQL Server..."
	docker-compose up -d sqlserver

# ========================================
# ☸️ COMANDOS KUBERNETES
# ========================================

## 🎯 Inicia Minikube e configura ambiente Kubernetes
k8s-start: 
	@echo "🎯 Iniciando Minikube..."
	minikube start --driver=docker
	@echo "✅ Minikube iniciado!"
	kubectl get nodes

## 🔨 Constrói imagem no ambiente Minikube
k8s-build:
	@echo "🔨 Configurando Docker do Minikube e construindo imagem..."
	@powershell -Command "minikube docker-env | Invoke-Expression; docker build -t vehicle-sales-api:latest ."
	@echo "✅ Imagem construída no Minikube!"

## 🚀 Faz deploy da aplicação no Kubernetes
k8s-deploy:
	@echo "🚀 Fazendo deploy no Kubernetes..."
	kubectl apply -f k8s/
	@echo "✅ Deploy realizado!"
	@echo "⏳ Aguarde os pods ficarem prontos..."
	kubectl wait --for=condition=ready pod -l app=vehicle-sales-api -n vehicle-sales --timeout=300s
	@echo "🔗 Para acessar a API, execute: make k8s-port-forward"
	@echo "🌐 Depois acesse: http://localhost:9000/swagger/index.html"

## 🗑️ Remove aplicação do Kubernetes
k8s-delete:
	@echo "🗑️ Removendo aplicacao do Kubernetes..."
	kubectl delete -f k8s/

## 📊 Mostra status dos recursos no Kubernetes
k8s-status: 
	@echo "📊 Status dos recursos Kubernetes:"
	kubectl get all -l app=vehicle-sales-api -n vehicle-sales
	@echo ""
	@echo "📋 Pods detalhados:"
	kubectl get pods -o wide -n vehicle-sales

## 🌐 Configurando portal manualmente - port-forward para acessar API (http://localhost:9000/swagger/index.html)
k8s-port-forward: 
	@echo "🌐 Configurando acesso a API via port-forward..."
	@echo "🔗 API disponivel em: http://localhost:9000/swagger/index.html"
	@echo "⏹️ Para parar: Ctrl+C"
	kubectl port-forward -n vehicle-sales service/vehicle-sales-api-service 9000:80

## 📋 Mostra logs da aplicação no Kubernetes
k8s-logs:
	@echo "📋 Logs da aplicação:"
	kubectl logs -l app=vehicle-sales-api -n vehicle-sales -f

## 🔧 Acessa shell do pod da aplicação
k8s-shell: 
	@echo "🔧 Acessando shell do pod..."
	kubectl exec -it $$(kubectl get pod -l app=vehicle-sales-api -n vehicle-sales -o jsonpath='{.items[0].metadata.name}') -- /bin/bash

## 🔄 Reinicia deployment no Kubernetes
k8s-restart: 
	@echo "🔄 Reiniciando deployment..."
	kubectl rollout restart deployment/vehicle-sales-api-deployment -n vehicle-sales
	kubectl rollout status deployment/vehicle-sales-api-deployment -n vehicle-sales

## 🧹 Para Minikube e limpa recursos
k8s-clean:
	@echo "🧹 Limpando ambiente Kubernetes..."
	kubectl delete all --all -n vehicle-sales
	kubectl delete namespace vehicle-sales
	minikube stop
	minikube delete

## 📊 Abre dashboard do Kubernetes
k8s-dashboard: 
	@echo "📊 Abrindo dashboard do Kubernetes..."
	minikube dashboard

## 🔄 Deploy rápido após mudanças no código
k8s-redeploy:
	@echo "🔄 Fazendo redeploy apos mudancas..."
	@echo "🔨 Reconstruindo imagem..."
	@powershell -Command "minikube docker-env | Invoke-Expression; docker build -t vehicle-sales-api:latest ."
	@echo "🔄 Reiniciando deployment..."
	kubectl rollout restart deployment/vehicle-sales-api-deployment -n vehicle-sales
	kubectl rollout status deployment/vehicle-sales-api-deployment -n vehicle-sales
	@echo "✅ Redeploy concluido!"
	@echo "🌐 API disponivel via: make k8s-port-forward"

## 🔍 Verificar recursos em todos os namespaces
k8s-check-all:
	@echo "🔍 Verificando recursos em todos os namespaces..."
	kubectl get all --all-namespaces | grep vehicle-sales || echo "❌ Nenhum recurso encontrado"
	@echo ""
	@echo "📋 Namespaces disponiveis:"
	kubectl get namespaces

# ========================================
# 🔄 WORKFLOWS COMPLETOS
# ========================================

## 🐳 Setup completo com Docker (limpa + constrói + executa)
full-docker-setup: docker-clean docker-build docker-run 
	@echo "✅ Setup Docker completo finalizado!"

## ☸️ Setup completo com Kubernetes (inicia + constrói + deploy)
full-k8s-setup: k8s-start k8s-build k8s-deploy 
	@echo "✅ Setup Kubernetes completo finalizado!"
	@echo "🌐 Execute 'make k8s-port-forward' para acessar a API"

## 🚀 Setup completo Minikube em um comando único
k8s-full-deploy:
	@echo "🚀 Iniciando setup completo do Minikube..."
	@echo "🎯 1/6 - Iniciando Minikube..."
	minikube start --driver=docker
	@echo "🔧 2/6 - Configurando Docker do Minikube e fazendo build..."
	@powershell -Command "minikube docker-env | Invoke-Expression; docker build -t vehicle-sales-api:latest ."
	@echo "📥 3/6 - Baixando imagem do Vehicle Catalog..."
	@powershell -Command "minikube docker-env | Invoke-Expression; docker pull ghcr.io/ohntrebor/vehicle-catalog:latest"
	@echo "🚀 4/6 - Fazendo deploy da aplicacao..."
	kubectl apply -f k8s/
	@echo "⏳ 5/6 - Aguardando bancos de dados ficarem prontos..."
	kubectl wait --for=condition=ready pod -l app=mongodb -n vehicle-sales --timeout=300s
	kubectl wait --for=condition=ready pod -l app=postgres -n vehicle-sales --timeout=300s
	@echo "⏳ Aguardando APIs ficarem prontas..."
	kubectl wait --for=condition=ready pod -l app=vehicle-catalog -n vehicle-sales --timeout=300s
	kubectl wait --for=condition=ready pod -l app=vehicle-sales-api -n vehicle-sales --timeout=300s
	@echo "🌐 6/6 - Configurando port-forward na porta 9000..."
	@echo ""
	@echo "✅ Setup Minikube completo finalizado!"
    kubectl port-forward -n vehicle-sales service/vehicle-catalog-service 5000:80
	kubectl port-forward -n vehicle-sales service/vehicle-sales-api-service 9000:80
	@echo "🔗 API disponivel em: http://localhost:9000/swagger/index.html"
	@echo "⏹️ Para parar o port-forward: Ctrl+C"

## 🧹 Limpeza completa do Minikube e Kubernetes
k8s-full-clean:
	@echo "🧹 Iniciando limpeza completa do Minikube..."
	@echo "🗑️ 1/4 - Removendo aplicacao do Kubernetes..."
	kubectl delete -f k8s/ || echo "⚠️ Alguns recursos ja foram removidos"
	@echo "🗑️ 2/4 - Removendo namespace..."
	kubectl delete namespace vehicle-sales || echo "⚠️ Namespace ja foi removido"
	@echo "🗑️ 3/4 - Parando Minikube..."
	minikube stop || echo "⚠️ Minikube ja estava parado"
	@echo "🗑️ 4/4 - Removendo cluster Minikube..."
	minikube delete || echo "⚠️ Cluster ja foi removido"
	@echo ""
	@echo "✅ Limpeza completa finalizada!"
	@echo "💡 Para recriar tudo: make k8s-full-deploy"


# ========================================
# 📚 COMANDOS DE INFORMAÇÃO
# ========================================

## ℹ️  Mostra informações do ambiente
info: 
	@echo "ℹ️ Informações do Ambiente:"
	@echo "📁 Projeto: Vehicle Sales API"
	@echo "🔧 .NET Version: $$(dotnet --version)"
	@echo "🐳 Docker Version: $$(docker --version)"
	@echo "☸️ Kubectl Version: $$(kubectl version --client --short 2>/dev/null || echo 'Kubectl não instalado')"
	@echo "🎯 Minikube Status: $$(minikube status 2>/dev/null || echo 'Minikube não iniciado')"