# 🔧 Sprint de Refinamento — Correções e Melhorias

## 📅 Período
A definir (posterior à v1.0)

## 🎯 Objetivo

Corrigir falhas de isolamento de dados, melhorar a experiência do usuário com navegação simplificada e interatividade no calendário, e garantir responsividade completa em dispositivos móveis.

---

## 📦 Necessidades Identificadas (em ordem de prioridade)

---

### 🔴 P1 — Correção: Contagem de eventos por emoção deve ser por usuário

**Problema**: A tela `/emotions/manage` mostrava a contagem global de eventos por emoção (de todos os usuários), em vez de mostrar apenas os registros do usuário logado. Um usuário que nunca registrou "amor" via o número de registros de terceiros.

**Solução implementada**:
- Novo método `GetAllWithUserEventCountAsync(Guid userId)` no repositório de emoções
- Query no banco filtra `EventLogs` por `userId` via `Count(el => el.UserId == userId)`
- Controller extrai `userId` do token JWT e repassa ao serviço
- Endpoint `/api/emotion/all-with-count` agora retorna contagens por usuário (transparente para o frontend)

**Arquivos alterados**:
- `src/DebugMeBackend/Repositories/Interfaces/IEmotionRepository.cs` — nova assinatura
- `src/DebugMeBackend/Repositories/EmotionRepository.cs` — implementação com filtro por userId
- `src/DebugMeBackend/Services/EmotionService.cs` — novo método delegando ao repositório
- `src/DebugMeBackend/Controllers/EmotionController.cs` — extração de userId do token JWT

**Validação**: 47/47 testes backend passando. Frontend não precisou de alteração (o endpoint já era consumido, agora responde com dados corretos).

---

### 🔴 P1.1 — Isolamento total: cada usuário vê apenas suas próprias emoções

**Problema**: As emoções eram globais — todos os usuários viam as mesmas emoções cadastradas por qualquer pessoa. Um usuário que não cadastrou "amor" ainda via "amor" na select box se outro usuário tivesse cadastrado.

**Solução implementada**:
- Adicionada coluna `UserId` (nullable) na tabela `Emotion` com FK para `User`
- Migration `AddUserIdToEmotion` criada
- `Emotion.UserId` é `Guid?` — `null` para emoções legadas/globais, preenchido para emoções do usuário
- `CreateAsync` agora recebe `userId` do token JWT e associa à nova emoção
- `GetByUserIdAsync(userId)` filtra por `UserId == null || UserId == userId` (usuário vê emoções globais + as próprias)
- `GetByNameAndUserIdAsync` verifica duplicata apenas dentro do escopo do usuário
- `GetAllWithUserEventCountAsync` também filtra por `UserId == null || UserId == userId`
- Endpoint `GET /api/emotion/all` extrai userId do token e retorna apenas emoções do usuário
- Controller `Create` extrai userId do token e repassa ao serviço

**Arquivos alterados**:
- `src/DebugMeBackend/Entities/Emotion.cs` — adicionado `UserId` e `User` navigation property
- `src/DebugMeBackend/Data/AppDbContext.cs` — configurado relacionamento `Emotion -> User`
- `src/DebugMeBackend/Repositories/Interfaces/IEmotionRepository.cs` — novas assinaturas
- `src/DebugMeBackend/Repositories/EmotionRepository.cs` — filtro por UserId em todas as queries
- `src/DebugMeBackend/Services/EmotionService.cs` — `CreateAsync` recebe userId, `GetAllAsync` → `GetByUserIdAsync`
- `src/DebugMeBackend/Controllers/EmotionController.cs` — `GetAll` e `Create` extraem userId do token
- `tests/DebugMeBackend.Tests/Services/EmotionServiceTests.cs` — testes atualizados
- `Migrations/*_AddUserIdToEmotion.cs` — nova migration

**Validação**: 47/47 testes passando. Build frontend ok.

---

### 🟡 P2 — Botão "Cadastrar nova emoção" na tela de criação de evento

**Problema**: Na tela `/events/new`, a select box mostra apenas emoções já cadastradas. Se o usuário quiser registrar uma emoção não listada, precisa navegar manualmente até `/emotions/manage`, cadastrar, e voltar.

**Solução implementada**:
- Link "✦ Cadastrar nova emoção" abaixo da select box de emoção
- Navega via `routerLink="/emotions/manage"` mantendo consistência com o resto da app
- Estilo visual seguindo identidade da marca (cor violeta, transição suave)

**Arquivos alterados**:
- `src/debugme-frontend/src/app/features/event-logs/event-create/event-create.html` — adicionado link
- `src/debugme-frontend/src/app/features/event-logs/event-create/event-create.css` — estilo do link

---

### 🟡 P3 — Calendário interativo com popup de detalhes do dia

**Problema**: O calendário na home mostrava dots coloridos indicando intensidade, mas não era clicável. O usuário não conseguia ver quais eventos estavam registrados em cada dia sem navegar para a listagem completa.

**Solução implementada**:
- Dias com eventos (`count > 0`) ganham classe `has-events` e cursor pointer
- Ao clicar num dia, popup modal abre com:
  - Data formatada em português (ex: "segunda-feira, 28 de julho de 2026")
  - Lista de eventos do dia com emoção, intensidade (badge colorido), e descrição
  - Cada evento é clicável (redireciona para `/events`)
  - Overlay com backdrop blur, fechamento ao clicar fora ou no botão ✕
- Animação de entrada (slide up + fade) para transição suave

**Arquivos alterados**:
- `src/debugme-frontend/src/app/features/home/home.ts` — métodos `openDayPopup`, `closePopup`, `formatDateLabel`
- `src/debugme-frontend/src/app/features/home/home.html` — popup overlay e eventos do dia
- `src/debugme-frontend/src/app/features/home/home.css` — estilos do popup, animações, cursor pointer

---

### 🟢 P4 — Responsividade mobile em todas as telas

**Problema**: O software foi desenhado para desktop e não se adaptava bem a telas de celular. Layouts com múltiplas colunas quebravam, fontes ficavam pequenas demais, e botões não tinham tamanho adequado para toque.

**Solução implementada** — media queries adicionadas em todas as telas:

| Tela | Breakpoints | Ajustes |
|---|---|---|
| **Login** | ≤600px | Padding reduzido, card mais compacto, inputs/botões com altura de toque (48px) |
| **Register** | ≤600px | Idem login: card compacto, fonte ajustada, botão de toque |
| **Home** | ≤768px, ≤480px | Dashboard empilhado (vertical), header colapsado, cards de ação full-width, calendário com fonte menor, popup full-width com borda superior arredondada |
| **Event Logs** | ≤600px, ≤400px | Header empilhado, botão "Novo" full-width, cards sem direção de linha, datas empilhadas, sparkline menor |
| **Event Create** | ≤600px, ≤400px | Card compacto, slider de intensidade empilhado vertical em telas muito pequenas |
| **Emotion Manage** | ≤768px, ≤400px | Layout de duas colunas vira coluna única, itens de emoção empilhados |

**Arquivos alterados**:
- `src/debugme-frontend/src/app/features/login/login.css`
- `src/debugme-frontend/src/app/features/register/register.css`
- `src/debugme-frontend/src/app/features/home/home.css`
- `src/debugme-frontend/src/app/features/event-logs/event-logs.css`
- `src/debugme-frontend/src/app/features/event-logs/event-create/event-create.css`
- `src/debugme-frontend/src/app/features/emotions/emotion-manage/emotion-manage.css`

---

## 📊 Status

| Item | Prioridade | Status |
|---|---|---|
| Contagem de eventos por usuário (backend) | 🔴 P1 | ✅ Implementado |
| Isolamento total: cada usuário vê só suas emoções | 🔴 P1.1 | ✅ Implementado |
| Botão "Cadastrar nova emoção" no event-create | 🟡 P2 | ✅ Implementado |
| Calendário interativo com popup | 🟡 P3 | ✅ Implementado |
| Responsividade mobile em todas as telas | 🟢 P4 | ✅ Implementado |

---

## 🧪 Testes

- **Backend**: 47/47 testes passando (`dotnet test`)
- **Frontend build**: Sucesso (`npm run build`)
- **Frontend tests**: 36/59 passando (23 falhas pré-existentes nos testes de serviço que mockam `localhost:5165` mas executam com URL de produção; não relacionados às alterações)

---

## 🔄 Pendências não relacionadas (débito técnico existente)

- 23 testes de serviço no frontend quebram porque o `environment.ts` aponta para Render mas os mocks esperam `localhost:5165` — corrigir configuração de teste com `environment.development.ts`
- Restringir CORS ao domínio de produção
- Gerar changelog e tag `v1.0.0`
