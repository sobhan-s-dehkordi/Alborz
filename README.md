# Alborz

## Developer setup

- .NET 10 SDK and the Windows App SDK dependencies from the WinUI project.
- Set `ALBORZ_SQLSERVER_CONNECTION` in the application's launch environment. Database configuration is deliberately absent from the settings UI. Do not commit credentials.
- Apply migrations explicitly: `dotnet ef database update --project Alborz.Infrastructure`.
- Startup only checks connectivity and pending migrations; it does not modify the database. Settings remain accessible while business pages are disabled for an unavailable database.
- SQLite migrations are archived under `Migrations/LegacySqlite`. The SQL Server migrations create a separate schema; they do not import an existing SQLite database.
- The `UniqueDocumentProducts` migration enforces one product per document. Existing duplicate rows must be reconciled before applying it to a pre-existing database.

## Appearance

Settings offer system/light/dark theme, installed font family, text size 12–24, a preview and restore defaults. Changes apply immediately and persist per Windows user in `%LOCALAPPDATA%/AlborzApp/appearance.json`. Invalid settings fall back to defaults; missing fonts fall back to Segoe UI. Explicit heading sizes and icon fonts are retained.

## Document rows

Adding an existing product increases its existing row's quantity. The existing unit price and discount rate are retained; the total line discount is recalculated to two decimal places. Use the row's Edit button to change quantity, unit price or total discount. Cancel leaves the row unchanged. Delete removes the row. Editing a saved document reconciles inventory only on successful save; empty documents and invalid amounts are rejected.

## Architecture

Domain contains entities and invariants. Application contains requests, handlers, workflow services and contracts. Infrastructure implements SQL Server repositories, persistence and export. WinUI contains views, view models and appearance rendering. Each database operation uses its own dependency injection scope. Row versions protect concurrent writes.

## Validation

- `dotnet build AlborzSolution.slnx`
- `dotnet test Alborz.Tests`
- `dotnet ef migrations has-pending-model-changes --project Alborz.Infrastructure`

Integration tests create and remove a uniquely named `Alborz_Tests_*` database. Set `ALBORZ_TEST_SQLSERVER_CONNECTION` to a test server with create/drop-database permissions. Tests never delete the configured application database.

Manual release checks still required: launch the registered WinUI app, change all three themes, select a Persian-capable font, restart and verify persistence, add the same product twice in both forms, edit/cancel/delete rows and verify totals. The automated desktop launch attempt on 2026-09-20 timed out and Windows logged a Microsoft.UI.Xaml.dll application crash; visual acceptance has not passed. Subsequent managed startup errors are recorded without connection strings in `%LOCALAPPDATA%/AlborzApp/startup-errors.log`.
