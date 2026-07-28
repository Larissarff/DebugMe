# 🔄 Sprint Retrospective — Sprint 03
**Projeto:** DebugMe
**Período:** 25/05/2026 – 30/06/2026 (36 dias)
**Data da Retrospectiva:** 28/07/2026

---

## 🎯 Objetivo da Retrospectiva

Analisar o desempenho da Sprint 03 para identificar:
- O que funcionou bem
- O que pode melhorar
- Ações práticas para a Sprint 04

---

## 📊 Visão Geral da Sprint

A Sprint 03 foi a sprint mais longa e ambiciosa do projeto até agora (36 dias), com o objetivo de transformar o DebugMe em uma **aplicação completa e utilizável pelo usuário final**. O escopo abrangeu 6 grandes frentes: banco de dados, segurança, frontend, design, testes e documentação.

**Resultado:** A sprint foi **majoritariamente bem-sucedida**, com avanços significativos em segurança, frontend e autenticação. Porém, a migração para MySQL e a cobertura de testes do frontend ficaram pendentes.

---

## 📦 Entregáveis da Sprint 3 — Status Real

| Entregável | Status | Evidência |
|---|---|---|
| Frontend Angular funcional (login + criação + listagem de eventos) | ✅ Concluído | 7 componentes (login, register, home, emotions, emotion-manage, event-logs, event-create) |
| Integração frontend ↔ backend completa | ✅ Concluído | ApiService com interceptor Bearer token, todos os fluxos CRUD funcionando |
| Segurança com BCrypt | ✅ Concluído | BCrypt.Net-Next 4.2.0 substituiu SHA256 |
| Health check endpoint (`GET /health`) | ✅ Concluído | DatabaseHealthCheck em `/health` |
| Migração do banco para MySQL | ❌ Não concluído | SQLite continua sendo o único banco; provider MySQL não instalado |
| Identidade visual aplicada nas telas | ✅ Concluído | Tema terapêutico violeta/lavanda, fontes Nunito + Inter, animações CSS |
| Testes expandidos (frontend + backend) | 🟡 Parcial | Backend: 47 testes passando. Frontend: 5 specs criados mas quebrados (vitest não configurado) |
| Documentação da Sprint 03 | ✅ Concluído | sprint-3.md e sprint-daily-context-2026-06-08.md |
| JWT Authentication (escopo extra) | ✅ Concluído | Access + refresh tokens implementados no backend e frontend |

---

## 🧩 Cards da Sprint 3 — Status Detalhado

### 🗄️ Infraestrutura (Migração de Banco)

| Card | Status |
|---|---|
| Configurar MySQL localmente | ❌ Não feito |
| Instalar provider MySQL no EF Core | ❌ Não feito |
| Ajustar connection string | ❌ Não feito |
| Executar migrations no MySQL | ❌ Não feito |
| Validar persistência de dados via API | ❌ Não feito |
| Remover dependência de SQLite | ❌ Não feito |
| Atualizar `Program.cs` para MySQL condicional | ❌ Não feito |

**Análise:** A migração para MySQL foi o maior bloqueio da sprint. Nenhum dos 7 cards desta frente foi concluído. O SQLite continua sendo o único banco, com a connection string hardcoded em `Program.cs`.

---

### 🧠 Segurança

| Card | Status |
|---|---|
| Instalar pacote BCrypt | ✅ BCrypt.Net-Next 4.2.0 |
| Substituir SHA256 por BCrypt no `UserService.cs` | ✅ HashPassword + Verify |
| Atualizar testes do `UserService` | ✅ 47 testes passando |
| Garantir 39+ testes passando | ✅ 47 passed, 0 failed |

**Análise:** Segurança 100% concluída. O BCrypt foi implementado corretamente com salt automático. Além disso, o escopo foi **expandido** com JWT (access tokens + refresh tokens), algo originalmente planejado apenas para a Sprint 4.

---

### 🎨 Frontend (Angular) — Estrutura Inicial

#### Configuração

| Card | Status |
|---|---|
| Criar `api.service.ts` | ✅ Com interceptor Bearer token |
| Configurar environments | ✅ `localhost:5165` em ambos |
| Adicionar `provideHttpClient()` | ✅ Em `app.config.ts` |
| Configurar rotas (`/login`, `/home`, `/events`, `/events/new`) | ✅ + `/register`, `/emotions`, `/emotions/manage` |

#### Modelos

| Card | Status |
|---|---|
| `user.model.ts` | ✅ Com TokenResponse, LoginRequest, CreateUserRequest, RefreshToken |
| `emotion.model.ts` | ✅ |
| `event-log.model.ts` | ✅ |

#### Serviços

| Card | Status |
|---|---|
| `auth.service.ts` | ✅ Login, register, logout, token management, isLoggedIn |
| `emotion.service.ts` | ✅ CRUD completo |
| `event-log.service.ts` | ✅ CRUD completo |

**Análise:** A estrutura do frontend foi entregue **acima do planejado**. Além dos modelos e serviços previstos, foram adicionados suporte a refresh token e tela de registro de usuário.

---

### 🎨 Frontend (Angular) — Telas

| Card | Status |
|---|---|
| **Tela de Login** — formulário, validação, feedback de erro, redirecionamento | ✅ |
| **AuthGuard** — proteção de rotas | ✅ |
| **Tela Home** — visão geral, nome do usuário, navegação, logout | ✅ |
| **Tela de Criação de Evento** — dropdown de emoção, slider 1-10, descrição, date picker, validação | ✅ |
| **Tela de Listagem de Eventos** — cards com emoção/intensidade/descrição/data, exclusão com confirmação, loading, mensagem "vazio" | ✅ |
| **Tela de Registro** (escopo extra) | ✅ |

**Análise:** Todas as telas previstas foram entregues, e o escopo foi expandido com tela de registro e gerenciamento de emoções (`emotion-manage`). O fluxo completo Login → Home → Criar Evento → Listar Eventos → Excluir funciona.

---

### 🎨 Design do Produto

| Card | Status |
|---|---|
| Definir paleta de cores oficial | ✅ Violeta/lavanda (#faf5ff, #fdf4ff, #2d1b4e) |
| Criar identidade visual | ✅ Tema terapêutico com gradientes suaves |
| Definir tipografia | ✅ Nunito + Inter |
| Criar logo inicial | ❌ Não feito |
| Aplicar estilo nas telas | ✅ CSS global + animações (fadeInUp, pulseGlow, float) |

**Análise:** A identidade visual foi aplicada com qualidade. O tema violeta/lavanda é consistente e as animações CSS dão polimento. A logo não foi criada.

---

### 🧪 Testes

| Card | Status |
|---|---|
| Atualizar `UserServiceTests` para BCrypt | ✅ |
| Criar testes para health check | ✅ Integrado aos 47 testes |
| Garantir 39+ testes backend | ✅ **47 passed, 0 failed** |
| Configurar testes frontend | ❌ 5 specs criados mas quebrados |
| Testar `AuthService` | ❌ |
| Testar `EventLogService` | ❌ |
| Testar `LoginComponent` | ❌ |

**Análise:** O backend está com 47 testes passando (8 acima da meta de 39). O frontend tem 5 arquivos `.spec.ts` criados (app, login, home, emotions, event-logs), mas todos falham com `ReferenceError: describe is not defined` — o vitest não está configurado corretamente. O Angular 21 usa `@angular/build:unit-test` como builder, mas os specs usam APIs do vitest sem import explícito e sem `vitest.config.ts`.

---

### 🔗 Integração

| Card | Status |
|---|---|
| Conectar frontend ao backend | ✅ |
| Validar fluxo completo (Login → Home → Criar → Listar → Excluir) | ✅ |
| Tratar erros de rede | ✅ Timeout 30s, mensagens de erro |
| Adicionar interceptors | ✅ Bearer token interceptor |

---

### 📚 Documentação

| Card | Status |
|---|---|
| Atualizar README | ✅ |
| Documentar endpoints da API | ✅ (no sprint-3.md) |
| Documentar arquitetura | ✅ (architecture.md mantido) |
| Criar documentação da Sprint 03 | ✅ sprint-3.md + sprint-daily-context-2026-06-08.md |

---

## 📊 Métricas da Sprint

| Métrica | Meta | Realizado | Status |
|---|---|---|---|
| Testes backend | 39+ | **47** | ✅ Superado |
| Testes frontend | 10+ | **0 (5 quebrados)** | ❌ |
| Telas funcionais | 3 (login, criar, listar) | **5** (login, register, home, criar, listar) | ✅ Superado |
| Fluxo completo | Login → CRUD eventos | ✅ Funcional | ✅ |
| Banco de dados | MySQL prod / SQLite dev | SQLite apenas | ❌ |
| Segurança | BCrypt | BCrypt + JWT | ✅ Superado |
| Documentação | README + endpoints + sprint | ✅ | ✅ |

---

## 🟢 O que funcionou bem (Keep)

### ✅ Segurança entregue com folga
BCrypt implementado + JWT com refresh tokens (escopo originalmente da Sprint 4). O sistema agora tem autenticação moderna com tokens de acesso e renovação.

**Impacto:** Base de segurança robusta para produção.

---

### ✅ Frontend funcional e com escopo expandido
5 telas entregues (vs 3 planejadas), incluindo registro de usuário e gerenciamento de emoções. O fluxo completo funciona ponta a ponta.

**Impacto:** A aplicação é utilizável pelo usuário final.

---

### ✅ Identidade visual coesa
Tema terapêutico violeta/lavanda aplicado consistentemente, com tipografia Nunito + Inter e animações CSS que dão polimento profissional.

**Impacto:** Experiência do usuário agradável e identidade visual definida.

---

### ✅ Backend sólido e testado
47 testes unitários, 0 falhas. Arquitetura em camadas mantida. JWT integrado aos controllers existentes.

**Impacto:** Confiabilidade do backend para evolução futura.

---

### ✅ Escopo bem priorizado
BCrypt → Frontend → JWT → Design. A ordem de dependências do sprint-3.md foi respeitada, e as tarefas bloqueantes foram resolvidas primeiro.

**Impacto:** Sem retrabalho por dependências quebradas.

---

## 🔴 O que pode melhorar (Improve)

### ⚠️ MySQL não saiu do papel
7 cards de banco de dados não foram tocados. O SQLite continua sendo o único banco, com connection string hardcoded. A migração para MySQL foi postergada pela terceira sprint consecutiva (planejada nas Sprints 1, 2 e 3).

**Impacto:** O sistema não está pronto para produção. SQLite não é adequado para ambientes com múltiplos usuários simultâneos.

**Causa raiz:** A ação de melhoria da Sprint 2 ("Postergar MySQL para depois do frontend funcional") foi seguida, mas o MySQL nunca foi retomado após o frontend ficar pronto.

---

### ⚠️ Testes do frontend quebrados
5 specs criados mas 0 funcionando. O vitest não está configurado — falta `vitest.config.ts` com `globals: true` e o runner não é o vitest, mas o builder nativo do Angular.

**Impacto:** Zero cobertura de testes no frontend. Regressões não serão detectadas.

**Causa raiz:** O `package.json` lista `vitest` como devDependency, mas o `angular.json` usa `@angular/build:unit-test` como builder. Há um descompasso entre o runner configurado e as APIs usadas nos testes.

---

### ⚠️ Logo não criada
A identidade visual está bem encaminhada (cores, fontes, animações), mas a logo — um elemento importante de branding — não foi desenvolvida.

**Impacto:** A aplicação não tem um símbolo visual distintivo.

---

### ⚠️ Tela de emoções genérica (emotions.ts)
O componente `emotions.ts` é um shell vazio — não lista nem gerencia emoções. A gestão de emoções está concentrada em `emotion-manage.ts`, mas a listagem pura não foi implementada.

**Impacto:** A tela `/emotions` existe mas não tem conteúdo.

---

### ⚠️ Sprint muito longa (36 dias)
36 dias é quase o dobro das sprints anteriores (~30 dias). O escopo ambicioso e a adição de JWT (não planejado originalmente) estenderam o prazo.

**Impacto:** Ritmo de entrega mais lento, feedback loops mais longos.

---

## 💡 Ações de Melhoria (Action Items)

### 🎯 MySQL na Sprint 4 — sem postergação
O MySQL foi postergado por 3 sprints. Precisa ser prioridade máxima na Sprint 4, possivelmente como primeira tarefa.

**Ação:** Configurar MySQL via Docker, instalar Pomelo.EntityFrameworkCore.MySql, migrar dados e validar antes de qualquer outra tarefa.

---

### 🎯 Corrigir testes do frontend
Os specs existentes precisam funcionar. Duas opções: (a) configurar `vitest.config.ts` com `globals: true` e trocar o builder, ou (b) manter o builder Angular e adicionar imports explícitos de `describe`/`it`/`expect` do vitest.

**Ação:** Escolher uma abordagem e fazer os 5 specs passarem. Depois expandir cobertura.

---

### 🎯 Criar logo
Elemento simples mas importante para identidade. Pode ser um SVG ou texto estilizado.

**Ação:** Desenvolver uma logo mínima (ícone + nome "DebugMe") e aplicar no header/navbar.

---

### 🎯 Preencher tela de emoções vazia
A tela `/emotions` deve listar emoções cadastradas ou redirecionar para `/emotions/manage`.

**Ação:** Implementar listagem de emoções com contagem de eventos vinculados.

---

### 🎯 Sprints mais curtas
Voltar ao ritmo de ~30 dias. A Sprint 4 deve ter escopo mais enxuto e focado.

**Ação:** Limitar a Sprint 4 a 4 semanas (28/07 – 25/08).

---

## 🧠 Insight Principal da Sprint

> "A Sprint 03 transformou o DebugMe de um backend funcional em uma aplicação completa. A segurança e a experiência do usuário evoluíram significativamente. O próximo passo é preparar o sistema para o mundo real: banco de produção e deploy."

---

## 🏁 Conclusão

A Sprint 03 foi **bem-sucedida** em seu objetivo principal: entregar uma aplicação funcional e utilizável. O frontend está completo com 5 telas funcionais, a segurança foi elevada com BCrypt + JWT, e a identidade visual está coesa.

Os dois gaps pendentes (MySQL e testes do frontend) são herdados de sprints anteriores e precisam ser resolvidos na Sprint 4 para que o sistema esteja pronto para deploy.

**Taxa de conclusão estimada:** ~75% (considerando cards concluídos vs total, com peso maior para os itens críticos)

---

## 📈 Evolução Comparativa das Sprints

| Métrica | Sprint 1 | Sprint 2 | Sprint 3 |
|---|---|---|---|
| Testes backend | 22 | 39 | **47** |
| Entidades | 2 (User, Emotion) | 3 (+EventLog) | 3 |
| Telas frontend | 0 | 0 | **5** |
| Segurança | SHA256 | SHA256 | **BCrypt + JWT** |
| Banco | SQLite | SQLite | SQLite |
| Documentação | Base | Expandida | Completa |
| Duração | 28 dias | 30 dias | 36 dias |
