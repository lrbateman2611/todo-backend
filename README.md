# Todo Backend API

A .NET 10 REST API for managing todo items, deployed to Azure Container Apps.

## Features

- ✅ RESTful API for todo management
- ✅ JWT authentication with Auth0
- ✅ Supabase database integration
- ✅ Bitwarden secrets management
- ✅ Docker containerization
- ✅ Azure Container Apps deployment
- ✅ CI/CD with GitHub Actions

## API Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/` | API status | No |
| GET | `/health` | Health check | No |
| GET | `/api/todo` | List all todos | Yes |
| POST | `/api/todo` | Create todo | Yes |
| PUT | `/api/todo/{id}` | Update todo | Yes |
| DELETE | `/api/todo/{id}` | Delete todo | Yes |

## Local Development

### Prerequisites

- .NET 10 SDK
- Docker Desktop
- Visual Studio 2026 or VS Code

### Setup

1. Clone the repository:
```bash
git clone https://github.com/lrbateman2611/todo-backend.git
cd todo-backend
```

2. Set up user secrets:
```bash
cd Todo.Api
dotnet user-secrets set "Bitwarden:Token" "your-bitwarden-token"
```

3. Run the application:
```bash
dotnet run
```

Or use Visual Studio and press F5.

## Deployment

### Azure Container Apps

The application is automatically deployed to Azure Container Apps when code is pushed to the `master` branch.

**Production URL:** https://todos-api.{your-container-app-domain}.azurecontainerapps.io

### CI/CD Setup

See [.github/DEPLOYMENT.md](.github/DEPLOYMENT.md) for detailed setup instructions.

Quick setup:
```powershell
# Run the setup script
.\.github\setup-azure-credentials.ps1
```

Then add the `AZURE_CREDENTIALS` secret to GitHub repository settings.

## Configuration

### Environment Variables

| Variable | Description | Required |
|----------|-------------|----------|
| `Bitwarden__Token` | Bitwarden access token | Yes |
| `ASPNETCORE_ENVIRONMENT` | Environment (Development/Production) | No |

### Azure Resources Required

- Azure Container Registry (ACR)
- Azure Container Apps
- Azure Container Apps Environment

## Docker

### Build locally
```bash
docker build -t todos-backend:latest -f Todo.Api/Dockerfile .
```

### Run locally
```bash
docker run -p 8080:8080 \
  -e Bitwarden__Token="your-token" \
  todos-backend:latest
```

## Technology Stack

- **Framework:** .NET 10
- **Authentication:** Auth0 with JWT
- **Database:** Supabase (PostgreSQL)
- **Secrets:** Bitwarden Secrets Manager
- **Hosting:** Azure Container Apps
- **CI/CD:** GitHub Actions
- **API Documentation:** Scalar (OpenAPI)

## Project Structure

```
todos-backend/
├── .github/
│   ├── workflows/
│   │   └── deploy-azure.yml      # CI/CD workflow
│   ├── DEPLOYMENT.md              # Deployment guide
│   └── setup-azure-credentials.ps1
├── Todo.Api/
│   ├── Configurations/            # Service configurations
│   ├── Controllers/               # API controllers
│   ├── Data/                      # Database entities & data access
│   ├── DTOs/                      # Data transfer objects
│   ├── Models/                    # Domain models
│   ├── Services/                  # Business logic
│   ├── Dockerfile                 # Container definition
│   └── Program.cs                 # Application entry point
└── README.md
```

## Contributing

1. Create a feature branch
2. Make your changes
3. Test locally
4. Create a pull request to `master`
5. Once merged, GitHub Actions will deploy automatically

## License

This project is private.

## Support

For issues or questions, please open an issue on GitHub.
