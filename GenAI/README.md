# Task Management API

Production-oriented REST API built with .NET 10 Minimal APIs, EF Core, SQL Server, OpenAPI, and xUnit.

## Structure

```text
src/TaskManagement.Api/
  Contracts/       Request/response schemas and PATCH JSON converter
  Data/            EF Core DbContext and migrations
  Dependencies/    `get_current_user` abstraction and authentication adapter
  Domain/          Task, User, and status models
  Errors/          Global RFC 7807 error handling
  Routes/          Versioned Minimal API endpoints
  Services/        Async business/data-access layer
tests/
  TaskManagement.UnitTests/
  TaskManagement.IntegrationTests/
```

## Setup

Prerequisites: .NET 10 SDK and Docker Desktop.

```powershell
Copy-Item .env.example .env
docker compose up -d sqlserver
dotnet tool restore
dotnet restore
dotnet ef database update --project src/TaskManagement.Api --startup-project src/TaskManagement.Api
dotnet run --project src/TaskManagement.Api --urls http://localhost:8080
```

Or build the API container after applying migrations:

```powershell
docker compose up --build
```

Configuration uses `ConnectionStrings__DefaultConnection`. Change the example SA password before using any shared environment.

## Authentication integration

`ICurrentUser` is the C# dependency equivalent of `get_current_user`. `HttpCurrentUser` reads the authenticated `nameidentifier` or `sub` claim. Because authentication itself is assumed, the included development adapter accepts `X-User-Id: <uuid>` so the standalone sample runs. Replace the `AddAuthentication(...).AddScheme(...)` registration in `Program.cs` with the existing JWT/cookie authentication; the routes and ownership checks need no changes.

## Migrations and tests

```powershell
dotnet ef migrations add DescribeChange --project src/TaskManagement.Api --startup-project src/TaskManagement.Api
dotnet ef database update --project src/TaskManagement.Api --startup-project src/TaskManagement.Api
dotnet test
```

Unit tests use EF Core InMemory to exercise the service. Integration tests host the real HTTP pipeline and cover CRUD, PATCH, validation, 404 behavior, ownership, status/due-date filters, and pagination.

## API

OpenAPI JSON is available at `GET /openapi/v1.json`. All task endpoints require `X-User-Id` in the standalone development setup.

| Method | Path | Description |
|---|---|---|
| POST | `/api/v1/tasks` | Create |
| GET | `/api/v1/tasks?status=pending&dueFrom=...&dueTo=...&page=1&pageSize=20` | Filtered, paged list |
| GET | `/api/v1/tasks/{task_id}` | Retrieve owned task |
| PUT/PATCH | `/api/v1/tasks/{task_id}` | Full or partial update |
| DELETE | `/api/v1/tasks/{task_id}` | Delete owned task |

Create request:

```json
{
  "title": "Ship API",
  "description": "Run the release checklist",
  "status": "pending",
  "due_date": "2026-08-10T18:00:00-05:00"
}
```

`201 Created` response:

```json
{
  "id": "8caf27ca-e1e7-4a1a-b340-442c93315f71",
  "title": "Ship API",
  "description": "Run the release checklist",
  "status": "pending",
  "due_date": "2026-08-10T18:00:00-05:00",
  "user_id": "11111111-1111-1111-1111-111111111111",
  "created_at": "2026-08-03T15:00:00+00:00",
  "updated_at": "2026-08-03T15:00:00+00:00"
}
```

PATCH accepts any subset; explicit `null` clears nullable fields:

```json
{ "status": "completed", "due_date": null }
```

Errors consistently use RFC 7807 JSON. Tasks that are absent or owned by someone else both return this non-enumerating response:

```json
{
  "type": "about:blank",
  "title": "Task not found",
  "status": 404,
  "detail": "The task does not exist or is not owned by the current user."
}
```

Ready-to-run HTTP examples are in `src/TaskManagement.Api/TaskManagement.Api.http`.
