# Applicant Tracking System (ATS)

A modern, full-stack Applicant Tracking System built with Vue 3, .NET 10, and PostgreSQL. Designed with responsive UI, internationalization (Spanish/English), and Docker containerization for production-ready deployment.

## Features

- **User Management**: Register as Candidate or Recruiter with secure JWT authentication
- **Position Management**: Create, update, and manage job positions (Recruiter/Admin only)
- **Application Tracking**: Submit applications and track status (Submitted → Reviewing → Accepted/Rejected)
- **Dashboard**: View statistics and recent application activity
- **Multilingual**: Full support for English and Spanish with localStorage persistence
- **Responsive Design**: Mobile-first approach using Tailwind CSS
- **Docker Ready**: Complete containerization for frontend, backend, and database

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    ATS System Architecture                   │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  Frontend (Port 5173)          Backend (Port 8080)           │
│  ┌──────────────────┐          ┌──────────────────┐          │
│  │  Vue 3 + Vite    │◄────────►│  .NET 10 API     │          │
│  │  - Router        │   HTTP   │  - Controllers   │          │
│  │  - Pinia Store   │  (JWT)   │  - Services      │          │
│  │  - Vue-i18n      │          │  - Repositories  │          │
│  │  - Tailwind CSS  │          │                  │          │
│  └──────────────────┘          └────────┬─────────┘          │
│                                         │                    │
│                            ┌────────────▼──────────────┐     │
│                            │   PostgreSQL (Port 5432)  │     │
│                            │   - Users                 │     │
│                            │   - Positions             │     │
│                            │   - Candidates            │     │
│                            │   - Applications          │     │
│                            └───────────────────────────┘     │
│                                                               │
└─────────────────────────────────────────────────────────────┘
```

## Tech Stack

### Frontend
- **Vue 3**: Progressive JavaScript framework with Composition API
- **Vite**: Lightning-fast build tool with HMR
- **Vue Router**: Client-side routing with auth guards
- **Pinia**: Lightweight state management
- **Vue-i18n**: Internationalization with locale switching
- **Tailwind CSS**: Utility-first CSS framework
- **Axios**: Promise-based HTTP client

### Backend
- **.NET 10**: Modern C# framework with async/await
- **Entity Framework Core**: ORM with migrations
- **PostgreSQL**: Reliable relational database
- **JWT**: Secure token-based authentication
- **BCrypt**: Password hashing with salt

### Infrastructure
- **Docker**: Container platform for all services
- **Docker Compose**: Multi-container orchestration
- **PostgreSQL Alpine**: Lightweight database image

## Quick Start

### Prerequisites
- Docker and Docker Compose (v1.29+)
- Git

### Running with Docker Compose

```bash
# Clone the repository
git clone <repository-url>
cd jdm_lidr_ATS_LTI

# Start all services
docker-compose up -d

# Wait for services to be healthy (30-60 seconds)
# Access the application:
# - Frontend: http://localhost:5173
# - Backend API: http://localhost:8080
# - Database: localhost:5432
```

### Stopping Services

```bash
docker-compose down
```

### Viewing Logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f backend
docker-compose logs -f frontend
docker-compose logs -f postgres
```

## Local Development Setup

### Frontend

```bash
cd frontend

# Install dependencies
npm install

# Create .env.local
cp .env.example .env.local

# Edit .env.local with your backend API URL
VITE_API_URL=http://localhost:8080/api

# Start development server
npm run dev

# Access at http://localhost:5173
```

### Backend

```bash
cd backend

# Create .env file (optional, uses defaults)
cp .env.example .env

# Restore dependencies
dotnet restore

# Apply database migrations
dotnet ef database update

# Run development server
dotnet run

# API available at http://localhost:8080
```

### Database

PostgreSQL must be running. Options:

**Option 1: Docker**
```bash
docker run -d \
  --name ats_postgres \
  -e POSTGRES_DB=ats_db \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  postgres:16-alpine
```

**Option 2: Local Installation**
Create a database named `ats_db` with user `postgres` and password `postgres`.

## Authentication Flow

```
┌─────────┐                           ┌────────┐
│ Client  │                           │ Server │
└────┬────┘                           └────┬───┘
     │                                     │
     │────── POST /register ──────────────►│
     │       (email, password, role)      │
     │                                     │ Hash password + Create user
     │◄────── 200 OK / 400 Error ────────│
     │                                     │
     │────── POST /login ────────────────►│
     │       (email, password)             │
     │                                     │ Verify password
     │◄────── { token: "jwt..." } ───────│
     │                                     │
     │ Store token in localStorage        │
     │                                     │
     │────── GET /profile ───────────────►│
     │       Authorization: Bearer token   │
     │                                     │ Validate JWT
     │◄────── { user: {...} } ───────────│
```

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login and get JWT token
- `GET /api/auth/profile` - Get current user profile (auth required)

### Positions
- `GET /api/positions` - List all positions
- `GET /api/positions/open` - List open positions
- `GET /api/positions/{id}` - Get position details
- `GET /api/positions/search/{query}` - Search positions
- `POST /api/positions` - Create position (Recruiter/Admin only)
- `PUT /api/positions/{id}` - Update position (Recruiter/Admin only)
- `PUT /api/positions/{id}/close` - Close position (Recruiter/Admin only)

### Applications
- `GET /api/applications/my-applications` - Get user's applications (auth required)
- `POST /api/applications` - Submit application (auth required)

## Database Schema

### Users
```sql
CREATE TABLE public."Users" (
  "Id" integer PRIMARY KEY,
  "Email" text NOT NULL UNIQUE,
  "Password" text NOT NULL,
  "FullName" text,
  "Role" text NOT NULL,
  "CreatedAt" timestamp,
  "LastLoginAt" timestamp
);
```

### Positions
```sql
CREATE TABLE public."Positions" (
  "Id" integer PRIMARY KEY,
  "Title" text NOT NULL,
  "Department" text,
  "Location" text,
  "Description" text,
  "SalaryMin" decimal,
  "SalaryMax" decimal,
  "Status" text NOT NULL (indexed),
  "CreatedById" integer REFERENCES "Users"("Id"),
  "CreatedAt" timestamp,
  "ClosedAt" timestamp
);
```

### Candidates
```sql
CREATE TABLE public."Candidates" (
  "Id" integer PRIMARY KEY,
  "UserId" integer UNIQUE REFERENCES "Users"("Id") ON DELETE CASCADE,
  "Experience" text,
  "Skills" text,
  "CreatedAt" timestamp
);
```

### Applications
```sql
CREATE TABLE public."Applications" (
  "Id" integer PRIMARY KEY,
  "CandidateId" integer REFERENCES "Candidates"("Id") ON DELETE CASCADE,
  "PositionId" integer REFERENCES "Positions"("Id") ON DELETE CASCADE,
  "Status" text,
  "ReviewNotes" text,
  "AppliedAt" timestamp,
  "UpdatedAt" timestamp,
  UNIQUE (CandidateId, PositionId)
);
```

## Environment Variables

### Frontend (.env.local)
```env
VITE_API_URL=http://localhost:8080/api
```

### Backend (.env)
```env
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=ats_db;Username=postgres;Password=postgres
JwtSettings__Secret=your-super-secret-key-change-this-in-production-at-least-32-characters-long
JwtSettings__Issuer=ats-api
JwtSettings__Audience=ats-frontend
JwtSettings__ExpirationMinutes=1440
```

## Internationalization (i18n)

The application supports English and Spanish with automatic locale detection and persistence.

### Adding New Translations

1. Edit `frontend/src/locales/en.json`:
```json
{
  "common": {
    "appName": "Applicant Tracking System",
    "newKey": "Your new text here"
  }
}
```

2. Edit `frontend/src/locales/es.json` with Spanish translations:
```json
{
  "common": {
    "appName": "Sistema de Seguimiento de Candidatos",
    "newKey": "Tu nuevo texto aquí"
  }
}
```

3. Use in components:
```vue
<h1>{{ $t('common.appName') }}</h1>
```

## Security Considerations

- **Passwords**: Hashed with BCrypt (10 rounds)
- **JWT Tokens**: 24-hour expiration, signed with secret key
- **CORS**: Configured to allow only frontend origin
- **SQL Injection**: Protected by Entity Framework parameterization
- **XSS**: Vue templates auto-escape by default
- **CSRF**: JWT tokens are secure against CSRF

### Production Security Checklist

- [ ] Change JWT secret to 32+ character random string
- [ ] Set `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Configure proper PostgreSQL username/password
- [ ] Enable HTTPS for API and frontend
- [ ] Set secure CORS origins (not localhost)
- [ ] Enable database backups
- [ ] Configure log aggregation
- [ ] Set up monitoring and alerting

## Troubleshooting

### Port Already in Use
```bash
# Find process using port
netstat -ano | findstr :5173  # Windows
lsof -i :5173                  # Mac/Linux

# Kill process and restart
docker-compose down
docker-compose up -d
```

### Database Connection Error
```bash
# Verify PostgreSQL is running
docker-compose logs postgres

# Check connection string in backend/.env
Host=postgres  # Docker hostname
Username=postgres
Password=postgres
Database=ats_db
```

### Frontend Won't Load
```bash
# Clear frontend build cache
rm -rf frontend/node_modules frontend/dist
cd frontend && npm install && npm run build

# Restart services
docker-compose restart frontend
```

### JWT Token Expired
- Tokens expire after 24 hours
- Clear localStorage and login again
- Or increase `JwtSettings__ExpirationMinutes` in backend

## Performance Optimization

- **Frontend**: Vite code splitting, lazy-loaded routes, image optimization
- **Backend**: Indexed database queries, async/await patterns, connection pooling
- **Database**: Composite indexes on frequently queried columns
- **Docker**: Multi-stage builds for reduced image size

## Contributing

1. Create feature branch: `git checkout -b feature/name`
2. Commit changes: `git commit -am 'Add feature'`
3. Push to branch: `git push origin feature/name`
4. Submit pull request

## Testing

### Frontend
```bash
cd frontend
npm run test
npm run coverage
```

### Backend
```bash
cd backend
dotnet test
```

## Deployment

### Docker Deployment Examples

**Azure Container Instances:**
```bash
az container create \
  --resource-group myResourceGroup \
  --name ats-app \
  --image myregistry.azurecr.io/ats:latest \
  --ports 80
```

**AWS ECS:**
```bash
aws ecs create-service \
  --cluster ats-cluster \
  --service-name ats-service \
  --task-definition ats-task:1
```

## License

See [LICENSE](LICENSE) file for details.

## Support

For issues, questions, or contributions, please open an issue on GitHub or contact the development team.
