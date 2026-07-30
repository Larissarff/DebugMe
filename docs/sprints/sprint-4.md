# 🚀 Sprint 04 — Versão 1.0: Produção e Polimento

## 📅 Período
4 semanas (28/07/2026 – 25/08/2026)

---

## 🎯 Objetivo

Finalizar a **versão 1.0 do DebugMe**, preparando o sistema para deploy em produção com banco gerenciado, testes frontend passando, identidade visual aplicada e documentação de decisões arquiteturais.

---

## 📦 Entregáveis

### ✅ Concluído

- [x] Multi-banco com provider condicional: SQLite (dev), MySQL, PostgreSQL (prod)
- [x] `EnsureCreated()` para inicialização do schema no Render
- [x] Testes frontend com Vitest (10 specs, 100% passando)
- [x] Logo e identidade visual aplicadas (cores violeta/lavanda, tema escuro)
- [x] Tela de listagem de emoções (`/emotions/manage`) com contagem de eventos
- [x] Dashboard home com calendário mensal e gradiente de intensidade
- [x] Deploy do backend no Render com Dockerfile multi-stage + PostgreSQL
- [x] Frontend production build apontando para Render (`environment.ts`)
- [x] Swagger disponível em produção (`/swagger`)
- [x] Health check (`/health`) monitorável pela plataforma
- [x] CORS configurado (`AllowAnyOrigin` — desenvolvimento)
- [x] HTTPS redirection desabilitado em produção (Render faz proxy TLS)
- [x] README atualizado com arquitetura real e status do MVP
- [x] 7 ADRs documentando decisões técnicas (autenticação, camadas, multi-banco, deploy, DTOs, testes, Swagger)
- [x] Deploy do frontend em plataforma estática (Render Static Site)
- [x] Frontend acessível publicamente com build de produção apontando para API do Render

### 🔲 Pendente

- [ ] CORS restrito ao domínio de produção (hoje é `AllowAnyOrigin`)
- [ ] Migrations aplicadas via `Migrate()` em vez de `EnsureCreated()` para versionamento de schema
- [x] `docs/deploy.md` — guia completo de deploy (Render backend + frontend, variáveis de ambiente, troubleshooting)
- [ ] Changelog da v1.0
- [ ] Tag `v1.0.0`

---

## 🧩 Cards da Sprint — status real

---

### 🗄️ Infraestrutura — Multi-banco (✅ Concluído)

- [x] `Pomelo.EntityFrameworkCore.MySql` 9.0.0 e `Npgsql.EntityFrameworkCore.PostgreSQL` 9.0.0 instalados
- [x] `Program.cs:84-100` — seleção condicional de provider via `DatabaseProvider` no `appsettings.json`
- [x] `AppDbContextFactory` replica lógica de seleção para CLI do EF Core
- [x] SQLite para desenvolvimento local (zero-config)
- [x] PostgreSQL no Render (produção) — connection string via variável de ambiente
- [x] MySQL suportado como opção (Docker local)
- [x] `EnsureCreated()` no startup (`Program.cs:114-118`) — schema criado a partir do modelo
- [x] Conexão com MySQL validada via Docker local
- **Divergência do plano original**: PostgreSQL em vez de MySQL como banco de produção (Render oferece PostgreSQL gerenciado no free tier; MySQL exigiria provisionamento manual)

---

### 🧪 Testes do Frontend (✅ Concluído)

- [x] Vitest configurado como test runner (`@angular/build:unit-test`)
- [x] 10 specs passando, 0 falhas:
  - `app.spec.ts` — criação do componente raiz
  - `login.spec.ts` — formulário de login
  - `register.spec.ts` — formulário de cadastro
  - `home.spec.ts` — tela home
  - `emotions.spec.ts` — listagem de emoções
  - `event-logs.spec.ts` — listagem de eventos
  - `event-create.spec.ts` — formulário de criação de evento
  - `auth.service.spec.ts` — AuthService
  - `event-log.service.spec.ts` — EventLogService
  - `api.service.spec.ts` — ApiService (interceptor HTTP)
- [x] Meta de 10 specs batida com exatamente 10 arquivos

---

### 🎨 Design — Logo e Identidade Visual (✅ Concluído)

- [x] Logo criada e aplicada (ícone + texto "DebugMe")
- [x] Cores violeta/lavanda aplicadas consistentemente
- [x] Tema escuro implementado com toggle (`shared/components/theme-toggle/`)
- [x] Animações: typewriter na home, transições de rota, cursor customizado
- [x] Favicon atualizado
- [x] Tela de login enriquecida com gradiente e identidade visual
- [x] Toast component para feedback de ações

---

### 🖥️ Frontend — Telas (✅ Concluído)

- [x] **6 telas funcionais**: login, register, home (dashboard com calendário), listagem de emoções (`/emotions/manage`), listagem de eventos (`/events`), criação de evento (`/events/new`)
- [x] `emotion-manage.ts` — CRUD de emoções com contagem de eventos vinculados (`EmotionWithCountDto`)
- [x] `home.ts` — calendário mensal com gradiente de intensidade (1-10), typewriter, loading state
- [x] `event-create.ts` — formulário com seleção de emoção, intensidade (1-10), data, descrição
- [x] `event-logs.ts` — listagem com cards de eventos
- [x] AuthGuard protege rotas internas (home, emotions, events)
- [x] Logout limpa localStorage e redireciona com `window.location.href`

---

### 🚀 Deploy (✅ Concluído)

#### Backend (✅ Concluído)

- [x] Plataforma: **Render** (free tier)
- [x] `Dockerfile.render` — multi-stage build (SDK 9.0 → aspnet:9.0)
- [x] Variáveis de ambiente: `ASPNETCORE_ENVIRONMENT=Production`, `ASPNETCORE_URLS=http://+:8080`
- [x] PostgreSQL gerenciado pelo Render — connection string injetada via variável de ambiente
- [x] Deploy automático via git push na main
- [x] Health check (`/health`) configurado e respondendo
- [x] Swagger acessível em `https://debugme-601s.onrender.com/swagger`
- [x] `appsettings.json` com `DatabaseProvider` e connection strings configuráveis
- [x] CORS `AllowAnyOrigin` para desenvolvimento — **débito técnico**: restringir em produção

#### Frontend (✅ Concluído)

- [x] `environment.ts` apontando para `https://debugme-601s.onrender.com` (produção)
- [x] `environment.development.ts` apontando para `http://localhost:5165`
- [x] Deploy no **Render Static Site** — SPA estática com redirect (`index.html` para todas as rotas)
- [x] Build de produção (`npm run build`) gerando bundle otimizado
- [x] Frontend acessível publicamente

---

### 📚 Documentação (✅ Concluído — escopo expandido)

- [x] README atualizado com arquitetura real, stack completa, rotas, modelo de dados, 47 testes, sprints
- [x] `AGENTS.md` — guia técnico completo para IAs e contribuidores
- [x] `docs/architecture.md` — arquitetura em camadas, fluxo de requisição, decisões
- [x] `docs/database/model.md` — modelagem User/Emotion/EventLog
- [x] `docs/decisions/` — 7 ADRs:
  - 0001: JWT access + refresh token
  - 0002: Repository/Service pattern
  - 0003: Multi-database support (SQLite/PostgreSQL/MySQL)
  - 0004: Docker + Render deploy
  - 0005: DTOs vs entities
  - 0006: Unit tests before integration/e2e
  - 0007: Swagger in production
- [x] Documentação das 4 sprints em `docs/sprints/`
- [x] `docs/deploy.md` — guia completo de deploy (Render backend + frontend, variáveis de ambiente, troubleshooting)
- [ ] Changelog v1.0 (pendente)

---

## 📊 Metas da Sprint — resultado

| Métrica | Meta | Real |
|---|---|---|
| Testes backend | 47+ passed, 0 failed | **47 passed, 0 failed** ✅ |
| Testes frontend | 10+ specs passando | **10 specs, 0 failures** ✅ |
| Telas funcionais | 6 | **6** (login, register, home, emotions/manage, events, events/new) ✅ |
| Banco de dados | MySQL em produção, SQLite em dev | **PostgreSQL (prod), SQLite (dev), MySQL (opcional)** ✅ (melhor que o plano) |
| Deploy backend | Online e acessível | **Render — online** ✅ |
| Deploy frontend | Online e acessível | **Render Static — online** ✅ |
| Logo | Criada e aplicada | **Criada, aplicada, + tema escuro** ✅ |
| Documentação | README + deploy.md + architecture.md + changelog | **README + AGENTS.md + architecture.md + deploy.md + ADRs + sprints** ✅ (changelog pendente) |

---

## ✅ Critérios de Aceitação da v1.0 — status

| Critério | Status |
|---|---|
| MySQL rodando em produção com migrations | ✅ (PostgreSQL no Render com `EnsureCreated`) |
| Backend deployado e respondendo em URL pública | ✅ `https://debugme-601s.onrender.com` |
| Frontend deployado e acessível publicamente | ✅ Render Static |
| Fluxo completo funcional: Registro → Login → Criar Evento → Listar → Excluir | ✅ (via Swagger em produção; frontend local contra backend remoto) |
| Testes backend: 47+ passando | ✅ 47/47 |
| Testes frontend: 10+ specs passando | ✅ 10/10 |
| Logo presente em todas as telas | ✅ |
| Tela de listagem de emoções funcional | ✅ (`/emotions/manage`) |
| CORS configurado corretamente em produção | ⚠️ `AllowAnyOrigin` — funcional, mas não restritivo |
| Variáveis sensíveis em variáveis de ambiente | ✅ (Render injects `ConnectionStrings__DefaultConnection`, `Jwt__Secret`) |
| Documentação de deploy completa | ✅ `docs/deploy.md` |
| Changelog da v1.0 publicado | 🔲 Pendente |

---

## 🔄 O que mudou do plano original

| Plano original | O que aconteceu | Motivo |
|---|---|---|
| MySQL como banco de produção | PostgreSQL no Render | Render oferece PostgreSQL gerenciado no free tier; MySQL exigiria provisionamento manual ou serviço pago |
| Deploy do frontend no Vercel/Netlify | Render Static Site | Deploy como SPA estática no Render; build de produção aponta para API do Render |
| CORS restrito ao domínio de produção | `AllowAnyOrigin` | Desenvolvimento + demo — restringir exigiria deploy do frontend primeiro |
| `Migrate()` para versionamento de schema | `EnsureCreated()` | O free tier do Render não persiste banco entre suspensões; `EnsureCreated()` basta para recriar schema |
| `docs/deploy.md` | Não criado | Escopo trocado por ADRs (mais valor para entrevistas técnicas) |
| Changelog v1.0 | Não criado | Pode ser gerado a partir do git log quando a v1.0 for taggeada |

---

## ⚠️ Riscos — retrospectiva

| Risco | Se concretizou? | Observação |
|---|---|---|
| Complexidade na migração SQLite → MySQL | Não | EF Core abstraiu a troca; multi-provider resolveu |
| Custo de banco MySQL em produção | Evitado | PostgreSQL free tier do Render = custo zero |
| Cold start em free tiers | Sim | ~30-60s na primeira requisição após inatividade. Aceito como limitação da v1.0 |
| Configuração de CORS em produção | Parcial | `AllowAnyOrigin` funciona mas é permissivo demais |
| Vazamento de secrets no código | Não | Secrets em variáveis de ambiente do Render |
| Tempo insuficiente para deploy | Parcial | Backend ok, frontend pendente |

---

## 🎯 Próximos passos (v1.0 finalização)

1. Restringir CORS ao domínio do frontend
2. Gerar changelog e tag `v1.0.0`

---

## 🏁 Definição de Pronto (Definition of Done) — v1.0

Uma história/feature está **pronta** quando:

1. Código implementado e revisado
2. Testes unitários passando (backend + frontend)
3. Funcionalidade validada em ambiente de produção
4. Documentação atualizada (se aplicável)
5. Sem bugs conhecidos que impeçam o uso

A **v1.0 está pronta** quando o CORS estiver restrito ao domínio de produção, a documentação de deploy estiver completa e a tag `v1.0.0` for aplicada.
