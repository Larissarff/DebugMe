# 🚀 Sprint 02 — Primeira Versão Funcional do Produto

## 📅 Período
30 dias (28/04/2026 - 27/05/2026)

---

## 🎯 Objetivo

Evoluir o projeto DebugMe de uma base estrutural para uma **primeira versão funcional**, implementando o fluxo completo de registro de eventos emocionais, consolidando o backend, iniciando telas do frontend e definindo a identidade visual do sistema.

---

## 📦 Entregáveis

- CRUD completo de EventLog
- Relacionamento entre User, Emotion e EventLog implementado
- Backend consolidado e validado
- Migração de SQLite para MySQL concluída
- Primeiras telas funcionais no frontend (Angular)
- Integração frontend ↔ backend realizada
- Identidade visual inicial definida
- Testes automatizados expandidos
- Documentação da Sprint 02

---

## 🧩 Cards da Sprint

---

### 🧠 Domínio e Banco de Dados

- [ ] [Domínio] Criar entidade EventLog completa
- [ ] [Domínio] Definir relacionamento:
  - User → EventLog
  - Emotion → EventLog
- [ ] [Banco] Atualizar `AppDbContext`
- [ ] [Banco] Criar mapeamentos EF Core
- [ ] [Banco] Gerar nova migration

---

### ⚙️ Backend

- [ ] [Backend] Criar Repository de EventLog
- [ ] [Backend] Criar Service de EventLog
- [ ] [Backend] Criar Controller de EventLog

#### Endpoints:

- [ ] POST `/eventlogs`
- [ ] GET `/eventlogs`
- [ ] GET `/eventlogs/{id}`
- [ ] DELETE `/eventlogs/{id}`

---

### 🧠 Regras de Negócio

- [ ] Validar criação de evento (campos obrigatórios)
- [ ] Validar vínculo com User e Emotion
- [ ] Limitar tamanho da descrição
- [ ] Validar data do evento

---

### 🧪 Testes

- [ ] Testar criação de EventLog
- [ ] Testar validações de negócio
- [ ] Testar listagem de eventos
- [ ] Testar cenários de erro
- [ ] Aumentar cobertura dos services

---

### 🗄️ Infraestrutura (Migração de Banco)

- [ ] Configurar MySQL localmente
- [ ] Ajustar connection string
- [ ] Instalar provider MySQL no EF Core
- [ ] Executar migrations no MySQL
- [ ] Validar persistência de dados
- [ ] Remover dependência de SQLite

---

### 🎨 Frontend (Angular)

#### Estrutura inicial

- [ ] Criar serviço de API (HTTP Client)
- [ ] Configurar environment para API

#### Telas

- [ ] Tela de criação de evento
- [ ] Tela de listagem de eventos
- [ ] Integração com backend

---

### 🎨 Design do Produto

- [ ] Definir paleta de cores oficial
- [ ] Criar identidade visual (tema terapêutico)
- [ ] Definir tipografia
- [ ] Criar logo inicial
- [ ] Aplicar estilo nas telas

---

### 🔗 Integração

- [ ] Conectar frontend ao backend
- [ ] Validar fluxo completo:
  - Criar evento
  - Listar eventos

---

### 📚 Documentação

- [ ] Atualizar README
- [ ] Documentar endpoints de EventLog
- [ ] Criar documentação da Sprint 02
- [ ] Atualizar arquitetura (se necessário)

---

## 📐 Modelagem (Atualizada)

Entidades principais:

- User
- Emotion
- EventLog

Relacionamentos:

- Um User possui vários EventLogs
- Um EventLog possui uma Emotion

---

## ✅ Critérios de Aceitação

- CRUD de EventLog funcional via API
- Dados persistidos no MySQL
- Backend funcionando sem dependência de SQLite
- Frontend capaz de:
  - Criar eventos
  - Listar eventos
- Integração frontend ↔ backend funcionando
- Testes cobrindo principais regras de negócio
- Identidade visual aplicada nas telas iniciais

---

## 📊 Métricas de Sucesso

- Primeira funcionalidade real entregue (EventLog)
- Backend considerado “estável”
- Integração completa entre frontend e backend
- Aplicação utilizável (mesmo que simples)
- Redução de retrabalho na camada de dados

---

## ⚠️ Riscos

- Complexidade na migração SQLite → MySQL
- Integração frontend/backend
- Curva de aprendizado do Angular
- Tempo elevado na definição de design

---

## 🔄 Próxima Sprint

- Edição de eventos
- Filtros e busca
- Melhorias de UX/UI
- Autenticação de usuários
- Deploy do projeto

---

## 🧠 Observações

Esta sprint marca a transição do projeto de uma base estrutural para uma aplicação funcional.

O foco principal deve ser:
- Entregar valor ao usuário
- Validar o fluxo principal do sistema
- Garantir estabilidade do backend

Evitar expansão excessiva de escopo para manter consistência na entrega.
