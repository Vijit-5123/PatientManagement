# Patient Management System

A small, realistic full-stack Patient Management application used as a **test target** for the Sentinel autonomous functional-testing agent and the Cybersecurity Agent.

> All patient data is synthetic. No real PHI is used anywhere in this repository.

## 1. Architecture

```
Angular (SPA)
   ↓ HTTP/JSON (Bearer JWT)
ASP.NET Core Web API (PatientManagement.Api)
   ↓
PatientManagement.Application  (DTOs, services, validation, interfaces)
   ↓
PatientManagement.Infrastructure (EF Core, SQLite, JWT, password hashing, seed data)
   ↓
PatientManagement.Domain (entities, enums — no external dependencies)
   ↓
SQLite (app.db)
```

Dependency direction: `Domain ← Application ← Infrastructure ← Api`. Controllers never touch EF Core entities directly — everything crosses the API boundary as a DTO.

## 2. Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (a `global.json` pins the repo to 8.0.x even if newer SDKs are installed)
- [Node.js 20+](https://nodejs.org/) and npm
- Angular CLI (`npm install -g @angular/cli`) — optional, `npx ng` also works

## 3. Project Structure

```
patient-management/
├── backend/
│   ├── PatientManagement.Api/            ASP.NET Core Web API, controllers, auth, middleware
│   ├── PatientManagement.Application/    DTOs, service interfaces/implementations, validation
│   ├── PatientManagement.Infrastructure/ EF Core DbContext, repositories, JWT, seed data
│   └── PatientManagement.Domain/         Entities, enums (no external dependencies)
├── frontend/
│   └── patient-management-ui/            Angular 21 standalone SPA
├── tests/
│   ├── backend/PatientManagement.Tests/  xUnit integration tests (auth, authz, CRUD)
│   └── e2e/                              Playwright end-to-end browser tests
├── global.json                           Pins the .NET SDK version
├── .env.example                          Documents backend environment variable overrides
└── README.md
```

## 4. Backend Setup

```bash
cd backend
dotnet restore
dotnet build
```

The SQLite database, EF Core migration, and deterministic seed data are all applied **automatically at startup** — there is no manual migration step required for local development.

## 5. Frontend Setup

```bash
cd frontend/patient-management-ui
npm install
```

## 6. Database

- Engine: SQLite, file `backend/PatientManagement.Api/app.db` (created automatically, git-ignored)
- ORM: EF Core, migration `InitialCreate` in `PatientManagement.Infrastructure/Persistence/Migrations`
- On startup, `Program.cs` calls `context.Database.Migrate()` then `DbSeeder.Seed(...)`, which seeds:
  - 2 users: `admin` (Admin role), `doctor` (Doctor role)
  - 10 synthetic patients with deterministic data (fixed names, dates, MRNs)
- Seeding is idempotent (`if (!context.Users.Any())` / `if (!context.Patients.Any())`), so restarting the app does not duplicate data. Delete `app.db*` to reset to the deterministic seed.

## 7. How to Run

**Terminal 1 — backend:**

```bash
cd backend/PatientManagement.Api
dotnet run
```

Backend listens on `http://localhost:5080` (set via `ASPNETCORE_URLS` — see below).

**Terminal 2 — frontend:**

```bash
cd frontend/patient-management-ui
npx ng serve --port 4200
```

Open `http://localhost:4200`.

### Ports used in this document

| Service | URL |
|---|---|
| Frontend (Angular) | http://localhost:4200 |
| Backend API | http://localhost:5080 |
| Swagger UI | http://localhost:5080/swagger |
| Health check | http://localhost:5080/health |

To run the backend on a different port, set `ASPNETCORE_URLS` (see `.env.example`) and update `frontend/patient-management-ui/src/environments/environment.ts` (`apiUrl`) and `backend/PatientManagement.Api/appsettings.json` (`Cors:AllowedOrigins`) to match.

## 8. API URL

`http://localhost:5080/api`

## 9. Swagger URL

`http://localhost:5080/swagger` (Development environment only). Use `POST /api/auth/login` to obtain a token, then click **Authorize** and enter `Bearer <token>` to call protected endpoints directly from Swagger.

## 10. Frontend URL

`http://localhost:4200`

## 11. Health Endpoint

- `GET /health` — liveness (`{"status":"healthy"}`)
- `GET /health/ready` — readiness, verifies the database is reachable (`{"status":"ready","database":"connected"}`)

## 12. Test Users (development-only credentials)

| Username | Password | Role |
|---|---|---|
| `admin` | `Admin123!` | Admin |
| `doctor` | `Doctor123!` | Doctor |

These are seeded deterministically by `DbSeeder` for local development/testing only. Passwords are hashed with BCrypt before being stored — plaintext is never persisted.

## 13. Roles / Permissions Matrix

| Operation | Admin | Doctor |
|---|---|---|
| List patients | ✅ | ✅ |
| View patient | ✅ | ✅ |
| Create patient | ✅ | ✅ |
| Update patient | ✅ | ✅ |
| Delete patient | ✅ | ❌ (403 Forbidden) |

Authorization is enforced with explicit ASP.NET Core policies (not scattered role strings):

- `PatientRead` — Admin, Doctor
- `PatientWrite` — Admin, Doctor (create/update)
- `PatientDelete` — Admin only
- `AdminOnly` — Admin

See `backend/PatientManagement.Api/Authorization/AuthorizationPolicyExtensions.cs`.

- Missing/invalid JWT → **401 Unauthorized**
- Valid JWT but insufficient role → **403 Forbidden**
- The frontend hides actions the current user isn't permitted to perform (see `PermissionsService`), but this is a UX convenience only — the API independently re-checks every request.

## 14. Running Tests

Backend (xUnit, uses `WebApplicationFactory` + a temp SQLite file per test run):

```bash
cd backend
dotnet test ../tests/backend/PatientManagement.Tests/PatientManagement.Tests.csproj
```

Covers: valid/invalid login, expired/invalid token rejection, unauthenticated access, Admin full CRUD, Doctor create/read/update allowed + delete forbidden, patient CRUD (create/read/update/delete/404/validation).

Frontend unit tests (Vitest, via Angular CLI):

```bash
cd frontend/patient-management-ui
npx ng test --watch=false
```

## 15. Running E2E Tests

Playwright tests drive a real browser against the real running backend + frontend (both must already be running per section 7).

```bash
cd tests/e2e
npm install
npx playwright install chromium   # first time only
npx playwright test
```

> Note: in this repository's original development sandbox, the downloaded Chromium binary could not be spawned by the OS, so `playwright.config.ts` launches via the `msedge` channel (pre-installed, code-signed) instead. If Edge is unavailable in your environment, switch the `channel` in `playwright.config.ts` to `chrome` or remove it to use Playwright's bundled Chromium.

Scenarios covered:

1. Admin login → patients → create → view → edit → delete
2. Doctor login → patients → create/view/edit → delete button hidden in UI → direct API delete call returns 403
3. Unauthenticated user visiting `/patients` is redirected to `/login`
4. Invalid login credentials show an inline error message

## 16. Project Structure (detail)

See section 3 above. Key files:

- `backend/PatientManagement.Api/Program.cs` — composition root: DI, JWT bearer auth, authorization policies, CORS, Swagger, exception middleware, automatic migrate+seed
- `backend/PatientManagement.Api/Authorization/` — policy names + policy-to-role mapping
- `backend/PatientManagement.Api/Middleware/ExceptionHandlingMiddleware.cs` — centralized error handling → RFC 7807 `application/problem+json`
- `backend/PatientManagement.Infrastructure/Seed/DbSeeder.cs` — deterministic seed data
- `frontend/patient-management-ui/src/app/core/` — `AuthService`, `PatientService`, `PermissionsService`, route guard, HTTP interceptor
- `frontend/patient-management-ui/src/app/features/` — login, patient list/form/detail pages

## 17. Security Notes

- **Authentication**: JWT bearer tokens (HMAC-SHA256), issued by `POST /api/auth/login`. The API independently validates signature, issuer, audience, and expiration (`Program.cs` → `AddJwtBearer`) on every request — the frontend's auth state is never trusted.
- **Password storage**: BCrypt (`BCrypt.Net-Next`), salted, one-way. Plaintext passwords are never persisted.
- **Authorization**: server-side only, via explicit ASP.NET Core policies mapped to roles. Frontend hides unavailable actions for UX, but this is never relied upon for security.
- **Validation**: DTO-level data annotations (`[Required]`, `[EmailAddress]`, `[MaxLength]`, etc.) plus explicit business rules (e.g. date of birth cannot be in the future) in `PatientService`.
- **Database access**: EF Core with parameterized queries throughout — no raw/dynamic SQL.
- **Error handling**: centralized middleware returns RFC 7807 ProblemDetails; stack traces, connection strings, and secrets are never included in responses.
- **CORS**: explicit allow-list (`http://localhost:4200` in development) — no `AllowAnyOrigin` + credentials, no wildcards.
- **Secrets**: the JWT signing key in `appsettings.Development.json` is a clearly-labeled, non-production development secret. Override via environment variables (see `.env.example`) for any other environment. No production secrets are committed.
- **Logging**: structured logging via `ILogger`, including EF Core command logging in development and warning/error logs for handled/unhandled exceptions.

## 18. Sentinel / Cybersecurity Agent Test Surface

This section is the canonical map of what an autonomous agent can discover and exercise.

### API Endpoints

| Method | Route | Auth | Policy |
|---|---|---|---|
| POST | `/api/auth/login` | None | — |
| GET | `/api/patients` | JWT | `PatientRead` (Admin, Doctor) |
| GET | `/api/patients/{id}` | JWT | `PatientRead` (Admin, Doctor) |
| POST | `/api/patients` | JWT | `PatientWrite` (Admin, Doctor) |
| PUT | `/api/patients/{id}` | JWT | `PatientWrite` (Admin, Doctor) |
| DELETE | `/api/patients/{id}` | JWT | `PatientDelete` (Admin only) |
| GET | `/health` | None | — |
| GET | `/health/ready` | None | — |

Full machine-readable schema: `GET /swagger/v1/swagger.json`.

### UI Routes

| Route | Guarded |
|---|---|
| `/login` | No |
| `/patients` | Yes (redirects to `/login`) |
| `/patients/new` | Yes |
| `/patients/:id` | Yes |
| `/patients/:id/edit` | Yes |

### Authentication Flow

`POST /api/auth/login` with `{ "username", "password" }` → `{ accessToken, expiresAt, user }`. Token stored in `localStorage` by the Angular `AuthService`, attached as `Authorization: Bearer <token>` by an HTTP interceptor on every API call. A `401` response anywhere triggers automatic logout + redirect to `/login`.

### Roles

`Admin`, `Doctor` — see permissions matrix in section 13.

### Important Workflows

- Login → list patients → search → create → view → edit → delete (Admin)
- Login → list patients → create → edit → attempt delete → expect 403 (Doctor)
- Visit any `/patients*` route unauthenticated → redirected to `/login`
- Submit invalid patient data (missing required field, malformed email, future date of birth) → expect `400` with field-level errors
- Request a nonexistent patient id → expect `404`

### Health Endpoint

`GET /health` and `GET /health/ready` — no authentication required, useful for readiness probing before running a test suite.

### Seeded Test Users

See section 12.

### Stable Selectors (for UI automation)

`data-testid` attributes are present on the key interactive elements: `login-username`, `login-password`, `login-submit`, `login-error`, `current-user`, `logout-button`, `patient-create`, `patient-search-input`, `patient-search-submit`, `patients-table`, `patients-loading`, `patients-empty`, `patients-error`, `patient-view-{id}`, `patient-edit-{id}`, `patient-delete-{id}`, `patient-form`, `patient-firstName`, `patient-lastName`, `patient-dateOfBirth`, `patient-gender`, `patient-email`, `patient-phone`, `patient-address`, `patient-city`, `patient-state`, `patient-postalCode`, `patient-form-submit`, `patient-form-cancel`, `patient-form-error`, `patient-detail-edit`, `patient-detail-delete`, `confirm-dialog-confirm`, `confirm-dialog-cancel`.

## 19. Future Extension Guidance

The architecture is intentionally layered so new features can be added without rewriting existing code:

- **New domain feature** (e.g. Appointments, Prescriptions, Departments): add an entity to `PatientManagement.Domain`, a service + DTOs to `PatientManagement.Application`, a repository + EF configuration to `PatientManagement.Infrastructure`, and a controller to `PatientManagement.Api`. Follow the same DTO-boundary and policy-based authorization pattern used for Patients.
- **New authorization policy**: add a policy name to `PolicyNames`, map it to roles in `AuthorizationPolicyExtensions`, and apply `[Authorize(Policy = ...)]` to the relevant controller action.
- **New roles**: extend `UserRole` enum and update policy mappings — no controller code changes required.
- **Refresh tokens, pagination on more resources, audit logs, file uploads**, etc. can be layered onto the existing `Infrastructure`/`Application` split without touching the Domain layer.
- **Frontend**: new features should follow the existing `core/` (services, guards, interceptors) vs `features/` (routed pages) vs `shared/` (reusable UI) split, and add UI-only permission checks to `PermissionsService` rather than scattering role checks across components.

## 20. Known Limitations

- No refresh-token flow — tokens simply expire (default 60 minutes) and the user must log in again.
- No pagination caching or optimistic UI updates — every list view re-fetches from the API.
- Angular Material's default theme fonts (Roboto, Material Icons) load from Google Fonts via `index.html`; in a fully offline environment icons will fall back to their text ligature names and text will use system fallback fonts, but core functionality is unaffected.
- Playwright's bundled Chromium could not be launched in this repository's original sandbox (OS-level spawn restriction observed in that environment only); the config falls back to the `msedge` channel. Swap this back to Playwright's bundled Chromium in unrestricted environments if preferred.
