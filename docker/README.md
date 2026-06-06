# Local database

The application uses PostgreSQL for local development.

Start the database:

```powershell
docker-compose up -d
```

Stop the database:

```powershell
docker-compose down
```

The compose file exposes PostgreSQL on `localhost:5433` to avoid conflicts with a locally installed PostgreSQL server on `5432`.

Development connection string:

```text
Host=localhost;Port=5433;Database=fitness_training_dev;Username=postgres;Password=abracadabra
```
