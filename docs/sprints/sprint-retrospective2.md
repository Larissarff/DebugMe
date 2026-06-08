# 🔄 Sprint Retrospective — Sprint 02
**Projeto:** DebugMe
**Período:** 28/04/2026 – 27/05/2026

---

## 🎯 Objetivo da Retrospectiva

Analisar o desempenho da Sprint 02 para identificar:
- O que funcionou bem
- O que pode melhorar
- Ações práticas para evolução contínua

---

## 🟢 O que funcionou bem (Keep)

### ✅ Correções arquiteturais concluídas
- `EmotionRepository` refatorado para usar `AppDbContext` (persistência real)
- `EmotionService` agora injeta `IEmotionRepository` via DI
- `UserService` padronizado com `SaveChangesAsync` encapsulado no Repository
- `IUserRepository` limpo, sem `SaveChangesAsync` público

**Impacto:** Consistência arquitetural restaurada — todo o sistema segue o padrão Repository.

---

### ✅ Entidade EventLog completa
- Entidade, Interface, Repository, Service, Controller, Testes (14), Mapeamento EF, DI
- Tudo implementado em uma única leva organizada

**Impacto:** Núcleo do produto entregue com qualidade e cobertura de testes.

---

### ✅ Migration do EventLog gerada e aplicada
- Tabela `EventLogs` criada no banco SQLite
- Relacionamentos com User e Emotion via chaves estrangeiras com Cascade

**Impacto:** API de EventLog agora persiste dados sem erro 500.

---

### ✅ Health Check implementado
- Middleware nativo do ASP.NET Core (`IHealthCheck`)
- Endpoint `GET /health` com resposta JSON padronizada
- Verificação de conectividade com o banco de dados

**Impacto:** Monitoramento da API habilitado para deploy.

---

### ✅ Testes mantidos verdes
- **39 testes, 0 falhas** — consistentes durante toda a sprint
- Testes do `EmotionService` agora refletem a implementação real

**Impacto:** Confiabilidade na base de código.

---

### ✅ Organização em branches
- Uso de branches `feature/*` para cada entrega
- Commits descritivos e atômicos

**Impacto:** Rastreabilidade clara das alterações.

---

## 🔴 O que pode melhorar (Improve)

### ⚠️ Frontend não foi iniciado
- O frontend Angular permanece no template padrão
- Nenhuma tela foi desenvolvida
- Rotas, serviços e componentes não foram criados

**Impacto:** A aplicação ainda não é utilizável pelo usuário final.

---

### ⚠️ BCrypt não implementado
- SHA256 continua sendo usado para hash de senhas
- Vulnerabilidade a ataques de dicionário e rainbow table

**Impacto:** Segurança dos usuários comprometida.

---

### ⚠️ MySQL não configurado
- SQLite ainda é o único banco em uso
- Connection string no `appsettings.json` aponta para SQL Server (inconsistente)
- Provider MySQL não instalado

**Impacto:** Sem preparação para ambiente de produção.

---

### ⚠️ Design do produto não iniciado
- Paleta de cores, tipografia e logo não definidos
- Sem identidade visual

**Impacto:** Experiência do usuário prejudicada.

---

### ⚠️ Dispersão de foco
- A sprint começou com correções arquiteturais (necessárias), consumindo tempo
- O frontend, BCrypt e MySQL ficaram para trás

**Impacto:** Entregas de valor ao usuário postergadas.

---

## 💡 Ações de Melhoria (Action Items)

### 🎯 Priorizar BCrypt no início da Sprint 3
- Segurança é base para o login e deve ser a primeira tarefa

**Ação:** Implementar BCrypt no `UserService` antes de qualquer outra tarefa.

---

### 🎯 Desenvolver frontend em paralelo com backend
- Não esperar o backend estar 100% para começar o frontend
- Usar mocks/data simulados para desenvolvimento paralelo

**Ação:** Criar serviços Angular e componentes simultaneamente às tarefas de backend.

---

### 🎯 Postergar MySQL para depois do frontend funcional
- SQLite é suficiente para desenvolvimento
- MySQL deve ser configurado apenas quando o frontend estiver funcional

**Ação:** Manter SQLite como banco de desenvolvimento; migrar para MySQL após telas prontas.

---

### 🎯 Definir identidade visual mínima viável
- Não buscar design perfeito, mas um mínimo consistente
- Paleta de 3-4 cores + 1 tipografia é suficiente para começar

**Ação:** Definir paleta e tipografia no início da Sprint 3, aplicar nas telas durante o desenvolvimento.

---

### 🎯 Quebrar tarefas grandes em ciclos menores
- Frontend dividido em: API service → Login → Home → Criar evento → Listar eventos

**Ação:** Cada ciclo de tela deve ser uma entrega independente e testável.

---

## 🚀 Plano de Evolução para Sprint 03

Foco principal:

> Transformar o DebugMe em uma aplicação utilizável pelo usuário final

**Prioridades:**
1. BCrypt (segurança)
2. Frontend Angular (login, criar evento, listar eventos)
3. Identidade visual mínima
4. MySQL (produção)
5. Documentação

---

## 🧠 Insight Principal da Sprint

> "A base técnica está sólida, mas o valor real só será entregue quando o usuário puder interagir com o sistema. O foco precisa migrar de infraestrutura para funcionalidade."

---

## 🏁 Conclusão

A Sprint 02 foi **bem-sucedida na consolidação técnica**, mas **falhou em entregar valor ao usuário final**. O backend está estável e testado, porém sem frontend, segurança adequada ou banco de produção, o sistema ainda não pode ser utilizado.

Os aprendizados desta sprint reforçam a necessidade de:
- Priorizar segurança desde o início
- Desenvolver frontend em paralelo
- Não postergar entregas de valor para o usuário

Com esses ajustes, a Sprint 03 tem potencial para entregar a primeira versão realmente funcional do DebugMe.
