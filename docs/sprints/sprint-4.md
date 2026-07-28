# 🚀 Sprint 04 — Versão 1.0: Produção e Polimento

## 📅 Período
4 semanas (28/07/2026 – 25/08/2026)

---

## 🎯 Objetivo

Finalizar a **versão 1.0 do DebugMe**, preparando o sistema para deploy em produção. Isso inclui a migração definitiva para MySQL, correção dos testes do frontend, criação da logo, preenchimento de telas vazias, e deploy da aplicação em ambiente cloud.

> **Pergunta-chave:** Esta sprint deve finalizar a versão 1.0 do software?

**Resposta:** Sim. Considerando que todas as funcionalidades centrais (CRUD de EventLog, autenticação JWT, frontend completo) já estão implementadas, os itens restantes são de infraestrutura (banco de produção, deploy) e polimento (testes, logo, UI). Uma sprint de 4 semanas é suficiente para concluir a v1.0.

---

## 📦 Entregáveis

- MySQL configurado como banco de produção (Docker)
- Migrations aplicadas no MySQL
- SQLite mantido apenas para desenvolvimento/testes
- Testes do frontend corrigidos e passando (mínimo 10 specs verdes)
- Logo criada e aplicada no sistema
- Tela de listagem de emoções funcional
- Deploy do backend em cloud (Render, Railway ou Azure)
- Deploy do frontend em cloud (Vercel, Netlify ou Render)
- Ambiente de produção funcional e acessível publicamente
- README atualizado com URLs de produção e instruções de deploy
- Documentação da Sprint 04

---

## 🧩 Cards da Sprint

---

### 🗄️ Infraestrutura — MySQL (🔴 Prioridade máxima)

- [ ] Instalar Docker Desktop (ou Podman) localmente
- [ ] Subir container MySQL 8.0:
  ```bash
  docker run --name debugme-mysql -e MYSQL_ROOT_PASSWORD=debugme123 -e MYSQL_DATABASE=debugme -p 3306:3306 -d mysql:8.0
  ```
- [ ] Instalar `Pomelo.EntityFrameworkCore.MySql` no projeto
  ```bash
  dotnet add src/DebugMeBackend package Pomelo.EntityFrameworkCore.MySql
  ```
- [ ] Configurar connection string MySQL no `appsettings.Development.json`
- [ ] Atualizar `Program.cs` para selecionar provider condicionalmente:
  - `Development` → SQLite (rápido, sem Docker)
  - `Production` → MySQL
- [ ] Remover connection string hardcoded — usar `appsettings.json`
- [ ] Gerar migrations no MySQL:
  ```bash
  dotnet ef migrations add InitialMySQL --project src/DebugMeBackend
  dotnet ef database update --project src/DebugMeBackend
  ```
- [ ] Validar persistência de dados (criar usuário, login, criar evento via Swagger)
- [ ] Documentar setup do MySQL no README

---

### 🧪 Testes do Frontend

- [ ] Diagnosticar configuração atual (builder vs vitest)
- [ ] Escolher abordagem e implementar:
  - **Opção A (recomendada):** Manter `@angular/build:unit-test`, adicionar `import { describe, it, expect } from 'vitest'` nos specs
  - **Opção B:** Trocar builder para vitest, criar `vitest.config.ts` com `globals: true`
- [ ] Corrigir `app.spec.ts` — teste de criação do componente raiz
- [ ] Corrigir `login.spec.ts` — teste do formulário de login
- [ ] Corrigir `home.spec.ts` — teste da tela home
- [ ] Corrigir `emotions.spec.ts` — teste da listagem de emoções
- [ ] Corrigir `event-logs.spec.ts` — teste da listagem de eventos
- [ ] Criar `auth.service.spec.ts` — teste do AuthService (login, logout, token)
- [ ] Criar `event-log.service.spec.ts` — teste do EventLogService (CRUD)
- [ ] Criar `event-create.spec.ts` — teste do formulário de criação de evento
- [ ] Criar `api.service.spec.ts` — teste do interceptor HTTP
- [ ] Meta: mínimo **10 specs passando, 0 falhas**

---

### 🎨 Design — Logo e Polimento

- [ ] Criar logo do DebugMe (SVG ou PNG):
  - Ícone: inseto/joaninha estilizada ou símbolo de "debug" (>) com coração
  - Texto: "DebugMe" em Nunito bold
  - Cores: paleta violeta/lavanda existente
- [ ] Aplicar logo no:
  - Favicon (`favicon.ico`)
  - Header/navbar de todas as telas
  - Tela de login e registro
- [ ] Adicionar footer com copyright e links
- [ ] Revisar responsividade das telas (mobile-first)
- [ ] Adicionar transições entre rotas (fade-in)

---

### 🖥️ Frontend — Telas Pendentes

- [ ] Implementar `emotions.ts` — listagem de emoções cadastradas:
  - Tabela/cards com nome da emoção
  - Contagem de eventos vinculados a cada emoção
  - Botão "Gerenciar" que redireciona para `/emotions/manage`
  - Loading e estado vazio
- [ ] Revisar e corrigir possíveis bugs visuais nas telas existentes
- [ ] Adicionar indicador de carregamento global (spinner no topo durante requisições)

---

### 🚀 Deploy

#### Backend

- [ ] Escolher plataforma de deploy:
  - **Recomendação:** Render (suporte nativo a .NET, free tier, deploy via git)
  - Alternativas: Railway, Azure App Service (free tier)
- [ ] Criar `Dockerfile` para o backend (multi-stage build)
- [ ] Configurar variáveis de ambiente de produção:
  - `Jwt__Secret` (chave secreta forte, NUNCA hardcodada)
  - `ConnectionStrings__DefaultConnection` (MySQL de produção)
  - `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Criar banco MySQL em produção (Render Managed MySQL, PlanetScale ou similar)
- [ ] Aplicar migrations no banco de produção
- [ ] Configurar CORS para permitir apenas o domínio do frontend de produção
- [ ] Validar health check em produção (`GET /health`)

#### Frontend

- [ ] Escolher plataforma de deploy:
  - **Recomendação:** Vercel ou Netlify (SPA estática, free tier, deploy via git)
- [ ] Atualizar `environment.production.ts` com a URL do backend em produção
- [ ] Gerar build de produção: `npm run build`
- [ ] Configurar redirecionamento SPA (todas as rotas → `index.html`)
- [ ] Validar fluxo completo em produção:
  - Acessar URL pública
  - Registrar usuário
  - Login
  - Criar evento
  - Listar eventos
  - Excluir evento

---

### 📚 Documentação

- [ ] Atualizar README com:
  - URLs de produção (frontend + backend)
  - Instruções de deploy
  - Setup local com Docker
  - Arquitetura atualizada (JWT, MySQL)
- [ ] Criar `docs/deploy.md` — guia completo de deploy
- [ ] Atualizar `docs/architecture.md` com JWT e MySQL
- [ ] Criar documentação da Sprint 04
- [ ] Criar changelog da v1.0

---

## 📊 Metas da Sprint (v1.0)

| Métrica | Meta |
|---|---|
| Testes backend | **47+ passed, 0 failed** (manter) |
| Testes frontend | **10+ specs passando** |
| Telas funcionais | **6** (login, register, home, criar evento, listar eventos, listar emoções) |
| Banco de dados | **MySQL em produção**, SQLite em dev |
| Deploy | **Frontend + Backend online e acessível** |
| Logo | **Criada e aplicada** |
| Documentação | **README + deploy.md + architecture.md + changelog** |

---

## ✅ Critérios de Aceitação da v1.0

- [ ] MySQL rodando em produção com migrations aplicadas
- [ ] Backend deployado e respondendo em URL pública
- [ ] Frontend deployado e acessível publicamente
- [ ] Fluxo completo funcional em produção: Registro → Login → Criar Evento → Listar → Excluir
- [ ] Testes backend: 47+ passando
- [ ] Testes frontend: 10+ specs passando
- [ ] Logo presente em todas as telas
- [ ] Tela de listagem de emoções funcional
- [ ] CORS configurado corretamente em produção
- [ ] Variáveis sensíveis (JWT secret, connection string) em variáveis de ambiente
- [ ] Documentação de deploy completa
- [ ] Changelog da v1.0 publicado

---

## ⚠️ Riscos

| Risco | Probabilidade | Impacto | Mitigação |
|---|---|---|---|
| Complexidade na migração SQLite → MySQL | Média | Alto | Fazer backup do SQLite, testar migrations em ambiente local primeiro |
| Custo de banco MySQL em produção | Baixa | Médio | Usar free tiers (Render MySQL grátis, PlanetScale hobby) |
| Cold start em free tiers | Alta | Baixo | Documentar que o primeiro acesso pode ser lento; aceitar como limitação da v1.0 |
| Configuração de CORS em produção | Média | Alto | Testar com Postman antes do frontend; configurar origens específicas |
| Vazamento de secrets no código | Baixa | Crítico | Revisar todo o código antes do deploy; usar `.gitignore` para arquivos de ambiente |
| Tempo insuficiente para deploy | Média | Alto | Priorizar deploy na semana 2; usar remainder para polimento |

---

## 🔄 Dependências entre Tarefas

```
MySQL local ──> Migrations ──> MySQL produção ──> Deploy backend
                                                      │
Frontend tests ──> Logo ──> Tela emoções ─────────────┤
                                                      │
                                              Deploy frontend ──> Validação completa
```

---

## 🗓️ Cronograma Semanal Sugerido

### Semana 1 (28/07 – 03/08): MySQL + Testes Frontend
- Docker + MySQL local
- Pomelo EF Core + connection strings
- Migrations no MySQL
- Corrigir configuração de testes frontend
- Fazer specs existentes passarem

### Semana 2 (04/08 – 10/08): Deploy Backend
- Dockerfile backend
- Deploy backend (Render/Railway)
- MySQL em produção
- Migrations em produção
- Health check em produção
- Criar specs de serviços (auth, event-log)

### Semana 3 (11/08 – 17/08): Deploy Frontend + Polimento
- Deploy frontend (Vercel/Netlify)
- Integração produção (CORS, environment)
- Logo + aplicação nas telas
- Tela de listagem de emoções
- Criar specs de componentes (event-create, api)

### Semana 4 (18/08 – 24/08): Validação + Documentação
- Validação completa do fluxo em produção
- Correção de bugs encontrados
- Documentação (README, deploy.md, changelog)
- Retrospective da Sprint 4
- Tag `v1.0.0`

---

## 🎯 Escopo Pós-v1.0 (visão inicial)

Itens que **não** farão parte da v1.0, mas são candidatos naturais para a v1.1:

- Filtros e busca de eventos (por emoção, data, intensidade)
- Edição de eventos existentes
- Dashboard com gráficos/estatísticas (emoções mais frequentes, tendências)
- Reset de senha (esqueci minha senha)
- Perfil do usuário (editar nome, email, trocar senha)
- Testes de integração e E2E (Playwright/Cypress)
- Pipeline CI/CD (GitHub Actions)
- Observabilidade (logs estruturados, métricas)
- Internacionalização (i18n — en/pt)
- Dark mode
- PWA (Progressive Web App)

---

## 📐 Stack da v1.0

| Camada | Tecnologia | Ambiente |
|---|---|---|
| Frontend | Angular 21 (standalone) | Vercel / Netlify |
| Backend | ASP.NET Core 9.0 | Render / Railway |
| Banco (dev) | SQLite | Local |
| Banco (prod) | MySQL 8.0 | Render MySQL / PlanetScale |
| Autenticação | JWT (access + refresh tokens) | Em memória (sem Redis) |
| Hash de senha | BCrypt.Net-Next | Backend |
| Testes backend | xUnit + Moq + FluentAssertions | CI local |
| Testes frontend | Vitest | CI local |
| Container | Docker (backend) | Build + deploy |

---

## 🧠 Observações

### Por que esta sprint finaliza a v1.0?

1. **Funcionalidades core estão prontas:** CRUD de EventLog, autenticação JWT, frontend com 5 telas. O que falta é infraestrutura (banco de produção, deploy) e polimento (testes, logo).

2. **MySQL é o último bloqueador técnico:** Sem MySQL, o sistema não escala para múltiplos usuários. Resolver isso + deploy = sistema pronto para uso real.

3. **O escopo pós-v1.0 são melhorias, não funcionalidades essenciais:** Filtros, dashboards, edição de eventos — tudo isso agrega valor mas não bloqueia o uso básico do sistema.

4. **Sprints de 4 semanas são suficientes:** 28 dias para MySQL + testes + logo + deploy é um escopo realista baseado no ritmo atual da equipe.

### Decisões conscientes para a v1.0:

- **Sem Redis para refresh tokens:** Tokens ficam em memória (reinicia com o servidor). Aceitável para v1.0.
- **Sem CI/CD:** Deploy manual via git push ou Docker. GitHub Actions fica para v1.1.
- **Sem E2E tests:** Cobertura de testes unitários no frontend + backend é suficiente para v1.0.
- **Free tiers:** Cold starts são aceitáveis. A v1.0 é uma prova de conceito funcional, não um produto de alta disponibilidade.

---

## 🏁 Definição de Pronto (Definition of Done) — v1.0

Uma história/feature está **pronta** quando:

1. Código implementado e revisado
2. Testes unitários passando (backend + frontend)
3. Funcionalidade validada em ambiente de produção
4. Documentação atualizada (se aplicável)
5. Sem bugs conhecidos que impeçam o uso

A **v1.0 está pronta** quando todos os critérios de aceitação acima forem atendidos e o sistema estiver acessível publicamente.
