# 🔄 Sprint Retrospective — Sprint 01  
**Projeto:** DebugMe  
**Período:** 30/03/2026 – 27/04/2026  

---

## 🎯 Objetivo da Retrospectiva

Analisar o desempenho da Sprint 01 para identificar:
- O que funcionou bem  
- O que pode melhorar  
- Ações práticas para evolução contínua  

---

## 🟢 O que funcionou bem (Keep)

### ✅ Planejamento estruturado
- Definição clara de objetivo, entregáveis e critérios de aceitação  
- Boa organização inicial da sprint  

**Impacto:**  
Facilitou execução consistente e visão do progresso  

---

### ✅ Organização do fluxo (Kanban)
- Board bem estruturado  
- Separação clara entre backlog, prioridade, em andamento e finalizado  

**Impacto:**  
Boa visibilidade das tarefas e controle do andamento  

---

### ✅ Base técnica sólida
- Backend estruturado com boas práticas (Controller, Service, Repository)  
- Uso de DTOs e separação de responsabilidades  
- Integração com banco de dados funcionando  

**Impacto:**  
Redução de retrabalho nas próximas sprints  

---

### ✅ Testes desde o início
- Criação de projeto de testes  
- Cobertura inicial de services  

**Impacto:**  
Aumento da confiabilidade do código  

---

### ✅ Documentação desde a base
- README criado  
- Estrutura `/docs` organizada  
- Arquitetura documentada  

**Impacto:**  
Facilidade de entendimento do projeto por terceiros  

---

## 🔴 O que pode melhorar (Improve)

### ⚠️ Escopo excessivo na sprint
- Muitas frentes sendo trabalhadas simultaneamente:
  - Backend  
  - Frontend  
  - Banco  
  - Testes  
  - Documentação  

**Impacto:**
- Aumento da carga cognitiva  
- Risco de atraso e perda de foco  

---

### ⚠️ Falta de priorização por valor
- Tarefas de infraestrutura tiveram o mesmo peso que funcionalidades do produto  

**Impacto:**
- Funcionalidade principal (EventLog) não foi entregue  

---

### ⚠️ Início antecipado do frontend
- Frontend iniciado antes da consolidação do backend  

**Impacto:**
- Possível retrabalho futuro  
- Desvio de foco do core do sistema  

---

### ⚠️ Métricas de acompanhamento limitadas
- Não houve medição de:
  - Tempo por tarefa  
  - Taxa de conclusão quantitativa  
  - Gargalos  

**Impacto:**
- Dificuldade em estimar futuras sprints com precisão  

---

### ⚠️ Falta de foco no core do produto
- EventLog (principal funcionalidade) não foi implementado  

**Impacto:**
- Valor entregue ao usuário ainda é baixo  

---

## 💡 Ações de Melhoria (Action Items)

### 🎯 Reduzir escopo por sprint
- Limitar quantidade de tarefas simultâneas  
- Priorizar profundidade em vez de abrangência  

**Ação:**
- Definir no máximo 1–2 grandes objetivos por sprint  

---

### 🎯 Priorizar valor de negócio
- Sempre começar pelo core do sistema  

**Ação:**
- Implementar EventLog como prioridade máxima na Sprint 02  

---

### 🎯 Postergar frontend quando necessário
- Só desenvolver frontend após backend estabilizado  

**Ação:**
- Utilizar Swagger/Postman como principal ferramenta de validação  

---

### 🎯 Introduzir métricas simples
- Acompanhar desempenho da sprint  

**Ação:**
- Medir:
  - % de tarefas concluídas  
  - tempo médio por card  
  - número de bloqueios  

---

### 🎯 Trabalhar em ciclos menores
- Quebrar tarefas grandes em subtarefas  

**Ação:**
- Exemplo:
  - "Criar EventLog" → dividir em:
    - entidade  
    - repository  
    - service  
    - controller  
    - testes  

---

## 🚀 Plano de Evolução para Sprint 02

Foco principal:

> Entregar valor real ao usuário

**Prioridades:**
- Implementar fluxo completo de EventLog  
- Garantir integração entre entidades  
- Criar primeira funcionalidade utilizável  

---

## 🧠 Insight Principal da Sprint

> "Uma base sólida foi construída, mas a próxima etapa exige foco em entregar valor funcional, não apenas estrutura."

---

## 🏁 Conclusão

A Sprint 01 foi altamente bem-sucedida na construção da base técnica e organizacional do projeto.

Os principais aprendizados foram:
- Reduzir escopo  
- Priorizar valor  
- Focar no core do produto  

Com esses ajustes, as próximas sprints tendem a ser mais eficientes e orientadas a resultado.
