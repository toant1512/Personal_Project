# MediaArchive

[![CI](https://github.com/toant1512/Personal_Project/actions/workflows/ci.yml/badge.svg)](https://github.com/toant1512/Personal_Project/actions/workflows/ci.yml)

MediaArchive is a backend application built with ASP.NET Core 9 following Clean Architecture principles.

The application allows authenticated users to archive media from YouTube by storing metadata, managing download jobs, and organizing downloaded content. Downloads are processed asynchronously through a background queue to improve responsiveness and scalability.

This project was built to demonstrate backend engineering skills including API development, authentication, database design, background processing, testing, containerization, CI automation, and effective use of modern AI-assisted development tools. And for personal use as a tool to download audio file on local machine too.

## Features

* JWT Authentication
* User Registration and Login
* Media Metadata Extraction
* Background Download Processing
* Duplicate Media Detection
* Search and Pagination
* PostgreSQL Database Integration
* Health Check Endpoint
* Structured Logging with Serilog
* Unit Testing with xUnit
* Dockerized Deployment
* GitHub Actions Continuous Integration

## Architecture

src
├── MediaArchive.Api
├── MediaArchive.Application
├── MediaArchive.Domain
└── MediaArchive.Infrastructure

## Tech Stack

- ASP.NET Core 9
- Entity Framework Core 9
- PostgreSQL
- JWT Authentication
- Serilog
- xUnit
- Docker
- GitHub Actions
- yt-dlp

## Running Locally

### Clone Repository

git clone https://github.com/toant1512/Personal_Project.git

### Restore Packages

dotnet restore

### Run API

dotnet run --project src/MediaArchive.Api

## Docker

docker compose up --build

Swagger:
http://localhost:5000/swagger

Health:
http://localhost:5000/health

The application automatically applies pending Entity Framework Core migrations during startup, allowing a fresh environment to be created with a single Docker Compose command.

## Running Tests

dotnet test

Unit tests cover:
- Duplicate detection
- Media creation
- Pagination
- Search
- User isolation
- Empty result scenarios

## AI-Assisted Development

This project was developed using an AI-assisted workflow alongside traditional software engineering practices.

Tools such as ChatGPT and GitHub Copilot/Codex were used to support:

- Architecture discussions
- Code reviews
- Test planning
- Documentation improvements
- Development productivity

All generated suggestions were reviewed, adapted, tested, and validated before being incorporated into the project.

The project owner remained responsible for all architectural decisions, implementation details, debugging, testing, and final code quality.