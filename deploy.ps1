# Vehicle Resale API Deployment Script for PowerShell
# Usage: .\deploy.ps1

Write-Host "🚀 Starting Vehicle Resale API Deployment" -ForegroundColor Green

# Verificar se Makefile existe
if (Test-Path "Makefile") {
    Write-Host "📋 Using Makefile for deployment..." -ForegroundColor Cyan
    
    # Verificar se é ambiente Minikube
    try {
        $minikubeStatus = minikube status 2>$null
        if ($LASTEXITCODE -eq 0) {
            Write-Host "🎯 Minikube detected - using full k8s setup" -ForegroundColor Yellow
            make k8s-full-deploy
            
            Write-Host ""
            Write-Host "🎉 Minikube deployment finished!" -ForegroundColor Green
            Write-Host "🌐 API available at: http://localhost:9000" -ForegroundColor Cyan
            Write-Host "📊 To access: make k8s-port-forward" -ForegroundColor Yellow
        }
        else {
            Write-Host "☁️ Production cluster detected - using standard deploy" -ForegroundColor Yellow
            
            Write-Host "📦 Building Docker image..." -ForegroundColor Yellow
            make docker-build
            
            Write-Host "☸️ Deploying to Kubernetes..." -ForegroundColor Yellow
            make k8s-deploy
            
            Write-Host "✅ Production deployment complete!" -ForegroundColor Green
            make k8s-status
            
            Write-Host "📊 To access the API:" -ForegroundColor Cyan
            Write-Host "kubectl port-forward -n vehicle-resale service/vehicle-resale-api-service 9000:80"
        }
    }
    catch {
        Write-Host "⚠️ Could not detect Minikube status, using standard deploy" -ForegroundColor Yellow
        
        Write-Host "📦 Building Docker image..." -ForegroundColor Yellow
        make docker-build
        
        Write-Host "☸️ Deploying to Kubernetes..." -ForegroundColor Yellow
        make k8s-deploy
        
        Write-Host "✅ Deployment complete!" -ForegroundColor Green
        make k8s-status
    }
}
else {
    Write-Host "❌ Makefile not found! Using individual commands..." -ForegroundColor Red
    
    # Fallback para comandos individuais
    Write-Host "📦 Building Docker image..." -ForegroundColor Yellow
    docker build -t vehicle-resale-api:latest .
    
    Write-Host "☸️ Deploying to Kubernetes..." -ForegroundColor Yellow
    kubectl apply -f k8s/00-namespace.yaml
    kubectl apply -f k8s/01-configmap.yaml
    kubectl apply -f k8s/02-secret.yaml
    kubectl apply -f k8s/03-postgres-pvc.yaml
    kubectl apply -f k8s/04-postgres-deployment.yaml
    kubectl apply -f k8s/05-postgres-service.yaml
    
    Write-Host "⏳ Waiting for PostgreSQL to be ready..." -ForegroundColor Yellow
    kubectl wait --for=condition=ready pod -l app=postgres -n vehicle-resale --timeout=120s
    
    kubectl apply -f k8s/06-api-deployment.yaml
    kubectl apply -f k8s/07-api-service.yaml
    
    Write-Host "✅ Deployment complete!" -ForegroundColor Green
    kubectl get all -n vehicle-resale
    
    Write-Host "📊 To access the API:" -ForegroundColor Cyan
    Write-Host "kubectl port-forward -n vehicle-resale service/vehicle-resale-api-service 9000:80"
}

Write-Host ""
Write-Host "🎉 Vehicle Resale API deployment finished!" -ForegroundColor Green
Write-Host "🔗 Repository: https://github.com/ohntrebor/vehicle-resale" -ForegroundColor Cyan