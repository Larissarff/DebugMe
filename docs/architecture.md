# 🧠 Arquitetura do Sistema — DebugMe

### 1. Visão Geral

O DebugMe é uma aplicação web desenvolvida com base na arquitetura cliente-servidor, com separação clara entre frontend e backend. O sistema foi projetado para permitir **evolução incremental** , organização modular e facilidade de manutenção.

A comunicação entre cliente e servidor ocorre por meio de uma API REST, utilizando requisições HTTP.

### 2. Estilo Arquitetural

O sistema adota os seguintes estilos arquiteturais:

 #### Cliente-Servidor

O frontend atua como cliente, responsável pela interface e interação com o usuário, enquanto o backend centraliza regras de negócio, persistência e processamento.

 #### API REST

O backend expõe endpoints RESTful para comunicação com o frontend, seguindo princípios como:

Uso de métodos HTTP (GET, POST, PUT, DELETE)
Recursos orientados a entidades (User, Emotion, EventLog)
Respostas padronizadas (status codes + payload


 #### Arquitetura em Camadas (Layered Architecture)
 
O backend é organizado em camadas bem definidas, garantindo separação de responsabilidades:

Controllers → Interface de entrada (HTTP)

Services → Regras de negócio

Repositories → Acesso a dados

Data → Contexto e persistência

Entities/Models → Estruturas de domínio


 #### Monólito Modular

O sistema é implementado como uma única aplicação backend, porém dividido internamente em módulos organizados, facilitando manutenção e futura evolução para microsserviços, se necessário.


### 3. Diagrama Conceitual da Arquitetura

    
                          [ Usuário ]
                              ↓
                  [ Frontend (Angular - SPA) ]
                              ↓ HTTP (REST API)
                        [ Controllers ]
                               ↓
                          [ Services ]
                               ↓
                        [ Repositories ]
                                ↓
                        [ Banco de Dados ]


### 4. Componentes do Sistema
#### 🖥️ Frontend (Cliente)

Responsável por:

Interface com o usuário; 
Captura de dados (inputs emocionais);
Consumo da API;
Experiência do usuário (UX);

Tecnologias previstas:

Angular (SPA);
HTML, CSS, TypeScript;

#### ⚙️ Backend (Servidor)

Responsável por:

Processamento de regras de negócio;
Validação de dados;
Autenticação (futuro);
Persistência de dados;
Exposição de endpoints REST;

Tecnologias:

C# com ASP.NET Core;
Entity Framework (ou alternativa futura);
Arquitetura baseada em Services e Repositories;


#### 🗄️ Banco de Dados

Responsável por armazenar:

Usuários;
Emoções;
Registros emocionais (EventLog);

Opções:

SQLite (desenvolvimento) e 
SQL Server / PostgreSQL (produção)


### 5. Organização de Pastas (Backend)

    /src/DebugMeBackend
    │
    ├── Controllers        → Endpoints da API
    ├── Services           → Regras de negócio
    ├── Repositories       → Acesso a dados
    ├── Entities           → Modelos de domínio
    ├── Data               → Contexto e configuração do banco
    ├── DTOs               → Objetos de transferência de dados
    └── Program.cs         → Configuração da aplicação
### 6. Fluxo de Requisição

Exemplo de fluxo ao criar um registro emocional:

    Usuário preenche dados no frontend
     ↓
    Frontend envia requisição POST /eventlog
     ↓
    Controller recebe a requisição
     ↓
    Service valida e aplica regras de negócio
     ↓
    Repository persiste os dados
     ↓
    Banco armazena o registro
     ↓
    Resposta retorna ao frontend
### 7. Princípios Aplicados

O sistema foi projetado com base em **boas práticas de engenharia de software**:

* Separação de responsabilidades (SRP)
* Baixo acoplamento
* Alta coesão
* Código testável
* Facilidade de manutenção
* Escalabilidade futura
### 8. Escalabilidade e Evolução

A arquitetura atual permite evolução para:

    Microsserviços (se necessário)
    Autenticação com JWT
    Integração com IA (análise emocional)
    Deploy em nuvem (Render, Railway, Azure)
    Camada de caching (Redis)
    Observabilidade (logs e métricas)
### 9. Decisões Arquiteturais
    Decisão	                    Justificativa

    Monólito modular	        Simplicidade inicial + evolução controlada
    API REST	                Compatibilidade com múltiplos clientes
    Separação frontend/backend	Escalabilidade e independência
    Arquitetura em camadas	    Organização e testabilidade
    Angular (SPA)	            Experiência rica e dinâmica
### 10. Considerações Finais

A arquitetura do DebugMe foi pensada para equilibrar:

- Simplicidade inicial
- Clareza estrutural
- Potencial de crescimento

O sistema não busca complexidade desnecessária, mas sim uma base sólida para evolução contínua, alinhada com práticas modernas de desenvolvimento web.