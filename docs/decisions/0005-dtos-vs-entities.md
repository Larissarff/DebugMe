# ADR 0005: DTOs em vez de expor entidades EF diretamente

## Status
Aceita

## Contexto
As entidades do EF Core (`User`, `Emotion`, `EventLog`) contêm campos que não devem ser expostos na API: `PasswordHash`, `RefreshToken`, `RefreshTokenExpiry`. Além disso, `Emotion` tem uma relação de navegação `ICollection<EventLog>` que, ao serializar, gera referência circular (`Emotion → EventLog → Emotion`). O `System.Text.Json` consegue quebrar o ciclo com `ReferenceHandler.IgnoreCycles`, mas isso é um paliativo — a forma do JSON de saída fica acoplada ao grafo de objetos do EF.

A autora tem experiência com separação entre contrato de API e modelo de persistência e adotou DTOs desde o início como prática de arquitetura limpa. A questão não era *se* usar DTOs, mas *quanta* consistência aplicar: alguns controllers (`EmotionController`) aceitam e retornam a entidade `Emotion` diretamente, enquanto `UserController` usa DTOs em todas as operações.

## Decisão
Criar **DTOs de request e response** separados das entidades, com mapeamento explícito nos controllers ou services:

- **Request DTOs**: `CreateUserDto`, `LoginUserDto`, `UpdateUserDto`, `ChangePasswordDto`, `RefreshTokenRequestDto`, `CreateEventLogDto` — todos com `[Required]` e `[Range]` data annotations para validação no `ModelState`.
- **Response DTOs**: `UserResponseDto` (Id, Name, Email, CreatedAt — sem `PasswordHash`), `EventLogResponseDto` (com `EmotionInfoDto` e `UserInfoDto` aninhados, recortando apenas os campos relevantes), `TokenResponseDto` (Token, RefreshToken, ExpiresAt, User).
- **DTOs de agregacão**: `EmotionWithCountDto` para a query de emoções com contagem de eventos (`GetAllWithEventCountAsync`).

O `UserResponseDto` é construído via `MapToResponse()` no `UserService` — o service é o ponto único de mapeamento, nunca o controller.

**Inconsistência conhecida**: `EmotionController` retorna a entidade `Emotion` diretamente em `GetAll`, `GetById`, `GetByName`, `Create`, `Update`. Isso significa que `Emotion.EventLogs` (a collection de navegação) pode ser incluída na resposta se o EF fizer lazy loading ou se for explicitamente carregada. É um débito técnico documentado — o AGENTS.md registra: "When adding new endpoints for emotions/events, consider using DTOs to match the User pattern."

## Alternativas consideradas

| Alternativa | Por que foi descartada |
|---|---|
| Expor entidades EF diretamente em todos os endpoints | Menos arquivos, mas vaza `PasswordHash` e `RefreshToken` a menos que se use `[JsonIgnore]` — o que polui o modelo de domínio com preocupações de serialização. Além disso, a referência circular `Emotion ↔ EventLog` quebraria a serialização sem `ReferenceHandler.IgnoreCycles`. |
| Usar `[JsonIgnore]` em todas as propriedades sensíveis + `ReferenceHandler.IgnoreCycles` como estratégia única | Funciona, mas o contrato da API fica acoplado ao schema do banco. Toda migração de coluna vira uma breaking change na API. |
| AutoMapper / Mapster | Reduz código boilerplate de mapeamento, mas adiciona dependência externa e "mágica" que esconde erros de mapeamento em runtime. Para 3 entidades, o mapeamento manual é trivial e explícito. |

## Consequências

**Positivas**
- `UserResponseDto` nunca expõe `PasswordHash` ou `RefreshToken` — impossível vazar por acidente, mesmo que alguém remova o `[JsonIgnore]` da entidade.
- Request DTOs com `[Required]` e `[Range]` permitem validação declarativa no `ModelState` antes de chegar ao service.
- `EventLogResponseDto` com `EmotionInfoDto`/`UserInfoDto` aninhados entrega exatamente os campos que o frontend precisa, sem expor `User.PasswordHash` ou `Emotion.EventLogs`.
- Trocar o schema do banco (ex.: renomear coluna) não quebra o contrato da API — só o mapeamento interno precisa ser ajustado.

**Negativas / trade-offs aceitos**
- 10 arquivos de DTOs para 3 entidades. Para um domínio pequeno, é mais indireção do que o estritamente necessário.
- Inconsistência: `EmotionController` expõe entidades, `UserController` e `EventLogController` usam DTOs. Isso gera duas formas diferentes de fazer a mesma coisa no mesmo projeto — débito técnico a ser unificado.
- `CreateUserDto.Password` trafega em texto plano do frontend para o backend. Em produção multi-tenant, isso exigiria HTTPS obrigatório. O projeto atual confia no HTTPS do Render como proxy reverso, mas o DTO em si não tem proteção adicional (ex.: não usa `SecureString`).

## Notas de implementação
- Request DTOs: `src/DebugMeBackend/DTOs/User/CreateUserDto.cs`, `LoginUserDto.cs`, `UpdateUserDto.cs`, `ChangePasswordDto.cs`, `RefreshTokenRequestDto.cs`
- Response DTOs: `src/DebugMeBackend/DTOs/User/UserResponseDto.cs`, `TokenResponseDto.cs`
- EventLog DTOs: `src/DebugMeBackend/DTOs/EventLog/CreateEventLogDto.cs`, `EventLogResponseDto.cs` (inclui `EmotionInfoDto` e `UserInfoDto`)
- Emotion DTO: `src/DebugMeBackend/DTOs/Emotion/EmotionWithCountDto.cs`
- Mapeamento User: `UserService.MapToResponse()` (método privado)
- Mapeamento EventLog: `EventLogController.MapToDto()` (método estático no controller)
- `[JsonIgnore]` nas entidades: `User.PasswordHash`, `User.RefreshToken`, `User.RefreshTokenExpiry`
- `ReferenceHandler.IgnoreCycles` em `Program.cs:18` (paliativo para Emotion enquanto não migra para DTO)
