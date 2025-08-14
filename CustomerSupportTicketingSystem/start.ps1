#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Startup script for the AI-Powered Customer Support Ticketing System

.DESCRIPTION
    This script helps you start the entire system using Docker Compose.
    It includes health checks and status monitoring.

.PARAMETER Profile
    The Docker Compose profile to use (default: all services)

.PARAMETER Build
    Whether to rebuild Docker images before starting

.EXAMPLE
    .\start.ps1
    .\start.ps1 -Profile monitoring
    .\start.ps1 -Build
#>

param(
    [string]$Profile = "",
    [switch]$Build
)

Write-Host "🚀 Starting AI-Powered Customer Support Ticketing System..." -ForegroundColor Green
Write-Host ""

# Check if Docker is running
try {
    docker version | Out-Null
    Write-Host "✅ Docker is running" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker is not running. Please start Docker Desktop first." -ForegroundColor Red
    exit 1
}

# Check if Docker Compose is available
try {
    docker-compose version | Out-Null
    Write-Host "✅ Docker Compose is available" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker Compose is not available. Please install Docker Compose." -ForegroundColor Red
    exit 1
}

# Build images if requested
if ($Build) {
    Write-Host "🔨 Building Docker images..." -ForegroundColor Yellow
    if ($Profile) {
        docker-compose --profile $Profile build
    } else {
        docker-compose build
    }
    Write-Host "✅ Build completed" -ForegroundColor Green
}

# Start services
Write-Host "🚀 Starting services..." -ForegroundColor Yellow
if ($Profile) {
    Write-Host "📋 Using profile: $Profile" -ForegroundColor Cyan
    docker-compose --profile $Profile up -d
} else {
    docker-compose up -d
}

# Wait for services to start
Write-Host "⏳ Waiting for services to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Check service status
Write-Host "📊 Checking service status..." -ForegroundColor Yellow
docker-compose ps

Write-Host ""
Write-Host "🎉 System startup initiated!" -ForegroundColor Green
Write-Host ""

# Display access information
Write-Host "🌐 Access Points:" -ForegroundColor Cyan
Write-Host "   • API Gateway: http://localhost:5000" -ForegroundColor White
Write-Host "   • Swagger UI: http://localhost:5000/swagger" -ForegroundColor White
Write-Host "   • Health Dashboard: http://localhost:5000/health" -ForegroundColor White
Write-Host "   • Ticket Service: http://localhost:5001" -ForegroundColor White
Write-Host "   • AI Service: http://localhost:5002" -ForegroundColor White
Write-Host "   • Priority Service: http://localhost:5003" -ForegroundColor White
Write-Host "   • User Service: http://localhost:5004" -ForegroundColor White
Write-Host "   • Notification Service: http://localhost:5005" -ForegroundColor White

if ($Profile -eq "monitoring") {
    Write-Host "   • Prometheus: http://localhost:9090" -ForegroundColor White
    Write-Host "   • Grafana: http://localhost:3000 (admin/admin123)" -ForegroundColor White
    Write-Host "   • Elasticsearch: http://localhost:9200" -ForegroundColor White
    Write-Host "   • Kibana: http://localhost:5601" -ForegroundColor White
}

Write-Host ""
Write-Host "📋 Useful Commands:" -ForegroundColor Cyan
Write-Host "   • View logs: docker-compose logs -f" -ForegroundColor White
Write-Host "   • Stop services: docker-compose down" -ForegroundColor White
Write-Host "   • Restart services: docker-compose restart" -ForegroundColor White
Write-Host "   • Check health: docker-compose ps" -ForegroundColor White

Write-Host ""
Write-Host "🔍 Monitoring startup progress..." -ForegroundColor Yellow
Write-Host "Press Ctrl+C to stop monitoring" -ForegroundColor Gray

# Monitor startup progress
try {
    while ($true) {
        $status = docker-compose ps --format "table {{.Name}}\t{{.Status}}\t{{.Ports}}"
        Clear-Host
        Write-Host "🚀 AI-Powered Customer Support Ticketing System - Status Monitor" -ForegroundColor Green
        Write-Host "=" * 70 -ForegroundColor Gray
        Write-Host $status -ForegroundColor White
        Write-Host ""
        Write-Host "⏰ Last updated: $(Get-Date)" -ForegroundColor Gray
        Write-Host "Press Ctrl+C to stop monitoring" -ForegroundColor Gray
        Start-Sleep -Seconds 5
    }
} catch {
    Write-Host ""
    Write-Host "👋 Monitoring stopped" -ForegroundColor Yellow
}
