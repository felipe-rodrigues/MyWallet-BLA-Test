# MyWallet Solution


This repository contains a multi-project solution for a digital wallet application, built with .NET 7 and Angular 15+. It follows a clean, layered architecture for maintainability and separation of concerns. The solution includes:

- **MyWallet.Web**: A Web API for wallet entries (CRUD operations) and an Angular frontend.
- **Identity.MyWallet.Api**: An API for user registration and authentication (login).
- **Unit Test Projects**: For both APIs, ensuring code quality and reliability.

---

## Architecture Overview

The solution is structured using a layered architecture, with clear separation between domain logic, application services, and infrastructure concerns:

### 1. Domain Layer (`MyWallet.Domain`)
- **Purpose**: Contains core business entities, interfaces, and domain logic.
- **Contents**: Entities (e.g., `User`, `WalletEntry`), repository interfaces, and domain contracts.

### 2. Application Layer (`MyWallet.Application`)
- **Purpose**: Implements business use cases, application services, validation, and DTOs.
- **Contents**: Service interfaces and implementations, validators, request/response models, and exception handling.

### 3. Infrastructure Layer (`MyWallet.Infrastructure`)
- **Purpose**: Handles data access, external integrations, and persistence.
- **Contents**: Repository implementations, database connection factories, and data initialization logic.

This architecture ensures that business rules are isolated from infrastructure and presentation concerns, making the solution easier to test and maintain.

---

## Projects Overview

### 1. MyWallet.Web
- **Backend**: ASP.NET Core 7 Web API
- **Frontend**: Angular 15+
- **Purpose**: Manages wallet entries (CRUD)

### 2. Identity.MyWallet.Api
- **Backend**: ASP.NET Core 7 Web API
- **Purpose**: Handles user registration and authentication

### 3. Unit Test Projects
- **Identity.Api.Tests.Unit**: Unit tests for the Identity API
- **MyWallet.Tests.Unit**: Unit tests for the Wallet API

---

## Getting Started

### Prerequisites
- [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [Node.js (v16+ recommended)](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli)

---

## Running the Solution


### 1. Start the Identity API

```
dotnet run --project Identity.MyWallet.Api/Identity.MyWallet.Api.csproj
```

The Identity API will start (by default) on:
- HTTPS: `https://localhost:7051`
- HTTP: `http://localhost:5226`


### 2. Start the Wallet Web API

```
dotnet run --project MyWallet.Web/MyWallet.Web.csproj
```

The Wallet Web API will start (by default) on:
- HTTPS: `https://localhost:7153`
- HTTP: `http://localhost:5056`

### 3. Start the Angular Frontend (Development Mode)

In a separate terminal, navigate to the `ClientApp` folder and run:

```
cd MyWallet.Web/ClientApp
npm install
npm run start
```


This will start the Angular development server, usually on `https://localhost:44423/` (default for this template). 
You can change the frontend port or proxy settings in `ClientApp/proxy.conf.js` if needed.

> **Note:** For full integration (API proxy), ensure both the Wallet Web API and the Angular frontend are running.

---

## Default User Credentials

To access the application, use the default admin user:

- **Email**: `admin@wallet.com`
- **Password**: `Admin@123`

---

## Running Unit Tests

You can run unit tests for both APIs using the following commands:

```
dotnet test Identity.Api.Tests.Unit/Identity.Api.Tests.Unit.csproj

dotnet test MyWallet.Tests.Unit/MyWallet.Tests.Unit.csproj
```

---

## Additional Notes

- Configuration files (`appsettings.json`, `appsettings.Development.json`) are provided for both APIs.
- The solution uses SQLite for local development (see `Wallet.db`).
- Make sure both APIs are running for the application to function correctly.

---


## Project Structure

```
MyWallet.sln
|-- Identity.MyWallet.Api/           # Identity API (User/Auth)
|-- MyWallet.Web/                    # Wallet API + Angular frontend
|-- MyWallet.Domain/                 # Domain layer (entities, interfaces)
|-- MyWallet.Application/            # Application layer (services, DTOs, validation)
|-- MyWallet.Infrastructure/         # Infrastructure layer (data access, repositories)
|-- Identity.Api.Tests.Unit/         # Unit tests for Identity API
|-- MyWallet.Tests.Unit/             # Unit tests for Wallet API
```

---

## License

This project is for educational and demonstration purposes.
