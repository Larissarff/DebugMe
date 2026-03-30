# 🚀 Sprint 01 — Fundação do Projeto

## 📅 Período
4 semanas (30/03/2026 - 27/04/2026)

---

## 🎯 Objetivo

Estabelecer a base organizacional, arquitetural e técnica do projeto **DebugMe**, incluindo a definição do fluxo de trabalho, inicialização dos projetos (backend, frontend e testes), modelagem inicial do banco de dados e estruturação da documentação.

---

## 📦 Entregáveis

- Board Kanban configurado e organizado
- Backend ASP.NET Core inicializado
- Projeto de testes automatizados criado
- Frontend Angular inicializado
- Estrutura inicial de pastas definida
- Entidades iniciais do domínio definidas
- Diagrama de banco de dados criado
- MySQL configurado
- Entity Framework Core integrado
- `DbContext` criado
- Migration inicial gerada
- Endpoint de health check funcional
- README inicial criado
- Pasta `/docs` estruturada com documentação base

---

## 🧩 Cards da Sprint

### 🗂️ Gestão do Projeto

- [ ] [Gestão] Criar board Kanban do projeto
- [ ] [Gestão] Definir colunas e fluxo de trabalho
- [ ] [Gestão] Criar backlog inicial do produto
- [ ] [Gestão] Definir estratégia de branches (`main`, `develop`, `feature/*`)
- [ ] [Gestão] Definir padrão de nomenclatura de tarefas

---

### 🏗️ Estrutura da Solução

- [ ] [Backend] Inicializar solução ASP.NET Core
- [ ] [Testes] Criar projeto de testes automatizados
- [ ] [Frontend] Inicializar aplicação Angular
- [ ] [Arquitetura] Definir estrutura inicial de pastas (backend, frontend, docs)
- [ ] [Infra] Configurar ambiente local de desenvolvimento

---

### 🧠 Domínio e Banco de Dados

- [ ] [Domínio] Definir entidades iniciais do sistema
- [ ] [Banco] Criar diagrama de banco de dados
- [ ] [Banco] Configurar MySQL
- [ ] [Banco] Instalar e configurar Entity Framework Core
- [ ] [Banco] Criar `AppDbContext`
- [ ] [Banco] Criar mapeamentos iniciais das entidades
- [ ] [Banco] Gerar migration inicial
- [ ] [Banco] Validar criação do banco de dados

---

### ⚙️ Base Funcional

- [ ] [Backend] Criar endpoint de health check (`/health`)
- [ ] [Backend] Validar execução local da API
- [ ] [Frontend] Validar execução local da aplicação Angular
- [ ] [Integração] Definir padrão de comunicação entre frontend e backend (API REST)

---

### 📚 Documentação

- [ ] [Docs] Criar README inicial do projeto
- [ ] [Docs] Criar estrutura da pasta `/docs`
- [ ] [Docs] Documentar visão do produto
- [ ] [Docs] Documentar arquitetura inicial
- [ ] [Docs] Documentar Sprint 01

---

## 📐 Modelagem Inicial (Visão Geral)

Entidades previstas para início do sistema:

- EventLog (registro de evento emocional)
- Emotion (tipo de emoção)
- User (usuário do sistema)

---

## ✅ Critérios de Aceitação

- O board Kanban deve estar criado e organizado
- Os projetos backend, frontend e testes devem compilar e executar localmente
- O banco de dados deve estar configurado e conectado ao backend
- O `DbContext` deve estar implementado corretamente
- A migration inicial deve ser gerada sem erros
- O endpoint `/health` deve responder corretamente
- A documentação inicial deve estar presente no repositório

---

## 📊 Métricas de Sucesso

- Estrutura completa do projeto criada
- Ambiente funcional localmente (frontend + backend + banco)
- Organização clara do fluxo de trabalho
- Documentação inicial consistente

---

## ⚠️ Riscos

- Complexidade na configuração inicial do ambiente
- Retrabalho na modelagem por falta de definição clara do domínio
- Integração inicial com MySQL e Entity Framework
- Sobrecarga com tarefas de infraestrutura

---

## 🔄 Próxima Sprint

- Implementação do fluxo de criação de eventos (EventLog)
- Persistência completa de dados
- Primeiras telas funcionais no frontend
- Listagem de eventos

---

## 🧠 Observações

Esta sprint foca na construção de uma base sólida para permitir evolução incremental do projeto, evitando retrabalho e garantindo organização desde o início.
