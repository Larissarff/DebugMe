# ADR 0002: Separação Repository / Service

## Status
Aceita

## Contexto
A autora tem experiência prévia com separação de responsabilidades em arquitetura limpa e valoriza essa divisão por facilitar manutenabilidade e leitura — mesmo em um projeto solo. Desde a primeira linha de código, a estrutura Controller → Service → Repository foi adotada como padrão, evitando que lógica de negócio, acesso a dados e tratamento HTTP se misturassem em uma única camada.

Além da organização, havia um objetivo prático: os testes unitários deveriam ser rápidos e independentes de infraestrutura, sem precisar subir container de banco para rodar `dotnet test`.

## Decisão
Separar em três camadas com injeção de dependência:

```
Controller → Service (regra de negócio) → Repository (interface) → EF Core
```

- **Controllers** (`UserController`, `EmotionController`, `EventLogController`): recebem HTTP, delegam para o Service, retornam status codes + DTOs. Não conhecem EF Core.
- **Services** (`UserService`, `EmotionService`, `EventLogService`): validam regras de negócio (ex.: intensidade entre 1-10, e-mail único, data não futura), chamam repositórios via interface. Injetam `IConfiguration` quando precisam de valores do `appsettings.json`.
- **Repositories** (`IUserRepository`/`UserRepository`, `IEmotionRepository`/`EmotionRepository`, `IEventLogRepository`/`EventLogRepository`): abstraem o `AppDbContext`. Cada repositório expõe métodos como `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`. O `EventLogService` injeta três interfaces (`IEventLogRepository`, `IUserRepository`, `IEmotionRepository`) para validar relacionamentos sem quebrar o encapsulamento.

A DI é configurada em `Program.cs:102-107` — todos registrados como `AddScoped`.

## Alternativas consideradas

| Alternativa | Por que foi descartada |
|---|---|
| Controller acessar `AppDbContext` diretamente | Mais rápido de escrever (1 arquivo por feature), mas impossível de testar sem banco real. Violava SRP e o controller ficava com 3-4 responsabilidades. |
| Service único ("God Service") sem repositórios | Menos arquivos, mas acoplava regra de negócio ao provider de dados. Trocar de ORM ou banco exigiria reescrever o serviço inteiro. |
| Padrão CQRS com MediatR | Overhead de biblioteca e indireção extra (`IRequest<T>` + handlers) para um domínio com 3 entidades. Não trazia benefício proporcional à complexidade adicionada. |

## Consequências

**Positivas**
- Os 47 testes unitários (`tests/DebugMeBackend.Tests/Services/`) mockam `IUserRepository`, `IEmotionRepository` e `IEventLogRepository` com **Moq**, sem instanciar `AppDbContext` nem EF Core. Todos rodam em < 2 segundos.
- `EventLogServiceTests` usa helpers `CreateMocks()` e `CreateService()` que injetam 3 mocks — o padrão escala bem para serviços com múltiplas dependências.
- Trocar o ORM ou o banco afeta apenas a camada de repositório e o `AppDbContext` — os serviços não sabem se os dados vêm de SQLite, PostgreSQL ou MySQL.

**Negativas / trade-offs aceitos**
- 3 interfaces + 3 implementações de repositório = 6 arquivos para 3 entidades. Para um domínio pequeno, é mais indireção do que o estritamente necessário.
- Curva de entrada: alguém lendo o código pela primeira vez precisa entender a DI chain (`Controller → Service → IRepository → Repository`) antes de conseguir rastrear uma requisição completa.
- `EventLogService.UpdateAsync()` acessa `IEmotionRepository` diretamente para validar `EmotionId` — a separação não é pura (um serviço depende de repositórios de outras entidades), mas evita duplicar validação em camada superior.

## Notas de implementação
- Interfaces: `src/DebugMeBackend/Repositories/Interfaces/IUserRepository.cs`, `IEmotionRepository.cs`, `IEventLogRepository.cs`
- Implementações: `src/DebugMeBackend/Repositories/UserRepository.cs`, `EmotionRepository.cs`, `EventLogRepository.cs`
- Services: `src/DebugMeBackend/Services/UserService.cs`, `EmotionService.cs`, `EventLogService.cs`
- Testes: `tests/DebugMeBackend.Tests/Services/UserServiceTests.cs`, `EmotionServiceTests.cs`, `EventLogServiceTests.cs`
- DI: `src/DebugMeBackend/Program.cs:102-107`
