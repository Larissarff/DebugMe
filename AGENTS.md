# AGENTS.md — DebugMe

## Project Overview

DebugMe is a mental-health tracking web app that applies software engineering metaphors (bugs, logs, stack traces, debugging, refactoring) to personal development. Users register emotional events, classify them by emotion type and intensity, and track patterns over time.

- **Backend**: C# ASP.NET Core Web API (.NET 9), EF Core, SQLite (dev), layered architecture
- **Frontend**: Angular 21 (SPA), TypeScript 5.9, RxJS, standalone components; no Angular modules
- **Tests**: xUnit + Moq + FluentAssertions (backend only; no frontend tests yet)
- **No CI/CD configured** — no GitHub Actions, Dockerfiles, or deployment scripts

## Essential Commands

### Backend (in `src/DebugMeBackend/`)

```bash
# Run the API (defaults to http://localhost:5165)
dotnet run

# Build
dotnet build

# EF Core migrations
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

### Frontend (in `src/debugme-frontend/`)

```bash
npm install        # first time only, or after package.json changes
npm start          # ng serve (dev server, usually port 4200)
npm run build      # production build
npm test           # ng test (vitest-based unit-test builder)
```

### Tests (in `tests/DebugMeBackend.Tests/`)

```bash
dotnet test
```

### Solution-level (root)

```bash
dotnet build DebugMe.sln
dotnet test DebugMe.sln
```

## Architecture & Data Flow

```
[Angular SPA] --HTTP REST--> [Controllers] --> [Services] --> [Repositories] --> [SQLite DB]
```

### Backend layers (top-to-bottom call direction)

| Layer | Path | Responsibility |
|---|---|---|
| Controllers | `Controllers/` | Receive HTTP requests, delegate to services, return HTTP responses |
| Services | `Services/` | Business logic, validation, orchestration |
| Repositories (interfaces) | `Repositories/Interfaces/` | Data access contracts |
| Repositories (impl) | `Repositories/` | EF Core queries against `AppDbContext` |
| Entities | `Entities/` | Domain models mapped to DB tables |
| DTOs | `DTOs/` | Request/response shapes (kept separate from entities) |

### Dependency injection flow

All services and repositories are registered as **scoped** in `Program.cs` (lines 45-50). DI chain:

- `UserController` → `UserService` → `IUserRepository` → `UserRepository` → `AppDbContext`
- `EmotionController` → `EmotionService` → `IEmotionRepository` → `EmotionRepository` → `AppDbContext`
- `EventLogController` → `EventLogService` → `IEventLogRepository` + `IUserRepository` + `IEmotionRepository` → all three repositories → `AppDbContext`

Controllers use only the service layer. Services use repository interfaces (never the concrete repository directly).

### Entity relationships

```
User (1) ──< (N) EventLog (N) >── (1) Emotion
```

- `EventLog` has FKs `UserId` and `EmotionId`, both cascade on delete
- `User.Email` has a unique index
- All entities use `Guid` PKs, auto-assigned in constructor
- All entities track `CreatedAt` / `UpdatedAt` timestamps

### Frontend layer structure

```
src/app/
├── core/           # Shared infrastructure
│   ├── services/   # ApiService (HTTP wrapper), AuthService, EmotionService, EventLogService
│   ├── models/     # TypeScript interfaces mirroring backend DTOs
│   └── guards/     # AuthGuard (route protection via localStorage)
├── features/       # Lazy route components (home, login, register, emotions, event-logs)
├── shared/         # Reusable UI components (currently empty)
├── app.routes.ts   # Route definitions
└── app.config.ts   # App bootstrap (provides router + HttpClient)
```

### Request flow (example: creating an event log)

1. User fills form in `features/event-logs/event-create/`
2. Component calls `EventLogService.create()` which calls `ApiService.post('/api/eventlog/create', body)`
3. `ApiService` sends HTTP POST to `http://localhost:5165/api/eventlog/create`
4. Backend `EventLogController.Create()` receives it
5. `EventLogService.CreateAsync()` validates (user exists, emotion exists, intensity 1-10, date not future)
6. `EventLogRepository.AddAsync()` inserts row via EF Core
7. Response DTO (with nested Emotion/User info) flows back

## Key Conventions & Gotchas

### Backend

- **Email normalization**: All emails are `.Trim().ToLower()` in `UserService` before storage/comparison
- **Emotion names**: Also normalized to lowercase on create; name uniqueness check is case-insensitive
- **Password hashing**: Uses `BCrypt.Net-Next` (BCrypt). `PasswordHash` is `[JsonIgnore]` so it never leaks via API.
- **`UserResponseDto` never includes `PasswordHash`** — the `MapToResponse` method explicitly excludes it.
- **Controllers return Portuguese error messages** (e.g., `"Usuário não encontrado."`)
- **DTOs use `[Required]` data annotations**; controllers check `ModelState.IsValid` before processing.
- **Some controllers accept entities directly** (`EmotionController`, `EventLogController`) while `UserController` uses DTOs. This is an inconsistency — when adding new endpoints for emotions/events, consider using DTOs to match the User pattern.
- **Mixed namespace styles**: Some files use block-scoped namespaces (`namespace X { }`), others file-scoped (`namespace X;`). Follow the existing style of the file you're editing.
- **Repository `UpdateAsync` pattern**: `EmotionRepository` and `EventLogRepository` fetch the existing entity, then selectively update properties and set `UpdatedAt`. `UserRepository` uses `_context.Users.Update(user)` instead — both patterns work but be consistent within each repository.
- **Health check** available at `/health` endpoint.
- **Swagger** at `/swagger` in development mode.
- **CORS** is wide open (`AllowAnyOrigin`) — configured for development convenience.

### Frontend

- **Standalone components only** — no `NgModule`. Use `imports: [...]` in `@Component` decorator. Do NOT create module files.
- **Auth is localStorage-based**: `AuthService` stores the `User` object as JSON under `'debugme_user'` key. `AuthGuard` reads from there. There is NO JWT, NO token refresh, NO HTTP-only cookies.
- **Logout uses `window.location.href = '/login'`** instead of Angular router — this causes a full page reload. This is intentional (clears all in-memory state).
- **`ApiService`** is a centralized HTTP wrapper: all requests go through `get/post/put/delete` methods with 30s timeout. Never call `HttpClient` directly from feature components.
- **Feature services** (Auth, Emotion, EventLog) extend the pattern: they inject `ApiService` and return typed `Observable<T>`.
- **`ApiService` logs all requests/responses** to console — useful for debugging but don't remove these logs.
- **Environment config**: `environment.development.ts` is used during `npm start`, `environment.ts` is used for production builds. Both currently point to `http://localhost:5165`.
- **Prettier** config: 100 char width, single quotes, 2-space indent. Also applies to HTML via Angular parser.
- **`.editorconfig`** enforces 2-space indent for all files, single quotes for `.ts` files.

### Tests

- **xUnit + Moq + FluentAssertions** is the testing stack
- Mock repositories using `new Mock<IXXXRepository>()`, never mock the service layer
- `EventLogServiceTests` has helper methods `CreateMocks()` and `CreateService()` — follow this pattern when adding tests for services with multiple dependencies
- Tests validate Portuguese exception messages (e.g., `"O nome da emoção é obrigatório."`)
- Tests currently cover all three services extensively; there are **no integration tests** and **no frontend tests**

## Database

- **SQLite** in development (connection string hardcoded in `Program.cs`: `Data Source=debugme.db`)
- The `appsettings.json` contains a SQL Server connection string but it's **not used** — `Program.cs` explicitly configures SQLite
- Migrations are in `Migrations/` folder, managed via EF Core CLI tools
- To reset the DB: delete `debugme.db` and run `dotnet ef database update`

## Angular Test Configuration

- Uses `vitest` (not Karma/Jasmine) — this is Angular 21's default
- Test config is in `tsconfig.spec.json` and the builder is `@angular/build:unit-test`
- `jsdom` is the test environment package (listed in devDependencies)
