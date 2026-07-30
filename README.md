# DebugMe 🧠🐞 — Mental Debugging as Software

> Trate pensamentos, emoções e comportamentos como um sistema — identifique bugs, analise stack traces e aplique refatorações na sua vida.

---

## 📌 Sobre o projeto

O **DebugMe** é uma aplicação web que aplica metáforas de engenharia de software ao desenvolvimento pessoal. Em vez de diários subjetivos, o usuário registra eventos emocionais como se estivesse depurando um sistema:

| Engenharia de Software | DebugMe |
|---|---|
| **Bug** | Reação emocional ou padrão negativo |
| **Log** | Registro de evento (data, intensidade, emoção) |
| **Stack trace** | Contexto e gatilhos do evento |
| **Debugging** | Análise de padrões ao longo do tempo |
| **Refatoração** | Mudança de comportamento ou pensamento |
| **Iteração** | Evolução contínua |

---

## 🎯 Problema

Pessoas enfrentam dificuldades emocionais e comportamentais, mas carecem de ferramentas estruturadas para **identificar padrões recorrentes**, **entender causas** e **medir evolução**. Soluções tradicionais (diários, terapia isolada) costumam ser subjetivas e difíceis de quantificar.

---

## 💡 Solução

O DebugMe oferece uma interface com:

- Registro cronológico de eventos emocionais (com intensidade de 1 a 10)
- Classificação por categorias de emoção (ansiedade, frustração, alegria, etc.)
- Dashboard pessoal com calendário de intensidade
- Associação usuário-evento blindada por autenticação JWT
- Metáfora de debugging que torna a autoanálise mais concreta e acionável

---

## ✅ Funcionalidades implementadas (MVP)

- [x] Cadastro e login de usuários (com hash BCrypt + JWT access/refresh tokens)
- [x] CRUD completo de emoções (criação, listagem, edição, exclusão)
- [x] CRUD completo de registros emocionais (event logs) vinculados ao usuário autenticado
- [x] Dashboard com calendário mensal e gradiente de intensidade
- [x] Listagem de emoções com contagem de eventos vinculados
- [x] Tema escuro e identidade visual própria
- [x] Animações (typewriter na home, transições de rota, cursor customizado)
- [x] Testes unitários backend (47 testes, 100% passando — xUnit + Moq + FluentAssertions)
- [x] Testes unitários frontend (Vitest)
- [x] Suporte a múltiplos bancos: SQLite (dev), PostgreSQL (produção), MySQL
- [x] Deploy no Render com Dockerfile + PostgreSQL
- [x] Health Check endpoint (`/health`)
- [x] Swagger em dev e produção (`/swagger`)

### Em backlog / próximos passos

- [ ] Dashboard com gráficos de padrões (frequência por emoção, séries temporais)
- [ ] Sugestões automáticas de "refatoração"
- [ ] Perfil de usuário editável
- [ ] Testes de integração e e2e
- [ ] PWA / mobile

---

## 🚀 Tecnologias

### Backend
| Tecnologia | Versão | Uso |
|---|---|---|
| C# / .NET | 9.0 | Linguagem e runtime |
| ASP.NET Core Web API | 9.0 | Framework REST |
| Entity Framework Core | 9.0 | ORM |
| BCrypt.Net-Next | 4.2.0 | Hash de senhas |
| JWT Bearer | 9.0.0 | Autenticação (access + refresh tokens) |
| Swagger (Swashbuckle) | 10.1.7 | Documentação da API |

### Banco de Dados
- **Desenvolvimento**: SQLite (zero configuração)
- **Produção**: PostgreSQL (Render) ou MySQL 8.0+ (Pomelo)

### Frontend
| Tecnologia | Versão | Uso |
|---|---|---|
| Angular (standalone) | 21.2 | Framework SPA |
| TypeScript | 5.9 | Linguagem |
| RxJS | 7.8 | Programação reativa |
| Vitest | 4.0 | Testes unitários |

### Testes
| Tecnologia | Uso |
|---|---|
| xUnit | Framework de testes backend |
| Moq | Mocking |
| FluentAssertions | Asserções fluentes |
| coverlet | Cobertura de código |
| Vitest + jsdom | Testes frontend |

### Infra
- Render (cloud deploy com Docker + PostgreSQL)
- Docker (containerização)

### Trello do projeto
https://trello.com/b/e6nq2Xog/debugme

---

## 🏗️ Arquitetura

```
[Angular SPA] ──HTTP REST──> [Controllers] ──> [Services] ──> [Repositories] ──> [EF Core] ──> [DB]
```

### Backend — arquitetura em camadas

```
src/DebugMeBackend/
├── Controllers/          # Endpoints REST (User, Emotion, EventLog)
├── Services/             # Regras de negócio e validação
├── Repositories/         # Acesso a dados (interfaces + implementações)
├── Entities/             # Modelos de domínio (User, Emotion, EventLog)
├── DTOs/                 # Objetos de transferência (request/response)
├── Data/                 # AppDbContext, DbContextFactory, scripts SQL
├── Migrations/           # Migrations EF Core
├── HealthChecks/         # Health check do banco
├── Program.cs            # Bootstrap, DI, middlewares, autenticação
└── appsettings.json      # Connection strings, JWT config
```

### DI Chain

```
UserController   → UserService   → IUserRepository   → UserRepository   → AppDbContext
EmotionController → EmotionService → IEmotionRepository → EmotionRepository → AppDbContext
EventLogController → EventLogService → IEventLogRepository + IUserRepository + IEmotionRepository → ...
```

### Frontend — estrutura modular

```
src/debugme-frontend/src/app/
├── core/
│   ├── guards/            # AuthGuard (proteção de rotas via localStorage)
│   ├── models/            # TypeScript interfaces (User, Emotion, EventLog)
│   └── services/          # ApiService (wrapper HTTP), Auth, Emotion, EventLog
├── features/
│   ├── login/             # Tela de login
│   ├── register/          # Tela de cadastro
│   ├── home/              # Dashboard com calendário de intensidade
│   ├── emotions/          # Listagem e gerenciamento de emoções
│   └── event-logs/        # Listagem e criação de registros emocionais
├── shared/components/     # Cursor customizado, theme toggle, toast
├── app.routes.ts          # Rotas da aplicação
├── app.config.ts          # Providers (router, http, animações)
└── app.ts                 # Componente raiz
```

### Rotas

| Rota | Componente | Auth |
|---|---|---|
| `/login` | Login | ❌ |
| `/register` | Register | ❌ |
| `/home` | Home (dashboard) | ✅ |
| `/emotions/manage` | EmotionManage | ✅ |
| `/events` | EventLogs | ✅ |
| `/events/new` | EventCreate | ✅ |

### Modelo de dados

```
User (1) ──< (N) EventLog (N) >── (1) Emotion
```

| Entidade | Campos principais |
|---|---|
| **User** | Id (Guid), Name, Email, PasswordHash, RefreshToken, CreatedAt |
| **Emotion** | Id (Guid), Name, Description, CreatedAt |
| **EventLog** | Id (Guid), UserId (FK), EmotionId (FK), Description, Intensity (1-10), EventDate, CreatedAt |

---

## 🔄 Metodologia

Desenvolvimento incremental com sprints de 4 semanas. Cada sprint entrega funcionalidades completas, testadas e integradas.

| Sprint | Período | Entregas |
|---|---|---|
| Sprint 1 | Mar–Abr 2026 | Estrutura inicial: backend .NET, EF Core, CRUD básico |
| Sprint 2 | Abr–Mai 2026 | Frontend Angular: login, cadastro, listagem de emoções |
| Sprint 3 | Mai–Jun 2026 | CRUD completo, autenticação JWT, dashboard, identidade visual |
| Sprint 4 | Jun–Jul 2026 | Deploy no Render, PostgreSQL, testes, documentação |

Documentação completa das sprints em [`docs/sprints/`](docs/sprints/).

---

## ⚡ Quick Start

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 20+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (opcional, para MySQL/PostgreSQL)

### Backend (desenvolvimento com SQLite)

```bash
cd src/DebugMeBackend
dotnet run
# API em http://localhost:5165
# Swagger em http://localhost:5165/swagger
# Health check em http://localhost:5165/health
```

### Frontend

```bash
cd src/debugme-frontend
npm install
npm start
# App em http://localhost:4200
```

### Testes

```bash
# Backend (47 testes em 3 serviços)
dotnet test tests/DebugMeBackend.Tests

# Frontend
cd src/debugme-frontend
npm test
```

### Banco de dados alternativo

Para usar MySQL ou PostgreSQL, configure o provider e a connection string:

```json
// appsettings.json
{
  "DatabaseProvider": "PostgreSql",    // ou "MySql" ou "Sqlite"
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=DebugMeDb;Username=postgres;Password=..."
  }
}
```

### MySQL local (Docker)

```bash
docker run --name debugme-mysql \
  -e MYSQL_ROOT_PASSWORD=debugme123 \
  -e MYSQL_DATABASE=DebugMeDb \
  -p 3306:3306 \
  -d mysql:8.0
```

---

## 📂 Documentação

| Documento | Descrição |
|---|---|
| [`AGENTS.md`](AGENTS.md) | Guia técnico completo para contribuidores e IAs |
| [`docs/architecture.md`](docs/architecture.md) | Arquitetura do sistema, decisões e princípios |
| [`docs/database/model.md`](docs/database/model.md) | Modelagem do banco de dados |
| [`docs/decisions/`](docs/decisions/) | ADRs — registro de decisões arquiteturais |
| [`docs/sprints/`](docs/sprints/) | Planejamento, reviews e retrospectivas de cada sprint |

---

## 🎨 Diferencial

O DebugMe une três disciplinas:

- **Engenharia de Software** — depuração, rastreamento, refatoração
- **Psicologia comportamental** — identificação de gatilhos e padrões
- **Análise de dados pessoais** — quantificação e visualização de progresso

O resultado é uma ferramenta que transforma a autoanálise em um processo estruturado, mensurável e acionável.

---

## 📈 Possíveis evoluções

- Integração com profissionais de saúde mental
- Análise preditiva de padrões emocionais (ML)
- Recomendações personalizadas de "refatoração"
- PWA / App mobile
- Compartilhamento anônimo de dados para pesquisa

---

## 📌 Status

🚧 **MVP concluído** 
— Deploy do backend em produção no Render: [debugme-601s.onrender.com](https://debugme-601s.onrender.com)
— Deploy do frontend em produção no Render: [debugme-frontend.onrender.com](https://debugme-frontend.onrender.com)

---

## 👩‍💻 Autora

**Larissa Ferreira**
Desenvolvedora de Software | C# | .NET | Engenharia de Software

---

## 🤝 Contribuição

Projeto pessoal — contribuições são bem-vindas. Leia [`AGENTS.md`](AGENTS.md) para orientações técnicas antes de abrir um PR.
