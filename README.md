# CrmPlatform

A multi-tenant CRM platform for HR, management, internal communication,
developer workflows, and social-media moderation.

## Current milestone

The first milestone establishes tenant isolation before business modules are
added. Every tenant-owned record is scoped by `TenantId` in the backend and
database access layer; the frontend is never trusted to choose that value.

## Solution layout

- `CrmPlatform.Domain`: business entities and invariants.
- `CrmPlatform.Application`: use-case contracts and application abstractions.
- `CrmPlatform.Infrastructure`: EF Core, persistence, and external integrations.
- `CrmPlatform.Api`: ASP.NET Core HTTP and realtime host.
- `CrmPlatform.UnitTests`: domain and application tests.
- `CrmPlatform.IntegrationTests`: database and tenant-isolation tests.

## Build

```bash
dotnet restore CrmPlatform.slnx
dotnet build CrmPlatform.slnx --no-restore
dotnet test CrmPlatform.slnx --no-build
```

The commercial product name is intentionally not embedded yet, so it can be
chosen without a costly rename during the foundation phase.
