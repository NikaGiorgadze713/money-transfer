# Money Transfer

A simulated money transfer application. It lets a fixed set of demo accounts send funds to one another and keeps a full transaction history. There is no real banking, authentication, or external payment integration involved, it exists purely to demonstrate a correctly implemented transfer flow (validation, atomicity, and concurrency safety) end to end, from database to UI.

## Tech Stack

- **.NET 8 Web API** — backend REST API
- **Entity Framework Core** — data access and schema management
- **Microsoft SQL Server 2022** — relational database
- **React + Vite** — frontend single-page application
- **nginx** — serves the built frontend and proxies API requests
- **Docker Compose** — orchestrates all three services

## Prerequisites

- Docker (with Docker Compose)

No other tools, runtimes, or package managers are required on the host machine, everything builds and runs inside containers.

## Running the App

From the repository root:

```bash
docker compose up --build
```

Once it's up:

- **Frontend:** http://localhost:3000
- **API:** http://localhost:5000
- **Swagger UI:** http://localhost:5000/swagger

> **Note:** The first startup can take about a minute. SQL Server needs time to initialize before the API can connect, apply the schema, and seed demo data; the API container waits for the database to report healthy before starting.

To stop the app:

```bash
docker compose down
```

To stop the app and wipe the database volume (next startup will re-seed from scratch):

```bash
docker compose down -v
```

## Demo Accounts

The database is seeded automatically on first run with three accounts:

| Id | Owner   | Balance |
|----|---------|---------|
| 1  | Alice   | 1000.00 |
| 2  | Bob     | 500.00  |
| 3  | Charlie | 250.00  |

## API Endpoints

| Method | Path               | Description                                  |
|--------|--------------------|-----------------------------------------------|
| GET    | `/api/accounts`     | List all accounts with their current balance |
| GET    | `/api/transactions` | List all completed transfers, newest first   |
| POST   | `/api/transfers`    | Execute a transfer between two accounts      |

### `GET /api/accounts`

```json
[
  { "id": 1, "owner": "Alice", "balance": 1000.00 },
  { "id": 2, "owner": "Bob", "balance": 500.00 },
  { "id": 3, "owner": "Charlie", "balance": 250.00 }
]
```

### `GET /api/transactions`

```json
[
  {
    "id": 1,
    "fromAccountId": 1,
    "fromOwner": "Alice",
    "toAccountId": 2,
    "toOwner": "Bob",
    "amount": 150.00,
    "createdAtUtc": "2026-07-10T20:04:07.14Z"
  }
]
```

### `POST /api/transfers`

Request body:

```json
{
  "fromAccountId": 1,
  "toAccountId": 2,
  "amount": 150.00
}
```

Successful response — `200 OK`:

```json
{ "message": "Transfer completed successfully." }
```

Error response — `400 Bad Request` (also returned for a nonexistent account, an amount that isn't positive, or a same-account transfer):

```json
{ "message": "Insufficient funds." }
```

## How Transfer Correctness Is Guaranteed

A transfer touches two account balances and inserts a transaction record, so it has to be all-or-nothing and safe when multiple transfers happen at the same time. This is enforced at three layers:

1. **Atomicity** — the balance updates and the transaction insert all happen inside a single database transaction. If any step fails, the whole transfer is rolled back, so an account can never be debited without the corresponding credit (or vice versa).
2. **Concurrency safety** — before modifying balances, both account rows are locked (`UPDLOCK`) in a deterministic order based on account id, regardless of which account is the sender or receiver. This serializes concurrent transfers that touch a shared account and prevents deadlocks that could otherwise occur if two simultaneous transfers locked the same pair of rows in opposite order.
3. **Database-level safety net** — a `CHECK` constraint on the `Accounts` table enforces `Balance >= 0` independently of the application logic. Even if a bug slipped past the service-layer validation, the database itself would reject any write that leaves an account with a negative balance.
