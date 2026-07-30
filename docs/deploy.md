# Guia de Deploy — DebugMe

Este documento descreve como fazer deploy do backend (ASP.NET Core) e frontend (Angular) em produção.

---

## 🌐 Ambiente de produção atual

| Componente | Plataforma | URL |
|---|---|---|
| Backend (API) | Render (Docker) | `https://debugme-601s.onrender.com` |
| Frontend (SPA) | Render Static Site | *(URL do static site)* |
| Banco de dados | Render PostgreSQL | Gerenciado pelo Render |
| Swagger | Habilitado em produção | `https://debugme-601s.onrender.com/swagger` |
| Health check | `/health` | `https://debugme-601s.onrender.com/health` |

---

## 📋 Pré-requisitos

- Conta no [Render](https://render.com)
- Repositório Git conectado ao Render
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (para build local e migrations)
- [Node.js 20+](https://nodejs.org/) (para build do frontend)

---

## 🖥️ Backend — Deploy no Render (Docker + PostgreSQL)

### 1. Criar o Web Service no Render

1. No dashboard do Render, clique em **New → Web Service**
2. Conecte o repositório Git
3. Configure:
   - **Name**: `debugme` (ou o nome desejado)
   - **Root Directory**: *(deixe vazio — o Dockerfile está na raiz)*
   - **Environment**: `Docker`
   - **Dockerfile Path**: `src/DebugMeBackend/Dockerfile.render`
   - **Plan**: Free

### 2. Configurar variáveis de ambiente

No Render, vá em **Environment → Environment Variables** e adicione:

| Variável | Valor | Descrição |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` | Ambiente de execução |
| `ASPNETCORE_URLS` | `http://+:8080` | Porta que o container escuta |
| `DatabaseProvider` | `PostgreSql` | Provider do EF Core |
| `ConnectionStrings__DefaultConnection` | *(fornecido pelo Render)* | Connection string do PostgreSQL |
| `Jwt__Secret` | *(chave secreta de 32+ caracteres)* | Chave de assinatura JWT |
| `Jwt__Issuer` | `DebugMe` | Emissor do token |
| `Jwt__Audience` | `DebugMe` | Audiência do token |
| `Jwt__AccessTokenExpiryMinutes` | `60` | Validade do access token (minutos) |
| `Jwt__RefreshTokenExpiryDays` | `7` | Validade do refresh token (dias) |

> **Importante**: `Jwt__Secret` no `appsettings.json` local contém um placeholder (`CHANGE_ME_IN_PRODUCTION...`). Em produção, a variável de ambiente do Render **substitui** esse valor automaticamente (o ASP.NET Core faz merge de configuração, com variáveis de ambiente tendo precedência).

### 3. Criar o banco PostgreSQL

1. No Render, vá em **New → PostgreSQL**
2. Configure:
   - **Name**: `debugme-db`
   - **Plan**: Free
3. Após criar, copie a **Internal Database URL** — ela será usada como `ConnectionStrings__DefaultConnection` no Web Service
4. Vincule o banco ao Web Service: no serviço `debugme`, vá em **Environment** e adicione a variável com a URL copiada

### 4. Primeiro deploy

O Render faz deploy automaticamente ao detectar push na branch configurada (geralmente `main`). O `Dockerfile.render`:

1. **Build stage**: restaura pacotes, compila e publica em `/out`
2. **Runtime stage**: copia os binários e inicia com `dotnet DebugMeBackend.dll`

No startup, `Program.cs:114-118` executa `EnsureCreated()` — o schema do banco é criado a partir do modelo EF Core. Não é necessário rodar migrations manualmente.

### 5. Verificar health check

```bash
curl https://debugme-601s.onrender.com/health
```

Resposta esperada:
```json
{
  "status": "Healthy",
  "duration": "...",
  "timestamp": "...",
  "entries": {
    "database": {
      "status": "Healthy",
      "description": "...",
      "duration": "..."
    }
  }
}
```

### 6. Dockerfile (referência)

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY src/DebugMeBackend/*.csproj ./
RUN dotnet restore

COPY src/DebugMeBackend/ ./
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /out ./

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "DebugMeBackend.dll"]
```

---

## 🎨 Frontend — Deploy no Render Static Site

### 1. Build de produção

```bash
cd src/debugme-frontend
npm install
npm run build
```

O build gera os arquivos estáticos em `dist/debugme-frontend/browser/`.

### 2. Configurar o Static Site no Render

1. No dashboard do Render, clique em **New → Static Site**
2. Conecte o repositório Git
3. Configure:
   - **Name**: `debugme-frontend`
   - **Root Directory**: `src/debugme-frontend`
   - **Build Command**: `npm install && npm run build`
   - **Publish Directory**: `dist/debugme-frontend/browser`
   - **Plan**: Free

### 3. Configurar redirect SPA

No Render Static Site, adicione uma **Rewrite Rule** para que todas as rotas sirvam `index.html`:

- **Source**: `/*`
- **Destination**: `/index.html`
- **Action**: Rewrite

Isso é necessário porque o Angular é uma SPA — rotas como `/home` ou `/events/new` não correspondem a arquivos físicos no servidor e precisam ser resolvidas pelo router do Angular.

### 4. Environment de produção

O arquivo `src/debugme-frontend/src/environments/environment.ts` já está configurado para apontar para o backend em produção:

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://debugme-601s.onrender.com'
};
```

Se a URL do backend mudar, atualize este arquivo e faça o deploy novamente.

---

## 🔧 Configuração local (desenvolvimento)

### Backend

```bash
cd src/DebugMeBackend

# SQLite — zero configuração (padrão em dev)
dotnet run
# API em http://localhost:5165
```

Para usar MySQL local (Docker):

```bash
docker run --name debugme-mysql \
  -e MYSQL_ROOT_PASSWORD=debugme123 \
  -e MYSQL_DATABASE=DebugMeDb \
  -p 3306:3306 \
  -d mysql:8.0

# No appsettings.Development.json:
# "DatabaseProvider": "MySql"
# "ConnectionStrings:DefaultConnection": "Server=localhost;Port=3306;Database=DebugMeDb;User=root;Password=debugme123;"
```

### Frontend

```bash
cd src/debugme-frontend
npm install
npm start
# App em http://localhost:4200
```

O frontend em dev aponta para `http://localhost:5165` (`environment.development.ts`).

---

## 🔐 CORS

Atualmente o backend usa `AllowAnyOrigin` em `Program.cs:22-30`. Isso funciona para desenvolvimento e demonstração, mas em produção deve ser restrito:

```csharp
// Program.cs — em produção, substituir por:
policy.WithOrigins("https://debugme-frontend.onrender.com")
      .AllowAnyHeader()
      .AllowAnyMethod();
```

---

## ⚠️ Limitações do free tier

- **Cold start**: Após inatividade, o Render suspende o Web Service. A primeira requisição pode levar 30-60s.
- **Banco não persistente**: O PostgreSQL free tier tem armazenamento limitado e pode ser recriado. O `EnsureCreated()` no startup garante que o schema seja recriado automaticamente.
- **HTTPS**: O Render gerencia TLS como proxy reverso — a aplicação interna escuta em HTTP (porta 8080). `UseHttpsRedirection` está desabilitado em produção (`Program.cs:131-134`).

---

## 🏷️ Variáveis de ambiente — hierarquia

O ASP.NET Core resolve configuração na seguinte ordem (última tem precedência):

1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. Variáveis de ambiente
4. Segredos do Render

Por isso, as variáveis configuradas no dashboard do Render sobrescrevem os valores padrão do `appsettings.json`.

---

## 🐛 Troubleshooting

### O backend não inicia no Render

1. Verifique os logs no dashboard do Render (aba **Logs**)
2. Confirme que `ConnectionStrings__DefaultConnection` está configurada com a URL interna do PostgreSQL
3. Confirme que `Jwt__Secret` tem pelo menos 32 caracteres
4. Verifique se o Dockerfile está no caminho correto (`src/DebugMeBackend/Dockerfile.render`)

### O frontend carrega mas não conecta na API

1. Verifique se `environment.ts` aponta para a URL correta do backend
2. Confirme que o CORS no backend permite a origem do frontend
3. Verifique no DevTools (aba Network) se as requisições estão indo para a URL esperada

### Erro 401 Unauthorized no Swagger

1. Faça login via `POST /api/user/login` e copie o `token` da resposta
2. No Swagger UI, clique em **Authorize** (cadeado no topo)
3. Cole o token no formato: `Bearer {seu_token}`
4. Clique em **Authorize** e feche o modal
