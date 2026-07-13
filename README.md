# IPTV Management

Professionelles .NET-9-IPTV-Verwaltungssystem für ausschließlich eigene, öffentlich freigegebene oder ordnungsgemäß lizenzierte Medieninhalte. Das System enthält Clean-Architecture-Projekte für Domain, Application, Infrastructure, REST API, Blazor Web App und Tests.

## Rechtlicher Hinweis

Dieses Projekt darf nicht für illegale Streams, gestohlene URLs, Pay-TV ohne Lizenz, DRM-Umgehung oder sonstige urheberrechtsverletzende Inhalte verwendet werden. Stream-URLs werden verschlüsselt gespeichert und im Frontend sowie in Logs nicht vollständig ausgegeben.

## Projekte

- `src/IptvManagement.Domain`: Entities, Enums, Domain-Regeln und Exceptions.
- `src/IptvManagement.Application`: DTOs, Interfaces, Validierung, Pagination und Use-Case-Verträge.
- `src/IptvManagement.Infrastructure`: EF Core, Identity, SQL Server, Token, Verschlüsselung, Dashboard, Playlist, Rechnungen und Background Services.
- `src/IptvManagement.Api`: REST API, Swagger, Rate Limiting, Health Checks, Playlist- und Stream-Endpunkte.
- `src/IptvManagement.Web`: Blazor Web App mit Interactive Server und dunklem deutschen Admin-Design.
- `tests/IptvManagement.UnitTests`: xUnit-Tests für Abonnementlogik, Token, Lizenzprüfung und Rechnungsberechnung.

## Voraussetzungen

- .NET SDK 9
- SQL Server 2022 oder Azure SQL
- Optional Docker und Docker Compose

## SQL Server einrichten

```bash
export IPTV_SQL_PASSWORD='Ein_langes_sicheres_Passwort_123!'
docker compose up -d sqlserver
```

## Connection String konfigurieren

Development per User-Secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=IptvManagement_Dev;User Id=sa;Password=...;TrustServerCertificate=True;Encrypt=True" --project src/IptvManagement.Api
```

Production per Umgebungsvariable:

```bash
export ConnectionStrings__DefaultConnection='Server=...;Database=IptvManagement;User Id=...;Password=...;Encrypt=True'
```

## Migrationen ausführen

```bash
dotnet ef database update --project src/IptvManagement.Infrastructure --startup-project src/IptvManagement.Api
```

Siehe `docs/database-migrations.md`.

## Administrator konfigurieren

Das initiale Administratorkonto wird beim Start aus Konfiguration erzeugt. Das Kennwort steht nie im Quellcode.

```bash
export InitialAdmin__Email='admin@example.com'
export InitialAdmin__Password='SehrLangesSicheresPasswort123!'
```

## Anwendung starten

```bash
dotnet restore
dotnet build
dotnet run --project src/IptvManagement.Api
# optional Admin-Oberfläche
dotnet run --project src/IptvManagement.Web
```

## Rollen und Berechtigungen

Beim Start werden `Administrator`, `Mitarbeiter`, `Reseller` und `NurLesen` angelegt. Policies schützen Verwaltungs- und Lese-Endpunkte.

## Playlist-Endpunkt verwenden

1. Abonnement erstellen.
2. Token über `POST /api/subscriptions/{id}/token` erzeugen.
3. M3U abrufen: `GET /api/playlists/{token}/playlist.m3u`.

Die Playlist enthält ausschließlich Gateway-URLs und niemals originale Stream-URLs.

## Gerät aktivieren

Aktivierungscodes werden gehasht gespeichert, laufen kurzzeitig ab und werden durch den Cleanup-Worker bereinigt. Geräte werden je Abonnement begrenzt und blockierte Geräte erhalten keinen Zugriff.

## Abonnement verlängern

`POST /api/subscriptions/{id}/renew/12` verlängert um zwölf Monate. Aktive Abonnements werden ab bestehendem Ablaufdatum verlängert; abgelaufene ab aktuellem UTC-Zeitpunkt.

## Rechnungen und Zahlungen

Rechnungsnummern werden fortlaufend erzeugt, EUR und österreichische Umsatzsteuer werden über `SystemSettings` konfiguriert. PDF-Ausgabe ist als Service kapselt und kann produktiv an ein PDF-Rendering angebunden werden.

## Produktionsbereitstellung

- HTTPS erzwingen.
- Secrets nur per Key Vault, User-Secrets oder Environment Variables.
- SQL-Verbindungen verschlüsseln.
- Serilog-Sinks und Retention konfigurieren.
- Reverse Proxy mit HSTS, CSP und Rate Limiting betreiben.

## IIS-Bereitstellung

```bash
dotnet publish src/IptvManagement.Api/IptvManagement.Api.csproj -c Release
```

Das Publish-Verzeichnis als IIS-Site mit ASP.NET Core Hosting Bundle bereitstellen.

## Docker-Bereitstellung

```bash
export IPTV_SQL_PASSWORD='...'
export IPTV_ADMIN_EMAIL='admin@example.com'
export IPTV_ADMIN_PASSWORD='...'
docker compose up --build
```

## Azure-Bereitstellung

- Azure App Service für API/Web.
- Azure SQL Database.
- Managed Identity/Key Vault für Secrets.
- Application Insights oder zentraler Serilog-Sink.

## Backup-Strategie

Tägliche SQL-Backups, getestete Restore-Prozesse, revisionssichere Rechnungsablage und getrennte Sicherung der Data-Protection-Keys.

## Sicherheitshinweise

Playlist-Tokens sind 256-Bit-Zufallswerte und werden nur gehasht gespeichert. Stream-URLs werden mit ASP.NET Core Data Protection verschlüsselt. Vollständige Stream-URLs, Klartexttoken und Passwörter dürfen nicht geloggt werden.

## Stream-Gateway und rechtliche Einschränkung

Das Gateway validiert Zugriffe. Für produktives Medienstreaming sind Reverse Proxy, HLS Manifest Rewriting, kurzlebige Signed URLs, CDN, Key Rotation, Bandbreitenkontrolle und Rechteverwaltung erforderlich. Eine unsichere einfache Weiterleitung wird bewusst nicht als produktionssicherer Restreaming-Dienst implementiert.

## Fehlerbehebung

- `dotnet --info` prüfen.
- Connection String und SQL-Erreichbarkeit prüfen.
- `GET /health` aufrufen.
- Serilog-Ausgabe kontrollieren.
- Migrationen gegen die korrekte Startup-Assembly ausführen.
