# 🚀 Sprint 03 — Aplicação Funcional com Frontend e Segurança

## 📅 Período
36 dias (25/05/2026 - 30/06/2026)

---

## 🎯 Objetivo

Consolidar o DebugMe como uma **aplicação funcional e utilizável**, entregando o frontend Angular completo com as telas de login, criação e listagem de eventos, implementando segurança com BCrypt, migrando para MySQL em produção e estabelecendo a identidade visual do produto.

---

## 📦 Entregáveis

- Frontend Angular funcional (login + criação + listagem de eventos)
- Integração frontend ↔ backend completa
- Segurança com BCrypt (hash de senha com salt)
- Health check endpoint implementado (`GET /health`)
- Migração do banco para MySQL
- Identidade visual aplicada nas telas
- Testes expandidos (frontend + backend)
- Documentação da Sprint 03

---

## 🧩 Cards da Sprint

---

### 🗄️ Infraestrutura (Migração de Banco)

- [ ] Configurar MySQL localmente (Docker ou instalador)
- [ ] Instalar provider MySQL no EF Core (`Pomelo.EntityFrameworkCore.MySql`)
- [ ] Ajustar connection string no `appsettings.json` e `appsettings.Development.json`
- [ ] Executar migrations no MySQL
- [ ] Validar persistência de dados via API
- [ ] Remover dependência de SQLite (manter apenas para testes)
- [ ] Atualizar `Program.cs` para usar MySQL condicionalmente (Development/Production)

---

### 🧠 Segurança

- [ ] Instalar pacote BCrypt (`BCrypt.Net-Next`)
- [ ] Substituir SHA256 por BCrypt no `UserService.cs`:
  - `CreateAsync` → `BCrypt.HashPassword(password)`
  - `LoginAsync` → `BCrypt.Verify(password, user.PasswordHash)`
  - `ChangePasswordAsync` → `BCrypt.Verify` + `BCrypt.HashPassword`
- [ ] Atualizar testes do `UserService` para validar BCrypt
- [ ] Garantir que todos os 39+ testes continuem passando

---

### 🎨 Frontend (Angular) — Estrutura Inicial

#### Configuração

- [ ] Criar `src/app/core/services/api.service.ts` — serviço HTTP genérico
- [ ] Configurar `src/environments/environment.ts` e `environment.development.ts` com URL da API
- [ ] Adicionar `provideHttpClient()` no `app.config.ts`
- [ ] Configurar rotas no `app.routes.ts`:
  - `/login` → LoginComponent
  - `/home` → HomeComponent
  - `/events` → EventListComponent
  - `/events/new` → EventCreateComponent

#### Modelos

- [ ] Criar `src/app/core/models/user.model.ts`
- [ ] Criar `src/app/core/models/emotion.model.ts`
- [ ] Criar `src/app/core/models/event-log.model.ts`

#### Serviços

- [ ] Criar `src/app/core/services/auth.service.ts` — login, logout, token storage
- [ ] Criar `src/app/core/services/emotion.service.ts` — CRUD de emoções
- [ ] Criar `src/app/core/services/event-log.service.ts` — CRUD de eventos

---

### 🎨 Frontend (Angular) — Telas

#### Tela de Login

- [ ] Criar `LoginComponent` com formulário de email + senha
- [ ] Validação de campos (email válido, senha não vazia)
- [ ] Feedback visual de erro (credenciais inválidas)
- [ ] Redirecionar para `/home` após login bem-sucedido
- [ ] Proteger rotas com guard (`AuthGuard`)

#### Tela Home

- [ ] Criar `HomeComponent` com visão geral
- [ ] Exibir nome do usuário logado
- [ ] Botões de navegação: "Novo Evento", "Ver Eventos", "Sair"

#### Tela de Criação de Evento

- [ ] Criar `EventCreateComponent` com formulário:
  - Seleção de emoção (dropdown carregado da API)
  - Campo de intensidade (slider ou número 1-10)
  - Campo de descrição (textarea com limite de 500 caracteres)
  - Data do evento (date picker)
- [ ] Validação de campos obrigatórios
- [ ] Feedback visual de sucesso/erro
- [ ] Redirecionar para listagem após criação

#### Tela de Listagem de Eventos

- [ ] Criar `EventListComponent` com tabela/cards
- [ ] Exibir: emoção, intensidade, descrição (truncada), data
- [ ] Botão de excluir evento com confirmação
- [ ] Indicador de loading enquanto carrega
- [ ] Mensagem "Nenhum evento encontrado" quando vazio

---

### 🎨 Design do Produto

- [ ] Definir paleta de cores oficial (tema terapêutico — tons suaves de azul, verde, lavanda)
- [ ] Criar identidade visual (tema terapêutico)
- [ ] Definir tipografia (Inter, Nunito ou similar)
- [ ] Criar logo inicial
- [ ] Aplicar estilo nas telas:
  - CSS global em `styles.css`
  - Componentes compartilhados em `shared/components/`

---

### 🧪 Testes

#### Backend

- [ ] Atualizar `UserServiceTests` para validar BCrypt (hash e verify)
- [ ] Criar testes para `HealthController` / health check endpoint
- [ ] Garantir 39+ testes passando

#### Frontend

- [ ] Configurar Jasmine/Karma para testes Angular
- [ ] Testar `AuthService` (login, logout, token)
- [ ] Testar `EventLogService` (CRUD via HTTP mocking)
- [ ] Testar `LoginComponent` (validação de formulário, submissão)

---

### 🔗 Integração

- [ ] Conectar frontend ao backend (URL da API nos environments)
- [ ] Validar fluxo completo:
  - Login → Home → Criar evento → Listar eventos → Excluir evento
- [ ] Tratar erros de rede no frontend (timeout, 500, 404)
- [ ] Adicionar interceptors para logging de requisições

---

### 📚 Documentação

- [ ] Atualizar README com instruções de execução (frontend + backend)
- [ ] Documentar endpoints da API (User, Emotion, EventLog, Health)
- [ ] Documentar arquitetura atualizada (se necessário)
- [ ] Criar documentação da Sprint 03

---

## 📐 Modelagem (Atualizada)

### Entidades

| Entidade | Descrição |
|----------|-----------|
| `User` | Usuário do sistema (com hash BCrypt) |
| `Emotion` | Categoria emocional (ex: alegria, tristeza) |
| `EventLog` | Registro de evento emocional do usuário |

### Relacionamentos

```
User (1) ──── (N) EventLog
Emotion (1) ── (N) EventLog
```

### Endpoints da API

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/health` | Health check da API |
| POST | `/api/user/create` | Criar usuário |
| POST | `/api/user/login` | Login (retorna dados do usuário) |
| GET | `/api/user/all` | Listar usuários |
| GET | `/api/user/id/{id}` | Buscar usuário por ID |
| PUT | `/api/user/update/{id}` | Atualizar usuário |
| DELETE | `/api/user/delete/{id}` | Deletar usuário |
| POST | `/api/emotion/create` | Criar emoção |
| GET | `/api/emotion/all` | Listar emoções |
| GET | `/api/emotion/id/{id}` | Buscar emoção por ID |
| GET | `/api/emotion/name/{name}` | Buscar emoção por nome |
| PUT | `/api/emotion/update/{id}` | Atualizar emoção |
| DELETE | `/api/emotion/delete/{id}` | Deletar emoção |
| POST | `/api/eventlog/create` | Criar evento |
| GET | `/api/eventlog/all` | Listar eventos |
| GET | `/api/eventlog/id/{id}` | Buscar evento por ID |
| PUT | `/api/eventlog/update/{id}` | Atualizar evento |
| DELETE | `/api/eventlog/delete/{id}` | Deletar evento |

---

## ✅ Critérios de Aceitação

- [ ] Frontend Angular rodando e consumindo a API
- [ ] Tela de login funcional (autenticação com BCrypt)
- [ ] Tela de criação de evento funcional
- [ ] Tela de listagem de eventos funcional
- [ ] Fluxo completo: Login → Criar evento → Listar → Excluir
- [ ] Health check endpoint respondendo (`GET /health`)
- [ ] MySQL configurado e migrations aplicadas
- [ ] Identidade visual aplicada nas telas
- [ ] Testes backend: 39+ passando
- [ ] Testes frontend: cobertura inicial dos serviços e componentes principais

---

## 📊 Métricas de Sucesso

| Métrica | Meta |
|---------|------|
| Testes unitários (backend) | **39+ passed, 0 failed** |
| Testes unitários (frontend) | **Mínimo 10 testes** |
| Cobertura de telas | **3 telas funcionais** (login, criar evento, listar eventos) |
| Fluxo completo validado | **Login → CRUD de eventos** |
| Banco de dados | **MySQL em produção, SQLite em desenvolvimento** |
| Segurança | **BCrypt implementado, SHA256 removido** |
| Documentação | **README + endpoints + sprint documentados** |

---

## ⚠️ Riscos

| Risco | Mitigação |
|-------|-----------|
| Complexidade na migração SQLite → MySQL | Manter SQLite como fallback em desenvolvimento |
| Curva de aprendizado do Angular | Usar Angular CLI e componentes standalone |
| Tempo elevado na definição de design | Priorizar funcionalidade, design evolui incrementalmente |
| Integração frontend/backend com CORS | Configurar CORS no `Program.cs` desde o início |
| Quebra de testes existentes ao alterar hash | Escrever testes específicos para BCrypt antes da migração |

---

## 🔄 Próxima Sprint (Sprint 04 — visão inicial)

- Autenticação com JWT (tokens de acesso)
- Filtros e busca de eventos
- Edição de eventos
- Melhorias de UX/UI
- Deploy do projeto (Render, Railway ou Azure)
- Pipeline CI/CD

---

## 🧠 Observações

Esta sprint é a **mais crítica do projeto até agora**, pois marca a transição de um backend funcional para uma **aplicação completa e utilizável pelo usuário final**.

### Prioridades claras:

1. **BCrypt primeiro** — a segurança é base para o login
2. **Frontend em paralelo** — após BCrypt, desenvolver telas
3. **MySQL após frontend funcional** — a migração de banco não deve bloquear o desenvolvimento das telas
4. **Design por último** — identidade visual aplicada após telas funcionarem

### Dependências entre tarefas:

```
BCrypt ──> Login ──> Home ──> Criar Evento ──> Listar Eventos
                                                │
MySQL ──────────────────────────────────────────┘ (persistência real)
                                                │
Health Check ──────────────────────────────────┘ (monitoramento)
```

### Stack definida para a sprint:

| Camada | Tecnologia |
|--------|-----------|
| Frontend | Angular 21 (standalone components) |
| Backend | ASP.NET Core 9.0 |
| Banco (dev) | SQLite |
| Banco (prod) | MySQL |
| Hash de senha | BCrypt (`BCrypt.Net-Next`) |
| Health Check | Middleware nativo ASP.NET Core |
