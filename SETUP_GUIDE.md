# ATS Setup Guide

Complete step-by-step instructions for setting up and running the Applicant Tracking System in various environments.

## Table of Contents
1. [System Requirements](#system-requirements)
2. [Docker Setup (Recommended)](#docker-setup-recommended)
3. [Local Development Setup](#local-development-setup)
4. [Database Setup](#database-setup)
5. [Configuration](#configuration)
6. [Running the Application](#running-the-application)
7. [Testing](#testing)
8. [Troubleshooting](#troubleshooting)

## System Requirements

### Minimum Requirements
- **CPU**: 2 cores
- **RAM**: 4GB
- **Storage**: 10GB (for Docker images and database)

### Required Software

#### For Docker Setup (Recommended)
- **Docker**: v20.10+ ([install](https://docs.docker.com/get-docker/))
- **Docker Compose**: v1.29+ (included with Docker Desktop)
- **Git** (optional, for cloning)

#### For Local Development
- **Node.js**: v20+ ([install](https://nodejs.org/))
- **.NET SDK**: v10.0+ ([install](https://dotnet.microsoft.com/download))
- **PostgreSQL**: v16+ ([install](https://www.postgresql.org/download/))
- **Git** (for version control)

## Docker Setup (Recommended)

### Step 1: Prepare the Environment

```bash
# Clone the repository
git clone <repository-url>
cd jdm_lidr_ATS_LTI

# Verify Docker is running
docker --version
docker-compose --version

# Expected output:
# Docker version 20.10+
# Docker Compose version 1.29+
```

### Step 2: Start All Services

```bash
# Start all containers in background
docker-compose up -d

# Monitor startup progress
docker-compose logs -f

# Wait for "healthy" status (30-60 seconds)
# Look for lines mentioning "health check passed"
```

### Step 3: Verify Services are Running

```bash
# Check container status
docker-compose ps

# Expected output:
# NAME              STATUS
# ats_postgres      Up (healthy)
# ats_backend       Up (healthy)
# ats_frontend      Up (healthy)

# Test connectivity
curl http://localhost:8080/health     # Backend health check
curl http://localhost:5173            # Frontend
```

### Step 4: Access the Application

Open your browser and navigate to:
```
http://localhost:5173
```

Default credentials:
- **Email**: user@example.com
- **Password**: Password123!

### Step 5: Stop Services

```bash
# Stop all services
docker-compose down

# Stop and remove volumes (delete database)
docker-compose down -v
```

## Local Development Setup

### Prerequisites Checklist
```bash
# Verify Node.js
node --version    # Should be v20+
npm --version     # Should be v10+

# Verify .NET
dotnet --version  # Should be v10+

# Verify PostgreSQL
psql --version    # Should be v16+
```

### Frontend Local Setup

```bash
# Navigate to frontend directory
cd frontend

# Copy environment file
cp .env.example .env.local

# Install dependencies (this may take 2-3 minutes)
npm install

# Start development server
npm run dev

# Output should show:
# VITE v5.0.0  ready in XXX ms
# 
# ➜  Local:   http://localhost:5173/
```

**Troubleshooting Frontend:**
```bash
# Clean install if issues occur
rm -rf node_modules package-lock.json
npm install

# Check Node version
node --version  # Must be v20+

# Clear npm cache
npm cache clean --force
```

### Backend Local Setup

```bash
# Navigate to backend directory
cd backend

# Copy environment file (optional)
cp .env.example .env

# Restore dependencies
dotnet restore

# Apply database migrations
dotnet ef database update

# Run in development mode
dotnet run

# Output should show:
# info: Microsoft.Hosting.Lifetime[14]
# Now listening on: http://localhost:5000 or http://localhost:5001 (HTTPS)
```

**Troubleshooting Backend:**
```bash
# Check .NET version
dotnet --version  # Must be v10+

# Clear NuGet cache
dotnet nuget locals all --clear

# Check database connection
dotnet ef database validate

# View migration files
dotnet ef migrations list
```

## Database Setup

### Option 1: Docker (Easiest)

```bash
# Start just the PostgreSQL container
docker run -d \
  --name ats_postgres \
  -e POSTGRES_DB=ats_db \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  postgres:16-alpine

# Verify connection
docker logs ats_postgres
```

### Option 2: Local PostgreSQL Installation

#### On Windows:
```powershell
# Check if PostgreSQL is installed and running
Get-Service postgresql-x64-16  # Replace 16 with your version

# Connect to PostgreSQL
psql -U postgres -h localhost

# Create database
CREATE DATABASE ats_db;

# Verify
\l  # List databases
\q  # Quit
```

#### On macOS (Homebrew):
```bash
# Install PostgreSQL
brew install postgresql@16

# Start PostgreSQL service
brew services start postgresql@16

# Create database
createdb -U postgres ats_db

# Verify
psql -U postgres -l
```

#### On Linux (Ubuntu/Debian):
```bash
# Install PostgreSQL
sudo apt-get update
sudo apt-get install postgresql postgresql-contrib

# Create database
sudo -u postgres createdb ats_db

# Verify
sudo -u postgres psql -l
```

### Database Connection Test

```bash
# Test connection string
# For local: postgresql://postgres:postgres@localhost:5432/ats_db
# For Docker: postgresql://postgres:postgres@postgres:5432/ats_db

# Using psql
psql postgresql://postgres:postgres@localhost:5432/ats_db

# Using .NET CLI
cd backend
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=ats_db;Username=postgres;Password=postgres"
```

## Configuration

### Frontend Configuration

**File**: `frontend/.env.local`

```env
# Backend API URL
VITE_API_URL=http://localhost:8080/api

# For production Docker deployment:
VITE_API_URL=http://backend:8080/api
```

### Backend Configuration

**File**: `backend/appsettings.json` or `backend/.env`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ats_db;Username=postgres;Password=postgres"
  },
  "JwtSettings": {
    "Secret": "your-256-bit-secret-key-minimum-32-characters-long",
    "Issuer": "ats-api",
    "Audience": "ats-frontend",
    "ExpirationMinutes": 1440
  }
}
```

**Environment-specific files:**
```
appsettings.json              # Base configuration
appsettings.Development.json  # Development overrides
appsettings.Production.json   # Production overrides
```

### Docker Configuration

**File**: `docker-compose.yml`

Check the following settings:
- PostgreSQL password: `POSTGRES_PASSWORD=postgres`
- Database name: `POSTGRES_DB=ats_db`
- Backend port: `8080`
- Frontend port: `5173`
- Database port: `5432`

## Running the Application

### Complete Docker Stack

```bash
# Terminal 1: Start all services
docker-compose up

# Wait for all services to report "healthy"
# Services start in this order: postgres → backend → frontend

# Terminal 2: Monitor specific service logs
docker-compose logs -f backend

# Access application
# Frontend: http://localhost:5173
# Backend API: http://localhost:8080
# Database: localhost:5432
```

### Individual Services (Local Development)

```bash
# Terminal 1: Start PostgreSQL (if using Docker)
docker run -d --name ats_postgres \
  -e POSTGRES_DB=ats_db \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  postgres:16-alpine

# Terminal 2: Start Backend
cd backend
dotnet run

# Terminal 3: Start Frontend
cd frontend
npm run dev
```

### Verify All Services

```bash
# Check backend health
curl -i http://localhost:8080/health

# Check frontend accessibility
curl -i http://localhost:5173

# Check database connection
psql -U postgres -d ats_db -c "SELECT VERSION();"
```

## Database Migrations

### View Migration Status

```bash
cd backend

# List all migrations
dotnet ef migrations list

# Check pending migrations
dotnet ef migrations list | grep "Pending"
```

### Apply Migrations

```bash
cd backend

# Apply pending migrations to local database
dotnet ef database update

# Apply to specific migration
dotnet ef database update 0  # Revert all
dotnet ef database update InitialCreate

# View migration scripts without applying
dotnet ef migrations script
```

### Create New Migration

```bash
cd backend

# After modifying Models.cs
dotnet ef migrations add YourMigrationName

# Review generated migration file
# Then apply:
dotnet ef database update
```

## Testing

### Frontend Testing

```bash
cd frontend

# Run unit tests
npm run test

# Run with coverage report
npm run test:coverage

# Run specific test file
npm run test -- src/stores/authStore.test.js

# Watch mode (re-run on file changes)
npm run test -- --watch
```

### Backend Testing

```bash
cd backend

# Run all tests
dotnet test

# Run specific test project
dotnet test ATS.Tests

# Run with verbose output
dotnet test --verbosity detailed

# Generate coverage report
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

## Manual Testing

### User Registration

```bash
# Using curl
curl -X POST http://localhost:8080/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "John Doe",
    "email": "john@example.com",
    "password": "Password123!",
    "role": "Candidate"
  }'

# Expected response:
# {"token":"eyJhbGc...","user":{"id":1,"email":"john@example.com"}}
```

### User Login

```bash
curl -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "password": "Password123!"
  }'
```

### Get Profile (Requires Auth)

```bash
# Replace TOKEN with actual JWT from login response
curl -X GET http://localhost:8080/api/auth/profile \
  -H "Authorization: Bearer TOKEN"
```

## Troubleshooting

### Common Issues and Solutions

#### Docker Containers Won't Start

**Problem**: `docker-compose up` shows errors

**Solution**:
```bash
# Clean slate
docker-compose down -v
docker system prune -a

# Rebuild images
docker-compose build --no-cache

# Start again
docker-compose up -d

# Check logs
docker-compose logs -f
```

#### Port Already in Use

**Problem**: `Error: Address already in use :::5173`

**Solution**:
```bash
# Find process using port (Windows)
netstat -ano | findstr :5173

# Find process using port (Mac/Linux)
lsof -i :5173

# Kill process and retry
docker-compose restart frontend
```

#### Database Connection Failed

**Problem**: `ERROR: relation "Users" does not exist`

**Solution**:
```bash
# Verify PostgreSQL is running
docker-compose ps postgres

# Re-apply migrations
cd backend
dotnet ef database drop  # WARNING: Deletes all data
dotnet ef database update

# Verify tables created
psql -U postgres -d ats_db
\dt  # List tables
```

#### Frontend Blank Screen

**Problem**: Frontend loads but shows nothing

**Solution**:
```bash
# Check browser console (F12) for errors
# Check if backend API is accessible
curl http://localhost:8080/api/positions

# Clear cache and rebuild
cd frontend
rm -rf node_modules dist
npm install
npm run build
docker-compose restart frontend
```

#### JWT Token Expired

**Problem**: "Unauthorized" after some time

**Solution**:
- Default expiration is 24 hours
- Clear browser localStorage and login again
- Or increase `JwtSettings__ExpirationMinutes` in backend

#### Search Not Working

**Problem**: Search endpoint returns 404

**Solution**:
```bash
# Verify endpoint path
curl http://localhost:8080/api/positions/search/keyword

# Check backend logs
docker-compose logs backend | grep -i search
```

### Debug Mode

#### Backend Debug

```bash
# Set log level to Debug
export ASPNETCORE_ENVIRONMENT=Development

# Run with debug logging
dotnet run --verbose

# Expected output shows detailed SQL queries and middleware traces
```

#### Frontend Debug

```bash
# Start with verbose logging
npm run dev -- --debug

# Open browser DevTools (F12)
# Check Network, Console, and Application tabs
```

#### Database Debug

```bash
# Enable query logging in PostgreSQL
psql -U postgres -d ats_db

# Then in backend, set Entity Framework logging level
ASPNETCORE_ENVIRONMENT=Development
```

## Performance Verification

### Frontend Performance

```bash
# Production build
cd frontend
npm run build

# Check bundle size
ls -lh dist/

# Should be < 500KB for main bundle
```

### Backend Performance

```bash
# Database query performance
cd backend

# Enable query logging
# In appsettings.Development.json:
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore.Database.Command": "Debug"
    }
  }
}
```

## Production Deployment Checklist

- [ ] Change all default passwords
- [ ] Set `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Generate strong JWT secret (32+ characters)
- [ ] Configure HTTPS/SSL certificates
- [ ] Set up database backups
- [ ] Configure logging and monitoring
- [ ] Test disaster recovery procedures
- [ ] Set up CI/CD pipeline
- [ ] Configure firewall rules
- [ ] Enable automated updates

## Next Steps

1. **Complete User Registration**: Test creating accounts with different roles
2. **Create Job Positions**: Post positions as a Recruiter
3. **Submit Applications**: Apply to positions as a Candidate
4. **Track Applications**: View dashboard and application status
5. **Test Multilingual**: Switch between English/Spanish

## Support and Documentation

- **Main README**: See [ATS_README.md](ATS_README.md)
- **API Documentation**: http://localhost:8080/swagger (if enabled)
- **Frontend Source**: `frontend/src/`
- **Backend Source**: `backend/`
- **Issues**: Open an issue on GitHub

---

**Last Updated**: 2024
**Version**: 1.0.0
