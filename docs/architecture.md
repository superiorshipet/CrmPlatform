# Architecture plan

## Product boundary

CrmPlatform is a business SaaS product. A tenant represents one subscribed
company. A company can later own branches, departments, users, employees,
projects, tasks, conversations, and external Meta/WhatsApp connections.

Platform administrators manage subscriptions and tenants. Tenant users can
only operate on data belonging to their current tenant.

## Architecture

Start as a modular monolith on .NET 10. This keeps deployment and transactions
simple while preserving clear module boundaries. A module can be extracted
only after load measurements or ownership requirements justify it.

Dependency direction:

```text
Api -> Application <- Infrastructure
          |
        Domain
```

The initial modules are:

1. Platform and subscriptions.
2. Identity, memberships, roles, and permissions.
3. HR and departments.
4. Management requests and approvals.
5. Projects and developer tasks.
6. Internal chat and notifications.
7. Meta, Instagram, and WhatsApp moderation.
8. Files, audit logs, reporting, and background jobs.

## Tenant isolation rules

- The trusted tenant identifier comes from the authenticated principal.
- Request bodies, routes, query strings, and browser storage cannot select the
  effective tenant.
- Every tenant-owned entity contains a non-null `TenantId`.
- EF Core applies a global tenant query filter.
- `SaveChanges` assigns `TenantId` on inserts and rejects cross-tenant updates
  or deletes.
- Unique indexes for tenant data include `TenantId`.
- Cache keys, object-storage paths, SignalR groups, outbox messages, and jobs
  include the tenant identifier.
- Platform-wide operations use an explicit administration path; they never
  silently disable tenant filters inside normal requests.
- Integration tests prove that two tenants cannot read or mutate each other's
  data.

## Data strategy

The default model is one SQL Server database with row-level tenant partitioning.
The application boundary will keep tenant resolution separate from persistence,
allowing a dedicated database for an enterprise tenant later if required.

Schema changes are explicit EF Core migrations. Migrations never run
automatically during API startup.

## Security rules

- ASP.NET Core Identity owns first-party accounts, password hashing, lockout,
  email confirmation, and MFA.
- A user may belong to more than one tenant through tenant memberships.
- Tenant roles are collections of named permissions; business code authorizes
  permissions rather than hard-coded role names.
- Provider credentials are encrypted and never returned to clients.
- Webhooks validate provider signatures and use idempotency keys.
- Audit records capture actor, tenant, action, target, timestamp, and trace ID.

## Performance and reliability

- Project only required columns for reads and paginate unbounded collections.
- Use `AsNoTracking` for read-only EF queries.
- Keep external side effects behind a transactional outbox and worker.
- Cache only derivable data and namespace every key by tenant.
- Add health checks, structured logs, traces, rate limits, and request timeouts.
- SignalR transports realtime events; SQL remains the source of truth.

## Delivery order

1. Tenant domain, tenant context, EF isolation, and isolation tests.
2. Identity, tenant memberships, permissions, and authentication.
3. Departments and employees.
4. Projects, boards, and developer tasks.
5. Internal conversations and notifications.
6. Management requests and approval workflows.
7. Meta/Instagram/WhatsApp connections and unified inbox.
8. Subscription limits, audit, reporting, and operational hardening.

Each milestone requires a clean build, focused automated tests, reviewed
migration, and an explicit Git commit before the next milestone starts.
