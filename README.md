# Personal Expense Manager

A full-stack application for managing personal expenses, built with Angular 16, ASP.NET Core Web API on .NET 10, and Microsoft SQL Server.

## Features

- JWT login and role-based authorization
- Personal expense CRUD with date, amount, and category
- Data isolation: users can access only their own expenses
- Administrator-only user creation and user list
- SQL Server persistence with an independent, script-based database project
- Unit and integration tests with xUnit
- Responsive Angular interface

## Clean Architecture

Dependencies point inward:

- `PersonalExpenses.Domain`: entities, invariants, and roles; no external dependencies
- `PersonalExpenses.Application`: use cases, DTOs, and repository/security ports
- `PersonalExpenses.Infrastructure`: EF Core SQL Server adapters, repositories, password hashing, and JWT creation
- `PersonalExpenses.Api`: HTTP controllers, authentication, and composition root
- `PersonalExpenses.Database`: SQL Server deployment executable and complete SQL source
- `PersonalExpenses.UnitTests`: fast domain and use-case specifications
- `PersonalExpenses.IntegrationTests`: HTTP and application integration tests using an isolated InMemory database
- `frontend/personal-expenses-ui`: Angular client

## Database project

The SQL source is stored in `database/PersonalExpenses.Database/Scripts` and runs in filename order:

1. `001_CreateDatabase.sql`: creates the `PersonalExpenses` database.
2. `Tables/010_CreateUsers.sql`: creates `dbo.Users` with its keys, defaults, and constraints.
3. `Tables/020_CreateExpenses.sql`: creates `dbo.Expenses` with its keys, constraints, and user relationship.
4. `Indexes/030_CreateIX_Expenses_UserId_Date.sql`: creates the expense query index.
5. `Seeds/040_SeedAdministrator.sql`: creates the initial administrator using a PBKDF2 password hash.

Every database object has its own SQL file. Files use a global numeric prefix so dependencies execute in a deterministic order across folders.

The scripts are idempotent. Run them against a local SQL Server with Windows authentication:

```powershell
dotnet run --project database/PersonalExpenses.Database
```

## Run

Requirements: .NET 10 SDK, SQL Server, Node.js 16+, and npm.

1. Create the database with the Database project.
2. Adjust `ConnectionStrings:Expenses` in `src/PersonalExpenses.Api/appsettings.json` if required.
3. Start the API:

```powershell
dotnet run --project src/PersonalExpenses.Api --urls http://localhost:5000
```

4. Start Angular in another terminal:

```powershell
cd frontend/personal-expenses-ui
npm install
npm start
```

Open `http://localhost:4200` and sign in with:

- Username: `admin`
- Password: `Admin123!`

Change the seeded password and `Jwt:Key` before production use.

## Tests and builds

```powershell
dotnet test
cd frontend/personal-expenses-ui
npm run build
```

HTTP integration tests use EF Core InMemory to isolate the Web API. Repository integration tests use a real SQL Server database, create a uniquely named temporary database, verify persistence with fresh DbContext instances, and drop it after the suite. On Windows they default to `(localdb)\\MSSQLLocalDB`; CI can provide another server through `PERSONAL_EXPENSES_TEST_SQLSERVER`. The test account must be allowed to create and drop databases.

## REST API

| Method | Route | Access | Purpose |
|---|---|---|---|
| POST | `/api/auth/login` | Anonymous | Authenticate |
| GET | `/api/expenses` | Authenticated | List own expenses |
| GET | `/api/expenses/{id}` | Authenticated | Read own expense |
| POST | `/api/expenses` | Authenticated | Create expense |
| PUT | `/api/expenses/{id}` | Authenticated | Update own expense |
| DELETE | `/api/expenses/{id}` | Authenticated | Delete own expense |
| GET | `/api/users` | Admin | List users |
| POST | `/api/users` | Admin | Create user |


