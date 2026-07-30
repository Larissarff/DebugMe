# ADR 0003: Suporte a múltiplos bancos via EF Core

## Status
Aceita

## Contexto
O ambiente de desenvolvimento precisava ser **zero-config** — clonar o repositório e rodar `dotnet run` sem instalar Docker, MySQL ou PostgreSQL. Ao mesmo tempo, a produção no Render exigia um banco gerenciado (PostgreSQL), e havia a intenção de manter compatibilidade com MySQL para ambientes locais via Docker.

Fixar um único banco desde o início (ex.: só PostgreSQL) forçaria todo desenvolvedor a subir um container para testar localmente — fricção desnecessária para um projeto solo.

## Decisão
Abstrair o provider de banco via configuração no `appsettings.json`:

```json
{
  "DatabaseProvider": "Sqlite",       // "Sqlite" | "MySql" | "PostgreSql"
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=debugme.db"
  }
}
```

O `Program.cs:84-100` lê `DatabaseProvider` e configura o `AppDbContext` com o provider correspondente:

- `Sqlite` → `UseSqlite(connectionString)` — desenvolvimento, arquivo local `debugme.db`
- `MySql` → `UseMySql(connectionString, ServerVersion.AutoDetect(...))` — Pomelo.EntityFrameworkCore.MySql 9.0.0
- `PostgreSql` → `UseNpgsql(connectionString)` — Npgsql.EntityFrameworkCore.PostgreSQL 9.0.0

O `AppDbContextFactory` (usado pelo CLI do EF Core para migrations) replica a mesma lógica de seleção de provider, garantindo que `dotnet ef migrations add` funcione independente do banco ativo.

Na inicialização, `Program.cs:114-118` usa `EnsureCreated()` em vez de `Migrate()` — isso cria o schema a partir do modelo sem executar o histórico de migrations. A decisão foi pragmática: o Render não persiste o banco entre deploys gratuitos, e `EnsureCreated()` é suficiente para recriar o schema.

## Alternativas consideradas

| Alternativa | Por que foi descartada |
|---|---|
| Fixar SQLite para dev e produção | SQLite não é adequado para deploy em plataforma cloud com múltiplas instâncias (concorrência de escrita). O Render oferece PostgreSQL gerenciado, que é a escolha natural. |
| Usar apenas PostgreSQL com container local obrigatório | Fricção de onboarding — exige Docker instalado e configurado antes do primeiro `dotnet run`. |
| Camada de abstração própria (Repository pattern "puro" com múltiplas implementações por banco) | Over-engineering. O EF Core já abstrai o provider — criar `SqliteUserRepository`, `PostgreSqlUserRepository` etc. seria duplicar o que o ORM faz. |

## Consequências

**Positivas**
- `dotnet run` funciona imediatamente após clone — SQLite cria o `debugme.db` automaticamente com `EnsureCreated()`.
- Deploy no Render usa PostgreSQL sem alteração de código — basta mudar `DatabaseProvider` e a connection string via variáveis de ambiente.
- Migrations podem ser geradas com qualquer provider ativo (`dotnet ef migrations add`), mantendo o histórico versionado.

**Negativas / trade-offs aceitos**
- Nem todo recurso específico de um banco pode ser usado. Ex.: `AUTO_INCREMENT` do MySQL, `SERIAL` do PostgreSQL, índices full-text — usar qualquer um quebra a portabilidade entre providers.
- `EnsureCreated()` não versiona o schema como `Migrate()` faria — se o modelo mudar e o banco já existir, `EnsureCreated()` não altera tabelas existentes. Em produção real (não-free), seria necessário migrar para `Migrate()`.
- O `AppDbContextFactory` tem lógica de seleção de provider duplicada em relação ao `Program.cs` — a manutenção exige alterar nos dois lugares.

## Notas de implementação
- Seleção de provider: `src/DebugMeBackend/Program.cs:84-100`
- DbContextFactory: `src/DebugMeBackend/Data/AppDbContextFactory.cs:20-31`
- Providers: `Microsoft.EntityFrameworkCore.Sqlite` 9.0.0, `Pomelo.EntityFrameworkCore.MySql` 9.0.0, `Npgsql.EntityFrameworkCore.PostgreSQL` 9.0.0
- Inicialização: `Program.cs:114-118` (`EnsureCreated()`)
- Configuração local: `appsettings.json` → `DatabaseProvider` + `ConnectionStrings:DefaultConnection`
