# ShumenTraffic - Development Setup Guide

## Prerequisites

### Required Software
- **.NET SDK 6.0 or higher** - [Download](https://dotnet.microsoft.com/download)
- **Node.js 16.0 or higher** - [Download](https://nodejs.org/)
- **MSSQL Server** - Local instance (already configured)
- **Git** - [Download](https://git-scm.com/)
- **Visual Studio Code** or **Visual Studio 2022** (recommended)

### Recommended Tools
- **SQL Server Management Studio (SSMS)** - For database management
- **Postman** or **Thunder Client** - For API testing
- **Git Extensions** or **GitHub Desktop** - For version control

## Project Structure

```
ShumenTraffic/
├── Client/                         # Next.js Frontend
│   ├── src/
│   │   ├── app/                    # App router pages
│   │   ├── components/             # React components
│   │   └── lib/                    # Utilities and helpers
│   ├── public/                     # Static assets
│   ├── package.json
│   ├── tsconfig.json
│   ├── tailwind.config.ts
│   └── next.config.ts
│
├── Server/                         # ASP.NET Core Backend
│   ├── ShumenTraffic.WebAPI/       # Web API project
│   │   ├── Controllers/            # API endpoints
│   │   ├── Models/                 # Request/Response models
│   │   ├── Services/               # Business logic
│   │   ├── Program.cs              # Startup configuration
│   │   └── appsettings.json        # Configuration
│   │
│   ├── ShumenTraffic.Data/         # Data access layer
│   │   ├── Models/                 # Entity models
│   │   ├── Context/                # DbContext
│   │   └── Migrations/             # Database migrations
│   │
│   └── ShumenTraffic.sln           # Solution file
│
├── Docs/                           # Documentation
│   ├── PROJECT_PLAN.md
│   ├── CLARIFICATIONS.md
│   └── DEVELOPMENT_SETUP.md
│
└── README.md
```

## Backend Setup

### 1. Navigate to Server Directory
```bash
cd Server
```

### 2. Restore NuGet Packages
```bash
dotnet restore
```

### 3. Build the Solution
```bash
dotnet build
```

### 4. Configure Database Connection

Edit `Server/ShumenTraffic.WebAPI/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ShumenTraffic;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

**Note**: Adjust the connection string based on your MSSQL setup:
- For Windows Authentication: `Server=localhost;Database=ShumenTraffic;Trusted_Connection=true;`
- For SQL Authentication: `Server=localhost;User Id=sa;Password=YourPassword;Database=ShumenTraffic;`

### 5. Create Database and Run Migrations

```bash
cd ShumenTraffic.WebAPI
dotnet ef database update
```

### 6. Run the Backend

```bash
dotnet run
```

The API will be available at `https://localhost:7000` (or the port shown in console)

## Frontend Setup

### 1. Navigate to Client Directory
```bash
cd Client
```

### 2. Install Dependencies
```bash
npm install
```

### 3. Configure Environment Variables

Create `Client/.env.local`:

```
NEXT_PUBLIC_API_URL=https://localhost:7000
```

### 4. Run the Frontend

```bash
npm run dev
```

The application will be available at `http://localhost:3000`

## Running Both Services

### Option 1: Separate Terminals

**Terminal 1 - Backend:**
```bash
cd Server/ShumenTraffic.WebAPI
dotnet run
```

**Terminal 2 - Frontend:**
```bash
cd Client
npm run dev
```

### Option 2: Using VS Code Tasks

Create `.vscode/tasks.json` in the root directory:

```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "Run Backend",
      "type": "shell",
      "command": "dotnet",
      "args": ["run"],
      "cwd": "${workspaceFolder}/Server/ShumenTraffic.WebAPI",
      "isBackground": true,
      "problemMatcher": "$msCompile"
    },
    {
      "label": "Run Frontend",
      "type": "shell",
      "command": "npm",
      "args": ["run", "dev"],
      "cwd": "${workspaceFolder}/Client",
      "isBackground": true,
      "problemMatcher": "$tsc"
    },
    {
      "label": "Run All",
      "dependsOn": ["Run Backend", "Run Frontend"],
      "problemMatcher": []
    }
  ]
}
```

Then run: `Ctrl+Shift+B` and select "Run All"

## Database Management

### Create a New Migration

```bash
cd Server/ShumenTraffic.WebAPI
dotnet ef migrations add MigrationName
```

### Update Database

```bash
dotnet ef database update
```

### Drop Database (Development Only)

```bash
dotnet ef database drop
```

## Testing the API

### Using Postman

1. Import the API endpoints
2. Set base URL to `https://localhost:7000`
3. Test endpoints as documented

### Using curl

```bash
# Example: Get all bus stops
curl -X GET https://localhost:7000/api/stops

# Example: Create a new bus stop
curl -X POST https://localhost:7000/api/stops \
  -H "Content-Type: application/json" \
  -d '{"name":"Stop Name","latitude":43.2,"longitude":25.4}'
```

## Troubleshooting

### Port Already in Use

If port 7000 or 3000 is already in use:

**Backend**: Edit `Server/ShumenTraffic.WebAPI/Properties/launchSettings.json`

**Frontend**: Run with different port:
```bash
npm run dev -- -p 3001
```

### Database Connection Issues

1. Verify MSSQL is running
2. Check connection string in `appsettings.json`
3. Ensure database exists or run migrations
4. Check firewall settings

### Node Modules Issues

```bash
# Clear cache and reinstall
cd Client
rm -r node_modules package-lock.json
npm install
```

### .NET Build Issues

```bash
# Clean and rebuild
cd Server
dotnet clean
dotnet restore
dotnet build
```

## Code Style & Standards

### Frontend (Next.js/React)
- Use TypeScript for type safety
- Follow ESLint configuration
- Use TailwindCSS for styling
- Component naming: PascalCase
- File naming: kebab-case for components

### Backend (ASP.NET Core)
- Follow C# naming conventions
- Use async/await for I/O operations
- Implement dependency injection
- Use Entity Framework Core for data access
- Add XML documentation comments

## Git Workflow

### Clone Repository
```bash
git clone https://github.com/yourusername/ShumenTraffic.git
cd ShumenTraffic
```

### Create Feature Branch
```bash
git checkout -b feature/feature-name
```

### Commit Changes
```bash
git add .
git commit -m "Description of changes"
```

### Push to Remote
```bash
git push origin feature/feature-name
```

### Create Pull Request
- Go to GitHub and create a PR
- Request review
- Merge after approval

## Useful Commands

### Backend
```bash
# Run tests
dotnet test

# Run with watch mode
dotnet watch run

# Publish for production
dotnet publish -c Release
```

### Frontend
```bash
# Build for production
npm run build

# Start production build
npm start

# Run linter
npm run lint

# Format code
npm run format
```

## Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Next.js Documentation](https://nextjs.org/docs)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [TailwindCSS Documentation](https://tailwindcss.com/docs)
- [Leaflet.js Documentation](https://leafletjs.com/)

## Support

For issues or questions:
1. Check the documentation
2. Review existing GitHub issues
3. Create a new issue with detailed description

