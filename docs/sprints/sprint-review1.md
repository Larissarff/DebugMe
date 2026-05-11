# 🚀 Sprint Review — Sprint 01

**Projeto:** DebugMe
**Período:** 30/03/2026 – 27/04/2026

---

## 🎯 Objetivo da Sprint

Estabelecer a base organizacional, arquitetural e técnica do projeto DebugMe, garantindo que o sistema estivesse preparado para evoluções incrementais nas próximas sprints.

---

## ✅ Resumo Executivo

A Sprint 01 foi concluída com nível moderado de sucesso. A base técnica do backend e testes foi estabelecida, mas há **2 problemas arquiteturais críticos no fluxo Emotion** que precisam ser corrigidos antes de avançar para a Sprint 2.

---

## 📦 Entregas Realizadas vs. Planejadas

| Planejado | Realizado | Status |
|---|---|---|
| Board Kanban configurado | Board criado e organizado | ✅ |
| Backend ASP.NET Core inicializado | [`Program.cs`](../src/DebugMeBackend/Program.cs) funcional com Swagger, DI, EF Core | ✅ |
| Projeto de testes criado | [`DebugMeBackend.Tests.csproj`](../tests/DebugMeBackend.Tests/DebugMeBackend.Tests.csproj) com xUnit, Moq, FluentAssertions | ✅ |
| Frontend Angular inicializado | [`package.json`](../src/debugme-frontend/package.json) Angular 21 criado | ✅ |
| Estrutura de pastas definida | Backend com Controllers/Services/Repositories/Entities/DTOs/Data; Frontend com features/core/shared | ✅ |
| Entidades iniciais definidas | [`User.cs`](../src/DebugMeBackend/Entities/User.cs) e [`Emotion.cs`](../src/DebugMeBackend/Entities/Emotion.cs) criadas | ✅ |
| Diagrama de banco de dados | [`docs/database/diagram.png`](../docs/database/diagram.png) presente | ✅ |
| MySQL configurado | ❌ **Não.** Usou SQLite | ❌ |
| Entity Framework Core integrado | [`AppDbContext.cs`](../src/DebugMeBackend/Data/AppDbContext.cs) com DbSets e mapeamentos | ✅ |
| Migration inicial gerada | 3 migrations geradas | ✅ |
| Endpoint de health check | ❌ **Não implementado** | ❌ |
| README inicial | [`README.md`](../README.md) completo | ✅ |
| Pasta `/docs` estruturada | [`docs/architecture.md`](../docs/architecture.md), diagramas | ✅ |

**Taxa de conclusão:** ~85% (11 de 13 itens)

---

## 🏗️ Backend — Qualidade Técnica

### ✅ Acertos

- **Arquitetura em camadas** bem aplicada: Controllers → Services → Repositories → Data
- **DTOs** bem definidos com Data Annotations (`[Required]`, `[MinLength]`, `[EmailAddress]`) — vide [`CreateUserDto.cs`](../src/DebugMeBackend/DTOs/User/CreateUserDto.cs)
- **Injeção de dependência** correta via [`Program.cs`](../src/DebugMeBackend/Program.cs#L28-L31)
- **Tratamento de erros** consistente com `InvalidOperationException` e respostas HTTP adequadas (201, 400, 404, 204)
- **Validações de negócio** sólidas no [`UserService.cs`](../src/DebugMeBackend/Services/UserService.cs) (normalização de email, hash de senha, validações de tamanho)
- **Swagger** configurado e funcional

### ⚠️ Problemas Arquiteturais Críticos

#### 1. `EmotionService` bypassa o Repository

O [`EmotionService.cs`](../src/DebugMeBackend/Services/EmotionService.cs) injeta `AppDbContext` diretamente, ignorando o `IEmotionRepository` que foi registrado em [`Program.cs`](../src/DebugMeBackend/Program.cs#L29). Enquanto o [`UserService.cs`](../src/DebugMeBackend/Services/UserService.cs) usa o repository corretamente, o `EmotionService` quebra o padrão.

**Impacto:** Inconsistência arquitetural — metade do sistema segue o padrão Repository, a outra metade não.

#### 2. `EmotionRepository` usa lista em memória

O [`EmotionRepository.cs`](../src/DebugMeBackend/Repositories/EmotionRepository.cs) armazena dados em uma `List<Emotion>` em vez de persistir no banco de dados via `AppDbContext`.

**Impacto:** Dados de emoções **não persistem** entre requisições. Ao reiniciar a API, todas as emoções cadastradas são perdidas.

#### 3. Falsa cobertura de testes no EmotionService

Os testes em [`EmotionServiceTests.cs`](../tests/DebugMeBackend.Tests/Services/EmotionServiceTests.cs) mockam `IEmotionRepository`, mas o `EmotionService` real **não consome essa interface** — ele usa `AppDbContext` diretamente. Os testes passam, mas não refletem o comportamento real em produção.

#### 4. Hash de senha inseguro

O [`UserService.cs`](../src/DebugMeBackend/Services/UserService.cs#L186-L194) usa SHA256 sem salt para hash de senhas. Isso é vulnerável a ataques de dicionário e rainbow table.

#### 5. Endpoint de health check não implementado

O endpoint `GET /health` era um entregável explícito da sprint e não foi criado.

---

## 🧪 Testes — Qualidade Técnica

### ✅ Acertos

- **22 testes** em [`UserServiceTests.cs`](../tests/DebugMeBackend.Tests/Services/UserServiceTests.cs) cobrindo: criação (validações de nome, email, senha), listagem, busca por ID, atualização, deleção
- **10 testes** em [`EmotionServiceTests.cs`](../tests/DebugMeBackend.Tests/Services/EmotionServiceTests.cs)
- Uso correto de **Moq** para mocking e **FluentAssertions** para asserções legíveis
- Bons cenários de borda (email vazio, nome muito curto, senha inválida)

### ⚠️ Problema

Conforme mencionado acima, os testes do `EmotionService` mockam `IEmotionRepository`, mas o service real não usa essa interface. Isso precisa ser corrigido em conjunto com a refatoração do service.

---

## 🗂️ Gestão do Projeto

- Board Kanban criado e organizado ✅
- Backlog inicial definido ✅
- Fluxo de trabalho estruturado ✅
- Cards priorizados e acompanhados ✅

---

## 📊 Indicadores da Sprint

| Indicador | Avaliação |
|---|---|
| Taxa de conclusão | ~85% (11/13 itens) |
| Qualidade técnica (backend - User) | Elevada |
| Qualidade técnica (backend - Emotion) | Média (2 violações arquiteturais) |
| Organização | Excelente |
| Consistência arquitetural | Média |

---

## 🟡 Pontos de Atenção

### ⚠️ Escopo Excessivo
A sprint concentrou muitas frentes simultâneas: backend, frontend, banco, testes, documentação. Isso gerou dispersão e a entidade principal (EventLog) não foi entregue.

### ⚠️ EventLog não implementado
A principal entidade do produto não foi entregue. O foco em infraestrutura consumiu tempo que poderia ser dedicado ao core do produto.

### ⚠️ Inconsistência Arquitetural no Fluxo Emotion
O `EmotionService` bypassa o `IEmotionRepository` e o `EmotionRepository` usa lista em memória. Isso precisa ser corrigido antes da Sprint 2.

---

## 🧠 Aprendizados da Sprint

1. **Importância de limitar o escopo** para manter foco e qualidade
2. **Valor de iniciar testes desde o início** — bem executado no UserService
3. **Necessidade de verificar consistência arquitetural** — o padrão Repository foi violado no fluxo Emotion
4. **Priorizar funcionalidades centrais** (EventLog) antes de frentes periféricas

---

## 🚀 Ações Corretivas Imediatas (pré-Sprint 2)

| # | Ação | Prioridade |
|---|---|---|
| 1 | Corrigir `EmotionRepository` para usar `AppDbContext` (persistência real) | 🔴 Crítica |
| 2 | Corrigir `EmotionService` para usar `IEmotionRepository` via DI | 🔴 Crítica |
| 3 | Corrigir testes do `EmotionService` para refletir a implementação real | 🔴 Crítica |
| 4 | Implementar endpoint `GET /health` | 🟡 Média |
| 5 | Substituir SHA256 por bcrypt/Argon2 para hash de senhas | 🟡 Média |

---

## 🏁 Conclusão

A Sprint 01 foi **parcialmente bem-sucedida**. A base técnica do backend (fluxo User) e testes é sólida, mas o **fluxo Emotion possui 2 problemas arquiteturais críticos** que precisam ser corrigidos antes de iniciar a Sprint 2:

1. `EmotionService` injeta `AppDbContext` diretamente em vez de usar `IEmotionRepository`
2. `EmotionRepository` usa lista em memória em vez de persistir no banco

👉 **Próximo passo:** Corrigir os problemas arquiteturais do fluxo Emotion e, em seguida, iniciar a implementação do EventLog conforme planejado na Sprint 2.
