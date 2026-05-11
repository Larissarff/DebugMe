# 📋 Alinhamento Sprint 2 — DebugMe

> **Data:** 11/05/2026
> **Propósito:** Registrar o progresso realizado, alinhar o backlog da Sprint 2 e definir o próximo passo.

---

## ✅ O que foi feito (Correções Pós-Sprint 1)

### 🔧 Arquitetura — Repository Pattern

| Item | Status | Detalhes |
|------|--------|----------|
| `EmotionRepository` refatorado | ✅ | Antes usava `List<Emotion>` em memória; agora usa `AppDbContext` com persistência real |
| `EmotionService` refatorado | ✅ | Antes injetava `AppDbContext` diretamente; agora injeta `IEmotionRepository` |
| `UserService` — remoção de `SaveChangesAsync` | ✅ | Agora segue o mesmo padrão do `EmotionService`: `SaveChangesAsync` encapsulado no Repository |
| `IUserRepository` — interface limpa | ✅ | Removeu `SaveChangesAsync()` da interface pública |
| `UserRepository` — `SaveChangesAsync` interno | ✅ | `AddAsync`, `UpdateAsync` e `DeleteAsync` chamam `SaveChangesAsync` internamente |

### 🧪 Testes

| Item | Status | Detalhes |
|------|--------|----------|
| `EmotionServiceTests` corrigidos | ✅ | Agora mockam `IEmotionRepository` (condizente com a implementação real) |
| `UserServiceTests` limpos | ✅ | Removidos 14 testes obsoletos de validação de DTOs (agora responsabilidade dos DTOs/Controllers) + remoção de referências a `SaveChangesAsync` |
| **Total de testes: 39 passed, 0 failed** | ✅ | |

### 🧹 Limpeza de Código

| Item | Status | Detalhes |
|------|--------|----------|
| `User.cs` — usings mortos | ✅ | Removeu `using Microsoft.AspNetCore.Authorization.Infrastructure` |
| `UserService.cs` — validação duplicada | ✅ | Removeu validações de DTO (Name, Email, Password) que já existem nas Data Annotations |
| `EmotionController.GetById` | ✅ | Agora retorna `404 NotFound` quando emoção não existe |
| `EmotionController.GetByName` | ✅ | Agora retorna `404 NotFound` quando emoção não existe |
| `EventLogController.Update` | ✅ | Agora captura `ArgumentNullException` |

### 🆕 Entidade EventLog (Criada do Zero)

| Camada | Arquivo | Status |
|--------|---------|--------|
| Entidade | `EventLog.cs` | ✅ |
| Interface Repository | `IEventLogRepository.cs` | ✅ |
| Repository | `EventLogRepository.cs` | ✅ |
| Service | `EventLogService.cs` | ✅ |
| Controller | `EventLogController.cs` | ✅ |
| Testes | `EventLogServiceTests.cs` (14 testes) | ✅ |
| Mapeamento EF | `AppDbContext.cs` (HasOne/WithMany, Cascade) | ✅ |
| DI | `Program.cs` (scoped services) | ✅ |

---

## 📌 Status da Sprint 2 — Backlog Atualizado

### 🟢 Concluído (já entregue)

- [x] Criar entidade EventLog completa
- [x] Definir relacionamento User → EventLog e Emotion → EventLog
- [x] Atualizar `AppDbContext` com DbSet e mapeamentos
- [x] Criar Repository de EventLog
- [x] Criar Service de EventLog
- [x] Criar Controller de EventLog
- [x] Testar criação de EventLog
- [x] Testar validações de negócio
- [x] Testar listagem de eventos
- [x] Testar cenários de erro
- [x] Aumentar cobertura dos services (UserService, EmotionService, EventLogService)

### 🟡 Pendente (para conclusão da Sprint 2)

#### 🗄️ Infraestrutura (Migração de Banco)

- [ ] Gerar migration do EventLog (`dotnet ef migrations add AddEventLogTable`)
- [ ] Configurar MySQL localmente
- [ ] Ajustar connection string
- [ ] Instalar provider MySQL no EF Core
- [ ] Executar migrations no MySQL
- [ ] Validar persistência de dados
- [ ] Remover dependência de SQLite

#### 🧠 Segurança

- [ ] Substituir SHA256 por BCrypt (hash de senha com salt)
- [ ] Implementar endpoint de health check (`GET /health`)

#### 🎨 Frontend (Angular)

- [ ] Criar serviço de API (HTTP Client)
- [ ] Configurar environment para API
- [ ] Tela de criação de evento
- [ ] Tela de listagem de eventos
- [ ] Integração com backend

#### 🎨 Design do Produto

- [ ] Definir paleta de cores oficial
- [ ] Criar identidade visual (tema terapêutico)
- [ ] Definir tipografia
- [ ] Criar logo inicial
- [ ] Aplicar estilo nas telas

#### 📚 Documentação

- [ ] Atualizar README
- [ ] Documentar endpoints de EventLog
- [ ] Atualizar arquitetura (se necessário)

---

## 🎯 Próximo Passo Recomendado

### **Gerar a migration do EventLog e aplicar no banco**

**Justificativa:** A entidade EventLog é o núcleo do sistema e já está 100% implementada em código (entidade, repositório, serviço, controller, testes — 39/39 passando). Porém, a tabela `EventLogs` **não existe no banco de dados** porque a migration ainda não foi gerada.

**Comando necessário:**
```bash
dotnet ef migrations add AddEventLogTable --project src/DebugMeBackend
dotnet ef database update --project src/DebugMeBackend
```

**Riscos de não fazer agora:**
- A API de EventLog retorna erro 500 ao tentar persistir dados
- Testes de integração falham
- Bloqueia o desenvolvimento do frontend (sem dados para consumir)

---

## 📊 Métricas Atuais do Projeto

| Métrica | Valor |
|---------|-------|
| Testes unitários | **39 passed, 0 failed** |
| Cobertura de testes | UserService (9), EmotionService (14), EventLogService (14), EmotionController (2) |
| Entidades | User, Emotion, EventLog |
| Endpoints | User (5), Emotion (5), EventLog (6) |
| Frontend | Template padrão Angular (não iniciado) |
| Banco | SQLite (desenvolvimento), MySQL (planejado) |
