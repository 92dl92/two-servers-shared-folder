# BlazorApp

A Blazor Web App (.NET 10) scaffold with a tabbed form and a navigation tray.

## Projects

| Project | Type | Responsibility | References |
|---|---|---|---|
| `BlazorApp.Server` | ASP.NET Core host | Hosts the app, prerenders pages, exposes the HTTP API (`/api/registrations/...`, `/api/users`); loads `users.json` | Client, Services, Shared |
| `BlazorApp.Client` | Blazor WebAssembly | Pages, layout (nav tray), reusable components (tabs), HTTP service implementations | Shared |
| `BlazorApp.Services` | Class library | Business logic; server-side implementations of Shared contracts | Shared |
| `BlazorApp.Shared` | Class library | DTOs, service interfaces, API routes | — |
| `tools/BlazorApp.Stress` | Console app | Stress tool: runs several Server processes on one data folder and checks nothing was lost | Services, Shared (runs Server as processes) |

Components depend only on interfaces in `Shared` (e.g. `IRegistrationService`), which are
implemented twice:

- **Server / prerendering** → `Services.InMemoryRegistrationService` (in-process).
- **Browser** → `Client.Services.HttpRegistrationService` (calls the Server API).

```
src/
├── BlazorApp.Server/
│   ├── Components/App.razor        # root HTML document (global WebAssembly render mode)
│   ├── Endpoints/                  # minimal API endpoints
│   └── wwwroot/app.css             # global styles + design tokens (light/dark)
├── BlazorApp.Client/
│   ├── Layout/MainLayout.razor     # top bar + tray + content
│   ├── Layout/NavTray.razor        # collapsible sidebar / mobile drawer
│   ├── Components/TabControl.razor # reusable tabs (TabControl + TabPage)
│   ├── Pages/Register.razor        # tabbed form: Personal / Address / Preferences / Review
│   ├── Pages/Records.razor         # list of records the acting user may see
│   ├── Pages/RecordDetail.razor    # one record: data, history, workflow buttons, conflict message
│   └── Services/                   # HTTP implementations of Shared contracts
├── BlazorApp.Services/
└── BlazorApp.Shared/
    ├── Contracts/                  # IRegistrationService, ApiRoutes
    └── Dtos/                       # RegistrationDto, SubmissionResultDto, ...
```

## Run

```bash
cd webapp
dotnet run --project src/BlazorApp.Server --launch-profile http
# → http://localhost:5080
```

## Features

- **Acting as (no login)** — users and roles come from `src/BlazorApp.Server/users.json`
  (path configurable with `--Users:Path`). The top-bar dropdown picks the acting user and
  stores it in the `acting-as` cookie. The server checks permissions on every call (401 with
  no/unknown user, 403 when the role isn't allowed); the UI only hides what the user can't do.

- **Records and workflow** — a record has a version, a status (Draft → Submitted →
  Approved/Rejected), `CreatedBy`/`UpdatedBy`/`UpdatedAt` and a history. Every change sends
  the version it started from; if the record changed meanwhile the API answers 409 and the
  page shows "Someone else changed this record" with a Reload button.

  | Method | Route | Action |
  |---|---|---|
  | GET | `/api/registrations` | list |
  | POST | `/api/registrations` | create (draft) |
  | GET | `/api/registrations/{id}` | view |
  | PUT | `/api/registrations/{id}` | update `{ expectedVersion, data }` |
  | POST | `/api/registrations/{id}/submit` \| `approve` \| `reject` | `{ expectedVersion }` |
  | POST | `/api/registrations/{id}/comments` | `{ expectedVersion, text }` |
  | DELETE | `/api/registrations/{id}?expectedVersion=N` | delete |

  Errors: 400 invalid, 401 no/unknown user, 403 not allowed, 404 not found, 409 out-of-date
  version, 422 not allowed in the current status.
- **Swappable storage** — records go through `IRegistrationStore`, chosen in settings:
  `--Storage:Strategy=InMemory` (default) or another strategy, plus `--Storage:Path=<folder>`
  for strategies that keep data in a shared folder. Every strategy must pass the shared suite
  in `tests/BlazorApp.Services.Tests/Storage`, which includes several separate processes
  (`tests/BlazorApp.StoreHelper`) saving at once.
- **Logging** — one record event per command (fixed `EventId` per action and outcome),
  store reads/writes/conflicts at Debug, one line per API request, and client events (page
  loads, button presses, results shown) sent from the browser. Every server writes
  `logs/server-<port>.jsonl` (JSON lines, with `ServerId`, `Source` and `CorrelationId` on
  each line), so one file tells the story of a user action and several servers' files can be
  merged. Levels: `--Logging:LogLevel:BlazorApp=Debug` turns on View and store events. No
  form values are ever logged.
- **Navigation tray** — docked sidebar on wide screens (`«` collapses it to an icon rail);
  slide-in drawer with backdrop on screens ≤ 768px (☰ opens it, closes on navigation).
  Add links in `NavTray.razor` (`_items`).
- **Tabbed form** — one `EditForm` across all tabs, validated with the data annotations on
  `RegistrationDto`. On an invalid submit the first tab containing an error is selected.
  The server re-validates and returns errors, which are shown inline.
- **Tabs component** — `<TabControl @bind-ActiveIndex="...">` with `<TabPage Title="...">`
  children; ARIA `tablist`/`tab`/`tabpanel` roles; inactive tabs stay rendered (hidden) so
  inputs keep their state.

## Test

```bash
dotnet test BlazorApp.slnx
```

| Project | Covers |
|---|---|
| `tests/BlazorApp.Services.Tests` | Rules, the service, and the shared storage suite every strategy must pass |
| `tests/BlazorApp.Server.Tests` | The HTTP API, in-process via `WebApplicationFactory` |
| `tests/BlazorApp.Client.Tests` | Components with bUnit: tabs, nav tray, Register page |
| `tests/BlazorApp.E2E.Tests` | Main journeys in headless Chromium against the Server run as its own process |
| `tests/BlazorApp.Stress.Tests` | The stress tool's checks, options, and known answers (in-memory: one server loses nothing, several servers do) |

## Stress tool

Starts several real Server processes sharing one data folder, sends colliding changes through
all of them at once as several "Acting as" users, then reads the data back through every
server and through the strategy itself and checks that every acknowledged change is there at
its version, every record deserialises, and every history follows the workflow rules.

```bash
dotnet run --project tools/BlazorApp.Stress -- --strategy PerRecordFile --servers 3 --ops 100
```

It writes `result.json` and `summary.md` (with the commit, settings, saves, 409s, 422s,
saves per second and latency) to `stress-runs/<time>-<strategy>/`, along with the data and
each server's log. Exit code 0 when every check passed, 1 when one failed, 2 when the run
couldn't be done. `--help` lists the options; design notes are in
[`docs/strategies/StressTool.md`](../docs/strategies/StressTool.md).

## Next steps

See [`docs/plan.md`](../docs/plan.md).
