# Datenbankmigrationen

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project src/IptvManagement.Infrastructure --startup-project src/IptvManagement.Api
dotnet ef database update --project src/IptvManagement.Infrastructure --startup-project src/IptvManagement.Api
```

Die im Repository enthaltene Initialdatei markiert den Migrationsbereich; für eine konkrete SQL-Server-Instanz wird die Migration mit obigen Befehlen neu erzeugt und geprüft.
