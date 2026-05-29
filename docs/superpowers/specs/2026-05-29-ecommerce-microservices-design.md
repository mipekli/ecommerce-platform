# Microservice Based E-Commerce Order & Inventory Management System

## Teknolojiler
- **Backend:** ASP.NET Core 9 Web API, EF Core, SQL Server, Redis, JWT, RabbitMQ, Serilog, FluentValidation, AutoMapper
- **Frontend:** React + Vite, TypeScript, TailwindCSS, React Query, Axios, Zustand
- **Infra:** Docker, Docker Compose

## Mimari
- Microservice Architecture
- API Gateway (YARP)
- Clean Architecture (Domain/Application/Infrastructure/API)
- CQRS Pattern
- Repository Pattern
- SOLID Principles

## Microservices

### 1. Identity Service (:5001)
- Register/Login (JWT + Refresh Token)
- User management
- SQL Server

### 2. Product Service (:5002)
- CRUD products
- Redis cache (cache-aside)
- SQL Server

### 3. Order Service (:5003)
- Create/Get orders
- Publishes OrderCreated event to RabbitMQ
- SQL Server

### 4. Inventory Service (:5004)
- Consumes OrderCreated event
- Stock management
- SQL Server

### 5. Notification Service (:5005)
- Consumes events
- Simulates email/push notification

### API Gateway (:5000)
- YARP reverse proxy
- Routes to services
- JWT validation

## Service Communication
- **Senkron:** Gateway → Services (REST/HTTP)
- **Asenkron:** RabbitMQ (OrderCreated → StockUpdate → Notification)

## Cache Strategy
- Product list/detail cached in Redis (TTL: 5 min)
- Cache-aside pattern
- Invalidation on write

## Databases
- SQL Server with proper indexing
- Stored procedures
- EF Core migrations

## Frontend Pages
- Login/Register, Dashboard, Product List/Detail, Cart, Checkout, Orders, Admin

## Docker
- 8 containers: sql-server, redis, rabbitmq, 5 services, api-gateway, frontend
