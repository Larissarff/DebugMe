# ADR 0006: Priorização de testes unitários sobre integração/e2e

## Status
Aceita

## Contexto
O projeto é desenvolvido por uma única pessoa, com tempo limitado entre sprints. Era necessário decidir onde investir o esforço de teste: testes de unidade (rápidos, isolados, sem infraestrutura), testes de integração (com banco real, testando repositórios + EF Core) ou testes end-to-end (frontend + backend + banco, simulando fluxo de usuário).

Testar tudo seria ideal, mas o custo de oportunidade era real — cada hora gasta escrevendo teste de integração era uma hora não gasta em feature, refatoração ou documentação. A pergunta não era "qual tipo de teste é melhor?", mas "qual tipo de teste entrega mais valor por unidade de esforço *neste estágio do projeto*?".

## Decisão
**Priorizar testes unitários nos Services**, cobrindo regras de negócio com mocks, e deixar testes de integração e e2e para depois.

A lógica é:

1. **Services concentram as regras de negócio** — validação de intensidade (1-10), unicidade de e-mail, data não futura, hash de senha, refresh token expiry. Bugs nessas regras têm maior impacto (dados inválidos no banco, falha de autenticação) e são mais baratos de pegar em teste unitário (sem subir banco, sem mockar HTTP).
2. **Repositories são majoritariamente delegates do EF Core** — `_context.Users.AddAsync(user)`, `_context.SaveChangesAsync()`. Testá-los com banco em memória (EF Core InMemory) valida o EF Core, não a lógica do projeto. Testá-los com banco real (teste de integração) valida o mapeamento e as queries, o que é valioso, mas menos provável de quebrar do que uma regra de negócio mal escrita.
3. **Controllers são finos** — delegam para o Service e mapeiam para DTO. O risco de bug é baixo comparado ao Service. Testes de controller exigiriam mockar `HttpContext`, `ModelState`, etc., com pouco retorno incremental sobre os testes de Service.

O backlog do `README.md` registra: "Dashboard com gráficos de padrões", "Sugestões automáticas de refatoração", "Testes de integração e e2e" — todos em `Em backlog / próximos passos`. A ordem não é acidental: features que entregam valor de produto vêm antes de testes de integração, mas testes unitários foram feitos junto com o código (não depois).

## Alternativas consideradas

| Alternativa | Por que foi descartada |
|---|---|
| Testar tudo (unit + integração + e2e) desde o início | Tempo inviável para projeto solo. A velocidade de entrega cairia drasticamente sem benefício proporcional no estágio MVP. |
| Pular testes unitários e fazer só integração | Testes de integração são mais lentos (banco real, setup/teardown) e tendem a validar caminho feliz. Testes unitários cobrem branches de erro (`ArgumentException`, `InvalidOperationException`, `null`) com muito menos código. |
| TDD estrito (teste antes do código) | A autora escreveu testes junto com o código, não antes. TDD exige design upfront das interfaces — custo cognitivo extra que não se pagou para um domínio que evoluiu durante as sprints. |
| Usar EF Core InMemory para testes de Service | Parece unitário, mas é teste de integração disfarçado — o InMemory provider não replica constraints de banco real (ex.: unique index). Testes passariam com dados que o PostgreSQL rejeitaria, dando falsa confiança. |

## Consequências

**Positivas**
- 47 testes unitários rodam em < 2 segundos (`dotnet test`). Feedback imediato durante desenvolvimento.
- Nenhuma infraestrutura externa necessária para rodar testes — basta clonar e `dotnet test`.
- Cobertura abrangente de branches de erro: `EventLogServiceTests` testa intensidade inválida, data futura, UserId vazio, EmotionId vazio, usuário/emoção inexistentes, update com entidade nula, update com registro inexistente. `UserServiceTests` testa criação, login, refresh token expirado, e-mail duplicado, normalização de e-mail.
- Mock com Moq torna explícito o contrato de cada dependência — qualquer mudança na interface do repositório quebra o teste em tempo de compilação.

**Negativas / trade-offs aceitos**
- **Zero testes de integração** — não há garantia de que as queries EF Core funcionam contra PostgreSQL real. O deploy no Render depende de `EnsureCreated()` e da confiança de que o mapeamento está correto. Risco: um `OnModelCreating` mal configurado só seria descoberto em produção.
- **Zero testes e2e** — regressões de fluxo completo (login → criar emoção → criar evento → visualizar dashboard) não são detectadas automaticamente.
- Testes de Service não validam o comportamento real do `AppDbContext` (tracking, lazy loading, SaveChanges). Um bug no repositório (ex.: esquecer de chamar `SaveChangesAsync`) não seria pego pelos testes unitários.
- A cobertura de `EmotionService` e `EventLogService` é boa, mas `UserServiceTests.RefreshTokenAsync` testa o fluxo feliz e expirado — não testa o cenário de token ausente (linha já coberta por mock, mas sem assert explícito de `null`).

## Notas de implementação
- 47 testes: `tests/DebugMeBackend.Tests/Services/UserServiceTests.cs` (14), `EmotionServiceTests.cs` (12), `EventLogServiceTests.cs` (21)
- Stack: xUnit 2.9.2, Moq 4.20.72, FluentAssertions 8.9.0, coverlet 6.0.2
- Padrão de mock: `EventLogServiceTests` usa helpers `CreateMocks()` e `CreateService()` — os outros arquivos de teste mockam inline
- Backlog registrado em `README.md`: "Testes de integração e e2e" como próximos passos
