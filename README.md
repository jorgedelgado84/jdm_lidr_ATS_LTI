# Applicant Tracking System (ATS)

A modern, production-ready Applicant Tracking System built with **Vue 3**, **.NET 10**, and **PostgreSQL**. Features responsive design, full internationalization (English/Spanish), and complete Docker containerization.

## 🚀 Quick Start

### With Docker (Recommended)
```bash
git clone <repository-url>
cd jdm_lidr_ATS_LTI
docker-compose up -d
```

Access the application at: **http://localhost:5173**

### Local Development
```bash
# Backend
cd backend && dotnet run

# Frontend (in new terminal)
cd frontend && npm install && npm run dev
```

Requires: Node.js 20+, .NET SDK 10+, PostgreSQL 16+

## 📋 Documentation

- **[ATS_README.md](ATS_README.md)** - Complete project documentation, architecture, API endpoints
- **[SETUP_GUIDE.md](SETUP_GUIDE.md)** - Detailed setup instructions for Docker and local development
- **[LICENSE](LICENSE)** - Project license

## ✨ Key Features

- **User Management** - Register as Candidate or Recruiter with JWT authentication
- **Position Management** - Create and manage job positions (Recruiter/Admin only)
- **Application Tracking** - Track application status: Submitted → Reviewing → Accepted/Rejected
- **Dashboard** - View statistics and recent application activity
- **Multilingual** - Full English/Spanish support with localStorage persistence
- **Responsive Design** - Mobile-first approach with Tailwind CSS
- **Docker Ready** - Complete containerization for frontend, backend, and database

## 🏗️ Architecture

```
Frontend (Vue 3 + Vite) ──► Backend (.NET 10 API) ──► PostgreSQL
  Port 5173                   Port 8080               Port 5432
```

## 📦 Tech Stack

| Layer | Technology |
|-------|-----------|
| **Frontend** | Vue 3, Vite, Pinia, Vue Router, Vue-i18n, Tailwind CSS, Axios |
| **Backend** | .NET 10, Entity Framework Core, JWT, BCrypt |
| **Database** | PostgreSQL 16 |
| **Infrastructure** | Docker, Docker Compose |

## 🔐 Security Features

- Passwords hashed with BCrypt (10 rounds)
- JWT token authentication (24-hour expiration)
- CORS configured for frontend origin
- SQL injection protection via Entity Framework
- XSS protection with Vue template escaping

## 📝 Default Credentials (Development)

- **Email**: user@example.com
- **Password**: Password123!

## 🛑 Stopping Services

```bash
# Stop all containers
docker-compose down

# Stop and remove volumes (delete database)
docker-compose down -v
```

## 🐛 Troubleshooting

**Port already in use?**
```bash
docker-compose down
docker-compose up -d
```

**Need database help?**
See [SETUP_GUIDE.md#database-setup](SETUP_GUIDE.md#database-setup)

**API not responding?**
```bash
docker-compose logs backend
```

## 📚 API Endpoints

- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `GET /api/positions` - List all positions
- `POST /api/positions` - Create position (Recruiter/Admin)
- `GET /api/applications/my-applications` - User's applications

See [ATS_README.md#api-endpoints](ATS_README.md#api-endpoints) for complete API documentation.

## 📱 Internationalization

Supports English and Spanish with automatic language selection. Change language in the navbar (top-right corner).

## 🚢 Deployment

For production deployment instructions, see [ATS_README.md#deployment](ATS_README.md#deployment).

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📞 Support

For detailed documentation and troubleshooting:
- See [SETUP_GUIDE.md](SETUP_GUIDE.md) for setup issues
- See [ATS_README.md](ATS_README.md) for feature documentation
- Check Docker logs: `docker-compose logs -f`

---

**Built with ❤️ | Version 1.0.0 | Last Updated: March 2026**