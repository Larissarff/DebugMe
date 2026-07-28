# DebugMe 🧠🐞 — Mental Debugging as Software 
### EM DESENVOLVIMENTO

> Mental debugging as a structured engineering process.

---

## 📌 Sobre o projeto

O **DebugMe** é uma aplicação web que propõe uma abordagem diferente para o desenvolvimento pessoal: tratar pensamentos, emoções e comportamentos como um sistema passível de análise, rastreamento e melhoria contínua.

Inspirado no fluxo de debugging de software, o sistema permite que o usuário:

- Registre situações (eventos do dia a dia)
- Identifique "erros" (reações emocionais ou padrões negativos)
- Analise causas (gatilhos e contexto)
- Aplique "refatorações" (mudanças de comportamento ou pensamento)
- Acompanhe evolução ao longo do tempo

- trello do repositorio:

      https://trello.com/b/e6nq2Xog/debugme

---

## 🎯 Problema

Pessoas enfrentam dificuldades emocionais e comportamentais, mas não possuem ferramentas estruturadas para:

- Identificar padrões recorrentes
- Entender causas reais
- Medir evolução pessoal

As soluções atuais são, em geral:
- Subjetivas
- Pouco estruturadas
- Difíceis de acompanhar ao longo do tempo

---

## 💡 Solução

O DebugMe aplica conceitos de engenharia de software ao desenvolvimento pessoal:

| Engenharia de Software | DebugMe |
|----------------------|--------|
| Bug                  | Erro emocional/comportamental |
| Log                  | Registro de evento |
| Stack trace          | Contexto e gatilhos |
| Debugging            | Análise de padrões |
| Refatoração          | Mudança de comportamento |
| Iteração             | Evolução contínua |

---

## 🧩 Funcionalidades (MVP)

- [ ] Registro de eventos (logs emocionais)
- [ ] Classificação de "erros" (ansiedade, irritação, etc.)
- [ ] Identificação de gatilhos
- [ ] Histórico de registros
- [ ] Dashboard com padrões
- [ ] Sugestões de "refatoração"
- [ ] Autenticação de usuários

---

## 🚀 Tecnologias

### Backend
- C#
- ASP.NET Core Web API (.NET 9)
- Entity Framework Core
- JWT Authentication (access + refresh tokens)
- BCrypt password hashing

### Banco de Dados
- **Desenvolvimento:** SQLite
- **Produção:** MySQL 8.0+ (via Pomelo.EntityFrameworkCore.MySql)

### Frontend
- Angular 21 (standalone components)
- TypeScript 5.9
- RxJS

### Infra
- GitHub
- Docker (MySQL local)

---

## 🏗️ Arquitetura

O projeto segue princípios de engenharia de software moderna:

- Arquitetura em camadas
- Separação de responsabilidades
- Boas práticas de API REST
- Organização baseada em domínio

Exemplo de estrutura:

backend/

├── Controllers/

├── Services/

├── Repositories/

├── Domain/

├── DTOs/


frontend/

├── src/

├── components/

├── services/

├── pages/



---

## 🔄 Metodologia de desenvolvimento

O projeto está sendo desenvolvido utilizando um ciclo de vida **incremental**, com sprints de 4 semanas.

Cada sprint entrega:
- Funcionalidades completas
- Código testável
- Incremento funcional no produto

Objetivo:
👉 evitar sobrecarga  
👉 manter consistência  
👉 garantir evolução contínua  

---

## 📈 Possíveis evoluções

- Integração com profissionais de saúde (psicólogos)
- Análise preditiva de padrões emocionais
- Machine Learning para recomendação de melhorias
- Sistema de acompanhamento terapêutico
- Aplicativo mobile

---

## 🧪 Testes

- Testes unitários no backend
- Validação de regras de negócio
- Testes de integração (futuro)

---

## 🎨 Diferencial do projeto

O DebugMe não é apenas um sistema técnico.

Ele combina:
- Engenharia de Software
- Psicologia comportamental
- Análise de dados pessoais

Criando uma abordagem inovadora para evolução individual.

---

## 📌 Status do projeto

🚧 Em desenvolvimento (MVP em construção)

---

## ⚡ Quick Start

### Pré-requisitos
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 20+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (para MySQL)

### Backend

```bash
# Desenvolvimento (SQLite - não requer Docker)
cd src/DebugMeBackend
dotnet run
# API disponível em http://localhost:5165
# Swagger em http://localhost:5165/swagger
```

### MySQL (Produção)

```bash
# Subir container MySQL
docker run --name debugme-mysql \
  -e MYSQL_ROOT_PASSWORD=debugme123 \
  -e MYSQL_DATABASE=DebugMeDb \
  -p 3306:3306 \
  -d mysql:8.0

# Inicializar tabelas
mysql -h 127.0.0.1 -u root -pdebugme123 DebugMeDb < src/DebugMeBackend/Data/init-mysql.sql

# Rodar backend com MySQL
set ASPNETCORE_ENVIRONMENT=Production
cd src/DebugMeBackend
dotnet run
```

### Frontend

```bash
cd src/debugme-frontend
npm install
npm start
# Disponível em http://localhost:4200
```

### Testes

```bash
# Backend (47 testes)
dotnet test tests/DebugMeBackend.Tests

# Frontend (em configuração)
cd src/debugme-frontend
npm test
```

---

## 🤝 Contribuição

Este projeto é pessoal, mas contribuições são bem-vindas.

---

## 👩‍💻 Autora

Larissa Ferreira  
Desenvolvedora de Software | C# | .NET | Engenharia de Software  

---
