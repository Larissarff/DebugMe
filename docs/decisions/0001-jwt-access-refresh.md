# ADR 0001: Autenticação com access token + refresh token (JWT)

## Status
Aceita

## Contexto
A aplicação precisava de autenticação stateless para uma SPA Angular. O frontend é um cliente separado, sem cookies de sessão. O projeto é solo — implementar um provider externo (Auth0, Firebase Auth) adicionaria complexidade operacional sem benefício proporcional. Era necessário que o token carregasse claims do usuário (id, email) para que os controllers lessem o `UserId` diretamente do token, sem consulta extra ao banco em cada requisição.

## Decisão
Usar **JWT com access token + refresh token** nativos do ASP.NET Core (`Microsoft.AspNetCore.Authentication.JwtBearer`). O fluxo é:

1. Login/cadastro retorna `TokenResponseDto` com `Token` (access, 60 min), `RefreshToken` (opaco, 7 dias) e `ExpiresAt`.
2. O frontend armazena o objeto `User` em `localStorage` (chave `debugme_user`). O `AuthGuard` lê dali para decidir se a rota é acessível — não há validação criptográfica no cliente.
3. O backend valida issuer, audience, lifetime e assinatura HMAC-SHA256 via `TokenValidationParameters` em `Program.cs`.
4. O refresh token é um GUID armazenado na coluna `RefreshToken` do `User` com `RefreshTokenExpiry`. Ao rotacionar, o anterior é substituído (não há lista de refresh tokens por usuário).

## Alternativas consideradas

| Alternativa | Por que foi descartada |
|---|---|
| Cookies de sessão (`HttpContext.SignInAsync`) | Incompatível com SPA em origem diferente — exige SameSite=None + Secure, e o fluxo de refresh fica menos explícito. |
| OAuth2 / OpenID Connect com provider externo | Overhead de configuração para um projeto solo com um único cliente. Seria considerado se houvesse múltiplos clients ou login social. |
| JWT sem refresh token (access de longa duração) | Risco de token vazado sem mecanismo de revogação. Refresh token permite access tokens curtos (60 min) e rotação a cada uso. |
| Token no `HttpOnly` cookie em vez de `localStorage` | Exigiria BFF (Backend for Frontend) para gerenciar o cookie, adicionando uma camada que o projeto não tem. |

## Consequências

**Positivas**
- Autenticação stateless — nenhuma sessão no servidor.
- O `EventLogController` extrai `UserId` de `ClaimTypes.NameIdentifier` sem query extra ao banco (`GetUserIdFromToken()`).
- 47 testes unitários mockam a camada de serviço sem tocar em autenticação — a validação de token é preocupação do middleware, não da lógica de negócio.

**Negativas / trade-offs aceitos**
- `localStorage` é vulnerável a XSS. O risco é mitigado pelo escopo do projeto (usuário único, sem injeção de scripts de terceiros), mas não é adequado para produção multi-tenant sem CSP rigorosa.
- Um único refresh token por usuário — fazer login em dois dispositivos invalida o token do anterior (rotação sem lista).
- Não há endpoint de logout que invalide tokens no servidor — o "logout" destrói o `localStorage` e redireciona com `window.location.href`.

## Notas de implementação
- `Microsoft.AspNetCore.Authentication.JwtBearer` 9.0.0
- Configuração em `appsettings.json`: `Jwt:Secret`, `Jwt:Issuer`, `Jwt:Audience`, `Jwt:AccessTokenExpiryMinutes` (60), `Jwt:RefreshTokenExpiryDays` (7)
- `User.RefreshToken` e `User.RefreshTokenExpiry` são `[JsonIgnore]` — nunca expostos em respostas da API
- `UserService.GenerateJwtToken()` em `src/DebugMeBackend/Services/UserService.cs:187`
- `Program.cs:32-49` — configuração do `AddJwtBearer`
- Middleware em `Program.cs:130-137`: `UseAuthentication()` antes de `UseAuthorization()`
