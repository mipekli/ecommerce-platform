# E-Commerce Microservice Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use subagent-driven-development (recommended) or executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build production-grade microservice-based e-commerce system with 5 microservices, API Gateway, and React frontend.

**Architecture:** Clean Architecture per service (Domain/Application/Infrastructure/API), CQRS, event-driven via RabbitMQ, Redis caching, SQL Server persistence.

**Tech Stack:** ASP.NET Core 9, EF Core, SQL Server, Redis, RabbitMQ, JWT, React+Vite+TypeScript+TailwindCSS

---

## Task 0: Scaffold — Create Directory Structure, Projects, and Solution

**Amaç:** Proje dizin yapısını oluşturmak, her mikroservis için Clean Architecture katmanlarını (Domain, Application, Infrastructure, API) kurmak ve .sln dosyasıyla tüm projeleri birbirine bağlamak.

**Dosyalar:**
- Directory tree (created via CLI)
- `Directory.Build.props`
- All `.csproj` files
- `ECommercePlatform.sln`

### Step 0.1: Create Directory Structure

```bash
#!/bin/bash
# Run from repository root
mkdir -p ecommerce-platform/src
cd ecommerce-platform/src

# Building Blocks
mkdir -p BuildingBlocks/Shared
mkdir -p BuildingBlocks/Shared.Messaging
mkdir -p BuildingBlocks/Shared.Caching

# Identity Service
mkdir -p Services/Identity/Identity.Domain/{Entities,Interfaces}
mkdir -p Services/Identity/Identity.Application/{Commands/Register,Commands/Login,Queries/GetProfile,DTOs,Interfaces}
mkdir -p Services/Identity/Identity.Infrastructure/{Data,Repositories,Services}
mkdir -p Services/Identity/Identity.API/{Controllers,Middleware}

# Product Service
mkdir -p Services/Product/Product.Domain/{Entities,Interfaces}
mkdir -p Services/Product/Product.Application/{Commands,Queries,DTOs}
mkdir -p Services/Product/Product.Infrastructure/{Data,Repositories}
mkdir -p Services/Product/Product.API/{Controllers}

# Order Service
mkdir -p Services/Order/Order.Domain/{Entities,Interfaces,ValueObjects,Events}
mkdir -p Services/Order/Order.Application/{Commands,Queries,DTOs,IntegrationEvents}
mkdir -p Services/Order/Order.Infrastructure/{Data,Repositories}
mkdir -p Services/Order/Order.API/{Controllers}

# Inventory Service
mkdir -p Services/Inventory/Inventory.Domain/{Entities,Interfaces}
mkdir -p Services/Inventory/Inventory.Application/{IntegrationEvents/OrderCreated}
mkdir -p Services/Inventory/Inventory.Infrastructure/{Data,Repositories}
mkdir -p Services/Inventory/Inventory.API/{Controllers,BackgroundServices}

# Notification Service
mkdir -p Services/Notification/Notification.Infrastructure/{Services}
mkdir -p Services/Notification/Notification.API/{BackgroundServices}

# API Gateway
mkdir -p ApiGateway

# Docker
mkdir -p docker/sql/init

# Frontend
mkdir -p frontend/src/{components,hooks,pages/{auth,products,cart,orders,admin},stores,types,api}
```

### Step 0.2: Directory.Build.props

**Dosya:** `Directory.Build.props`

```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    <AnalysisLevel>latest</AnalysisLevel>
  </PropertyGroup>
</Project>
```

### Step 0.3: Shared.csproj

**Dosya:** `src/BuildingBlocks/Shared/Shared.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>BuildingBlocks.Shared</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="MediatR" Version="12.4.1" />
    <PackageReference Include="FluentValidation" Version="11.11.0" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="9.0.0" />
  </ItemGroup>
</Project>
```

### Step 0.4: Shared.Messaging.csproj

**Dosya:** `src/BuildingBlocks/Shared.Messaging/Shared.Messaging.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>BuildingBlocks.Shared.Messaging</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="RabbitMQ.Client" Version="6.8.1" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="9.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="9.0.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="9.0.0" />
    <PackageReference Include="Polly" Version="8.5.0" />
    <PackageReference Include="System.Text.Json" Version="9.0.0" />
  </ItemGroup>
</Project>
```

### Step 0.5: Shared.Caching.csproj

**Dosya:** `src/BuildingBlocks/Shared.Caching/Shared.Caching.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>BuildingBlocks.Shared.Caching</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Caching.StackExchangeRedis" Version="9.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="9.0.0" />
  </ItemGroup>
</Project>
```

### Step 0.6: Identity.Domain.csproj

**Dosya:** `src/Services/Identity/Identity.Domain/Identity.Domain.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>Identity.Domain</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\..\BuildingBlocks\Shared\Shared.csproj" />
  </ItemGroup>
</Project>
```

### Step 0.7: Identity.Application.csproj

**Dosya:** `src/Services/Identity/Identity.Application/Identity.Application.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>Identity.Application</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\Identity.Domain\Identity.Domain.csproj" />
  </ItemGroup>
</Project>
```

### Step 0.8: Identity.Infrastructure.csproj

**Dosya:** `src/Services/Identity/Identity.Infrastructure/Identity.Infrastructure.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>Identity.Infrastructure</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\Identity.Application\Identity.Application.csproj" />
  </ItemGroup>
</Project>
```

### Step 0.9: Identity.API.csproj

**Dosya:** `src/Services/Identity/Identity.API/Identity.API.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <RootNamespace>Identity.API</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Swashbuckle.AspNetCore" Version="7.2.0" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\Identity.Infrastructure\Identity.Infrastructure.csproj" />
  </ItemGroup>
</Project>
```

### Step 0.10: Product.Domain.csproj

**Dosya:** `src/Services/Product/Product.Domain/Product.Domain.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>Product.Domain</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\..\BuildingBlocks\Shared\Shared.csproj" />
  </ItemGroup>
</Project>
```

### Step 0.11: Product.Application.csproj

**Dosya:** `src/Services/Product/Product.Application/Product.Application.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>Product.Application</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\Product.Domain\Product.Domain.csproj" />
    <ProjectReference Include="..\..\..\BuildingBlocks\Shared.Caching\Shared.Caching.csproj" />
  </ItemGroup>
</Project>
```

### Step 0.12: Product.Infrastructure.csproj

**Dosya:** `src/Services/Product/Product.Infrastructure/Product.Infrastructure.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>Product.Infrastructure</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\Product.Application\Product.Application.csproj" />
  </ItemGroup>
</Project>
```

### Step 0.13: Product.API.csproj

**Dosya:** `src/Services/Product/Product.API/Product.API.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <RootNamespace>Product.API</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Swashbuckle.AspNetCore" Version="7.2.0" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\Product.Infrastructure\Product.Infrastructure.csproj" />
  </ItemGroup>
</Project>
```

### Step 0.14: Order.Domain.csproj

**Dosya:** `src/Services/Order/Order.Domain/Order.Domain.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>Order.Domain</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\..\BuildingBlocks\Shared\Shared.csproj" />
    <ProjectReference Include="..\..\..\BuildingBlocks\Shared.Messaging\Shared.Messaging.csproj" />
  </ItemGroup>
</Project>
```

### Step 0.15: Order.Application.csproj

**Dosya:** `src/Services/Order/Order.Application/Order.Application.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>Order.Application</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\Order.Domain\Order.Domain.csproj" />
    <ProjectReference Include="..\..\..\BuildingBlocks\Shared.Messaging\Shared.Messaging.csproj" />
  </ItemGroup>
</Project>
```

### Step 0.16: Order.Infrastructure.csproj

**Dosya:** `src/Services/Order/Order.Infrastructure/Order.Infrastructure.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>Order.Infrastructure</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\Order.Application\Order.Application.csproj" />
  </ItemGroup>
</Project>
```

### Step 0.17: Order.API.csproj

**Dosya:** `src/Services/Order/Order.API/Order.API.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <RootNamespace>Order.API</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Swashbuckle.AspNetCore" Version="7.2.0" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\Order.Infrastructure\Order.Infrastructure.csproj" />
  </ItemGroup>
</Project>
```

### Step 0.18: Inventory.Domain.csproj

**Dosya:** `src/Services/Inventory/Inventory.Domain/Inventory.Domain.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>Inventory.Domain</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\..\BuildingBlocks\Shared\Shared.csproj" />
  </ItemGroup>
</Project>
```

### Step 0.19: Inventory.Application.csproj

**Dosya:** `src/Services/Inventory/Inventory.Application/Inventory.Application.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>Inventory.Application</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\Inventory.Domain\Inventory.Domain.csproj" />
    <ProjectReference Include="..\..\..\BuildingBlocks\Shared.Messaging\Shared.Messaging.csproj" />
  </ItemGroup>
</Project>
```

### Step 0.20: Inventory.Infrastructure.csproj

**Dosya:** `src/Services/Inventory/Inventory.Infrastructure/Inventory.Infrastructure.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <RootNamespace>Inventory.Infrastructure</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\Inventory.Application\Inventory.Application.csproj" />
  </ItemGroup>
</Project>
```

### Step 0.21: Inventory.API.csproj

**Dosya:** `src/Services/Inventory/Inventory.API/Inventory.API.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <RootNamespace>Inventory.API</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Swashbuckle.AspNetCore" Version="7.2.0" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\Inventory.Infrastructure\Inventory.Infrastructure.csproj" />
  </ItemGroup>
</Project>
```

### Step 0.22: Notification.API.csproj

**Dosya:** `src/Services/Notification/Notification.API/Notification.API.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <RootNamespace>Notification.API</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="9.0.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="7.2.0" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\..\BuildingBlocks\Shared.Messaging\Shared.Messaging.csproj" />
  </ItemGroup>
</Project>
```

### Step 0.23: ApiGateway.csproj

**Dosya:** `src/ApiGateway/ApiGateway.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <RootNamespace>ApiGateway</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Yarp.ReverseProxy" Version="2.2.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="7.2.0" />
  </ItemGroup>
</Project>
```

### Step 0.24: Solution File

**Dosya:** `src/ECommercePlatform.sln`

```
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.0.31903.59
MinimumVisualStudioVersion = 10.0.40219.1
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Shared", "BuildingBlocks\Shared\Shared.csproj", "{A1B2C3D4-E5F6-7890-ABCD-EF1234567890}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Shared.Messaging", "BuildingBlocks\Shared.Messaging\Shared.Messaging.csproj", "{B2C3D4E5-F6A7-8901-BCDE-F12345678901}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Shared.Caching", "BuildingBlocks\Shared.Caching\Shared.Caching.csproj", "{C3D4E5F6-A7B8-9012-CDEF-123456789012}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Identity.Domain", "Services\Identity\Identity.Domain\Identity.Domain.csproj", "{D4E5F6A7-B8C9-0123-DEF1-234567890123}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Identity.Application", "Services\Identity\Identity.Application\Identity.Application.csproj", "{E5F6A7B8-C9D0-1234-EF12-345678901234}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Identity.Infrastructure", "Services\Identity\Identity.Infrastructure\Identity.Infrastructure.csproj", "{F6A7B8C9-D0E1-2345-F123-456789012345}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Identity.API", "Services\Identity\Identity.API\Identity.API.csproj", "{A7B8C9D0-E1F2-3456-1234-567890123456}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Product.Domain", "Services\Product\Product.Domain\Product.Domain.csproj", "{B8C9D0E1-F2A3-4567-2345-678901234567}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Product.Application", "Services\Product\Product.Application\Product.Application.csproj", "{C9D0E1F2-A3B4-5678-3456-789012345678}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Product.Infrastructure", "Services\Product\Product.Infrastructure\Product.Infrastructure.csproj", "{D0E1F2A3-B4C5-6789-4567-890123456789}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Product.API", "Services\Product\Product.API\Product.API.csproj", "{E1F2A3B4-C5D6-7890-5678-901234567890}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Order.Domain", "Services\Order\Order.Domain\Order.Domain.csproj", "{F2A3B4C5-D6E7-8901-6789-012345678901}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Order.Application", "Services\Order\Order.Application\Order.Application.csproj", "{A3B4C5D6-E7F8-9012-7890-123456789012}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Order.Infrastructure", "Services\Order\Order.Infrastructure\Order.Infrastructure.csproj", "{B4C5D6E7-F8A9-0123-8901-234567890123}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Order.API", "Services\Order\Order.API\Order.API.csproj", "{C5D6E7F8-A9B0-1234-9012-345678901234}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Inventory.Domain", "Services\Inventory\Inventory.Domain\Inventory.Domain.csproj", "{D6E7F8A9-B0C1-2345-0123-456789012345}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Inventory.Application", "Services\Inventory\Inventory.Application\Inventory.Application.csproj", "{E7F8A9B0-C1D2-3456-1234-567890123456}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Inventory.Infrastructure", "Services\Inventory\Inventory.Infrastructure\Inventory.Infrastructure.csproj", "{F8A9B0C1-D2E3-4567-2345-678901234567}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Inventory.API", "Services\Inventory\Inventory.API\Inventory.API.csproj", "{A9B0C1D2-E3F4-5678-3456-789012345678}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Notification.API", "Services\Notification\Notification.API\Notification.API.csproj", "{B0C1D2E3-F4A5-6789-4567-890123456789}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "ApiGateway", "ApiGateway\ApiGateway.csproj", "{C1D2E3F4-A5B6-7890-5678-901234567890}"
EndProject
Global
  GlobalSection(SolutionConfigurationPlatforms) = preSolution
    Debug|Any CPU = Debug|Any CPU
    Release|Any CPU = Release|Any CPU
  EndGlobalSection
  GlobalSection(ProjectConfigurationPlatforms) = postSolution
    {A1B2C3D4-E5F6-7890-ABCD-EF1234567890}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {A1B2C3D4-E5F6-7890-ABCD-EF1234567890}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {A1B2C3D4-E5F6-7890-ABCD-EF1234567890}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {A1B2C3D4-E5F6-7890-ABCD-EF1234567890}.Release|Any CPU.Build.0 = Release|Any CPU
    {B2C3D4E5-F6A7-8901-BCDE-F12345678901}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {B2C3D4E5-F6A7-8901-BCDE-F12345678901}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {B2C3D4E5-F6A7-8901-BCDE-F12345678901}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {B2C3D4E5-F6A7-8901-BCDE-F12345678901}.Release|Any CPU.Build.0 = Release|Any CPU
    {C3D4E5F6-A7B8-9012-CDEF-123456789012}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {C3D4E5F6-A7B8-9012-CDEF-123456789012}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {C3D4E5F6-A7B8-9012-CDEF-123456789012}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {C3D4E5F6-A7B8-9012-CDEF-123456789012}.Release|Any CPU.Build.0 = Release|Any CPU
    {D4E5F6A7-B8C9-0123-DEF1-234567890123}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {D4E5F6A7-B8C9-0123-DEF1-234567890123}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {D4E5F6A7-B8C9-0123-DEF1-234567890123}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {D4E5F6A7-B8C9-0123-DEF1-234567890123}.Release|Any CPU.Build.0 = Release|Any CPU
    {E5F6A7B8-C9D0-1234-EF12-345678901234}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {E5F6A7B8-C9D0-1234-EF12-345678901234}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {E5F6A7B8-C9D0-1234-EF12-345678901234}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {E5F6A7B8-C9D0-1234-EF12-345678901234}.Release|Any CPU.Build.0 = Release|Any CPU
    {F6A7B8C9-D0E1-2345-F123-456789012345}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {F6A7B8C9-D0E1-2345-F123-456789012345}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {F6A7B8C9-D0E1-2345-F123-456789012345}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {F6A7B8C9-D0E1-2345-F123-456789012345}.Release|Any CPU.Build.0 = Release|Any CPU
    {A7B8C9D0-E1F2-3456-1234-567890123456}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {A7B8C9D0-E1F2-3456-1234-567890123456}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {A7B8C9D0-E1F2-3456-1234-567890123456}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {A7B8C9D0-E1F2-3456-1234-567890123456}.Release|Any CPU.Build.0 = Release|Any CPU
    {B8C9D0E1-F2A3-4567-2345-678901234567}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {B8C9D0E1-F2A3-4567-2345-678901234567}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {B8C9D0E1-F2A3-4567-2345-678901234567}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {B8C9D0E1-F2A3-4567-2345-678901234567}.Release|Any CPU.Build.0 = Release|Any CPU
    {C9D0E1F2-A3B4-5678-3456-789012345678}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {C9D0E1F2-A3B4-5678-3456-789012345678}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {C9D0E1F2-A3B4-5678-3456-789012345678}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {C9D0E1F2-A3B4-5678-3456-789012345678}.Release|Any CPU.Build.0 = Release|Any CPU
    {D0E1F2A3-B4C5-6789-4567-890123456789}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {D0E1F2A3-B4C5-6789-4567-890123456789}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {D0E1F2A3-B4C5-6789-4567-890123456789}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {D0E1F2A3-B4C5-6789-4567-890123456789}.Release|Any CPU.Build.0 = Release|Any CPU
    {E1F2A3B4-C5D6-7890-5678-901234567890}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {E1F2A3B4-C5D6-7890-5678-901234567890}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {E1F2A3B4-C5D6-7890-5678-901234567890}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {E1F2A3B4-C5D6-7890-5678-901234567890}.Release|Any CPU.Build.0 = Release|Any CPU
    {F2A3B4C5-D6E7-8901-6789-012345678901}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {F2A3B4C5-D6E7-8901-6789-012345678901}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {F2A3B4C5-D6E7-8901-6789-012345678901}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {F2A3B4C5-D6E7-8901-6789-012345678901}.Release|Any CPU.Build.0 = Release|Any CPU
    {A3B4C5D6-E7F8-9012-7890-123456789012}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {A3B4C5D6-E7F8-9012-7890-123456789012}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {A3B4C5D6-E7F8-9012-7890-123456789012}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {A3B4C5D6-E7F8-9012-7890-123456789012}.Release|Any CPU.Build.0 = Release|Any CPU
    {B4C5D6E7-F8A9-0123-8901-234567890123}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {B4C5D6E7-F8A9-0123-8901-234567890123}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {B4C5D6E7-F8A9-0123-8901-234567890123}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {B4C5D6E7-F8A9-0123-8901-234567890123}.Release|Any CPU.Build.0 = Release|Any CPU
    {C5D6E7F8-A9B0-1234-9012-345678901234}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {C5D6E7F8-A9B0-1234-9012-345678901234}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {C5D6E7F8-A9B0-1234-9012-345678901234}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {C5D6E7F8-A9B0-1234-9012-345678901234}.Release|Any CPU.Build.0 = Release|Any CPU
    {D6E7F8A9-B0C1-2345-0123-456789012345}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {D6E7F8A9-B0C1-2345-0123-456789012345}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {D6E7F8A9-B0C1-2345-0123-456789012345}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {D6E7F8A9-B0C1-2345-0123-456789012345}.Release|Any CPU.Build.0 = Release|Any CPU
    {E7F8A9B0-C1D2-3456-1234-567890123456}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {E7F8A9B0-C1D2-3456-1234-567890123456}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {E7F8A9B0-C1D2-3456-1234-567890123456}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {E7F8A9B0-C1D2-3456-1234-567890123456}.Release|Any CPU.Build.0 = Release|Any CPU
    {F8A9B0C1-D2E3-4567-2345-678901234567}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {F8A9B0C1-D2E3-4567-2345-678901234567}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {F8A9B0C1-D2E3-4567-2345-678901234567}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {F8A9B0C1-D2E3-4567-2345-678901234567}.Release|Any CPU.Build.0 = Release|Any CPU
    {A9B0C1D2-E3F4-5678-3456-789012345678}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {A9B0C1D2-E3F4-5678-3456-789012345678}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {A9B0C1D2-E3F4-5678-3456-789012345678}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {A9B0C1D2-E3F4-5678-3456-789012345678}.Release|Any CPU.Build.0 = Release|Any CPU
    {B0C1D2E3-F4A5-6789-4567-890123456789}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {B0C1D2E3-F4A5-6789-4567-890123456789}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {B0C1D2E3-F4A5-6789-4567-890123456789}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {B0C1D2E3-F4A5-6789-4567-890123456789}.Release|Any CPU.Build.0 = Release|Any CPU
    {C1D2E3F4-A5B6-7890-5678-901234567890}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {C1D2E3F4-A5B6-7890-5678-901234567890}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {C1D2E3F4-A5B6-7890-5678-901234567890}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {C1D2E3F4-A5B6-7890-5678-901234567890}.Release|Any CPU.Build.0 = Release|Any CPU
  EndGlobalSection
EndGlobal
```

---

## Task 1: BuildingBlocks.Shared — BaseEntity, ValueObject, BaseEvent

**Amaç:** Tüm mikroservislerde kullanılacak ortak domain temel sınıflarını oluşturmak.

**Dosyalar:**
- `src/BuildingBlocks/Shared/BaseEntity.cs`
- `src/BuildingBlocks/Shared/ValueObject.cs`
- `src/BuildingBlocks/Shared/BaseEvent.cs`

### BaseEntity.cs

**Dosya:** `src/BuildingBlocks/Shared/BaseEntity.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Shared;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }
    public bool IsDeleted { get; protected set; }

    private readonly List<BaseEvent> _domainEvents = [];
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
}
```

### ValueObject.cs

**Dosya:** `src/BuildingBlocks/Shared/ValueObject.cs`

```csharp
namespace BuildingBlocks.Shared;

public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        return obj is ValueObject other && ValuesAreEqual(other);
    }

    public bool Equals(ValueObject? other)
    {
        return other is not null && ValuesAreEqual(other);
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    private bool ValuesAreEqual(ValueObject other)
    {
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !Equals(left, right);
    }
}
```

### BaseEvent.cs

**Dosya:** `src/BuildingBlocks/Shared/BaseEvent.cs`

```csharp
using MediatR;

namespace BuildingBlocks.Shared;

public abstract record BaseEvent : INotification
{
    public Guid EventId { get; protected set; } = Guid.NewGuid();
    public DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
    public string EventType => GetType().Name;
}
```

---

## Task 2: BuildingBlocks.Shared.Messaging — Event Bus and Integration Events

**Amaç:** RabbitMQ tabanlı event bus altyapısını kurmak, servisler arası iletişimi sağlamak.

**Dosyalar:**
- `src/BuildingBlocks/Shared.Messaging/IEventBus.cs`
- `src/BuildingBlocks/Shared.Messaging/IntegrationEvent.cs`
- `src/BuildingBlocks/Shared.Messaging/EventBusRabbitMQ.cs`
- `src/BuildingBlocks/Shared.Messaging/DependencyInjection.cs`

### IEventBus.cs

**Dosya:** `src/BuildingBlocks/Shared.Messaging/IEventBus.cs`

```csharp
namespace BuildingBlocks.Shared.Messaging;

public interface IEventBus
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IntegrationEvent;
    Task SubscribeAsync<T, THandler>()
        where T : IntegrationEvent
        where THandler : IIntegrationEventHandler<T>;
}
```

### IntegrationEvent.cs

**Dosya:** `src/BuildingBlocks/Shared.Messaging/IntegrationEvent.cs`

```csharp
namespace BuildingBlocks.Shared.Messaging;

public abstract record IntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public string EventType => GetType().Name;
}

public interface IIntegrationEventHandler<in T> where T : IntegrationEvent
{
    Task HandleAsync(T @event, CancellationToken cancellationToken = default);
}
```

### EventBusRabbitMQ.cs

**Dosya:** `src/BuildingBlocks/Shared.Messaging/EventBusRabbitMQ.cs`

```csharp
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace BuildingBlocks.Shared.Messaging;

public class EventBusRabbitMQ : IEventBus, IDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<EventBusRabbitMQ> _logger;
    private readonly IConfiguration _configuration;
    private readonly Dictionary<string, Type> _eventTypes = [];
    private readonly Dictionary<string, object> _handlers = [];
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly string _exchangeName;
    private readonly string _queueName;
    private readonly int _retryCount;
    private bool _disposed;

    public EventBusRabbitMQ(
        IConnectionFactory connectionFactory,
        ILogger<EventBusRabbitMQ> logger,
        IConfiguration configuration)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
        _configuration = configuration;
        _exchangeName = configuration.GetValue<string>("EventBus:ExchangeName") ?? "ecommerce_exchange";
        _queueName = configuration.GetValue<string>("EventBus:QueueName") ?? "ecommerce_queue";
        _retryCount = configuration.GetValue<int>("EventBus:RetryCount", 5);
    }

    private async Task EnsureConnectionAsync()
    {
        if (_connection is { IsOpen: true })
            return;

        var policy = Policy.Handle<BrokerUnreachableException>()
            .Or<IOException>()
            .WaitAndRetryAsync(
                _retryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (ex, time) =>
                {
                    _logger.LogWarning(ex, "Could not connect to RabbitMQ after {TimeOut}s", $"{time.TotalSeconds:N1}");
                });

        _connection = await policy.ExecuteAsync(async () =>
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            return connection;
        });

        _channel = await _connection.CreateChannelAsync();
        await _channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Direct, durable: true);
        await _channel.QueueDeclareAsync(_queueName, durable: true, exclusive: false, autoDelete: false);
        await _channel.QueueBindAsync(_queueName, _exchangeName, _queueName);
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IntegrationEvent
    {
        await EnsureConnectionAsync();

        var policy = Policy.Handle<BrokerUnreachableException>()
            .Or<IOException>()
            .WaitAndRetryAsync(_retryCount, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        await policy.ExecuteAsync(async () =>
        {
            var body = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType());
            var properties = new BasicProperties
            {
                Persistent = true,
                Type = @event.EventType
            };

            if (_channel is not null)
            {
                await _channel.BasicPublishAsync(
                    exchange: _exchangeName,
                    routingKey: _queueName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: cancellationToken);
            }
        });
    }

    public async Task SubscribeAsync<T, THandler>()
        where T : IntegrationEvent
        where THandler : IIntegrationEventHandler<T>
    {
        await EnsureConnectionAsync();

        var eventName = typeof(T).Name;
        _eventTypes[eventName] = typeof(T);

        if (_channel is not null)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var eventNameReceived = ea.BasicProperties.Type;
                var body = ea.Body.ToArray();

                if (_eventTypes.TryGetValue(eventNameReceived, out var eventType))
                {
                    var integrationEvent = JsonSerializer.Deserialize(body, eventType) as T;
                    if (integrationEvent is not null && _handlers.TryGetValue(eventNameReceived, out var handlerObj))
                    {
                        try
                        {
                            if (handlerObj is IIntegrationEventHandler<T> handler)
                            {
                                await handler.HandleAsync(integrationEvent);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error handling event {EventName}", eventNameReceived);
                        }
                    }
                }

                if (_channel is not null)
                {
                    await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            await _channel.BasicConsumeAsync(_queueName, autoAck: false, consumer: consumer);
        }
    }

    public void RegisterHandler<T>(IIntegrationEventHandler<T> handler) where T : IntegrationEvent
    {
        var eventName = typeof(T).Name;
        _handlers[eventName] = handler;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _channel?.CloseAsync();
        _connection?.CloseAsync();
    }
}
```

### DependencyInjection.cs

**Dosya:** `src/BuildingBlocks/Shared.Messaging/DependencyInjection.cs`

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace BuildingBlocks.Shared.Messaging;

public static class MessagingDependencyInjection
{
    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionFactory>(sp =>
        {
            var config = configuration.GetSection("EventBus");
            return new ConnectionFactory
            {
                HostName = config.GetValue<string>("HostName") ?? "localhost",
                Port = config.GetValue<int>("Port", 5672),
                UserName = config.GetValue<string>("UserName") ?? "guest",
                Password = config.GetValue<string>("Password") ?? "guest",
                VirtualHost = config.GetValue<string>("VirtualHost") ?? "/",
                DispatchConsumersAsync = true
            };
        });

        services.AddSingleton<IEventBus, EventBusRabbitMQ>();
        return services;
    }
}
```

---

## Task 3: BuildingBlocks.Shared.Caching — Redis Cache Service

**Amaç:** Redis tabanlı önbellekleme altyapısını oluşturmak.

**Dosyalar:**
- `src/BuildingBlocks/Shared.Caching/ICacheService.cs`
- `src/BuildingBlocks/Shared.Caching/CacheService.cs`
- `src/BuildingBlocks/Shared.Caching/DependencyInjection.cs`

### ICacheService.cs

**Dosya:** `src/BuildingBlocks/Shared.Caching/ICacheService.cs`

```csharp
namespace BuildingBlocks.Shared.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
}
```

### CacheService.cs

**Dosya:** `src/BuildingBlocks/Shared.Caching/CacheService.cs`

```csharp
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Shared.Caching;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheService> _logger;
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(15);

    public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var cached = await _cache.GetStringAsync(key, cancellationToken);
            if (cached is null) return null;
            return JsonSerializer.Deserialize<T>(cached);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache get failed for key {Key}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration
            };
            var serialized = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, serialized, options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache set failed for key {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache remove failed for key {Key}", key);
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var cached = await _cache.GetStringAsync(key, cancellationToken);
            return cached is not null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache exists check failed for key {Key}", key);
            return false;
        }
    }
}
```

### DependencyInjection.cs

**Dosya:** `src/BuildingBlocks/Shared.Caching/DependencyInjection.cs`

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Shared.Caching;

public static class CachingDependencyInjection
{
    public static IServiceCollection AddCacheService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        });
        services.AddSingleton<ICacheService, CacheService>();
        return services;
    }
}
```

---

## Task 4: Identity.Domain — User Entity and Repository Interface

**Amaç:** Identity mikroservisi domain katmanını oluşturmak: User entity, IUserRepository.

**Dosyalar:**
- `src/Services/Identity/Identity.Domain/Entities/User.cs`
- `src/Services/Identity/Identity.Domain/Interfaces/IUserRepository.cs`

### User.cs

**Dosya:** `src/Services/Identity/Identity.Domain/Entities/User.cs`

```csharp
using BuildingBlocks.Shared;

namespace Identity.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string PasswordHash { get; private set; }
    public string Role { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiry { get; private set; }

    private User() { }

    public User(string email, string firstName, string lastName, string passwordHash, string role = "Customer")
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        PasswordHash = passwordHash;
        Role = role;
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        MarkAsUpdated();
    }

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        MarkAsUpdated();
    }

    public void SetRefreshToken(string refreshToken, DateTime expiry)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiry = expiry;
        MarkAsUpdated();
    }

    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiry = null;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }
}
```

### IUserRepository.cs

**Dosya:** `src/Services/Identity/Identity.Domain/Interfaces/IUserRepository.cs`

```csharp
using Identity.Domain.Entities;

namespace Identity.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(User user, CancellationToken cancellationToken = default);
}
```

---

## Task 5: Identity.Application — CQRS Commands, Queries, DTOs, Validators, JwtService Interface

**Amaç:** Identity mikroservisi application katmanını oluşturmak: Register/Login komutları, GetProfile sorgusu, DTO'lar, validatörler, JwtService arayüzü.

**Dosyalar:**
- `src/Services/Identity/Identity.Application/DTOs/AuthDtos.cs`
- `src/Services/Identity/Identity.Application/Interfaces/IJwtService.cs`
- `src/Services/Identity/Identity.Application/Commands/Register/RegisterCommand.cs`
- `src/Services/Identity/Identity.Application/Commands/Register/RegisterCommandHandler.cs`
- `src/Services/Identity/Identity.Application/Commands/Register/RegisterCommandValidator.cs`
- `src/Services/Identity/Identity.Application/Commands/Login/LoginCommand.cs`
- `src/Services/Identity/Identity.Application/Commands/Login/LoginCommandHandler.cs`
- `src/Services/Identity/Identity.Application/Commands/Login/LoginCommandValidator.cs`
- `src/Services/Identity/Identity.Application/Queries/GetProfile/GetProfileQuery.cs`
- `src/Services/Identity/Identity.Application/Queries/GetProfile/GetProfileQueryHandler.cs`

### AuthDtos.cs

**Dosya:** `src/Services/Identity/Identity.Application/DTOs/AuthDtos.cs`

```csharp
namespace Identity.Application.DTOs;

public record RegisterRequest(string Email, string FirstName, string LastName, string Password, string ConfirmPassword);
public record LoginRequest(string Email, string Password);
public record RefreshTokenRequest(string RefreshToken);

public record AuthResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public UserDto User { get; init; } = null!;
}

public record UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}
```

### IJwtService.cs

**Dosya:** `src/Services/Identity/Identity.Application/Interfaces/IJwtService.cs`

```csharp
using Identity.Domain.Entities;

namespace Identity.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    (string Token, DateTime ExpiresAt) GenerateTokenPair(User user);
    Guid? ValidateToken(string token);
}
```

### RegisterCommand.cs

**Dosya:** `src/Services/Identity/Identity.Application/Commands/Register/RegisterCommand.cs`

```csharp
using MediatR;
using Identity.Application.DTOs;

namespace Identity.Application.Commands.Register;

public record RegisterCommand(string Email, string FirstName, string LastName, string Password) : IRequest<AuthResponse>;
```

### RegisterCommandHandler.cs

**Dosya:** `src/Services/Identity/Identity.Application/Commands/Register/RegisterCommandHandler.cs`

```csharp
using MediatR;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Application.DTOs;
using Identity.Application.Interfaces;

namespace Identity.Application.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public RegisterCommandHandler(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
            throw new InvalidOperationException("Bu e-posta adresi ile kayıtlı bir kullanıcı zaten var.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User(request.Email, request.FirstName, request.LastName, passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);

        var (token, expiresAt) = _jwtService.GenerateTokenPair(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        user.SetRefreshToken(refreshToken, expiresAt);
        await _userRepository.UpdateAsync(user, cancellationToken);

        return new AuthResponse
        {
            AccessToken = token,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            }
        };
    }
}
```

### RegisterCommandValidator.cs

**Dosya:** `src/Services/Identity/Identity.Application/Commands/Register/RegisterCommandValidator.cs`

```csharp
using FluentValidation;

namespace Identity.Application.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresi zorunludur.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad zorunludur.")
            .MaximumLength(100).WithMessage("Ad en fazla 100 karakter olabilir.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad zorunludur.")
            .MaximumLength(100).WithMessage("Soyad en fazla 100 karakter olabilir.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre zorunludur.")
            .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
            .Matches("[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
            .Matches("[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
            .Matches("[0-9]").WithMessage("Şifre en az bir rakam içermelidir.");
    }
}
```

### LoginCommand.cs

**Dosya:** `src/Services/Identity/Identity.Application/Commands/Login/LoginCommand.cs`

```csharp
using MediatR;
using Identity.Application.DTOs;

namespace Identity.Application.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;
```

### LoginCommandHandler.cs

**Dosya:** `src/Services/Identity/Identity.Application/Commands/Login/LoginCommandHandler.cs`

```csharp
using MediatR;
using Identity.Domain.Interfaces;
using Identity.Application.DTOs;
using Identity.Application.Interfaces;

namespace Identity.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("E-posta adresi veya şifre hatalı.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Hesabınız pasif durumda. Lütfen yöneticinizle iletişime geçin.");

        var (token, expiresAt) = _jwtService.GenerateTokenPair(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        user.SetRefreshToken(refreshToken, expiresAt);
        await _userRepository.UpdateAsync(user, cancellationToken);

        return new AuthResponse
        {
            AccessToken = token,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            }
        };
    }
}
```

### LoginCommandValidator.cs

**Dosya:** `src/Services/Identity/Identity.Application/Commands/Login/LoginCommandValidator.cs`

```csharp
using FluentValidation;

namespace Identity.Application.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresi zorunludur.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre zorunludur.");
    }
}
```

### GetProfileQuery.cs

**Dosya:** `src/Services/Identity/Identity.Application/Queries/GetProfile/GetProfileQuery.cs`

```csharp
using MediatR;
using Identity.Application.DTOs;

namespace Identity.Application.Queries.GetProfile;

public record GetProfileQuery(Guid UserId) : IRequest<UserDto>;
```

### GetProfileQueryHandler.cs

**Dosya:** `src/Services/Identity/Identity.Application/Queries/GetProfile/GetProfileQueryHandler.cs`

```csharp
using MediatR;
using Identity.Domain.Interfaces;
using Identity.Application.DTOs;

namespace Identity.Application.Queries.GetProfile;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, UserDto>
{
    private readonly IUserRepository _userRepository;

    public GetProfileQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            throw new KeyNotFoundException("Kullanıcı bulunamadı.");

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role
        };
    }
}
```

---

## Task 6: Identity.Infrastructure — DbContext, UserRepository, JwtService

**Amaç:** Identity mikroservisi infrastructure katmanını oluşturmak: EF Core DbContext, UserRepository, JwtService implementasyonu.

**Dosyalar:**
- `src/Services/Identity/Identity.Infrastructure/Data/IdentityDbContext.cs`
- `src/Services/Identity/Identity.Infrastructure/Repositories/UserRepository.cs`
- `src/Services/Identity/Identity.Infrastructure/Services/JwtService.cs`
- `src/Services/Identity/Identity.Infrastructure/DependencyInjection.cs`

### IdentityDbContext.cs

**Dosya:** `src/Services/Identity/Identity.Infrastructure/Data/IdentityDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Identity.Domain.Entities;

namespace Identity.Infrastructure.Data;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            entity.Property(e => e.RefreshToken).HasMaxLength(500);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }
}
```

### UserRepository.cs

**Dosya:** `src/Services/Identity/Identity.Infrastructure/Repositories/UserRepository.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Data;

namespace Identity.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _context;

    public UserRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users.ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        user.MarkAsDeleted();
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
```

### JwtService.cs

**Dosya:** `src/Services/Identity/Identity.Infrastructure/Services/JwtService.cs`

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;

namespace Identity.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                _configuration.GetValue<int>("Jwt:AccessTokenExpirationMinutes", 15)),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public (string Token, DateTime ExpiresAt) GenerateTokenPair(User user)
    {
        var token = GenerateAccessToken(user);
        var expiresAt = DateTime.UtcNow.AddMinutes(
            _configuration.GetValue<int>("Jwt:AccessTokenExpirationMinutes", 15));
        return (token, expiresAt);
    }

    public Guid? ValidateToken(string token)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured")));

        var handler = new JwtSecurityTokenHandler();
        try
        {
            var result = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            var userId = result.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId is not null ? Guid.Parse(userId) : null;
        }
        catch
        {
            return null;
        }
    }
}
```

### DependencyInjection.cs

**Dosya:** `src/Services/Identity/Identity.Infrastructure/DependencyInjection.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Identity.Application.Interfaces;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Repositories;
using Identity.Infrastructure.Services;

namespace Identity.Infrastructure;

public static class IdentityInfrastructureDependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("IdentityDb"),
                b => b.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }
}
```

---

## Task 7: Identity.API — Controllers, Middleware, Program.cs, Dockerfile

**Amaç:** Identity mikroservisi API katmanını oluşturmak: AuthController, GlobalExceptionMiddleware, Program.cs, Dockerfile.

**Dosyalar:**
- `src/Services/Identity/Identity.API/Controllers/AuthController.cs`
- `src/Services/Identity/Identity.API/Middleware/GlobalExceptionMiddleware.cs`
- `src/Services/Identity/Identity.API/Program.cs`
- `src/Services/Identity/Identity.API/Dockerfile`
- `src/Services/Identity/Identity.API/appsettings.json`

### AuthController.cs

**Dosya:** `src/Services/Identity/Identity.API/Controllers/AuthController.cs`

```csharp
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Identity.Application.Commands.Register;
using Identity.Application.Commands.Login;
using Identity.Application.Queries.GetProfile;
using Identity.Application.DTOs;
using System.Security.Claims;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(
        RegisterRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(request.Email, request.FirstName, request.LastName, request.Password);
        var response = await _mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var response = await _mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<UserDto>> GetProfile(CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var query = new GetProfileQuery(userId);
        var response = await _mediator.Send(query, cancellationToken);
        return Ok(response);
    }
}
```

### GlobalExceptionMiddleware.cs

**Dosya:** `src/Services/Identity/Identity.API/Middleware/GlobalExceptionMiddleware.cs`

```csharp
using System.Net;
using System.Text.Json;

namespace Identity.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            UnauthorizedAccessException => ((int)HttpStatusCode.Unauthorized, exception.Message),
            KeyNotFoundException => ((int)HttpStatusCode.NotFound, exception.Message),
            InvalidOperationException => ((int)HttpStatusCode.BadRequest, exception.Message),
            ArgumentException => ((int)HttpStatusCode.BadRequest, exception.Message),
            _ => ((int)HttpStatusCode.InternalServerError, "Bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.")
        };

        context.Response.StatusCode = statusCode;

        var errorResponse = new
        {
            StatusCode = statusCode,
            Message = message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
}
```

### Program.cs

**Dosya:** `src/Services/Identity/Identity.API/Program.cs`

```csharp
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Identity.API.Middleware;
using Identity.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured"))),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddIdentityInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### Dockerfile

**Dosya:** `src/Services/Identity/Identity.API/Dockerfile`

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/Directory.Build.props .
COPY src/BuildingBlocks/Shared/Shared.csproj BuildingBlocks/Shared/
COPY src/BuildingBlocks/Shared.Messaging/Shared.Messaging.csproj BuildingBlocks/Shared.Messaging/
COPY src/BuildingBlocks/Shared.Caching/Shared.Caching.csproj BuildingBlocks/Shared.Caching/
COPY src/Services/Identity/Identity.Domain/Identity.Domain.csproj Services/Identity/Identity.Domain/
COPY src/Services/Identity/Identity.Application/Identity.Application.csproj Services/Identity/Identity.Application/
COPY src/Services/Identity/Identity.Infrastructure/Identity.Infrastructure.csproj Services/Identity/Identity.Infrastructure/
COPY src/Services/Identity/Identity.API/Identity.API.csproj Services/Identity/Identity.API/
RUN dotnet restore Services/Identity/Identity.API/Identity.API.csproj

COPY src/ .
RUN dotnet publish Services/Identity/Identity.API/Identity.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Identity.API.dll"]
```

### appsettings.json

**Dosya:** `src/Services/Identity/Identity.API/appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "IdentityDb": "Server=sqlserver;Database=IdentityDb;User Id=sa;Password=Passw0rd!;TrustServerCertificate=true;"
  },
  "Jwt": {
    "Secret": "SuperSecretKeyForJwtTokenGeneration2024!@#$%",
    "Issuer": "ECommercePlatform",
    "Audience": "ECommercePlatformClient",
    "AccessTokenExpirationMinutes": 15
  },
  "AllowedHosts": "*"
}
```

---

## Task 8: Product.Domain — Product, Category Entities and Repository Interfaces

**Amaç:** Product mikroservisi domain katmanını oluşturmak: Product ve Category entity'leri, repository interface'leri.

**Dosyalar:**
- `src/Services/Product/Product.Domain/Entities/Product.cs`
- `src/Services/Product/Product.Domain/Entities/Category.cs`
- `src/Services/Product/Product.Domain/Interfaces/IProductRepository.cs`
- `src/Services/Product/Product.Domain/Interfaces/ICategoryRepository.cs`

### Product.cs

**Dosya:** `src/Services/Product/Product.Domain/Entities/Product.cs`

```csharp
using BuildingBlocks.Shared;

namespace Product.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public string Currency { get; private set; } = "TRY";
    public string ImageUrl { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;
    public string SKU { get; private set; }
    public bool IsPublished { get; private set; }

    private Product() { }

    public Product(string name, string description, decimal price, string imageUrl, Guid categoryId, string sku)
    {
        Name = name;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
        CategoryId = categoryId;
        SKU = sku;
    }

    public void UpdateDetails(string name, string description, decimal price, string imageUrl, Guid categoryId)
    {
        Name = name;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
        CategoryId = categoryId;
        MarkAsUpdated();
    }

    public void Publish()
    {
        IsPublished = true;
        MarkAsUpdated();
    }

    public void Unpublish()
    {
        IsPublished = false;
        MarkAsUpdated();
    }

    public void UpdatePrice(decimal newPrice)
    {
        Price = newPrice;
        MarkAsUpdated();
    }
}
```

### Category.cs

**Dosya:** `src/Services/Product/Product.Domain/Entities/Category.cs`

```csharp
using BuildingBlocks.Shared;

namespace Product.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string? ImageUrl { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    public Category? ParentCategory { get; private set; }
    public ICollection<Product> Products { get; private set; } = [];

    private Category() { }

    public Category(string name, string description, string? imageUrl = null, Guid? parentCategoryId = null)
    {
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
        ParentCategoryId = parentCategoryId;
    }

    public void UpdateDetails(string name, string description, string? imageUrl)
    {
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
        MarkAsUpdated();
    }
}
```

### IProductRepository.cs

**Dosya:** `src/Services/Product/Product.Domain/Interfaces/IProductRepository.cs`

```csharp
using Product.Domain.Entities;

namespace Product.Domain.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task DeleteAsync(Product product, CancellationToken cancellationToken = default);
}
```

### ICategoryRepository.cs

**Dosya:** `src/Services/Product/Product.Domain/Interfaces/ICategoryRepository.cs`

```csharp
using Product.Domain.Entities;

namespace Product.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Category category, CancellationToken cancellationToken = default);
    Task UpdateAsync(Category category, CancellationToken cancellationToken = default);
    Task DeleteAsync(Category category, CancellationToken cancellationToken = default);
}
```

---

## Task 9: Product.Application — CQRS Commands, Queries, DTOs

**Amaç:** Product mikroservisi application katmanını oluşturmak: CRUD komutları/sorguları, DTO'lar.

**Dosyalar:**
- `src/Services/Product/Product.Application/DTOs/ProductDtos.cs`
- `src/Services/Product/Product.Application/Commands/CreateProductCommand.cs`
- `src/Services/Product/Product.Application/Commands/CreateProductCommandHandler.cs`
- `src/Services/Product/Product.Application/Commands/UpdateProductCommand.cs`
- `src/Services/Product/Product.Application/Commands/UpdateProductCommandHandler.cs`
- `src/Services/Product/Product.Application/Commands/DeleteProductCommand.cs`
- `src/Services/Product/Product.Application/Commands/DeleteProductCommandHandler.cs`
- `src/Services/Product/Product.Application/Queries/GetAllProductsQuery.cs`
- `src/Services/Product/Product.Application/Queries/GetAllProductsQueryHandler.cs`
- `src/Services/Product/Product.Application/Queries/GetProductByIdQuery.cs`
- `src/Services/Product/Product.Application/Queries/GetProductByIdQueryHandler.cs`
- `src/Services/Product/Product.Application/Queries/SearchProductsQuery.cs`
- `src/Services/Product/Product.Application/Queries/SearchProductsQueryHandler.cs`

### ProductDtos.cs

**Dosya:** `src/Services/Product/Product.Application/DTOs/ProductDtos.cs`

```csharp
namespace Product.Application.DTOs;

public record ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Currency { get; init; } = "TRY";
    public string ImageUrl { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }
    public string SKU { get; init; } = string.Empty;
    public bool IsPublished { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record CreateProductRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }
    public string SKU { get; init; } = string.Empty;
}

public record UpdateProductRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }
}

public record CategoryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public int ProductCount { get; init; }
}
```

### CreateProductCommand.cs

**Dosya:** `src/Services/Product/Product.Application/Commands/CreateProductCommand.cs`

```csharp
using MediatR;
using Product.Application.DTOs;

namespace Product.Application.Commands;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    Guid CategoryId,
    string SKU) : IRequest<ProductDto>;
```

### CreateProductCommandHandler.cs

**Dosya:** `src/Services/Product/Product.Application/Commands/CreateProductCommandHandler.cs`

```csharp
using MediatR;
using Product.Domain.Entities;
using Product.Domain.Interfaces;
using Product.Application.DTOs;

namespace Product.Application.Commands;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
            throw new KeyNotFoundException("Kategori bulunamadı.");

        var existingProduct = await _productRepository.ExistsBySkuAsync(request.SKU, cancellationToken);
        if (existingProduct)
            throw new InvalidOperationException("Bu SKU ile kayıtlı bir ürün zaten var.");

        var product = new Product(request.Name, request.Description, request.Price, request.ImageUrl, request.CategoryId, request.SKU);
        await _productRepository.AddAsync(product, cancellationToken);

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Currency = product.Currency,
            ImageUrl = product.ImageUrl,
            CategoryName = category.Name,
            CategoryId = product.CategoryId,
            SKU = product.SKU,
            IsPublished = product.IsPublished,
            CreatedAt = product.CreatedAt
        };
    }
}
```

### UpdateProductCommand.cs

**Dosya:** `src/Services/Product/Product.Application/Commands/UpdateProductCommand.cs`

```csharp
using MediatR;
using Product.Application.DTOs;

namespace Product.Application.Commands;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    Guid CategoryId) : IRequest<ProductDto>;
```

### UpdateProductCommandHandler.cs

**Dosya:** `src/Services/Product/Product.Application/Commands/UpdateProductCommandHandler.cs`

```csharp
using MediatR;
using Product.Domain.Interfaces;
using Product.Application.DTOs;

namespace Product.Application.Commands;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public UpdateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            throw new KeyNotFoundException("Ürün bulunamadı.");

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
            throw new KeyNotFoundException("Kategori bulunamadı.");

        product.UpdateDetails(request.Name, request.Description, request.Price, request.ImageUrl, request.CategoryId);
        await _productRepository.UpdateAsync(product, cancellationToken);

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Currency = product.Currency,
            ImageUrl = product.ImageUrl,
            CategoryName = category.Name,
            CategoryId = product.CategoryId,
            SKU = product.SKU,
            IsPublished = product.IsPublished,
            CreatedAt = product.CreatedAt
        };
    }
}
```

### DeleteProductCommand.cs

**Dosya:** `src/Services/Product/Product.Application/Commands/DeleteProductCommand.cs`

```csharp
using MediatR;

namespace Product.Application.Commands;

public record DeleteProductCommand(Guid Id) : IRequest<Unit>;
```

### DeleteProductCommandHandler.cs

**Dosya:** `src/Services/Product/Product.Application/Commands/DeleteProductCommandHandler.cs`

```csharp
using MediatR;
using Product.Domain.Interfaces;

namespace Product.Application.Commands;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            throw new KeyNotFoundException("Ürün bulunamadı.");

        await _productRepository.DeleteAsync(product, cancellationToken);
        return Unit.Value;
    }
}
```

### GetAllProductsQuery.cs

**Dosya:** `src/Services/Product/Product.Application/Queries/GetAllProductsQuery.cs`

```csharp
using MediatR;
using Product.Application.DTOs;

namespace Product.Application.Queries;

public record GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>;
```

### GetAllProductsQueryHandler.cs

**Dosya:** `src/Services/Product/Product.Application/Queries/GetAllProductsQueryHandler.cs`

```csharp
using MediatR;
using Product.Domain.Interfaces;
using Product.Application.DTOs;

namespace Product.Application.Queries;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetAllProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Currency = p.Currency,
            ImageUrl = p.ImageUrl,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name ?? string.Empty,
            SKU = p.SKU,
            IsPublished = p.IsPublished,
            CreatedAt = p.CreatedAt
        });
    }
}
```

### GetProductByIdQuery.cs

**Dosya:** `src/Services/Product/Product.Application/Queries/GetProductByIdQuery.cs`

```csharp
using MediatR;
using Product.Application.DTOs;

namespace Product.Application.Queries;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;
```

### GetProductByIdQueryHandler.cs

**Dosya:** `src/Services/Product/Product.Application/Queries/GetProductByIdQueryHandler.cs`

```csharp
using MediatR;
using Product.Domain.Interfaces;
using Product.Application.DTOs;

namespace Product.Application.Queries;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            throw new KeyNotFoundException("Ürün bulunamadı.");

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Currency = product.Currency,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            SKU = product.SKU,
            IsPublished = product.IsPublished,
            CreatedAt = product.CreatedAt
        };
    }
}
```

### SearchProductsQuery.cs

**Dosya:** `src/Services/Product/Product.Application/Queries/SearchProductsQuery.cs`

```csharp
using MediatR;
using Product.Application.DTOs;

namespace Product.Application.Queries;

public record SearchProductsQuery(string SearchTerm) : IRequest<IEnumerable<ProductDto>>;
```

### SearchProductsQueryHandler.cs

**Dosya:** `src/Services/Product/Product.Application/Queries/SearchProductsQueryHandler.cs`

```csharp
using MediatR;
using Product.Domain.Interfaces;
using Product.Application.DTOs;

namespace Product.Application.Queries;

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public SearchProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.SearchAsync(request.SearchTerm, cancellationToken);
        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Currency = p.Currency,
            ImageUrl = p.ImageUrl,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name ?? string.Empty,
            SKU = p.SKU,
            IsPublished = p.IsPublished,
            CreatedAt = p.CreatedAt
        });
    }
}
```

---

## Task 10: Product.Infrastructure — DbContext, Repositories

**Amaç:** Product mikroservisi infrastructure katmanını oluşturmak.

**Dosyalar:**
- `src/Services/Product/Product.Infrastructure/Data/ProductDbContext.cs`
- `src/Services/Product/Product.Infrastructure/Repositories/ProductRepository.cs`
- `src/Services/Product/Product.Infrastructure/Repositories/CategoryRepository.cs`
- `src/Services/Product/Product.Infrastructure/DependencyInjection.cs`

### ProductDbContext.cs

**Dosya:** `src/Services/Product/Product.Infrastructure/Data/ProductDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Product.Domain.Entities;

namespace Product.Infrastructure.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

    public DbSet<Product.Domain.Entities.Product> Products => Set<Product.Domain.Entities.Product>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product.Domain.Entities.Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SKU).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.SKU).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(e => e.CategoryId);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.HasOne(e => e.ParentCategory)
                  .WithMany()
                  .HasForeignKey(e => e.ParentCategoryId)
                  .OnDelete(DeleteBehavior.NoAction);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<Category>().HasData(
            new Category("Elektronik", "Elektronik ürünler") { Id = Guid.Parse("10000000-0000-0000-0000-000000000001") },
            new Category("Giyim", "Giyim ürünleri") { Id = Guid.Parse("10000000-0000-0000-0000-000000000002") },
            new Category("Kitap", "Kitap ürünleri") { Id = Guid.Parse("10000000-0000-0000-0000-000000000003") },
            new Category("Ev & Yaşam", "Ev ve yaşam ürünleri") { Id = Guid.Parse("10000000-0000-0000-0000-000000000004") },
            new Category("Spor", "Spor ürünleri") { Id = Guid.Parse("10000000-0000-0000-0000-000000000005") }
        );
    }
}
```

### ProductRepository.cs

**Dosya:** `src/Services/Product/Product.Infrastructure/Repositories/ProductRepository.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Product.Domain.Entities;
using Product.Domain.Interfaces;
using Product.Infrastructure.Data;

namespace Product.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;

    public ProductRepository(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<Product.Domain.Entities.Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Product.Domain.Entities.Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Category)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product.Domain.Entities.Product>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product.Domain.Entities.Product>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm) || p.SKU.Contains(searchTerm))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await _context.Products.AnyAsync(p => p.SKU == sku, cancellationToken);
    }

    public async Task AddAsync(Product.Domain.Entities.Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product.Domain.Entities.Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Product.Domain.Entities.Product product, CancellationToken cancellationToken = default)
    {
        product.MarkAsDeleted();
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
```

### CategoryRepository.cs

**Dosya:** `src/Services/Product/Product.Infrastructure/Repositories/CategoryRepository.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Product.Domain.Entities;
using Product.Domain.Interfaces;
using Product.Infrastructure.Data;

namespace Product.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ProductDbContext _context;

    public CategoryRepository(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories.FindAsync([id], cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.Products)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        await _context.Categories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Category category, CancellationToken cancellationToken = default)
    {
        category.MarkAsDeleted();
        _context.Categories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
```

### DependencyInjection.cs

**Dosya:** `src/Services/Product/Product.Infrastructure/DependencyInjection.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Product.Domain.Interfaces;
using Product.Infrastructure.Data;
using Product.Infrastructure.Repositories;

namespace Product.Infrastructure;

public static class ProductInfrastructureDependencyInjection
{
    public static IServiceCollection AddProductInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ProductDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("ProductDb"),
                b => b.MigrationsAssembly(typeof(ProductDbContext).Assembly.FullName)));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        return services;
    }
}
```

---

## Task 11: Product.API — Controllers with Redis Cache, Program.cs

**Amaç:** Product mikroservisi API katmanını oluşturmak: Redis cache ile Controllers.

**Dosyalar:**
- `src/Services/Product/Product.API/Controllers/ProductsController.cs`
- `src/Services/Product/Product.API/Controllers/CategoriesController.cs`
- `src/Services/Product/Product.API/Program.cs`
- `src/Services/Product/Product.API/Dockerfile`
- `src/Services/Product/Product.API/appsettings.json`

### ProductsController.cs

**Dosya:** `src/Services/Product/Product.API/Controllers/ProductsController.cs`

```csharp
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Application.Commands;
using Product.Application.DTOs;
using Product.Application.Queries;
using BuildingBlocks.Shared.Caching;

namespace Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICacheService _cacheService;

    public ProductsController(IMediator mediator, ICacheService cacheService)
    {
        _mediator = mediator;
        _cacheService = cacheService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(CancellationToken cancellationToken)
    {
        const string cacheKey = "products:all";
        var cached = await _cacheService.GetAsync<IEnumerable<ProductDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return Ok(cached);

        var products = await _mediator.Send(new GetAllProductsQuery(), cancellationToken);
        await _cacheService.SetAsync(cacheKey, products, TimeSpan.FromMinutes(5), cancellationToken);
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var cacheKey = $"products:{id}";
        var cached = await _cacheService.GetAsync<ProductDto>(cacheKey, cancellationToken);
        if (cached is not null)
            return Ok(cached);

        var product = await _mediator.Send(new GetProductByIdQuery(id), cancellationToken);
        await _cacheService.SetAsync(cacheKey, product, TimeSpan.FromMinutes(5), cancellationToken);
        return Ok(product);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> Search(
        [FromQuery] string q, CancellationToken cancellationToken)
    {
        var products = await _mediator.Send(new SearchProductsQuery(q), cancellationToken);
        return Ok(products);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(
        CreateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(
            request.Name, request.Description, request.Price,
            request.ImageUrl, request.CategoryId, request.SKU);
        var product = await _mediator.Send(command, cancellationToken);
        await _cacheService.RemoveAsync("products:all", cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductDto>> Update(
        Guid id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(
            id, request.Name, request.Description, request.Price,
            request.ImageUrl, request.CategoryId);
        var product = await _mediator.Send(command, cancellationToken);
        await _cacheService.RemoveAsync($"products:{id}", cancellationToken);
        await _cacheService.RemoveAsync("products:all", cancellationToken);
        return Ok(product);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteProductCommand(id), cancellationToken);
        await _cacheService.RemoveAsync($"products:{id}", cancellationToken);
        await _cacheService.RemoveAsync("products:all", cancellationToken);
        return NoContent();
    }
}
```

### CategoriesController.cs

**Dosya:** `src/Services/Product/Product.API/Controllers/CategoriesController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Product.Domain.Interfaces;
using Product.Application.DTOs;
using BuildingBlocks.Shared.Caching;

namespace Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICacheService _cacheService;

    public CategoriesController(ICategoryRepository categoryRepository, ICacheService cacheService)
    {
        _categoryRepository = categoryRepository;
        _cacheService = cacheService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        const string cacheKey = "categories:all";
        var cached = await _cacheService.GetAsync<IEnumerable<CategoryDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return Ok(cached);

        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        var dtos = categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            ImageUrl = c.ImageUrl,
            ProductCount = c.Products.Count
        });

        await _cacheService.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(10), cancellationToken);
        return Ok(dtos);
    }
}
```

### Program.cs

**Dosya:** `src/Services/Product/Product.API/Program.cs`

```csharp
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Product.Infrastructure;
using BuildingBlocks.Shared.Caching;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured"))),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddProductInfrastructure(builder.Configuration);
builder.Services.AddCacheService(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### Dockerfile

**Dosya:** `src/Services/Product/Product.API/Dockerfile`

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/Directory.Build.props .
COPY src/BuildingBlocks/Shared/Shared.csproj BuildingBlocks/Shared/
COPY src/BuildingBlocks/Shared.Messaging/Shared.Messaging.csproj BuildingBlocks/Shared.Messaging/
COPY src/BuildingBlocks/Shared.Caching/Shared.Caching.csproj BuildingBlocks/Shared.Caching/
COPY src/Services/Product/Product.Domain/Product.Domain.csproj Services/Product/Product.Domain/
COPY src/Services/Product/Product.Application/Product.Application.csproj Services/Product/Product.Application/
COPY src/Services/Product/Product.Infrastructure/Product.Infrastructure.csproj Services/Product/Product.Infrastructure/
COPY src/Services/Product/Product.API/Product.API.csproj Services/Product/Product.API/
RUN dotnet restore Services/Product/Product.API/Product.API.csproj

COPY src/ .
RUN dotnet publish Services/Product/Product.API/Product.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Product.API.dll"]
```

### appsettings.json

**Dosya:** `src/Services/Product/Product.API/appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "ProductDb": "Server=sqlserver;Database=ProductDb;User Id=sa;Password=Passw0rd!;TrustServerCertificate=true;",
    "Redis": "redis:6379"
  },
  "Jwt": {
    "Secret": "SuperSecretKeyForJwtTokenGeneration2024!@#$%",
    "Issuer": "ECommercePlatform",
    "Audience": "ECommercePlatformClient"
  },
  "AllowedHosts": "*"
}
```

---

## Task 12: Order.Domain — Order, OrderItem, Address Value Object

**Amaç:** Order mikroservisi domain katmanını oluşturmak.

**Dosyalar:**
- `src/Services/Order/Order.Domain/ValueObjects/Address.cs`
- `src/Services/Order/Order.Domain/Entities/OrderItem.cs`
- `src/Services/Order/Order.Domain/Entities/Order.cs`
- `src/Services/Order/Order.Domain/Interfaces/IOrderRepository.cs`
- `src/Services/Order/Order.Domain/Events/OrderCreatedDomainEvent.cs`

### Address.cs

**Dosya:** `src/Services/Order/Order.Domain/ValueObjects/Address.cs`

```csharp
using BuildingBlocks.Shared;

namespace Order.Domain.ValueObjects;

public class Address : ValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string ZipCode { get; private set; }
    public string Country { get; private set; }

    private Address() { }

    public Address(string street, string city, string state, string zipCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return ZipCode;
        yield return Country;
    }
}
```

### OrderItem.cs

**Dosya:** `src/Services/Order/Order.Domain/Entities/OrderItem.cs`

```csharp
using BuildingBlocks.Shared;

namespace Order.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Order Order { get; private set; } = null!;
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice => UnitPrice * Quantity;

    private OrderItem() { }

    public OrderItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
        MarkAsUpdated();
    }
}
```

### Order.cs

**Dosya:** `src/Services/Order/Order.Domain/Entities/Order.cs`

```csharp
using BuildingBlocks.Shared;
using Order.Domain.ValueObjects;
using Order.Domain.Events;

namespace Order.Domain.Entities;

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}

public class Order : BaseEntity
{
    public Guid UserId { get; private set; }
    public string OrderNumber { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public Address ShippingAddress { get; private set; } = null!;
    public Address BillingAddress { get; private set; } = null!;
    public decimal SubTotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal ShippingCost { get; private set; }
    public decimal TotalAmount => SubTotal + TaxAmount + ShippingCost;
    public string? Notes { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    private readonly List<OrderItem> _items = [];

    private Order() { }

    public Order(Guid userId, Address shippingAddress, Address billingAddress)
    {
        UserId = userId;
        OrderNumber = GenerateOrderNumber();
        ShippingAddress = shippingAddress;
        BillingAddress = billingAddress;
    }

    public void AddItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem is not null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            _items.Add(new OrderItem(productId, productName, unitPrice, quantity));
        }
        RecalculateTotals();
        MarkAsUpdated();
    }

    public void RemoveItem(Guid productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is not null)
        {
            _items.Remove(item);
            RecalculateTotals();
            MarkAsUpdated();
        }
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Yalnızca bekleyen siparişler onaylanabilir.");

        Status = OrderStatus.Confirmed;
        AddDomainEvent(new OrderCreatedDomainEvent(this));
        MarkAsUpdated();
    }

    public void Ship()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Yalnızca onaylanmış siparişler kargoya verilebilir.");

        Status = OrderStatus.Shipped;
        MarkAsUpdated();
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Yalnızca kargoya verilmiş siparişler teslim edilebilir.");

        Status = OrderStatus.Delivered;
        MarkAsUpdated();
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Teslim edilmiş siparişler iptal edilemez.");

        Status = OrderStatus.Cancelled;
        MarkAsUpdated();
    }

    public void UpdateNotes(string notes)
    {
        Notes = notes;
        MarkAsUpdated();
    }

    private void RecalculateTotals()
    {
        SubTotal = _items.Sum(i => i.TotalPrice);
        TaxAmount = SubTotal * 0.18m;
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}"[..20].ToUpper();
    }
}
```

### IOrderRepository.cs

**Dosya:** `src/Services/Order/Order.Domain/Interfaces/IOrderRepository.cs`

```csharp
using Order.Domain.Entities;

namespace Order.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
    Task DeleteAsync(Order order, CancellationToken cancellationToken = default);
}
```

### OrderCreatedDomainEvent.cs

**Dosya:** `src/Services/Order/Order.Domain/Events/OrderCreatedDomainEvent.cs`

```csharp
using BuildingBlocks.Shared;

namespace Order.Domain.Events;

public record OrderCreatedDomainEvent(Entities.Order Order) : BaseEvent;
```

---

## Task 13: Order.Application — CQRS, DTOs, IntegrationEvent

**Amaç:** Order mikroservisi application katmanını oluşturmak.

**Dosyalar:**
- `src/Services/Order/Order.Application/DTOs/OrderDtos.cs`
- `src/Services/Order/Order.Application/Commands/CreateOrderCommand.cs`
- `src/Services/Order/Order.Application/Commands/CreateOrderCommandHandler.cs`
- `src/Services/Order/Order.Application/Commands/ConfirmOrderCommand.cs`
- `src/Services/Order/Order.Application/Commands/ConfirmOrderCommandHandler.cs`
- `src/Services/Order/Order.Application/Commands/CancelOrderCommand.cs`
- `src/Services/Order/Order.Application/Commands/CancelOrderCommandHandler.cs`
- `src/Services/Order/Order.Application/Queries/GetOrdersQuery.cs`
- `src/Services/Order/Order.Application/Queries/GetOrdersQueryHandler.cs`
- `src/Services/Order/Order.Application/Queries/GetOrderByIdQuery.cs`
- `src/Services/Order/Order.Application/Queries/GetOrderByIdQueryHandler.cs`
- `src/Services/Order/Order.Application/IntegrationEvents/OrderCreatedIntegrationEvent.cs`
- `src/Services/Order/Order.Application/IntegrationEvents/OrderCreatedIntegrationEventHandler.cs`

### OrderDtos.cs

**Dosya:** `src/Services/Order/Order.Application/DTOs/OrderDtos.cs`

```csharp
namespace Order.Application.DTOs;

public record OrderDto
{
    public Guid Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal SubTotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal ShippingCost { get; init; }
    public decimal TotalAmount { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
    public AddressDto ShippingAddress { get; init; } = null!;
    public AddressDto BillingAddress { get; init; } = null!;
    public List<OrderItemDto> Items { get; init; } = [];
}

public record OrderItemDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
    public decimal TotalPrice { get; init; }
}

public record AddressDto
{
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
}

public record CreateOrderRequest
{
    public List<OrderItemRequest> Items { get; init; } = [];
    public AddressRequest ShippingAddress { get; init; } = null!;
    public AddressRequest BillingAddress { get; init; } = null!;
    public string? Notes { get; init; }
}

public record OrderItemRequest
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
}

public record AddressRequest
{
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
}
```

### CreateOrderCommand.cs

**Dosya:** `src/Services/Order/Order.Application/Commands/CreateOrderCommand.cs`

```csharp
using MediatR;
using Order.Application.DTOs;

namespace Order.Application.Commands;

public record CreateOrderCommand(
    Guid UserId,
    List<OrderItemRequest> Items,
    AddressRequest ShippingAddress,
    AddressRequest BillingAddress,
    string? Notes = null) : IRequest<OrderDto>;
```

### CreateOrderCommandHandler.cs

**Dosya:** `src/Services/Order/Order.Application/Commands/CreateOrderCommandHandler.cs`

```csharp
using MediatR;
using Order.Domain.Entities;
using Order.Domain.Interfaces;
using Order.Domain.ValueObjects;
using Order.Application.DTOs;

namespace Order.Application.Commands;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var shippingAddress = new Address(
            request.ShippingAddress.Street,
            request.ShippingAddress.City,
            request.ShippingAddress.State,
            request.ShippingAddress.ZipCode,
            request.ShippingAddress.Country);

        var billingAddress = new Address(
            request.BillingAddress.Street,
            request.BillingAddress.City,
            request.BillingAddress.State,
            request.BillingAddress.ZipCode,
            request.BillingAddress.Country);

        var order = new Order(request.UserId, shippingAddress, billingAddress);

        foreach (var item in request.Items)
        {
            order.AddItem(item.ProductId, item.ProductName, item.UnitPrice, item.Quantity);
        }

        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            order.UpdateNotes(request.Notes);
        }

        await _orderRepository.AddAsync(order, cancellationToken);

        return MapToDto(order);
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString(),
            SubTotal = order.SubTotal,
            TaxAmount = order.TaxAmount,
            ShippingCost = order.ShippingCost,
            TotalAmount = order.TotalAmount,
            Notes = order.Notes,
            CreatedAt = order.CreatedAt,
            ShippingAddress = new AddressDto
            {
                Street = order.ShippingAddress.Street,
                City = order.ShippingAddress.City,
                State = order.ShippingAddress.State,
                ZipCode = order.ShippingAddress.ZipCode,
                Country = order.ShippingAddress.Country
            },
            BillingAddress = new AddressDto
            {
                Street = order.BillingAddress.Street,
                City = order.BillingAddress.City,
                State = order.BillingAddress.State,
                ZipCode = order.BillingAddress.ZipCode,
                Country = order.BillingAddress.Country
            },
            Items = order.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                TotalPrice = i.TotalPrice
            }).ToList()
        };
    }
}
```

### ConfirmOrderCommand.cs

**Dosya:** `src/Services/Order/Order.Application/Commands/ConfirmOrderCommand.cs`

```csharp
using MediatR;
using Order.Application.DTOs;

namespace Order.Application.Commands;

public record ConfirmOrderCommand(Guid OrderId) : IRequest<OrderDto>;
```

### ConfirmOrderCommandHandler.cs

**Dosya:** `src/Services/Order/Order.Application/Commands/ConfirmOrderCommandHandler.cs`

```csharp
using MediatR;
using Order.Domain.Interfaces;
using Order.Application.DTOs;

namespace Order.Application.Commands;

public class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;

    public ConfirmOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
            throw new KeyNotFoundException("Sipariş bulunamadı.");

        order.Confirm();
        order.ClearDomainEvents();
        await _orderRepository.UpdateAsync(order, cancellationToken);

        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString(),
            SubTotal = order.SubTotal,
            TaxAmount = order.TaxAmount,
            ShippingCost = order.ShippingCost,
            TotalAmount = order.TotalAmount,
            Notes = order.Notes,
            CreatedAt = order.CreatedAt,
            ShippingAddress = new AddressDto
            {
                Street = order.ShippingAddress.Street,
                City = order.ShippingAddress.City,
                State = order.ShippingAddress.State,
                ZipCode = order.ShippingAddress.ZipCode,
                Country = order.ShippingAddress.Country
            },
            BillingAddress = new AddressDto
            {
                Street = order.BillingAddress.Street,
                City = order.BillingAddress.City,
                State = order.BillingAddress.State,
                ZipCode = order.BillingAddress.ZipCode,
                Country = order.BillingAddress.Country
            },
            Items = order.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                TotalPrice = i.TotalPrice
            }).ToList()
        };
    }
}
```

### CancelOrderCommand.cs

**Dosya:** `src/Services/Order/Order.Application/Commands/CancelOrderCommand.cs`

```csharp
using MediatR;

namespace Order.Application.Commands;

public record CancelOrderCommand(Guid OrderId) : IRequest<Unit>;
```

### CancelOrderCommandHandler.cs

**Dosya:** `src/Services/Order/Order.Application/Commands/CancelOrderCommandHandler.cs`

```csharp
using MediatR;
using Order.Domain.Interfaces;

namespace Order.Application.Commands;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;

    public CancelOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Unit> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
            throw new KeyNotFoundException("Sipariş bulunamadı.");

        order.Cancel();
        await _orderRepository.UpdateAsync(order, cancellationToken);
        return Unit.Value;
    }
}
```

### GetOrdersQuery.cs

**Dosya:** `src/Services/Order/Order.Application/Queries/GetOrdersQuery.cs`

```csharp
using MediatR;
using Order.Application.DTOs;

namespace Order.Application.Queries;

public record GetOrdersQuery(Guid? UserId = null) : IRequest<IEnumerable<OrderDto>>;
```

### GetOrdersQueryHandler.cs

**Dosya:** `src/Services/Order/Order.Application/Queries/GetOrdersQueryHandler.cs`

```csharp
using MediatR;
using Order.Domain.Interfaces;
using Order.Application.DTOs;

namespace Order.Application.Queries;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IEnumerable<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IEnumerable<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Order.Domain.Entities.Order> orders;
        if (request.UserId.HasValue)
            orders = await _orderRepository.GetByUserIdAsync(request.UserId.Value, cancellationToken);
        else
            orders = await _orderRepository.GetAllAsync(cancellationToken);

        return orders.Select(order => new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString(),
            SubTotal = order.SubTotal,
            TaxAmount = order.TaxAmount,
            ShippingCost = order.ShippingCost,
            TotalAmount = order.TotalAmount,
            Notes = order.Notes,
            CreatedAt = order.CreatedAt,
            ShippingAddress = new AddressDto
            {
                Street = order.ShippingAddress.Street,
                City = order.ShippingAddress.City,
                State = order.ShippingAddress.State,
                ZipCode = order.ShippingAddress.ZipCode,
                Country = order.ShippingAddress.Country
            },
            BillingAddress = new AddressDto
            {
                Street = order.BillingAddress.Street,
                City = order.BillingAddress.City,
                State = order.BillingAddress.State,
                ZipCode = order.BillingAddress.ZipCode,
                Country = order.BillingAddress.Country
            },
            Items = order.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                TotalPrice = i.TotalPrice
            }).ToList()
        });
    }
}
```

### GetOrderByIdQuery.cs

**Dosya:** `src/Services/Order/Order.Application/Queries/GetOrderByIdQuery.cs`

```csharp
using MediatR;
using Order.Application.DTOs;

namespace Order.Application.Queries;

public record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto>;
```

### GetOrderByIdQueryHandler.cs

**Dosya:** `src/Services/Order/Order.Application/Queries/GetOrderByIdQueryHandler.cs`

```csharp
using MediatR;
using Order.Domain.Interfaces;
using Order.Application.DTOs;

namespace Order.Application.Queries;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (order is null)
            throw new KeyNotFoundException("Sipariş bulunamadı.");

        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString(),
            SubTotal = order.SubTotal,
            TaxAmount = order.TaxAmount,
            ShippingCost = order.ShippingCost,
            TotalAmount = order.TotalAmount,
            Notes = order.Notes,
            CreatedAt = order.CreatedAt,
            ShippingAddress = new AddressDto
            {
                Street = order.ShippingAddress.Street,
                City = order.ShippingAddress.City,
                State = order.ShippingAddress.State,
                ZipCode = order.ShippingAddress.ZipCode,
                Country = order.ShippingAddress.Country
            },
            BillingAddress = new AddressDto
            {
                Street = order.BillingAddress.Street,
                City = order.BillingAddress.City,
                State = order.BillingAddress.State,
                ZipCode = order.BillingAddress.ZipCode,
                Country = order.BillingAddress.Country
            },
            Items = order.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                TotalPrice = i.TotalPrice
            }).ToList()
        };
    }
}
```

### OrderCreatedIntegrationEvent.cs

**Dosya:** `src/Services/Order/Order.Application/IntegrationEvents/OrderCreatedIntegrationEvent.cs`

```csharp
using BuildingBlocks.Shared.Messaging;

namespace Order.Application.IntegrationEvents;

public record OrderCreatedIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public List<OrderItemEvent> Items { get; init; } = [];
    public DateTime CreatedAt { get; init; }
}

public record OrderItemEvent
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
}
```

### OrderCreatedIntegrationEventHandler.cs

**Dosya:** `src/Services/Order/Order.Application/IntegrationEvents/OrderCreatedIntegrationEventHandler.cs`

```csharp
using MediatR;
using BuildingBlocks.Shared.Messaging;
using Order.Domain.Interfaces;

namespace Order.Application.IntegrationEvents;

public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly IEventBus _eventBus;

    public OrderCreatedIntegrationEventHandler(IMediator mediator, IEventBus eventBus)
    {
        _mediator = mediator;
        _eventBus = eventBus;
    }

    public async Task HandleAsync(OrderCreatedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        await _eventBus.PublishAsync(@event, cancellationToken);
    }
}
```

---

## Task 14: Order.Infrastructure — DbContext, Repositories

**Amaç:** Order mikroservisi infrastructure katmanını oluşturmak.

**Dosyalar:**
- `src/Services/Order/Order.Infrastructure/Data/OrderDbContext.cs`
- `src/Services/Order/Order.Infrastructure/Repositories/OrderRepository.cs`
- `src/Services/Order/Order.Infrastructure/DependencyInjection.cs`

### OrderDbContext.cs

**Dosya:** `src/Services/Order/Order.Infrastructure/Data/OrderDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Order.Domain.Entities;
using Order.Domain.ValueObjects;

namespace Order.Infrastructure.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<Order.Domain.Entities.Order> Orders => Set<Order.Domain.Entities.Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order.Domain.Entities.Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.OrderNumber).IsUnique();
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ShippingCost).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.OwnsOne(e => e.ShippingAddress, a =>
            {
                a.Property(p => p.Street).HasColumnName("ShippingStreet").HasMaxLength(200);
                a.Property(p => p.City).HasColumnName("ShippingCity").HasMaxLength(100);
                a.Property(p => p.State).HasColumnName("ShippingState").HasMaxLength(100);
                a.Property(p => p.ZipCode).HasColumnName("ShippingZipCode").HasMaxLength(20);
                a.Property(p => p.Country).HasColumnName("ShippingCountry").HasMaxLength(100);
            });

            entity.OwnsOne(e => e.BillingAddress, a =>
            {
                a.Property(p => p.Street).HasColumnName("BillingStreet").HasMaxLength(200);
                a.Property(p => p.City).HasColumnName("BillingCity").HasMaxLength(100);
                a.Property(p => p.State).HasColumnName("BillingState").HasMaxLength(100);
                a.Property(p => p.ZipCode).HasColumnName("BillingZipCode").HasMaxLength(20);
                a.Property(p => p.Country).HasColumnName("BillingCountry").HasMaxLength(100);
            });

            entity.HasMany(e => e.Items)
                  .WithOne(e => e.Order)
                  .HasForeignKey(e => e.OrderId);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }
}
```

### OrderRepository.cs

**Dosya:** `src/Services/Order/Order.Infrastructure/Repositories/OrderRepository.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Order.Domain.Entities;
using Order.Domain.Interfaces;
using Order.Infrastructure.Data;

namespace Order.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<Order.Domain.Entities.Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Order.Domain.Entities.Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Order.Domain.Entities.Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Order.Domain.Entities.Order order, CancellationToken cancellationToken = default)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Order.Domain.Entities.Order order, CancellationToken cancellationToken = default)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Order.Domain.Entities.Order order, CancellationToken cancellationToken = default)
    {
        order.MarkAsDeleted();
        _context.Orders.Update(order);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
```

### DependencyInjection.cs

**Dosya:** `src/Services/Order/Order.Infrastructure/DependencyInjection.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Domain.Interfaces;
using Order.Infrastructure.Data;
using Order.Infrastructure.Repositories;

namespace Order.Infrastructure;

public static class OrderInfrastructureDependencyInjection
{
    public static IServiceCollection AddOrderInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("OrderDb"),
                b => b.MigrationsAssembly(typeof(OrderDbContext).Assembly.FullName)));

        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}
```

---

## Task 15: Order.API — Controllers, Program.cs

**Amaç:** Order mikroservisi API katmanını oluşturmak.

**Dosyalar:**
- `src/Services/Order/Order.API/Controllers/OrdersController.cs`
- `src/Services/Order/Order.API/Program.cs`
- `src/Services/Order/Order.API/Dockerfile`
- `src/Services/Order/Order.API/appsettings.json`

### OrdersController.cs

**Dosya:** `src/Services/Order/Order.API/Controllers/OrdersController.cs`

```csharp
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Commands;
using Order.Application.DTOs;
using Order.Application.Queries;

namespace Order.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetMyOrders(CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var query = new GetOrdersQuery(userId);
        var orders = await _mediator.Send(query, cancellationToken);
        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdQuery(id);
        var order = await _mediator.Send(query, cancellationToken);
        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(
        CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new CreateOrderCommand(
            userId,
            request.Items,
            request.ShippingAddress,
            request.BillingAddress,
            request.Notes);
        var order = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpPut("{id:guid}/confirm")]
    public async Task<ActionResult<OrderDto>> Confirm(Guid id, CancellationToken cancellationToken)
    {
        var command = new ConfirmOrderCommand(id);
        var order = await _mediator.Send(command, cancellationToken);
        return Ok(order);
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<ActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var command = new CancelOrderCommand(id);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
```

### Program.cs

**Dosya:** `src/Services/Order/Order.API/Program.cs`

```csharp
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Order.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured"))),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddOrderInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### Dockerfile

**Dosya:** `src/Services/Order/Order.API/Dockerfile`

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/Directory.Build.props .
COPY src/BuildingBlocks/Shared/Shared.csproj BuildingBlocks/Shared/
COPY src/BuildingBlocks/Shared.Messaging/Shared.Messaging.csproj BuildingBlocks/Shared.Messaging/
COPY src/Services/Order/Order.Domain/Order.Domain.csproj Services/Order/Order.Domain/
COPY src/Services/Order/Order.Application/Order.Application.csproj Services/Order/Order.Application/
COPY src/Services/Order/Order.Infrastructure/Order.Infrastructure.csproj Services/Order/Order.Infrastructure/
COPY src/Services/Order/Order.API/Order.API.csproj Services/Order/Order.API/
RUN dotnet restore Services/Order/Order.API/Order.API.csproj

COPY src/ .
RUN dotnet publish Services/Order/Order.API/Order.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Order.API.dll"]
```

### appsettings.json

**Dosya:** `src/Services/Order/Order.API/appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "OrderDb": "Server=sqlserver;Database=OrderDb;User Id=sa;Password=Passw0rd!;TrustServerCertificate=true;"
  },
  "Jwt": {
    "Secret": "SuperSecretKeyForJwtTokenGeneration2024!@#$%",
    "Issuer": "ECommercePlatform",
    "Audience": "ECommercePlatformClient"
  },
  "AllowedHosts": "*"
}
```

---

## Task 16: Inventory.Domain — StockItem Entity and Repository Interface

**Amaç:** Inventory mikroservisi domain katmanını oluşturmak: StockItem entity, IStockItemRepository.

**Dosyalar:**
- `src/Services/Inventory/Inventory.Domain/Entities/StockItem.cs`
- `src/Services/Inventory/Inventory.Domain/Interfaces/IStockItemRepository.cs`

### StockItem.cs

**Dosya:** `src/Services/Inventory/Inventory.Domain/Entities/StockItem.cs`

```csharp
using BuildingBlocks.Shared;

namespace Inventory.Domain.Entities;

public class StockItem : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public string SKU { get; private set; }
    public int QuantityOnHand { get; private set; }
    public int ReservedQuantity { get; private set; }
    public int AvailableQuantity => QuantityOnHand - ReservedQuantity;
    public int LowStockThreshold { get; private set; } = 10;
    public string WarehouseLocation { get; private set; }

    private StockItem() { }

    public StockItem(Guid productId, string productName, string sku, int initialQuantity, string warehouseLocation = "Main")
    {
        ProductId = productId;
        ProductName = productName;
        SKU = sku;
        QuantityOnHand = initialQuantity;
        WarehouseLocation = warehouseLocation;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Eklenen miktar pozitif olmalıdır.");
        QuantityOnHand += quantity;
        MarkAsUpdated();
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Çıkarılan miktar pozitif olmalıdır.");
        if (quantity > AvailableQuantity)
            throw new InvalidOperationException("Yetersiz stok.");
        QuantityOnHand -= quantity;
        MarkAsUpdated();
    }

    public void ReserveStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Rezerve miktar pozitif olmalıdır.");
        if (quantity > AvailableQuantity)
            throw new InvalidOperationException("Rezervasyon için yetersiz stok.");
        ReservedQuantity += quantity;
        MarkAsUpdated();
    }

    public void ReleaseReservedStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Serbest bırakılan miktar pozitif olmalıdır.");
        if (quantity > ReservedQuantity)
            throw new InvalidOperationException("Serbest bırakılacak rezerve stok miktarı aşıldı.");
        ReservedQuantity -= quantity;
        MarkAsUpdated();
    }

    public bool IsLowStock() => AvailableQuantity <= LowStockThreshold;

    public void UpdateLowStockThreshold(int threshold)
    {
        if (threshold < 0)
            throw new ArgumentException("Düşük stok eşiği negatif olamaz.");
        LowStockThreshold = threshold;
        MarkAsUpdated();
    }

    public void UpdateProductName(string productName)
    {
        ProductName = productName;
        MarkAsUpdated();
    }
}
```

### IStockItemRepository.cs

**Dosya:** `src/Services/Inventory/Inventory.Domain/Interfaces/IStockItemRepository.cs`

```csharp
using Inventory.Domain.Entities;

namespace Inventory.Domain.Interfaces;

public interface IStockItemRepository
{
    Task<StockItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StockItem?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<StockItem?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<IEnumerable<StockItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<StockItem>> GetLowStockItemsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(StockItem stockItem, CancellationToken cancellationToken = default);
    Task UpdateAsync(StockItem stockItem, CancellationToken cancellationToken = default);
    Task DeleteAsync(StockItem stockItem, CancellationToken cancellationToken = default);
}
```

---

## Task 17: Inventory.Application — OrderCreatedIntegrationEvent Handler

**Amaç:** Inventory mikroservisi application katmanını oluşturmak.

**Dosyalar:**
- `src/Services/Inventory/Inventory.Application/IntegrationEvents/OrderCreated/OrderCreatedIntegrationEvent.cs`
- `src/Services/Inventory/Inventory.Application/IntegrationEvents/OrderCreated/OrderCreatedEventHandler.cs`

### OrderCreatedIntegrationEvent.cs

**Dosya:** `src/Services/Inventory/Inventory.Application/IntegrationEvents/OrderCreated/OrderCreatedIntegrationEvent.cs`

```csharp
using BuildingBlocks.Shared.Messaging;

namespace Inventory.Application.IntegrationEvents.OrderCreated;

// Re-use the same event type name for message routing
public record OrderCreatedEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public List<OrderItemEvent> Items { get; init; } = [];
}

public record OrderItemEvent
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}
```

### OrderCreatedEventHandler.cs

**Dosya:** `src/Services/Inventory/Inventory.Application/IntegrationEvents/OrderCreated/OrderCreatedEventHandler.cs`

```csharp
using BuildingBlocks.Shared.Messaging;
using Inventory.Domain.Interfaces;

namespace Inventory.Application.IntegrationEvents.OrderCreated;

public class OrderCreatedEventHandler : IIntegrationEventHandler<OrderCreatedEvent>
{
    private readonly IStockItemRepository _stockItemRepository;

    public OrderCreatedEventHandler(IStockItemRepository stockItemRepository)
    {
        _stockItemRepository = stockItemRepository;
    }

    public async Task HandleAsync(OrderCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        foreach (var item in @event.Items)
        {
            var stockItem = await _stockItemRepository.GetByProductIdAsync(item.ProductId, cancellationToken);
            if (stockItem is null)
                throw new KeyNotFoundException($"Product {item.ProductId} için stok kaydı bulunamadı.");

            stockItem.ReserveStock(item.Quantity);
            await _stockItemRepository.UpdateAsync(stockItem, cancellationToken);
        }
    }
}
```

---

## Task 18: Inventory.Infrastructure — DbContext, Repositories

**Amaç:** Inventory mikroservisi infrastructure katmanını oluşturmak.

**Dosyalar:**
- `src/Services/Inventory/Inventory.Infrastructure/Data/InventoryDbContext.cs`
- `src/Services/Inventory/Inventory.Infrastructure/Repositories/StockItemRepository.cs`
- `src/Services/Inventory/Inventory.Infrastructure/DependencyInjection.cs`

### InventoryDbContext.cs

**Dosya:** `src/Services/Inventory/Inventory.Infrastructure/Data/InventoryDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Inventory.Domain.Entities;

namespace Inventory.Infrastructure.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

    public DbSet<StockItem> StockItems => Set<StockItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProductId).IsUnique();
            entity.HasIndex(e => e.SKU).IsUnique();
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.SKU).IsRequired().HasMaxLength(100);
            entity.Property(e => e.WarehouseLocation).IsRequired().HasMaxLength(100);
            entity.Property(e => e.QuantityOnHand);
            entity.Property(e => e.ReservedQuantity);
            entity.Property(e => e.LowStockThreshold);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }
}
```

### StockItemRepository.cs

**Dosya:** `src/Services/Inventory/Inventory.Infrastructure/Repositories/StockItemRepository.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure.Data;

namespace Inventory.Infrastructure.Repositories;

public class StockItemRepository : IStockItemRepository
{
    private readonly InventoryDbContext _context;

    public StockItemRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<StockItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.StockItems.FindAsync([id], cancellationToken);
    }

    public async Task<StockItem?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.StockItems.FirstOrDefaultAsync(s => s.ProductId == productId, cancellationToken);
    }

    public async Task<StockItem?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        return await _context.StockItems.FirstOrDefaultAsync(s => s.SKU == sku, cancellationToken);
    }

    public async Task<IEnumerable<StockItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StockItems.OrderBy(s => s.ProductName).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<StockItem>> GetLowStockItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.StockItems
            .Where(s => (s.QuantityOnHand - s.ReservedQuantity) <= s.LowStockThreshold)
            .OrderBy(s => s.ProductName)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(StockItem stockItem, CancellationToken cancellationToken = default)
    {
        await _context.StockItems.AddAsync(stockItem, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(StockItem stockItem, CancellationToken cancellationToken = default)
    {
        _context.StockItems.Update(stockItem);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(StockItem stockItem, CancellationToken cancellationToken = default)
    {
        stockItem.MarkAsDeleted();
        _context.StockItems.Update(stockItem);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
```

### DependencyInjection.cs

**Dosya:** `src/Services/Inventory/Inventory.Infrastructure/DependencyInjection.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure.Data;
using Inventory.Infrastructure.Repositories;

namespace Inventory.Infrastructure;

public static class InventoryInfrastructureDependencyInjection
{
    public static IServiceCollection AddInventoryInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<InventoryDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("InventoryDb"),
                b => b.MigrationsAssembly(typeof(InventoryDbContext).Assembly.FullName)));

        services.AddScoped<IStockItemRepository, StockItemRepository>();

        return services;
    }
}
```

---

## Task 19: Inventory.API — StockController, BackgroundService Consumer, Program.cs

**Amaç:** Inventory mikroservisi API katmanını oluşturmak.

**Dosyalar:**
- `src/Services/Inventory/Inventory.API/Controllers/StockController.cs`
- `src/Services/Inventory/Inventory.API/BackgroundServices/OrderCreatedConsumerService.cs`
- `src/Services/Inventory/Inventory.API/Program.cs`
- `src/Services/Inventory/Inventory.API/Dockerfile`
- `src/Services/Inventory/Inventory.API/appsettings.json`

### StockController.cs

**Dosya:** `src/Services/Inventory/Inventory.API/Controllers/StockController.cs`

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Inventory.Domain.Interfaces;
using Inventory.Domain.Entities;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class StockController : ControllerBase
{
    private readonly IStockItemRepository _stockItemRepository;

    public StockController(IStockItemRepository stockItemRepository)
    {
        _stockItemRepository = stockItemRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockItem>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _stockItemRepository.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<StockItem>>> GetLowStock(CancellationToken cancellationToken)
    {
        var items = await _stockItemRepository.GetLowStockItemsAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("product/{productId:guid}")]
    public async Task<ActionResult<StockItem>> GetByProductId(Guid productId, CancellationToken cancellationToken)
    {
        var item = await _stockItemRepository.GetByProductIdAsync(productId, cancellationToken);
        if (item is null)
            return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<StockItem>> Create(
        [FromBody] CreateStockRequest request, CancellationToken cancellationToken)
    {
        var existing = await _stockItemRepository.GetByProductIdAsync(request.ProductId, cancellationToken);
        if (existing is not null)
            return BadRequest("Bu ürün için stok kaydı zaten mevcut.");

        var stockItem = new StockItem(request.ProductId, request.ProductName, request.SKU, request.InitialQuantity, request.WarehouseLocation);
        await _stockItemRepository.AddAsync(stockItem, cancellationToken);
        return CreatedAtAction(nameof(GetByProductId), new { productId = stockItem.ProductId }, stockItem);
    }

    [HttpPut("{id:guid}/add-stock")]
    public async Task<ActionResult> AddStock(Guid id, [FromBody] int quantity, CancellationToken cancellationToken)
    {
        var item = await _stockItemRepository.GetByIdAsync(id, cancellationToken);
        if (item is null) return NotFound();
        item.AddStock(quantity);
        await _stockItemRepository.UpdateAsync(item, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/remove-stock")]
    public async Task<ActionResult> RemoveStock(Guid id, [FromBody] int quantity, CancellationToken cancellationToken)
    {
        var item = await _stockItemRepository.GetByIdAsync(id, cancellationToken);
        if (item is null) return NotFound();
        item.RemoveStock(quantity);
        await _stockItemRepository.UpdateAsync(item, cancellationToken);
        return NoContent();
    }
}

public record CreateStockRequest
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string SKU { get; init; } = string.Empty;
    public int InitialQuantity { get; init; }
    public string WarehouseLocation { get; init; } = "Main";
}
```

### OrderCreatedConsumerService.cs

**Dosya:** `src/Services/Inventory/Inventory.API/BackgroundServices/OrderCreatedConsumerService.cs`

```csharp
using BuildingBlocks.Shared.Messaging;
using Inventory.Application.IntegrationEvents.OrderCreated;

namespace Inventory.API.BackgroundServices;

public class OrderCreatedConsumerService : BackgroundService
{
    private readonly IEventBus _eventBus;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OrderCreatedConsumerService> _logger;

    public OrderCreatedConsumerService(
        IEventBus eventBus,
        IServiceProvider serviceProvider,
        ILogger<OrderCreatedConsumerService> logger)
    {
        _eventBus = eventBus;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OrderCreated consumer service starting...");

        var handler = new OrderCreatedEventHandler(
            _serviceProvider.GetRequiredService<Inventory.Domain.Interfaces.IStockItemRepository>());

        _eventBus.RegisterHandler(handler);
        await _eventBus.SubscribeAsync<OrderCreatedEvent, OrderCreatedEventHandler>();

        _logger.LogInformation("OrderCreated consumer service started successfully.");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
```

### Program.cs

**Dosya:** `src/Services/Inventory/Inventory.API/Program.cs`

```csharp
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Inventory.Infrastructure;
using BuildingBlocks.Shared.Messaging;
using Inventory.API.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured"))),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddInventoryInfrastructure(builder.Configuration);
builder.Services.AddEventBus(builder.Configuration);
builder.Services.AddHostedService<OrderCreatedConsumerService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### Dockerfile

**Dosya:** `src/Services/Inventory/Inventory.API/Dockerfile`

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/Directory.Build.props .
COPY src/BuildingBlocks/Shared/Shared.csproj BuildingBlocks/Shared/
COPY src/BuildingBlocks/Shared.Messaging/Shared.Messaging.csproj BuildingBlocks/Shared.Messaging/
COPY src/Services/Inventory/Inventory.Domain/Inventory.Domain.csproj Services/Inventory/Inventory.Domain/
COPY src/Services/Inventory/Inventory.Application/Inventory.Application.csproj Services/Inventory/Inventory.Application/
COPY src/Services/Inventory/Inventory.Infrastructure/Inventory.Infrastructure.csproj Services/Inventory/Inventory.Infrastructure/
COPY src/Services/Inventory/Inventory.API/Inventory.API.csproj Services/Inventory/Inventory.API/
RUN dotnet restore Services/Inventory/Inventory.API/Inventory.API.csproj

COPY src/ .
RUN dotnet publish Services/Inventory/Inventory.API/Inventory.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Inventory.API.dll"]
```

### appsettings.json

**Dosya:** `src/Services/Inventory/Inventory.API/appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "InventoryDb": "Server=sqlserver;Database=InventoryDb;User Id=sa;Password=Passw0rd!;TrustServerCertificate=true;"
  },
  "Jwt": {
    "Secret": "SuperSecretKeyForJwtTokenGeneration2024!@#$%",
    "Issuer": "ECommercePlatform",
    "Audience": "ECommercePlatformClient"
  },
  "EventBus": {
    "HostName": "rabbitmq",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "ExchangeName": "ecommerce_exchange",
    "QueueName": "inventory_order_created",
    "RetryCount": 5
  },
  "AllowedHosts": "*"
}
```

---

## Task 20: Notification Service — EventHandler, BackgroundService, Program.cs

**Amaç:** Bildirim mikroservisini oluşturmak: OrderCreated olayını dinleyip e-posta bildirimi göndermek.

**Dosyalar:**
- `src/Services/Notification/Notification.Infrastructure/Services/EmailService.cs`
- `src/Services/Notification/Notification.API/BackgroundServices/NotificationConsumerService.cs`
- `src/Services/Notification/Notification.API/Program.cs`
- `src/Services/Notification/Notification.API/Dockerfile`
- `src/Services/Notification/Notification.API/appsettings.json`

### EmailService.cs

**Dosya:** `src/Services/Notification/Notification.Infrastructure/Services/EmailService.cs`

```csharp
using Microsoft.Extensions.Logging;

namespace Notification.Infrastructure.Services;

public interface IEmailService
{
    Task SendOrderConfirmationAsync(string to, string orderNumber, decimal totalAmount, CancellationToken cancellationToken = default);
}

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendOrderConfirmationAsync(string to, string orderNumber, decimal totalAmount, CancellationToken cancellationToken = default)
    {
        // In production, integrate with SMTP or email provider (SendGrid, etc.)
        _logger.LogInformation(
            "Sending order confirmation email to {To}: Order {OrderNumber} with total {TotalAmount:C}",
            to, orderNumber, totalAmount);

        await Task.Delay(100, cancellationToken); // Simulate email sending
    }
}
```

### NotificationConsumerService.cs

**Dosya:** `src/Services/Notification/Notification.API/BackgroundServices/NotificationConsumerService.cs`

```csharp
using BuildingBlocks.Shared.Messaging;
using Notification.Infrastructure.Services;

namespace Notification.API.BackgroundServices;

// For simplicity, using a shared IntegrationEvent class
// In production, use a shared contracts NuGet package
public record NotificationOrderCreatedEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
}

public class NotificationOrderCreatedEventHandler : IIntegrationEventHandler<NotificationOrderCreatedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<NotificationOrderCreatedEventHandler> _logger;

    public NotificationOrderCreatedEventHandler(IEmailService emailService, ILogger<NotificationOrderCreatedEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task HandleAsync(NotificationOrderCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing notification for order {OrderNumber}", @event.OrderNumber);
        await _emailService.SendOrderConfirmationAsync("customer@example.com", @event.OrderNumber, @event.TotalAmount, cancellationToken);
    }
}

public class NotificationConsumerService : BackgroundService
{
    private readonly IEventBus _eventBus;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationConsumerService> _logger;

    public NotificationConsumerService(
        IEventBus eventBus,
        IServiceProvider serviceProvider,
        ILogger<NotificationConsumerService> logger)
    {
        _eventBus = eventBus;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Notification consumer service starting...");

        var handler = new NotificationOrderCreatedEventHandler(
            _serviceProvider.GetRequiredService<IEmailService>(),
            _serviceProvider.GetRequiredService<ILogger<NotificationOrderCreatedEventHandler>>());

        _eventBus.RegisterHandler(handler);
        await _eventBus.SubscribeAsync<NotificationOrderCreatedEvent, NotificationOrderCreatedEventHandler>();

        _logger.LogInformation("Notification consumer service started successfully.");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
```

### Program.cs

**Dosya:** `src/Services/Notification/Notification.API/Program.cs`

```csharp
using Notification.Infrastructure.Services;
using BuildingBlocks.Shared.Messaging;
using Notification.API.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddEventBus(builder.Configuration);
builder.Services.AddHostedService<NotificationConsumerService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.MapControllers();

app.Run();
```

### Dockerfile

**Dosya:** `src/Services/Notification/Notification.API/Dockerfile`

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/Directory.Build.props .
COPY src/BuildingBlocks/Shared.Messaging/Shared.Messaging.csproj BuildingBlocks/Shared.Messaging/
COPY src/Services/Notification/Notification.API/Notification.API.csproj Services/Notification/Notification.API/
RUN dotnet restore Services/Notification/Notification.API/Notification.API.csproj

COPY src/ .
RUN dotnet publish Services/Notification/Notification.API/Notification.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Notification.API.dll"]
```

### appsettings.json

**Dosya:** `src/Services/Notification/Notification.API/appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "EventBus": {
    "HostName": "rabbitmq",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "ExchangeName": "ecommerce_exchange",
    "QueueName": "notification_order_created",
    "RetryCount": 5
  },
  "AllowedHosts": "*"
}
```

---

## Task 21: API Gateway — YARP Reverse Proxy, Program.cs, appsettings.json

**Amaç:** YARP ile API Gateway oluşturmak, tüm servislere yönlendirme yapmak.

**Dosyalar:**
- `src/ApiGateway/Program.cs`
- `src/ApiGateway/Dockerfile`
- `src/ApiGateway/appsettings.json`

### Program.cs

**Dosya:** `src/ApiGateway/Program.cs`

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");
app.MapReverseProxy();

app.Run();
```

### Dockerfile

**Dosya:** `src/ApiGateway/Dockerfile`

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/ApiGateway/ApiGateway.csproj ApiGateway/
RUN dotnet restore ApiGateway/ApiGateway.csproj

COPY src/ .
RUN dotnet publish ApiGateway/ApiGateway.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
```

### appsettings.json

**Dosya:** `src/ApiGateway/appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ReverseProxy": {
    "Routes": {
      "identity-route": {
        "ClusterId": "identity-cluster",
        "Match": {
          "Path": "/api/auth/{**catch-all}"
        }
      },
      "product-route": {
        "ClusterId": "product-cluster",
        "Match": {
          "Path": "/api/products/{**catch-all}"
        }
      },
      "category-route": {
        "ClusterId": "product-cluster",
        "Match": {
          "Path": "/api/categories/{**catch-all}"
        }
      },
      "order-route": {
        "ClusterId": "order-cluster",
        "Match": {
          "Path": "/api/orders/{**catch-all}"
        }
      },
      "inventory-route": {
        "ClusterId": "inventory-cluster",
        "Match": {
          "Path": "/api/stock/{**catch-all}"
        }
      }
    },
    "Clusters": {
      "identity-cluster": {
        "Destinations": {
          "identity-destination": {
            "Address": "http://identity-api:8080/"
          }
        }
      },
      "product-cluster": {
        "Destinations": {
          "product-destination": {
            "Address": "http://product-api:8080/"
          }
        }
      },
      "order-cluster": {
        "Destinations": {
          "order-destination": {
            "Address": "http://order-api:8080/"
          }
        }
      },
      "inventory-cluster": {
        "Destinations": {
          "inventory-destination": {
            "Address": "http://inventory-api:8080/"
          }
        }
      }
    }
  },
  "AllowedHosts": "*"
}
```

---

## Task 22: Docker Compose + SQL Initialization

**Amaç:** Tüm servisleri ayağa kaldıracak Docker Compose dosyasını ve SQL Server başlatma scriptlerini oluşturmak.

**Dosyalar:**
- `docker-compose.yml`
- `docker/sql/init/init.sql`

### docker-compose.yml

**Dosya:** `ecommerce-platform/docker-compose.yml`

```yaml
version: '3.8'

services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: ecommerce-sqlserver
    environment:
      SA_PASSWORD: "Passw0rd!"
      ACCEPT_EULA: "Y"
      MSSQL_PID: "Developer"
    ports:
      - "1433:1433"
    volumes:
      - sqlserver-data:/var/opt/mssql
      - ./docker/sql/init:/docker-entrypoint-initdb.d
    healthcheck:
      test: /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Passw0rd!" -C -Q "SELECT 1" || exit 1
      interval: 10s
      timeout: 5s
      retries: 10

  redis:
    image: redis:7-alpine
    container_name: ecommerce-redis
    ports:
      - "6379:6379"
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 5s
      timeout: 3s
      retries: 5

  rabbitmq:
    image: rabbitmq:3-management-alpine
    container_name: ecommerce-rabbitmq
    ports:
      - "5672:5672"
      - "15672:15672"
    environment:
      RABBITMQ_DEFAULT_USER: guest
      RABBITMQ_DEFAULT_PASS: guest
    healthcheck:
      test: ["CMD", "rabbitmq-diagnostics", "check_port_connectivity"]
      interval: 10s
      timeout: 5s
      retries: 5

  identity-api:
    build:
      context: ./src
      dockerfile: Services/Identity/Identity.API/Dockerfile
    container_name: ecommerce-identity-api
    ports:
      - "5001:8080"
    depends_on:
      sqlserver:
        condition: service_healthy
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    restart: unless-stopped

  product-api:
    build:
      context: ./src
      dockerfile: Services/Product/Product.API/Dockerfile
    container_name: ecommerce-product-api
    ports:
      - "5002:8080"
    depends_on:
      sqlserver:
        condition: service_healthy
      redis:
        condition: service_healthy
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    restart: unless-stopped

  order-api:
    build:
      context: ./src
      dockerfile: Services/Order/Order.API/Dockerfile
    container_name: ecommerce-order-api
    ports:
      - "5003:8080"
    depends_on:
      sqlserver:
        condition: service_healthy
      rabbitmq:
        condition: service_healthy
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    restart: unless-stopped

  inventory-api:
    build:
      context: ./src
      dockerfile: Services/Inventory/Inventory.API/Dockerfile
    container_name: ecommerce-inventory-api
    ports:
      - "5004:8080"
    depends_on:
      sqlserver:
        condition: service_healthy
      rabbitmq:
        condition: service_healthy
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    restart: unless-stopped

  notification-api:
    build:
      context: ./src
      dockerfile: Services/Notification/Notification.API/Dockerfile
    container_name: ecommerce-notification-api
    ports:
      - "5005:8080"
    depends_on:
      rabbitmq:
        condition: service_healthy
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    restart: unless-stopped

  api-gateway:
    build:
      context: ./src
      dockerfile: ApiGateway/Dockerfile
    container_name: ecommerce-api-gateway
    ports:
      - "5000:8080"
    depends_on:
      - identity-api
      - product-api
      - order-api
      - inventory-api
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    restart: unless-stopped

volumes:
  sqlserver-data:
```

### init.sql

**Dosya:** `docker/sql/init/init.sql`

```sql
-- Identity Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'IdentityDb')
BEGIN
    CREATE DATABASE [IdentityDb];
END
GO

USE [IdentityDb];
GO

-- Product Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ProductDb')
BEGIN
    CREATE DATABASE [ProductDb];
END
GO

-- Order Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'OrderDb')
BEGIN
    CREATE DATABASE [OrderDb];
END
GO

-- Inventory Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'InventoryDb')
BEGIN
    CREATE DATABASE [InventoryDb];
END
GO
```

---

## Task 23: Frontend Setup — Package.json, Vite, Tailwind, Types, API, Stores, App.tsx

**Amaç:** React + Vite + TypeScript + TailwindCSS frontend projesini kurmak.

**Dosyalar:**
- `frontend/package.json`
- `frontend/vite.config.ts`
- `frontend/tsconfig.json`
- `frontend/tsconfig.app.json`
- `frontend/tailwind.config.js`
- `frontend/postcss.config.js`
- `frontend/index.html`
- `frontend/src/types/index.ts`
- `frontend/src/api/client.ts`
- `frontend/src/api/auth.ts`
- `frontend/src/api/products.ts`
- `frontend/src/api/orders.ts`
- `frontend/src/stores/authStore.ts`
- `frontend/src/stores/cartStore.ts`
- `frontend/src/lib/utils.ts`
- `frontend/src/App.tsx`
- `frontend/src/main.tsx`
- `frontend/src/index.css`

### package.json

**Dosya:** `frontend/package.json`

```json
{
  "name": "ecommerce-frontend",
  "private": true,
  "version": "1.0.0",
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "tsc -b && vite build",
    "lint": "eslint .",
    "preview": "vite preview"
  },
  "dependencies": {
    "react": "^19.0.0",
    "react-dom": "^19.0.0",
    "react-router-dom": "^7.1.0",
    "zustand": "^5.0.0",
    "axios": "^1.7.0",
    "react-hot-toast": "^2.4.1",
    "lucide-react": "^0.468.0"
  },
  "devDependencies": {
    "@eslint/js": "^9.17.0",
    "@types/react": "^19.0.0",
    "@types/react-dom": "^19.0.0",
    "@vitejs/plugin-react": "^4.3.0",
    "autoprefixer": "^10.4.0",
    "eslint": "^9.17.0",
    "eslint-plugin-react-hooks": "^5.0.0",
    "eslint-plugin-react-refresh": "^0.4.0",
    "globals": "^15.0.0",
    "postcss": "^8.4.0",
    "tailwindcss": "^3.4.0",
    "typescript": "~5.7.0",
    "typescript-eslint": "^8.0.0",
    "vite": "^6.0.0"
  }
}
```

### vite.config.ts

**Dosya:** `frontend/vite.config.ts`

```typescript
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    port: 3000,
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
      },
    },
  },
});
```

### tsconfig.json

**Dosya:** `frontend/tsconfig.json`

```json
{
  "files": [],
  "references": [
    { "path": "./tsconfig.app.json" }
  ]
}
```

### tsconfig.app.json

**Dosya:** `frontend/tsconfig.app.json`

```json
{
  "compilerOptions": {
    "target": "ES2020",
    "useDefineForClassFields": true,
    "lib": ["ES2020", "DOM", "DOM.Iterable"],
    "module": "ESNext",
    "skipLibCheck": true,
    "moduleResolution": "bundler",
    "allowImportingTsExtensions": true,
    "isolatedModules": true,
    "moduleDetection": "force",
    "noEmit": true,
    "jsx": "react-jsx",
    "strict": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true,
    "noFallthroughCasesInSwitch": true,
    "noUncheckedSideEffectImports": true
  },
  "include": ["src"]
}
```

### tailwind.config.js

**Dosya:** `frontend/tailwind.config.js`

```javascript
/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#eff6ff',
          100: '#dbeafe',
          200: '#bfdbfe',
          300: '#93c5fd',
          400: '#60a5fa',
          500: '#3b82f6',
          600: '#2563eb',
          700: '#1d4ed8',
          800: '#1e40af',
          900: '#1e3a8a',
        },
      },
    },
  },
  plugins: [],
};
```

### postcss.config.js

**Dosya:** `frontend/postcss.config.js`

```javascript
export default {
  plugins: {
    tailwindcss: {},
    autoprefixer: {},
  },
};
```

### index.html

**Dosya:** `frontend/index.html`

```html
<!doctype html>
<html lang="tr">
  <head>
    <meta charset="UTF-8" />
    <link rel="icon" type="image/svg+xml" href="/vite.svg" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>E-Commerce Platform</title>
  </head>
  <body class="bg-gray-50">
    <div id="root"></div>
    <script type="module" src="/src/main.tsx"></script>
  </body>
</html>
```

### types/index.ts

**Dosya:** `frontend/src/types/index.ts`

```typescript
export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: 'Customer' | 'Admin';
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: User;
}

export interface Product {
  id: string;
  name: string;
  description: string;
  price: number;
  currency: string;
  imageUrl: string;
  categoryName: string;
  categoryId: string;
  sku: string;
  isPublished: boolean;
  createdAt: string;
}

export interface Category {
  id: string;
  name: string;
  description: string;
  imageUrl?: string;
  productCount: number;
}

export interface CartItem {
  productId: string;
  productName: string;
  unitPrice: number;
  quantity: number;
  imageUrl: string;
}

export interface Address {
  street: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
}

export interface OrderItem {
  id: string;
  productId: string;
  productName: string;
  unitPrice: number;
  quantity: number;
  totalPrice: number;
}

export interface Order {
  id: string;
  orderNumber: string;
  status: string;
  subTotal: number;
  taxAmount: number;
  shippingCost: number;
  totalAmount: number;
  notes?: string;
  createdAt: string;
  shippingAddress: Address;
  billingAddress: Address;
  items: OrderItem[];
}
```

### api/client.ts

**Dosya:** `frontend/src/api/client.ts`

```typescript
import axios from 'axios';

const apiClient = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('accessToken');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default apiClient;
```

### api/auth.ts

**Dosya:** `frontend/src/api/auth.ts`

```typescript
import apiClient from './client';
import { AuthResponse } from '../types';

export const authApi = {
  register: async (data: { email: string; firstName: string; lastName: string; password: string; confirmPassword: string }) => {
    const response = await apiClient.post<AuthResponse>('/auth/register', data);
    return response.data;
  },

  login: async (data: { email: string; password: string }) => {
    const response = await apiClient.post<AuthResponse>('/auth/login', data);
    return response.data;
  },

  getProfile: async () => {
    const response = await apiClient.get<AuthResponse['user']>('/auth/profile');
    return response.data;
  },
};
```

### api/products.ts

**Dosya:** `frontend/src/api/products.ts`

```typescript
import apiClient from './client';
import { Product, Category } from '../types';

export const productsApi = {
  getAll: async () => {
    const response = await apiClient.get<Product[]>('/products');
    return response.data;
  },

  getById: async (id: string) => {
    const response = await apiClient.get<Product>(`/products/${id}`);
    return response.data;
  },

  search: async (query: string) => {
    const response = await apiClient.get<Product[]>('/products/search', { params: { q: query } });
    return response.data;
  },

  create: async (data: { name: string; description: string; price: number; imageUrl: string; categoryId: string; sku: string }) => {
    const response = await apiClient.post<Product>('/products', data);
    return response.data;
  },

  update: async (id: string, data: { name: string; description: string; price: number; imageUrl: string; categoryId: string }) => {
    const response = await apiClient.put<Product>(`/products/${id}`, data);
    return response.data;
  },

  delete: async (id: string) => {
    await apiClient.delete(`/products/${id}`);
  },

  getCategories: async () => {
    const response = await apiClient.get<Category[]>('/categories');
    return response.data;
  },
};
```

### api/orders.ts

**Dosya:** `frontend/src/api/orders.ts`

```typescript
import apiClient from './client';
import { Order } from '../types';

export const ordersApi = {
  getMyOrders: async () => {
    const response = await apiClient.get<Order[]>('/orders');
    return response.data;
  },

  getById: async (id: string) => {
    const response = await apiClient.get<Order>(`/orders/${id}`);
    return response.data;
  },

  create: async (data: {
    items: { productId: string; productName: string; unitPrice: number; quantity: number }[];
    shippingAddress: { street: string; city: string; state: string; zipCode: string; country: string };
    billingAddress: { street: string; city: string; state: string; zipCode: string; country: string };
    notes?: string;
  }) => {
    const response = await apiClient.post<Order>('/orders', data);
    return response.data;
  },

  confirm: async (id: string) => {
    const response = await apiClient.put<Order>(`/orders/${id}/confirm`);
    return response.data;
  },

  cancel: async (id: string) => {
    await apiClient.put(`/orders/${id}/cancel`);
  },
};
```

### stores/authStore.ts

**Dosya:** `frontend/src/stores/authStore.ts`

```typescript
import { create } from 'zustand';
import { User } from '../types';
import { authApi } from '../api/auth';

interface AuthState {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (data: { email: string; firstName: string; lastName: string; password: string; confirmPassword: string }) => Promise<void>;
  logout: () => void;
  loadUser: () => Promise<void>;
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  isAuthenticated: !!localStorage.getItem('accessToken'),
  isLoading: false,

  login: async (email, password) => {
    const response = await authApi.login({ email, password });
    localStorage.setItem('accessToken', response.accessToken);
    localStorage.setItem('refreshToken', response.refreshToken);
    localStorage.setItem('user', JSON.stringify(response.user));
    set({ user: response.user, isAuthenticated: true });
  },

  register: async (data) => {
    const response = await authApi.register(data);
    localStorage.setItem('accessToken', response.accessToken);
    localStorage.setItem('refreshToken', response.refreshToken);
    localStorage.setItem('user', JSON.stringify(response.user));
    set({ user: response.user, isAuthenticated: true });
  },

  logout: () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
    set({ user: null, isAuthenticated: false });
  },

  loadUser: async () => {
    try {
      set({ isLoading: true });
      const storedUser = localStorage.getItem('user');
      if (storedUser) {
        set({ user: JSON.parse(storedUser), isAuthenticated: true });
      }
    } finally {
      set({ isLoading: false });
    }
  },
}));
```

### stores/cartStore.ts

**Dosya:** `frontend/src/stores/cartStore.ts`

```typescript
import { create } from 'zustand';
import { CartItem } from '../types';

interface CartState {
  items: CartItem[];
  addItem: (item: CartItem) => void;
  removeItem: (productId: string) => void;
  updateQuantity: (productId: string, quantity: number) => void;
  clearCart: () => void;
  getTotal: () => number;
  getItemCount: () => number;
}

export const useCartStore = create<CartState>((set, get) => ({
  items: [],

  addItem: (item) => {
    set((state) => {
      const existing = state.items.find((i) => i.productId === item.productId);
      if (existing) {
        return {
          items: state.items.map((i) =>
            i.productId === item.productId
              ? { ...i, quantity: i.quantity + item.quantity }
              : i
          ),
        };
      }
      return { items: [...state.items, item] };
    });
  },

  removeItem: (productId) => {
    set((state) => ({
      items: state.items.filter((i) => i.productId !== productId),
    }));
  },

  updateQuantity: (productId, quantity) => {
    set((state) => ({
      items: state.items.map((i) =>
        i.productId === productId ? { ...i, quantity } : i
      ),
    }));
  },

  clearCart: () => set({ items: [] }),

  getTotal: () => {
    return get().items.reduce((total, item) => total + item.unitPrice * item.quantity, 0);
  },

  getItemCount: () => {
    return get().items.reduce((count, item) => count + item.quantity, 0);
  },
}));
```

### lib/utils.ts

**Dosya:** `frontend/src/lib/utils.ts`

```typescript
export function formatPrice(price: number, currency: string = 'TRY'): string {
  return new Intl.NumberFormat('tr-TR', {
    style: 'currency',
    currency,
  }).format(price);
}

export function formatDate(date: string): string {
  return new Date(date).toLocaleDateString('tr-TR', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
}

export function getStatusColor(status: string): string {
  const colors: Record<string, string> = {
    Pending: 'bg-yellow-100 text-yellow-800',
    Confirmed: 'bg-blue-100 text-blue-800',
    Shipped: 'bg-purple-100 text-purple-800',
    Delivered: 'bg-green-100 text-green-800',
    Cancelled: 'bg-red-100 text-red-800',
  };
  return colors[status] || 'bg-gray-100 text-gray-800';
}

export function getStatusText(status: string): string {
  const texts: Record<string, string> = {
    Pending: 'Beklemede',
    Confirmed: 'Onaylandı',
    Shipped: 'Kargoda',
    Delivered: 'Teslim Edildi',
    Cancelled: 'İptal Edildi',
  };
  return texts[status] || status;
}

export function cn(...classes: (string | boolean | undefined | null)[]): string {
  return classes.filter(Boolean).join(' ');
}
```

### main.tsx

**Dosya:** `frontend/src/main.tsx`

```typescript
import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import App from './App';
import './index.css';

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <BrowserRouter>
      <App />
    </BrowserRouter>
  </React.StrictMode>
);
```

### index.css

**Dosya:** `frontend/src/index.css`

```css
@tailwind base;
@tailwind components;
@tailwind utilities;

@layer base {
  body {
    @apply text-gray-900 antialiased;
  }
}

@layer components {
  .btn-primary {
    @apply bg-primary-600 text-white px-4 py-2 rounded-lg hover:bg-primary-700 transition-colors duration-200 font-medium disabled:opacity-50 disabled:cursor-not-allowed;
  }

  .btn-secondary {
    @apply bg-gray-200 text-gray-800 px-4 py-2 rounded-lg hover:bg-gray-300 transition-colors duration-200 font-medium;
  }

  .btn-danger {
    @apply bg-red-600 text-white px-4 py-2 rounded-lg hover:bg-red-700 transition-colors duration-200 font-medium;
  }

  .input-field {
    @apply w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent transition-all duration-200;
  }

  .card {
    @apply bg-white rounded-xl shadow-sm border border-gray-100 p-6;
  }
}
```

### App.tsx

**Dosya:** `frontend/src/App.tsx`

```typescript
import { Routes, Route, Navigate } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import { useEffect } from 'react';
import { useAuthStore } from './stores/authStore';
import Layout from './components/Layout';
import ProtectedRoute from './components/ProtectedRoute';
import LoginPage from './pages/auth/LoginPage';
import RegisterPage from './pages/auth/RegisterPage';
import ProductsPage from './pages/products/ProductsPage';
import ProductDetailPage from './pages/products/ProductDetailPage';
import CartPage from './pages/cart/CartPage';
import CheckoutPage from './pages/cart/CheckoutPage';
import OrdersPage from './pages/orders/OrdersPage';
import OrderDetailPage from './pages/orders/OrderDetailPage';
import DashboardPage from './pages/orders/DashboardPage';
import AdminPage from './pages/admin/AdminPage';

function App() {
  const { loadUser, isAuthenticated } = useAuthStore();

  useEffect(() => {
    if (isAuthenticated) {
      loadUser();
    }
  }, [isAuthenticated, loadUser]);

  return (
    <>
      <Toaster position="top-right" />
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route element={<Layout />}>
          <Route path="/" element={<Navigate to="/products" replace />} />
          <Route path="/products" element={<ProductsPage />} />
          <Route path="/products/:id" element={<ProductDetailPage />} />
          <Route path="/cart" element={<CartPage />} />
          <Route path="/checkout" element={<ProtectedRoute><CheckoutPage /></ProtectedRoute>} />
          <Route path="/orders" element={<ProtectedRoute><OrdersPage /></ProtectedRoute>} />
          <Route path="/orders/:id" element={<ProtectedRoute><OrderDetailPage /></ProtectedRoute>} />
          <Route path="/dashboard" element={<ProtectedRoute><DashboardPage /></ProtectedRoute>} />
          <Route path="/admin" element={<ProtectedRoute requireAdmin><AdminPage /></ProtectedRoute>} />
        </Route>
      </Routes>
    </>
  );
}

export default App;
```

---

## Task 24: Frontend Components — Navbar, Layout, ProtectedRoute

**Amaç:** Ortak frontend bileşenlerini oluşturmak.

**Dosyalar:**
- `frontend/src/components/Navbar.tsx`
- `frontend/src/components/Layout.tsx`
- `frontend/src/components/ProtectedRoute.tsx`

### Navbar.tsx

**Dosya:** `frontend/src/components/Navbar.tsx`

```typescript
import { Link, useNavigate } from 'react-router-dom';
import { ShoppingCart, Package, LayoutDashboard, Settings, LogOut, User } from 'lucide-react';
import { useAuthStore } from '../stores/authStore';
import { useCartStore } from '../stores/cartStore';

export default function Navbar() {
  const { user, isAuthenticated, logout } = useAuthStore();
  const itemCount = useCartStore((state) => state.getItemCount());
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav className="bg-white shadow-sm border-b border-gray-200">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16">
          <div className="flex items-center space-x-8">
            <Link to="/products" className="text-xl font-bold text-primary-600">
              E-Commerce
            </Link>
            <Link to="/products" className="text-gray-600 hover:text-gray-900 transition-colors">
              Ürünler
            </Link>
            {isAuthenticated && (
              <>
                <Link to="/orders" className="text-gray-600 hover:text-gray-900 transition-colors flex items-center gap-1">
                  <Package size={18} />
                  Siparişlerim
                </Link>
                <Link to="/dashboard" className="text-gray-600 hover:text-gray-900 transition-colors flex items-center gap-1">
                  <LayoutDashboard size={18} />
                  Panel
                </Link>
                {user?.role === 'Admin' && (
                  <Link to="/admin" className="text-gray-600 hover:text-gray-900 transition-colors flex items-center gap-1">
                    <Settings size={18} />
                    Yönetim
                  </Link>
                )}
              </>
            )}
          </div>
          <div className="flex items-center space-x-4">
            <Link to="/cart" className="relative text-gray-600 hover:text-gray-900 transition-colors">
              <ShoppingCart size={24} />
              {itemCount > 0 && (
                <span className="absolute -top-2 -right-2 bg-primary-600 text-white text-xs rounded-full h-5 w-5 flex items-center justify-center">
                  {itemCount}
                </span>
              )}
            </Link>
            {isAuthenticated ? (
              <div className="flex items-center space-x-4">
                <div className="flex items-center gap-2 text-sm text-gray-600">
                  <User size={18} />
                  <span>{user?.firstName} {user?.lastName}</span>
                </div>
                <button onClick={handleLogout} className="text-gray-600 hover:text-red-600 transition-colors">
                  <LogOut size={20} />
                </button>
              </div>
            ) : (
              <div className="flex items-center space-x-2">
                <Link to="/login" className="btn-secondary text-sm">Giriş Yap</Link>
                <Link to="/register" className="btn-primary text-sm">Kayıt Ol</Link>
              </div>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
}
```

### Layout.tsx

**Dosya:** `frontend/src/components/Layout.tsx`

```typescript
import { Outlet } from 'react-router-dom';
import Navbar from './Navbar';

export default function Layout() {
  return (
    <div className="min-h-screen bg-gray-50">
      <Navbar />
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <Outlet />
      </main>
    </div>
  );
}
```

### ProtectedRoute.tsx

**Dosya:** `frontend/src/components/ProtectedRoute.tsx`

```typescript
import { Navigate } from 'react-router-dom';
import { useAuthStore } from '../stores/authStore';

interface Props {
  children: React.ReactNode;
  requireAdmin?: boolean;
}

export default function ProtectedRoute({ children, requireAdmin = false }: Props) {
  const { isAuthenticated, user } = useAuthStore();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (requireAdmin && user?.role !== 'Admin') {
    return <Navigate to="/products" replace />;
  }

  return <>{children}</>;
}
```

---

## Task 25: Frontend Hooks — useAuth, useProducts, useOrders

**Amaç:** Frontend custom hook'larını oluşturmak.

**Dosyalar:**
- `frontend/src/hooks/useAuth.ts`
- `frontend/src/hooks/useProducts.ts`
- `frontend/src/hooks/useOrders.ts`

### useAuth.ts

**Dosya:** `frontend/src/hooks/useAuth.ts`

```typescript
import { useState } from 'react';
import { useAuthStore } from '../stores/authStore';
import toast from 'react-hot-toast';

export function useAuth() {
  const { login, register, logout, user, isAuthenticated } = useAuthStore();
  const [isLoading, setIsLoading] = useState(false);

  const handleLogin = async (email: string, password: string) => {
    try {
      setIsLoading(true);
      await login(email, password);
      toast.success('Başarıyla giriş yaptınız!');
      return true;
    } catch (error: any) {
      const message = error.response?.data?.message || 'Giriş yapılırken bir hata oluştu.';
      toast.error(message);
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  const handleRegister = async (data: { email: string; firstName: string; lastName: string; password: string; confirmPassword: string }) => {
    try {
      setIsLoading(true);
      await register(data);
      toast.success('Kayıt başarıyla tamamlandı!');
      return true;
    } catch (error: any) {
      const message = error.response?.data?.message || 'Kayıt olurken bir hata oluştu.';
      toast.error(message);
      return false;
    } finally {
      setIsLoading(false);
    }
  };

  return { handleLogin, handleRegister, logout, user, isAuthenticated, isLoading };
}
```

### useProducts.ts

**Dosya:** `frontend/src/hooks/useProducts.ts`

```typescript
import { useState, useEffect } from 'react';
import { Product, Category } from '../types';
import { productsApi } from '../api/products';
import toast from 'react-hot-toast';

export function useProducts() {
  const [products, setProducts] = useState<Product[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  const fetchProducts = async () => {
    try {
      setIsLoading(true);
      const data = await productsApi.getAll();
      setProducts(data);
    } catch (error) {
      toast.error('Ürünler yüklenirken bir hata oluştu.');
    } finally {
      setIsLoading(false);
    }
  };

  const fetchCategories = async () => {
    try {
      const data = await productsApi.getCategories();
      setCategories(data);
    } catch (error) {
      toast.error('Kategoriler yüklenirken bir hata oluştu.');
    }
  };

  useEffect(() => {
    fetchProducts();
    fetchCategories();
  }, []);

  const createProduct = async (data: { name: string; description: string; price: number; imageUrl: string; categoryId: string; sku: string }) => {
    try {
      const product = await productsApi.create(data);
      setProducts((prev) => [product, ...prev]);
      toast.success('Ürün başarıyla oluşturuldu!');
      return true;
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Ürün oluşturulurken hata oluştu.');
      return false;
    }
  };

  const updateProduct = async (id: string, data: { name: string; description: string; price: number; imageUrl: string; categoryId: string }) => {
    try {
      const updated = await productsApi.update(id, data);
      setProducts((prev) => prev.map((p) => (p.id === id ? updated : p)));
      toast.success('Ürün başarıyla güncellendi!');
      return true;
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Ürün güncellenirken hata oluştu.');
      return false;
    }
  };

  const deleteProduct = async (id: string) => {
    try {
      await productsApi.delete(id);
      setProducts((prev) => prev.filter((p) => p.id !== id));
      toast.success('Ürün başarıyla silindi!');
      return true;
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Ürün silinirken hata oluştu.');
      return false;
    }
  };

  return { products, categories, isLoading, createProduct, updateProduct, deleteProduct, refresh: fetchProducts };
}
```

### useOrders.ts

**Dosya:** `frontend/src/hooks/useOrders.ts`

```typescript
import { useState, useEffect } from 'react';
import { Order } from '../types';
import { ordersApi } from '../api/orders';
import toast from 'react-hot-toast';

export function useOrders() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  const fetchOrders = async () => {
    try {
      setIsLoading(true);
      const data = await ordersApi.getMyOrders();
      setOrders(data);
    } catch (error) {
      toast.error('Siparişler yüklenirken bir hata oluştu.');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchOrders();
  }, []);

  const createOrder = async (data: {
    items: { productId: string; productName: string; unitPrice: number; quantity: number }[];
    shippingAddress: { street: string; city: string; state: string; zipCode: string; country: string };
    billingAddress: { street: string; city: string; state: string; zipCode: string; country: string };
    notes?: string;
  }) => {
    try {
      const order = await ordersApi.create(data);
      setOrders((prev) => [order, ...prev]);
      toast.success('Sipariş başarıyla oluşturuldu!');
      return order;
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Sipariş oluşturulurken hata oluştu.');
      return null;
    }
  };

  const cancelOrder = async (id: string) => {
    try {
      await ordersApi.cancel(id);
      setOrders((prev) => prev.map((o) => (o.id === id ? { ...o, status: 'Cancelled' } : o)));
      toast.success('Sipariş iptal edildi.');
      return true;
    } catch (error: any) {
      toast.error(error.response?.data?.message || 'Sipariş iptal edilirken hata oluştu.');
      return false;
    }
  };

  return { orders, isLoading, createOrder, cancelOrder, refresh: fetchOrders };
}
```

---

## Task 26: Frontend Auth Pages — Login, Register

**Amaç:** Kimlik doğrulama sayfalarını oluşturmak.

**Dosyalar:**
- `frontend/src/pages/auth/LoginPage.tsx`
- `frontend/src/pages/auth/RegisterPage.tsx`

### LoginPage.tsx

**Dosya:** `frontend/src/pages/auth/LoginPage.tsx`

```typescript
import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';

export default function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const { handleLogin, isLoading } = useAuth();
  const navigate = useNavigate();

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const success = await handleLogin(email, password);
    if (success) navigate('/products');
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
      <div className="max-w-md w-full space-y-8">
        <div className="text-center">
          <h1 className="text-3xl font-bold text-gray-900">Giriş Yap</h1>
          <p className="mt-2 text-gray-600">Hesabınıza giriş yapmak için bilgilerinizi girin.</p>
        </div>
        <form onSubmit={onSubmit} className="card space-y-6">
          <div>
            <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-1">E-posta</label>
            <input id="email" type="email" required value={email} onChange={(e) => setEmail(e.target.value)} className="input-field" placeholder="ornek@email.com" />
          </div>
          <div>
            <label htmlFor="password" className="block text-sm font-medium text-gray-700 mb-1">Şifre</label>
            <input id="password" type="password" required value={password} onChange={(e) => setPassword(e.target.value)} className="input-field" placeholder="••••••••" />
          </div>
          <button type="submit" disabled={isLoading} className="btn-primary w-full">
            {isLoading ? 'Giriş yapılıyor...' : 'Giriş Yap'}
          </button>
          <p className="text-center text-sm text-gray-600">
            Hesabınız yok mu? <Link to="/register" className="text-primary-600 hover:underline">Kayıt Ol</Link>
          </p>
        </form>
      </div>
    </div>
  );
}
```

### RegisterPage.tsx

**Dosya:** `frontend/src/pages/auth/RegisterPage.tsx`

```typescript
import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';

export default function RegisterPage() {
  const [form, setForm] = useState({ email: '', firstName: '', lastName: '', password: '', confirmPassword: '' });
  const { handleRegister, isLoading } = useAuth();
  const navigate = useNavigate();

  const updateField = (field: string) => (e: React.ChangeEvent<HTMLInputElement>) =>
    setForm((prev) => ({ ...prev, [field]: e.target.value }));

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (form.password !== form.confirmPassword) {
      alert('Şifreler eşleşmiyor.');
      return;
    }
    const success = await handleRegister(form);
    if (success) navigate('/products');
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
      <div className="max-w-md w-full space-y-8">
        <div className="text-center">
          <h1 className="text-3xl font-bold text-gray-900">Kayıt Ol</h1>
          <p className="mt-2 text-gray-600">Yeni bir hesap oluşturun.</p>
        </div>
        <form onSubmit={onSubmit} className="card space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Ad</label>
            <input type="text" required value={form.firstName} onChange={updateField('firstName')} className="input-field" placeholder="Adınız" />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Soyad</label>
            <input type="text" required value={form.lastName} onChange={updateField('lastName')} className="input-field" placeholder="Soyadınız" />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">E-posta</label>
            <input type="email" required value={form.email} onChange={updateField('email')} className="input-field" placeholder="ornek@email.com" />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Şifre</label>
            <input type="password" required value={form.password} onChange={updateField('password')} className="input-field" placeholder="••••••••" />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Şifre Tekrar</label>
            <input type="password" required value={form.confirmPassword} onChange={updateField('confirmPassword')} className="input-field" placeholder="••••••••" />
          </div>
          <button type="submit" disabled={isLoading} className="btn-primary w-full">
            {isLoading ? 'Kaydediliyor...' : 'Kayıt Ol'}
          </button>
          <p className="text-center text-sm text-gray-600">
            Zaten hesabınız var mı? <Link to="/login" className="text-primary-600 hover:underline">Giriş Yap</Link>
          </p>
        </form>
      </div>
    </div>
  );
}
```

---

## Task 27: Frontend Product Pages — Products, ProductDetail

**Amaç:** Ürün listeleme ve detay sayfalarını oluşturmak.

**Dosyalar:**
- `frontend/src/pages/products/ProductsPage.tsx`
- `frontend/src/pages/products/ProductDetailPage.tsx`

### ProductsPage.tsx

**Dosya:** `frontend/src/pages/products/ProductsPage.tsx`

```typescript
import { useState } from 'react';
import { Link } from 'react-router-dom';
import { Search, ShoppingCartPlus } from 'lucide-react';
import { useProducts } from '../../hooks/useProducts';
import { useCartStore } from '../../stores/cartStore';
import { formatPrice } from '../../lib/utils';
import toast from 'react-hot-toast';

export default function ProductsPage() {
  const { products, categories, isLoading } = useProducts();
  const [search, setSearch] = useState('');
  const [selectedCategory, setSelectedCategory] = useState<string>('');
  const addItem = useCartStore((state) => state.addItem);

  const filtered = products.filter((p) => {
    const matchesSearch = p.name.toLowerCase().includes(search.toLowerCase()) ||
      p.description.toLowerCase().includes(search.toLowerCase());
    const matchesCategory = !selectedCategory || p.categoryId === selectedCategory;
    return matchesSearch && matchesCategory;
  });

  const handleAddToCart = (product: typeof products[0]) => {
    addItem({
      productId: product.id,
      productName: product.name,
      unitPrice: product.price,
      quantity: 1,
      imageUrl: product.imageUrl,
    });
    toast.success(`${product.name} sepete eklendi!`);
  };

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row gap-4">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" size={20} />
          <input type="text" value={search} onChange={(e) => setSearch(e.target.value)} placeholder="Ürün ara..." className="input-field pl-10" />
        </div>
        <select value={selectedCategory} onChange={(e) => setSelectedCategory(e.target.value)} className="input-field sm:w-48">
          <option value="">Tüm Kategoriler</option>
          {categories.map((c) => (
            <option key={c.id} value={c.id}>{c.name}</option>
          ))}
        </select>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        {filtered.map((product) => (
          <div key={product.id} className="card hover:shadow-md transition-shadow">
            <Link to={`/products/${product.id}`}>
              <div className="aspect-square bg-gray-100 rounded-lg mb-4 overflow-hidden">
                {product.imageUrl ? (
                  <img src={product.imageUrl} alt={product.name} className="w-full h-full object-cover" />
                ) : (
                  <div className="w-full h-full flex items-center justify-center text-gray-400">No Image</div>
                )}
              </div>
              <h3 className="font-semibold text-gray-900 mb-1">{product.name}</h3>
              <p className="text-sm text-gray-500 mb-2 line-clamp-2">{product.description}</p>
              <p className="text-lg font-bold text-primary-600">{formatPrice(product.price)}</p>
            </Link>
            <button onClick={() => handleAddToCart(product)} className="btn-primary w-full mt-4 flex items-center justify-center gap-2">
              <ShoppingCartPlus size={18} /> Sepete Ekle
            </button>
          </div>
        ))}
      </div>

      {filtered.length === 0 && (
        <div className="text-center py-12 text-gray-500">Ürün bulunamadı.</div>
      )}
    </div>
  );
}
```

### ProductDetailPage.tsx

**Dosya:** `frontend/src/pages/products/ProductDetailPage.tsx`

```typescript
import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { ArrowLeft, ShoppingCartPlus } from 'lucide-react';
import { Product } from '../../types';
import { productsApi } from '../../api/products';
import { useCartStore } from '../../stores/cartStore';
import { formatPrice, formatDate } from '../../lib/utils';
import toast from 'react-hot-toast';

export default function ProductDetailPage() {
  const { id } = useParams<{ id: string }>();
  const [product, setProduct] = useState<Product | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const addItem = useCartStore((state) => state.addItem);

  useEffect(() => {
    if (!id) return;
    productsApi.getById(id).then(setProduct).catch(() => toast.error('Ürün bulunamadı.')).finally(() => setIsLoading(false));
  }, [id]);

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  if (!product) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500 mb-4">Ürün bulunamadı.</p>
        <Link to="/products" className="btn-primary">Ürünlere Dön</Link>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto">
      <Link to="/products" className="inline-flex items-center gap-2 text-gray-600 hover:text-gray-900 mb-6">
        <ArrowLeft size={20} /> Ürünlere Dön
      </Link>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
        <div className="aspect-square bg-gray-100 rounded-xl overflow-hidden">
          {product.imageUrl ? (
            <img src={product.imageUrl} alt={product.name} className="w-full h-full object-cover" />
          ) : (
            <div className="w-full h-full flex items-center justify-center text-gray-400">No Image</div>
          )}
        </div>
        <div className="space-y-4">
          <h1 className="text-3xl font-bold text-gray-900">{product.name}</h1>
          <span className="inline-block bg-primary-100 text-primary-800 text-sm px-3 py-1 rounded-full">
            {product.categoryName}
          </span>
          <p className="text-4xl font-bold text-primary-600">{formatPrice(product.price)}</p>
          <p className="text-gray-600">{product.description}</p>
          <div className="border-t pt-4 space-y-2 text-sm text-gray-500">
            <p><strong>SKU:</strong> {product.sku}</p>
            <p><strong>Oluşturulma:</strong> {formatDate(product.createdAt)}</p>
          </div>
          <button onClick={() => { addItem({ productId: product.id, productName: product.name, unitPrice: product.price, quantity: 1, imageUrl: product.imageUrl }); toast.success('Sepete eklendi!'); }} className="btn-primary w-full flex items-center justify-center gap-2 py-3 text-lg">
            <ShoppingCartPlus size={24} /> Sepete Ekle
          </button>
        </div>
      </div>
    </div>
  );
}
```

---

## Task 28: Frontend Cart & Checkout Pages

**Amaç:** Sepet ve ödeme sayfalarını oluşturmak.

**Dosyalar:**
- `frontend/src/pages/cart/CartPage.tsx`
- `frontend/src/pages/cart/CheckoutPage.tsx`

### CartPage.tsx

**Dosya:** `frontend/src/pages/cart/CartPage.tsx`

```typescript
import { Link } from 'react-router-dom';
import { Trash2, Minus, Plus, ShoppingBag } from 'lucide-react';
import { useCartStore } from '../../stores/cartStore';
import { formatPrice } from '../../lib/utils';

export default function CartPage() {
  const { items, removeItem, updateQuantity, getTotal } = useCartStore();

  if (items.length === 0) {
    return (
      <div className="text-center py-16">
        <ShoppingBag size={64} className="mx-auto text-gray-300 mb-4" />
        <h2 className="text-xl font-semibold text-gray-900 mb-2">Sepetiniz Boş</h2>
        <p className="text-gray-500 mb-6">Alışverişe başlamak için ürünleri keşfedin.</p>
        <Link to="/products" className="btn-primary">Alışverişe Başla</Link>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto">
      <h1 className="text-2xl font-bold text-gray-900 mb-6">Sepetim</h1>
      <div className="space-y-4">
        {items.map((item) => (
          <div key={item.productId} className="card flex items-center gap-4">
            <div className="w-20 h-20 bg-gray-100 rounded-lg overflow-hidden flex-shrink-0">
              {item.imageUrl ? <img src={item.imageUrl} alt={item.productName} className="w-full h-full object-cover" /> : <div className="w-full h-full flex items-center justify-center text-gray-400">Img</div>}
            </div>
            <div className="flex-1 min-w-0">
              <h3 className="font-semibold text-gray-900 truncate">{item.productName}</h3>
              <p className="text-primary-600 font-medium">{formatPrice(item.unitPrice)}</p>
            </div>
            <div className="flex items-center gap-2">
              <button onClick={() => item.quantity > 1 && updateQuantity(item.productId, item.quantity - 1)} className="p-1 rounded hover:bg-gray-100"><Minus size={18} /></button>
              <span className="w-8 text-center font-medium">{item.quantity}</span>
              <button onClick={() => updateQuantity(item.productId, item.quantity + 1)} className="p-1 rounded hover:bg-gray-100"><Plus size={18} /></button>
            </div>
            <p className="font-bold text-gray-900 w-24 text-right">{formatPrice(item.unitPrice * item.quantity)}</p>
            <button onClick={() => removeItem(item.productId)} className="p-2 text-red-500 hover:bg-red-50 rounded-lg transition-colors"><Trash2 size={20} /></button>
          </div>
        ))}
      </div>
      <div className="card mt-6">
        <div className="flex justify-between items-center mb-4">
          <span className="text-lg font-semibold">Toplam</span>
          <span className="text-2xl font-bold text-primary-600">{formatPrice(getTotal())}</span>
        </div>
        <Link to="/checkout" className="btn-primary w-full block text-center">Siparişi Tamamla</Link>
      </div>
    </div>
  );
}
```

### CheckoutPage.tsx

**Dosya:** `frontend/src/pages/cart/CheckoutPage.tsx`

```typescript
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useCartStore } from '../../stores/cartStore';
import { useOrders } from '../../hooks/useOrders';
import { formatPrice } from '../../lib/utils';

export default function CheckoutPage() {
  const { items, getTotal, clearCart } = useCartStore();
  const { createOrder } = useOrders();
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);
  const [form, setForm] = useState({
    street: '', city: '', state: '', zipCode: '', country: 'Türkiye',
    bStreet: '', bCity: '', bState: '', bZipCode: '', bCountry: 'Türkiye',
    notes: '',
  });

  const update = (f: string) => (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) =>
    setForm((p) => ({ ...p, [f]: e.target.value }));

  const sameAddress = form.street === form.bStreet && form.city === form.bCity;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (items.length === 0) return;
    setIsLoading(true);

    const orderData = {
      items: items.map((i) => ({ productId: i.productId, productName: i.productName, unitPrice: i.unitPrice, quantity: i.quantity })),
      shippingAddress: { street: form.street, city: form.city, state: form.state, zipCode: form.zipCode, country: form.country },
      billingAddress: { street: form.bStreet || form.street, city: form.bCity || form.city, state: form.bState || form.state, zipCode: form.bZipCode || form.zipCode, country: form.bCountry || form.country },
      notes: form.notes || undefined,
    };

    const order = await createOrder(orderData);
    setIsLoading(false);
    if (order) {
      clearCart();
      navigate(`/orders/${order.id}`);
    }
  };

  if (items.length === 0) {
    navigate('/cart');
    return null;
  }

  return (
    <div className="max-w-4xl mx-auto">
      <h1 className="text-2xl font-bold text-gray-900 mb-6">Siparişi Tamamla</h1>
      <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div className="card space-y-4">
          <h2 className="font-semibold text-lg">Teslimat Adresi</h2>
          <input required placeholder="Cadde/Sokak" value={form.street} onChange={update('street')} className="input-field" />
          <div className="grid grid-cols-2 gap-4">
            <input required placeholder="Şehir" value={form.city} onChange={update('city')} className="input-field" />
            <input required placeholder="İlçe" value={form.state} onChange={update('state')} className="input-field" />
          </div>
          <div className="grid grid-cols-2 gap-4">
            <input required placeholder="Posta Kodu" value={form.zipCode} onChange={update('zipCode')} className="input-field" />
            <input required placeholder="Ülke" value={form.country} onChange={update('country')} className="input-field" />
          </div>
        </div>
        <div className="card space-y-4">
          <h2 className="font-semibold text-lg">Fatura Adresi</h2>
          <input required placeholder="Cadde/Sokak" value={form.bStreet} onChange={update('bStreet')} className="input-field" />
          <div className="grid grid-cols-2 gap-4">
            <input required placeholder="Şehir" value={form.bCity} onChange={update('bCity')} className="input-field" />
            <input required placeholder="İlçe" value={form.bState} onChange={update('bState')} className="input-field" />
          </div>
          <div className="grid grid-cols-2 gap-4">
            <input required placeholder="Posta Kodu" value={form.bZipCode} onChange={update('bZipCode')} className="input-field" />
            <input required placeholder="Ülke" value={form.bCountry} onChange={update('bCountry')} className="input-field" />
          </div>
          {!sameAddress && <p className="text-sm text-yellow-600">Fatura adresiniz teslimat adresinizden farklı.</p>}
        </div>
        <div className="md:col-span-2 card space-y-4">
          <h2 className="font-semibold text-lg">Sipariş Özeti</h2>
          <div className="space-y-2">
            {items.map((i) => (
              <div key={i.productId} className="flex justify-between text-sm">
                <span>{i.productName} x {i.quantity}</span>
                <span>{formatPrice(i.unitPrice * i.quantity)}</span>
              </div>
            ))}
          </div>
          <div className="border-t pt-4 flex justify-between items-center">
            <span className="font-bold text-lg">Toplam</span>
            <span className="font-bold text-2xl text-primary-600">{formatPrice(getTotal())}</span>
          </div>
          <textarea placeholder="Sipariş notu (isteğe bağlı)" value={form.notes} onChange={update('notes')} className="input-field" rows={3} />
          <button type="submit" disabled={isLoading} className="btn-primary w-full py-3 text-lg">
            {isLoading ? 'Sipariş oluşturuluyor...' : `Siparişi Tamamla (${formatPrice(getTotal())})`}
          </button>
        </div>
      </form>
    </div>
  );
}
```

---

## Task 29: Frontend Orders & Dashboard Pages

**Amaç:** Sipariş listeleme, detay ve dashboard sayfalarını oluşturmak.

**Dosyalar:**
- `frontend/src/pages/orders/OrdersPage.tsx`
- `frontend/src/pages/orders/OrderDetailPage.tsx`
- `frontend/src/pages/orders/DashboardPage.tsx`

### OrdersPage.tsx

**Dosya:** `frontend/src/pages/orders/OrdersPage.tsx`

```typescript
import { Link } from 'react-router-dom';
import { Package } from 'lucide-react';
import { useOrders } from '../../hooks/useOrders';
import { formatPrice, formatDate, getStatusColor, getStatusText } from '../../lib/utils';

export default function OrdersPage() {
  const { orders, isLoading } = useOrders();

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  if (orders.length === 0) {
    return (
      <div className="text-center py-16">
        <Package size={64} className="mx-auto text-gray-300 mb-4" />
        <h2 className="text-xl font-semibold text-gray-900 mb-2">Henüz Siparişiniz Yok</h2>
        <p className="text-gray-500 mb-6">Alışverişe başlayın ve ilk siparişinizi oluşturun.</p>
        <Link to="/products" className="btn-primary">Alışverişe Başla</Link>
      </div>
    );
  }

  return (
    <div>
      <h1 className="text-2xl font-bold text-gray-900 mb-6">Siparişlerim</h1>
      <div className="space-y-4">
        {orders.map((order) => (
          <Link key={order.id} to={`/orders/${order.id}`} className="card block hover:shadow-md transition-shadow">
            <div className="flex justify-between items-start mb-2">
              <div>
                <p className="font-semibold text-gray-900">{order.orderNumber}</p>
                <p className="text-sm text-gray-500">{formatDate(order.createdAt)}</p>
              </div>
              <span className={`px-3 py-1 rounded-full text-sm font-medium ${getStatusColor(order.status)}`}>
                {getStatusText(order.status)}
              </span>
            </div>
            <div className="flex justify-between items-center mt-4 pt-4 border-t">
              <p className="text-sm text-gray-600">{order.items.length} ürün</p>
              <p className="font-bold text-primary-600">{formatPrice(order.totalAmount)}</p>
            </div>
          </Link>
        ))}
      </div>
    </div>
  );
}
```

### OrderDetailPage.tsx

**Dosya:** `frontend/src/pages/orders/OrderDetailPage.tsx`

```typescript
import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { ArrowLeft } from 'lucide-react';
import { ordersApi } from '../../api/orders';
import { Order } from '../../types';
import { formatPrice, formatDate, getStatusColor, getStatusText } from '../../lib/utils';
import { useOrders } from '../../hooks/useOrders';
import toast from 'react-hot-toast';

export default function OrderDetailPage() {
  const { id } = useParams<{ id: string }>();
  const [order, setOrder] = useState<Order | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const { cancelOrder } = useOrders();

  useEffect(() => {
    if (!id) return;
    ordersApi.getById(id).then(setOrder).catch(() => toast.error('Sipariş bulunamadı.')).finally(() => setIsLoading(false));
  }, [id]);

  const handleCancel = async () => {
    if (!id) return;
    const success = await cancelOrder(id);
    if (success) setOrder((prev) => prev ? { ...prev, status: 'Cancelled' } : null);
  };

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  if (!order) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500 mb-4">Sipariş bulunamadı.</p>
        <Link to="/orders" className="btn-primary">Siparişlerime Dön</Link>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto">
      <Link to="/orders" className="inline-flex items-center gap-2 text-gray-600 hover:text-gray-900 mb-6">
        <ArrowLeft size={20} /> Siparişlerime Dön
      </Link>
      <div className="card mb-6">
        <div className="flex justify-between items-start mb-4">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">{order.orderNumber}</h1>
            <p className="text-gray-500">{formatDate(order.createdAt)}</p>
          </div>
          <span className={`px-3 py-1 rounded-full text-sm font-medium ${getStatusColor(order.status)}`}>
            {getStatusText(order.status)}
          </span>
        </div>
        {order.status === 'Pending' && (
          <button onClick={handleCancel} className="btn-danger">Siparişi İptal Et</button>
        )}
      </div>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
        <div className="card">
          <h2 className="font-semibold mb-2">Teslimat Adresi</h2>
          <p className="text-gray-600">{order.shippingAddress.street}</p>
          <p className="text-gray-600">{order.shippingAddress.city}, {order.shippingAddress.state} {order.shippingAddress.zipCode}</p>
          <p className="text-gray-600">{order.shippingAddress.country}</p>
        </div>
        <div className="card">
          <h2 className="font-semibold mb-2">Fatura Adresi</h2>
          <p className="text-gray-600">{order.billingAddress.street}</p>
          <p className="text-gray-600">{order.billingAddress.city}, {order.billingAddress.state} {order.billingAddress.zipCode}</p>
          <p className="text-gray-600">{order.billingAddress.country}</p>
        </div>
      </div>
      <div className="card">
        <h2 className="font-semibold mb-4">Sipariş Kalemleri</h2>
        <div className="space-y-3">
          {order.items.map((item) => (
            <div key={item.id} className="flex justify-between items-center py-2 border-b last:border-0">
              <div>
                <p className="font-medium text-gray-900">{item.productName}</p>
                <p className="text-sm text-gray-500">{item.quantity} x {formatPrice(item.unitPrice)}</p>
              </div>
              <p className="font-bold">{formatPrice(item.totalPrice)}</p>
            </div>
          ))}
        </div>
        <div className="border-t mt-4 pt-4 space-y-1">
          <div className="flex justify-between text-gray-600"><span>Ara Toplam</span><span>{formatPrice(order.subTotal)}</span></div>
          <div className="flex justify-between text-gray-600"><span>KDV (%18)</span><span>{formatPrice(order.taxAmount)}</span></div>
          <div className="flex justify-between text-gray-600"><span>Kargo</span><span>{formatPrice(order.shippingCost)}</span></div>
          <div className="flex justify-between text-lg font-bold text-primary-600 border-t pt-2">
            <span>Toplam</span><span>{formatPrice(order.totalAmount)}</span>
          </div>
        </div>
      </div>
    </div>
  );
}
```

### DashboardPage.tsx

**Dosya:** `frontend/src/pages/orders/DashboardPage.tsx`

```typescript
import { useOrders } from '../../hooks/useOrders';
import { formatPrice, getStatusText } from '../../lib/utils';
import { Package, Clock, CheckCircle, XCircle, TrendingUp } from 'lucide-react';

export default function DashboardPage() {
  const { orders, isLoading } = useOrders();

  const stats = {
    total: orders.length,
    pending: orders.filter((o) => o.status === 'Pending').length,
    delivered: orders.filter((o) => o.status === 'Delivered').length,
    cancelled: orders.filter((o) => o.status === 'Cancelled').length,
    totalSpent: orders.reduce((sum, o) => sum + o.totalAmount, 0),
  };

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Panelim</h1>
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <div className="card flex items-center gap-4">
          <div className="p-3 bg-blue-100 rounded-lg"><Package className="text-blue-600" size={24} /></div>
          <div><p className="text-sm text-gray-500">Toplam Sipariş</p><p className="text-2xl font-bold">{stats.total}</p></div>
        </div>
        <div className="card flex items-center gap-4">
          <div className="p-3 bg-yellow-100 rounded-lg"><Clock className="text-yellow-600" size={24} /></div>
          <div><p className="text-sm text-gray-500">Bekleyen</p><p className="text-2xl font-bold">{stats.pending}</p></div>
        </div>
        <div className="card flex items-center gap-4">
          <div className="p-3 bg-green-100 rounded-lg"><CheckCircle className="text-green-600" size={24} /></div>
          <div><p className="text-sm text-gray-500">Teslim Edilen</p><p className="text-2xl font-bold">{stats.delivered}</p></div>
        </div>
        <div className="card flex items-center gap-4">
          <div className="p-3 bg-purple-100 rounded-lg"><TrendingUp className="text-purple-600" size={24} /></div>
          <div><p className="text-sm text-gray-500">Toplam Harcama</p><p className="text-2xl font-bold">{formatPrice(stats.totalSpent)}</p></div>
        </div>
      </div>
      <div className="card">
        <h2 className="font-semibold text-lg mb-4">Son Siparişler</h2>
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead><tr className="border-b text-left"><th className="pb-3 font-medium text-gray-500">Sipariş No</th><th className="pb-3 font-medium text-gray-500">Durum</th><th className="pb-3 font-medium text-gray-500">Tutar</th><th className="pb-3 font-medium text-gray-500">Tarih</th></tr></thead>
            <tbody>
              {orders.slice(0, 10).map((o) => (
                <tr key={o.id} className="border-b last:border-0">
                  <td className="py-3 font-medium">{o.orderNumber}</td>
                  <td className="py-3">{getStatusText(o.status)}</td>
                  <td className="py-3">{formatPrice(o.totalAmount)}</td>
                  <td className="py-3 text-gray-500">{new Date(o.createdAt).toLocaleDateString('tr-TR')}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        {orders.length === 0 && <p className="text-gray-500 text-center py-4">Henüz siparişiniz bulunmuyor.</p>}
      </div>
    </div>
  );
}
```

---

## Task 30: Frontend Admin Page

**Amaç:** Admin yönetim sayfasını oluşturmak.

**Dosyalar:**
- `frontend/src/pages/admin/AdminPage.tsx`

### AdminPage.tsx

**Dosya:** `frontend/src/pages/admin/AdminPage.tsx`

```typescript
import { useState } from 'react';
import { useProducts } from '../../hooks/useProducts';
import { formatPrice } from '../../lib/utils';
import { Plus, Edit2, Trash2, X } from 'lucide-react';
import toast from 'react-hot-toast';

interface ProductForm {
  name: string; description: string; price: string; imageUrl: string; categoryId: string; sku: string;
}

const emptyForm: ProductForm = { name: '', description: '', price: '', imageUrl: '', categoryId: '', sku: '' };

export default function AdminPage() {
  const { products, categories, createProduct, updateProduct, deleteProduct } = useProducts();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [form, setForm] = useState<ProductForm>(emptyForm);

  const openCreate = () => { setEditingId(null); setForm(emptyForm); setIsModalOpen(true); };

  const openEdit = (id: string) => {
    const product = products.find((p) => p.id === id);
    if (!product) return;
    setEditingId(id);
    setForm({ name: product.name, description: product.description, price: product.price.toString(), imageUrl: product.imageUrl, categoryId: product.categoryId, sku: product.sku });
    setIsModalOpen(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const data = { ...form, price: parseFloat(form.price) };
    if (isNaN(data.price)) { toast.error('Geçerli bir fiyat girin.'); return; }

    let success: boolean;
    if (editingId) {
      success = await updateProduct(editingId, data);
    } else {
      success = await createProduct(data);
    }
    if (success) { setIsModalOpen(false); setForm(emptyForm); setEditingId(null); }
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm('Bu ürünü silmek istediğinize emin misiniz?')) return;
    await deleteProduct(id);
  };

  const updateField = (f: keyof ProductForm) => (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) =>
    setForm((p) => ({ ...p, [f]: e.target.value }));

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-gray-900">Ürün Yönetimi</h1>
        <button onClick={openCreate} className="btn-primary flex items-center gap-2"><Plus size={20} /> Yeni Ürün</button>
      </div>

      <div className="card overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead><tr className="border-b text-left bg-gray-50"><th className="p-3 font-medium text-gray-500">Ürün</th><th className="p-3 font-medium text-gray-500">SKU</th><th className="p-3 font-medium text-gray-500">Kategori</th><th className="p-3 font-medium text-gray-500">Fiyat</th><th className="p-3 font-medium text-gray-500">Durum</th><th className="p-3 font-medium text-gray-500">İşlemler</th></tr></thead>
            <tbody>
              {products.map((p) => (
                <tr key={p.id} className="border-b last:border-0 hover:bg-gray-50">
                  <td className="p-3"><p className="font-medium">{p.name}</p></td>
                  <td className="p-3 text-gray-500">{p.sku}</td>
                  <td className="p-3">{p.categoryName}</td>
                  <td className="p-3 font-medium">{formatPrice(p.price)}</td>
                  <td className="p-3"><span className={`px-2 py-1 rounded-full text-xs ${p.isPublished ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'}`}>{p.isPublished ? 'Yayında' : 'Taslak'}</span></td>
                  <td className="p-3">
                    <div className="flex gap-2">
                      <button onClick={() => openEdit(p.id)} className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg"><Edit2 size={16} /></button>
                      <button onClick={() => handleDelete(p.id)} className="p-2 text-red-600 hover:bg-red-50 rounded-lg"><Trash2 size={16} /></button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        {products.length === 0 && <p className="text-center py-8 text-gray-500">Henüz ürün eklenmemiş.</p>}
      </div>

      {isModalOpen && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4" onClick={() => setIsModalOpen(false)}>
          <div className="bg-white rounded-xl max-w-lg w-full max-h-[90vh] overflow-y-auto p-6" onClick={(e) => e.stopPropagation()}>
            <div className="flex justify-between items-center mb-6">
              <h2 className="text-xl font-bold">{editingId ? 'Ürün Düzenle' : 'Yeni Ürün'}</h2>
              <button onClick={() => setIsModalOpen(false)} className="p-1 hover:bg-gray-100 rounded"><X size={24} /></button>
            </div>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div><label className="block text-sm font-medium text-gray-700 mb-1">Ürün Adı</label><input required value={form.name} onChange={updateField('name')} className="input-field" /></div>
              <div><label className="block text-sm font-medium text-gray-700 mb-1">Açıklama</label><textarea required value={form.description} onChange={updateField('description')} className="input-field" rows={3} /></div>
              <div className="grid grid-cols-2 gap-4">
                <div><label className="block text-sm font-medium text-gray-700 mb-1">Fiyat (₺)</label><input required type="number" step="0.01" value={form.price} onChange={updateField('price')} className="input-field" /></div>
                <div><label className="block text-sm font-medium text-gray-700 mb-1">Kategori</label><select required value={form.categoryId} onChange={updateField('categoryId')} className="input-field"><option value="">Seçiniz</option>{categories.map((c) => (<option key={c.id} value={c.id}>{c.name}</option>))}</select></div>
              </div>
              <div><label className="block text-sm font-medium text-gray-700 mb-1">Görsel URL</label><input value={form.imageUrl} onChange={updateField('imageUrl')} className="input-field" placeholder="https://..." /></div>
              {!editingId && (<div><label className="block text-sm font-medium text-gray-700 mb-1">SKU</label><input required value={form.sku} onChange={updateField('sku')} className="input-field" /></div>)}
              <div className="flex gap-3 pt-2">
                <button type="submit" className="btn-primary flex-1">{editingId ? 'Güncelle' : 'Oluştur'}</button>
                <button type="button" onClick={() => setIsModalOpen(false)} className="btn-secondary flex-1">İptal</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
```

---

## Execution Notes

> **For agentic workers:** This plan generates a complete e-commerce platform codebase. When implementing:
> 1. Each task can be executed independently using subagent-driven-development
> 2. Backend tasks (0-22) should be implemented before frontend tasks (23-30)
> 3. After all tasks are complete, run `docker-compose up --build` from the `ecommerce-platform/` directory
> 4. Apply EF Core migrations for each service: `dotnet ef migrations add InitialCreate` in each API project
> 5. Access the application at `http://localhost:5000` (API Gateway) and `http://localhost:3000` (Frontend)
> 6. Default admin credentials can be seeded via database migrations
> 7. The frontend proxies `/api` calls to the API Gateway at port 5000
