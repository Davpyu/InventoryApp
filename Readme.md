# API DOCS : 
- https://documenter.getpostman.com/view/4473147/TzJoDKvC

# Pre Requirement :
-   .Net sdk 8.0
-   .Net Entity Framework
-   MSSQL 2019
-   NATs
-   Redis

# Manual Quick Start API:
-   Run DB MSSQL 2019 : (by docker : `docker run --name mssqldock -e "ACCEPT_EULA=Y" -v sqldata:./mssqldata -e "MSSQL_SA_PASSWORD=Administrat@r123" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-latest`)
-   Run Redis (by docker : `docker run --name redisdock -p6379:6379 -d redis`)
-   Run NATs (by docker : `docker run --name natsdock -p 4222:4222 -p 6222:6222 -p 8222:8222 -d nats`)
-   Install .Net Entity Framework : https://learn.microsoft.com/en-us/ef/core/get-started/overview/install
-   Copy file `appsettings.example.json` ubah ke `appsettings.json` kemudian setting konfigurasinya
-   Run `dotnet ef database update` untuk migrate database
-   Run `dotnet run`

# Directory structure :
```
├── Constants
│   ├── Cache
│   ├── CircuitBreaker
│   ├── Event
│   ├── Logger
│   └── Storage
├── Domain
│   ├── Auth
│   │   ├── Data
│   │   └── Services
│   ├── Logging
│   │   ├── Listeners
│   │   └── Services
│   ├── Permission
│   │   ├── Repositories
│   │   └── Services
│   ├── Role
│   │   ├── Repositories
│   │   └── Services
│   ├── RolePermission
│   │   ├── Repositories
│   │   └── Services
│   ├── User
│   │   ├── Repositories
│   │   └── Services
│   └── UserRole
│       ├── Repositories
│       └── Services
├── Http
│   └── API
│       └── Version1
│           ├── Controllers
│           │   ├── Auth
│           │   └── IAM
│           ├── Requests
│           │   ├── Auth
│           │   ├── Permission
│           │   ├── Role
│           │   ├── RolePermission
│           │   ├── User
│           │   └── UserRole
│           └── Responses
│               ├── Auth
│               ├── Permission
│               ├── Role
│               ├── RolePermission
│               ├── User
│               └── UserRole
├── Immutables
├── Infrastructure
│   ├── BackgroundHosted
│   ├── Databases
│   ├── Events
│   ├── Exceptions
│   ├── Filters
│   ├── Helpers
│   ├── Integrations
│   │   ├── Http
│   │   └── NATs
│   ├── Middlewares
│   ├── Queues
│   ├── Shareds
│   └── Subsribtions
├── Log
├── Migrations
├── Models
├── Properties
├── Types
└── storage
```