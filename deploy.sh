#!/bin/bash

echo "🚀 Starting Vehicle Resale API Deployment"

# Verificar se Makefile existe
if [ -f "Makefile" ]; then
    echo "📋 Using Makefile for deployment..."
    make k8s-full-deploy
    
    echo ""
    echo "🎉 Deployment finished!"
    echo "🔗 Repository: https://github.com/ohntrebor/vehicle-resale"
    echo "🌐 API available at: http://localhost:9000"
    echo "📊 To access: make k8s-port-forward"
    
else
    echo "❌ Makefile not found!"
    echo "Please ensure Makefile exists or run commands manually:"
    echo "1. docker build -t vehicle-resale-api:latest ."
    echo "2. kubectl apply -f k8s/"
    echo "3. kubectl port-forward -n vehicle-resale service/vehicle-resale-api-service 9000:80"
    exit 1
fi