# 🚀 AI-Powered Customer Support Ticketing System - Documentation

Welcome to the comprehensive documentation for the AI-Powered Customer Support Ticketing System. This document provides detailed information about the system architecture, setup, deployment, and usage.

## 📚 Table of Contents

- [System Overview](#system-overview)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [API Documentation](#api-documentation)
- [Deployment](#deployment)
- [Configuration](#configuration)
- [Monitoring & Logging](#monitoring--logging)
- [Development](#development)
- [Troubleshooting](#troubleshooting)

---

## 🌟 System Overview

The AI-Powered Customer Support Ticketing System is a modern, cloud-native solution that leverages artificial intelligence to automatically categorize, prioritize, and route customer support tickets. Built with microservices architecture, it provides scalability, resilience, and continuous learning capabilities.

### ✨ Key Features

- 🤖 **AI-Powered Intelligence**: ML.NET-based ticket categorization and prioritization
- 🏗️ **Microservices Architecture**: Independent, scalable service components
- 🔄 **Continuous Learning**: Self-improving AI models through MLOps pipelines
- 🐳 **Containerized**: Docker-based deployment with Kubernetes support
- 📊 **Real-time Analytics**: Live insights into support operations
- 🔒 **Enterprise Security**: Role-based access control and secure APIs

---

## 🏗️ Architecture

### System Components

```
┌─────────────────────────────────────────────────────────────────┐
│                    🌐 API Gateway (Ocelot)                     │
│                         Port: 5000                            │
└─────────────────────┬───────────────────────────────────────────┘
                      │
    ┌─────────────────┼─────────────────┐
    │                 │                 │
┌───▼───┐         ┌───▼───┐         ┌───▼───┐
│ 🎫    │         │ 🤖    │         │ 👤    │
│Ticket  │         │AI     │         │User   │
│Mgmt    │         │Categor│         │Mgmt   │
│Service │         │Service│         │Service│
│Port:   │         │Port:  │         │Port:  │
│5001    │         │5002   │         │5004   │
└───────┘         └───────┘         └───────┘
    │                 │                 │
    └─────────────────┼─────────────────┘
                      │
    ┌─────────────────┼─────────────────┐
    │                 │                 │
┌───▼───┐         ┌───▼───┐         ┌───▼───┐
│ ⚡    │         │ 📧    │         │ 🗄️    │
│Priority│         │Notif  │         │MySQL  │
│Service │         │Service│         │Database│
│Port:   │         │Port:  │         │Port:  │
│5003    │         │5005   │         │3306   │
└───────┘         └───────┘         └───────┘
```

### Service Details

| Service | Purpose | Technology | Port | Database |
|---------|---------|------------|------|----------|
| 🎫 **TicketManagementService** | Core ticket CRUD operations | ASP.NET Core 9.0 | 5001 | MySQL |
| 🤖 **AICategorizationService** | ML-powered ticket classification | ML.NET, ASP.NET Core | 5002 | None (ML Models) |
| ⚡ **PrioritizationService** | AI-driven priority assignment | ASP.NET Core 9.0 | 5003 | None |
| 👤 **UserManagementService** | Authentication & authorization | ASP.NET Core Identity | 5004 | MySQL |
| 📧 **NotificationService** | Email/SMS notifications | ASP.NET Core 9.0 | 5005 | None |
| 🌐 **API Gateway** | Service routing & security | Ocelot | 5000 | None |

### Infrastructure Services

| Service | Purpose | Port | Notes |
|---------|---------|------|-------|
| **MySQL** | Primary database | 3306 | Persistent storage for tickets and users |
| **Redis** | Caching & sessions | 6379 | Session management and response caching |
| **RabbitMQ** | Message queuing | 5672, 15672 | Inter-service communication |
| **Prometheus** | Metrics collection | 9090 | System and application metrics |
| **Grafana** | Metrics visualization | 3000 | Dashboards and monitoring |
| **Elasticsearch** | Log aggregation | 9200 | Centralized logging |
| **Kibana** | Log visualization | 5601 | Log search and analysis |

---

## 🚀 Getting Started

### Prerequisites

- 🖥️ **.NET 9.0 SDK** or later
- 🐳 **Docker Desktop** with Docker Compose
- ☸️ **Kubernetes** cluster (optional, for production)
- 🗄️ **MySQL** server (optional, for local development)
- 🛠️ **Git** for version control

### Quick Start

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/ai-customer-support-ticketing.git
   cd ai-customer-support-ticketing
   ```

2. **Navigate to the project directory**
   ```bash
   cd CustomerSupportTicketingSystem
   ```

3. **Start the system with Docker Compose**
   ```bash
   docker-compose up --build
   ```

4. **Access the system**
   - 🌐 API Gateway: http://localhost:5000
   - 📚 Swagger UI: http://localhost:5000/swagger
   - 📊 Health Dashboard: http://localhost:5000/health
   - 🎫 Ticket Service: http://localhost:5001
   - 🤖 AI Service: http://localhost:5002

### Development Setup

1. **Build the solution**
   ```bash
   dotnet build
   ```

2. **Run tests**
   ```bash
   dotnet test
   ```

3. **Start individual services**
   ```bash
   # Ticket Management Service
   cd src/TicketManagementService
   dotnet run
   
   # AI Categorization Service
   cd src/AICategorizationService
   dotnet run
   ```

---

## 📖 API Documentation

### Ticket Management Service

#### Base URL: `http://localhost:5001/api/tickets`

| Method | Endpoint | Description | Request Body | Response |
|--------|----------|-------------|--------------|----------|
| `GET` | `/` | Get all tickets | None | `List<Ticket>` |
| `GET` | `/{id}` | Get ticket by ID | None | `Ticket` |
| `POST` | `/` | Create new ticket | `CreateTicketDto` | `Ticket` |
| `PUT` | `/{id}` | Update ticket | `UpdateTicketDto` | `Ticket` |
| `DELETE` | `/{id}` | Delete ticket | None | `204 No Content` |
| `GET` | `/status/{status}` | Get tickets by status | None | `List<Ticket>` |
| `GET` | `/priority/{priority}` | Get tickets by priority | None | `List<Ticket>` |
| `GET` | `/search?q={term}` | Search tickets | None | `List<Ticket>` |
| `GET` | `/overdue` | Get overdue tickets | None | `List<Ticket>` |
| `POST` | `/{id}/assign` | Assign ticket to agent | `AssignTicketDto` | `Ticket` |
| `PATCH` | `/{id}/status` | Update ticket status | `UpdateStatusDto` | `Ticket` |
| `POST` | `/{id}/resolve` | Resolve ticket | `ResolveTicketDto` | `Ticket` |
| `GET` | `/stats/status-counts` | Get status counts | None | `Dictionary<Status, int>` |
| `GET` | `/ai/analysis` | Get tickets for AI analysis | None | `List<Ticket>` |

#### Example Request - Create Ticket

```json
POST /api/tickets
Content-Type: application/json

{
  "title": "Login issue with mobile app",
  "description": "I cannot log in to the mobile app. Getting error message 'Invalid credentials'",
  "customerEmail": "customer@example.com",
  "customerName": "John Doe",
  "category": "Technical",
  "subCategory": "Authentication",
  "estimatedHours": 2,
  "tags": "mobile,login,authentication"
}
```

#### Example Response

```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "title": "Login issue with mobile app",
  "description": "I cannot log in to the mobile app. Getting error message 'Invalid credentials'",
  "status": "Open",
  "priority": "Medium",
  "category": "Technical",
  "subCategory": "Authentication",
  "customerEmail": "customer@example.com",
  "customerName": "John Doe",
  "createdAt": "2024-01-15T10:30:00Z",
  "estimatedHours": 2,
  "tags": "mobile,login,authentication"
}
```

### AI Categorization Service

#### Base URL: `http://localhost:5002/api/aicategorization`

| Method | Endpoint | Description | Request Body | Response |
|--------|----------|-------------|--------------|----------|
| `POST` | `/analyze` | Analyze single ticket | `TicketAnalysisRequest` | `TicketAnalysisResponse` |
| `POST` | `/analyze/batch` | Analyze multiple tickets | `BatchAnalysisRequest` | `BatchAnalysisResponse` |
| `POST` | `/train` | Train ML model | `TrainModelRequest` | Success message |
| `POST` | `/update` | Update ML model | `UpdateModelRequest` | Success message |
| `GET` | `/model/info` | Get model information | None | `ModelInfo` |
| `POST` | `/evaluate` | Evaluate model performance | `EvaluateModelRequest` | `ModelEvaluationResponse` |
| `GET` | `/categories` | Get available categories | None | `List<string>` |
| `GET` | `/categories/{category}/subcategories` | Get sub-categories | None | `List<string>` |
| `GET` | `/ready` | Check if model is ready | None | `ModelReadyResponse` |
| `POST` | `/probabilities/categories` | Get category probabilities | `TextAnalysisRequest` | `Dictionary<string, double>` |
| `POST` | `/probabilities/subcategories` | Get sub-category probabilities | `SubCategoryAnalysisRequest` | `Dictionary<string, double>` |

#### Example Request - Analyze Ticket

```json
POST /api/aicategorization/analyze
Content-Type: application/json

{
  "ticketId": "123e4567-e89b-12d3-a456-426614174000",
  "title": "Login issue with mobile app",
  "description": "I cannot log in to the mobile app. Getting error message 'Invalid credentials'",
  "customerEmail": "customer@example.com",
  "category": "Technical",
  "subCategory": "Authentication"
}
```

#### Example Response

```json
{
  "ticketId": "123e4567-e89b-12d3-a456-426614174000",
  "predictedCategory": "Technical",
  "categoryConfidence": 0.95,
  "predictedSubCategory": "Authentication",
  "subCategoryConfidence": 0.88,
  "suggestedTags": ["technical", "mobile", "login", "authentication"],
  "overallConfidence": 0.915,
  "modelVersion": "1.0.0",
  "analysisTimestamp": "2024-01-15T10:30:00Z",
  "categoryProbabilities": {
    "Technical": 0.95,
    "Billing": 0.02,
    "General": 0.03
  },
  "subCategoryProbabilities": {
    "Authentication": 0.88,
    "Software": 0.08,
    "Hardware": 0.04
  }
}
```

---

## 🚀 Deployment

### Docker Compose Deployment

The system includes a comprehensive `docker-compose.yaml` file that sets up all services:

```bash
# Start all services
docker-compose up -d

# Start with specific profiles
docker-compose --profile monitoring up -d
docker-compose --profile training up -d

# View logs
docker-compose logs -f

# Stop all services
docker-compose down
```

### Kubernetes Deployment

For production deployments, use the Kubernetes manifests in the `kubernetes/` directory:

```bash
# Apply all manifests
kubectl apply -f kubernetes/

# Check deployment status
kubectl get pods

# Access services
kubectl port-forward svc/api-gateway 5000:80
```

### Environment Variables

Configure the system using environment variables:

```bash
# Database
ConnectionStrings__DefaultConnection=Server=mysql;Database=CustomerSupportTickets;User=root;Password=password;Port=3306;

# AI Service
MODEL_OUTPUT_PATH=/app/Models

# Notification Service
SENDGRID_API_KEY=your_sendgrid_api_key
TWILIO_ACCOUNT_SID=your_twilio_account_sid
TWILIO_AUTH_TOKEN=your_twilio_auth_token

# Monitoring
PROMETHEUS_ENDPOINT=http://prometheus:9090
GRAFANA_ENDPOINT=http://grafana:3000
```

---

## ⚙️ Configuration

### Database Configuration

The system uses MySQL as the primary database. Configure the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CustomerSupportTickets;User=root;Password=password;Port=3306;"
  }
}
```

### ML Model Configuration

AI models are stored in the `Models/` directory. The system automatically loads models on startup:

- `category_model.zip` - Category classification model
- `subcategory_model.zip` - Sub-category classification model

### Logging Configuration

Configure logging levels in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

---

## 📊 Monitoring & Logging

### Health Checks

All services include health check endpoints:

- `/health` - Basic health status
- `/health/ready` - Service readiness
- `/health/live` - Service liveness

### Metrics Collection

Prometheus collects metrics from all services:

- Request rates and latencies
- Error rates and response codes
- Database connection status
- ML model performance metrics

### Logging

Centralized logging with Elasticsearch and Kibana:

- Structured logging in JSON format
- Correlation IDs for request tracing
- Log aggregation and search
- Performance monitoring

### Dashboards

Grafana provides pre-configured dashboards:

- System overview and health
- Service performance metrics
- Database performance
- ML model accuracy and performance

---

## 🛠️ Development

### Project Structure

```
CustomerSupportTicketingSystem/
├── src/                           # Source code
│   ├── TicketManagementService/   # Core ticket operations
│   ├── AICategorizationService/   # ML-powered categorization
│   ├── PrioritizationService/     # AI-driven prioritization
│   ├── UserManagementService/     # Authentication & authorization
│   └── NotificationService/       # Email/SMS notifications
├── tests/                         # Unit and integration tests
├── api-gateway/                   # Ocelot API gateway
├── docker/                        # Docker configurations
├── kubernetes/                    # Kubernetes manifests
├── docs/                          # Documentation
└── build/                         # Build artifacts
```

### Adding New Services

1. **Create service project**
   ```bash
   dotnet new webapi -n NewService -o ./src/NewService
   ```

2. **Add to solution**
   ```bash
   dotnet sln add ./src/NewService/NewService.csproj
   ```

3. **Update docker-compose.yaml**
   ```yaml
   new-service:
     build:
       context: ./src/NewService
       dockerfile: Dockerfile
     ports:
       - "5006:80"
   ```

4. **Update API Gateway routing**

### Testing

Run tests for all services:

```bash
# All tests
dotnet test

# Specific service
dotnet test tests/TicketManagementService.Tests/

# With coverage
dotnet test --collect:"XPlat Code Coverage"
```

---

## 🔧 Troubleshooting

### Common Issues

#### Service Won't Start

1. **Check dependencies**
   ```bash
   docker-compose ps
   docker-compose logs service-name
   ```

2. **Verify ports**
   ```bash
   netstat -an | grep :5000
   ```

3. **Check database connection**
   ```bash
   docker-compose exec mysql mysql -u root -p
   ```

#### ML Model Issues

1. **Verify model files exist**
   ```bash
   ls -la src/AICategorizationService/Models/
   ```

2. **Check model loading logs**
   ```bash
   docker-compose logs ai-categorization-service
   ```

3. **Retrain models if needed**
   ```bash
   curl -X POST http://localhost:5002/api/aicategorization/train \
     -H "Content-Type: application/json" \
     -d '{"trainingDataPath": "/path/to/data.csv"}'
   ```

#### Performance Issues

1. **Check resource usage**
   ```bash
   docker stats
   ```

2. **Monitor database performance**
   ```bash
   docker-compose exec mysql mysql -u root -p -e "SHOW PROCESSLIST;"
   ```

3. **Review application logs**
   ```bash
   docker-compose logs -f --tail=100
   ```

### Debug Mode

Enable debug logging:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Debug"
    }
  }
}
```

### Support

For additional support:

- 📖 Check this documentation
- 🐛 Report issues on GitHub
- 💬 Join community discussions
- 📧 Contact the development team

---

## 📄 License

This project is licensed under the MIT License. See the [LICENSE](../LICENSE) file for details.

---

## 🙏 Acknowledgments

- 🎯 **ML.NET Team** for the excellent .NET machine learning framework
- 🐳 **Docker Community** for containerization tools
- ☸️ **Kubernetes Community** for orchestration platform
- 👥 **Open Source Community** for continuous innovation

---

*Last updated: January 2024*
