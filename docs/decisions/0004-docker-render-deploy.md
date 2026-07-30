# ADR 0004: Containerização com Docker e deploy no Render

## Status
Aceita

## Contexto
O projeto precisava de um ambiente de produção que fosse reproduzível e não dependesse de configuração manual de servidor. A autora é a única desenvolvedora — gerenciar uma VPS (atualizações de SO, firewall, HTTPS, reinicialização de processos) não era viável. O Render oferece um tier gratuito com PostgreSQL gerenciado e deploy a partir de Dockerfile, alinhado com a stack.

O backend é uma API ASP.NET Core que escuta em HTTP (porta 8080 no container). O Render atua como proxy reverso com HTTPS — a aplicação não precisa gerenciar certificados TLS.

## Decisão
**Dockerfile multi-stage** (`src/DebugMeBackend/Dockerfile.render`) + **deploy gerenciado no Render**:

1. **Build stage**: usa `mcr.microsoft.com/dotnet/sdk:9.0`, restaura pacotes, compila e publica em `/out`.
2. **Runtime stage**: usa `mcr.microsoft.com/dotnet/aspnet:9.0`, copia os binários, expõe porta 8080.
3. `ASPNETCORE_URLS=http://+:8080` — escuta em HTTP na porta que o Render espera.
4. `ASPNETCORE_ENVIRONMENT=Production` — ativa Swagger (`Program.cs:120` permite Swagger em dev e produção), mas desativa HTTPS redirection (`Program.cs:131-134` pula `UseHttpsRedirection()` em produção porque o Render já lida com TLS).
5. O Render injeta a connection string do PostgreSQL via variável de ambiente `ConnectionStrings__DefaultConnection`, que o ASP.NET Core resolve automaticamente.

O `render-start.sh` existe como entrypoint alternativo (tenta rodar migrações antes de iniciar), mas o Dockerfile atual usa `ENTRYPOINT ["dotnet", "DebugMeBackend.dll"]` diretamente.

## Alternativas consideradas

| Alternativa | Por que foi descartada |
|---|---|
| Deploy manual em VPS (DigitalOcean, EC2) | Controle total, mas exige gerenciar SO, certificados SSL, reinicialização de processo, logs. Custo de manutenção desproporcional para projeto solo. |
| Azure App Service / AWS Elastic Beanstalk | Plataformas gerenciadas, mas o tier gratuito é limitado ou inexistente. Curva de configuração maior que o Render. |
| Serverless (Azure Functions, AWS Lambda) | Cold start e modelo stateless não combinam bem com EF Core + migrations. A API tem estado (conexão com banco relacional) que se beneficia de um processo contínuo. |
| Deploy sem container (Render native build) | O Render suporta build nativo .NET, mas o Dockerfile garante que o ambiente de build é idêntico ao local — sem surpresas de versão do SDK. |

## Consequências

**Positivas**
- Deploy é um `git push` para a branch principal — o Render detecta, faz build do Dockerfile e sobe a nova imagem.
- Health check em `/health` (`Program.cs:138-161`) permite que o Render monitore a aplicação e reinicie se necessário.
- PostgreSQL gerenciado — backups, atualizações e disponibilidade são responsabilidade da plataforma.
- Swagger disponível em produção (`/swagger`) facilita teste manual da API sem Postman.

**Negativas / trade-offs aceitos**
- Dependência da plataforma Render — migrar para outro provider exigiria adaptar o Dockerfile e as variáveis de ambiente.
- Tier gratuito do Render desliga a aplicação após inatividade (cold start de ~30-60s na primeira requisição). Aceitável para um projeto portfólio, não para produção com SLA.
- `EnsureCreated()` em vez de `Migrate()` significa que o schema é recriado do zero a cada deploy que perde o banco. Em produção paga, seria necessário migrar para `Migrate()` com backups.

## Notas de implementação
- `src/DebugMeBackend/Dockerfile.render` — Dockerfile multi-stage
- `src/DebugMeBackend/render-start.sh` — script de inicialização alternativo (não usado pelo Dockerfile atual)
- `Program.cs:120` — Swagger habilitado em dev e produção
- `Program.cs:131-134` — `UseHttpsRedirection()` desabilitado em produção
- `Program.cs:138-161` — Health check endpoint `/health`
- Ambiente `ASPNETCORE_ENVIRONMENT=Production`, `ASPNETCORE_URLS=http://+:8080`
