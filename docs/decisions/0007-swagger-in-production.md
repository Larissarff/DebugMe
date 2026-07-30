# ADR 0007: Swagger habilitado em produção

## Status
Aceita (com ressalva consciente)

## Contexto
O DebugMe é um projeto de portfólio — precisa ser demonstrável sem exigir que quem avalia clone o repositório e rode localmente. O Swagger (`/swagger`) serve como documentação viva da API e permite testar endpoints diretamente do navegador, sem Postman ou curl. Em um contexto de entrevista técnica, a pessoa avaliadora pode abrir a URL do Render, acessar `/swagger`, criar um usuário e testar os endpoints em segundos.

Ao mesmo tempo, habilitar Swagger em produção é uma decisão que, em um sistema com dados reais de usuários, seria reavaliada — expõe o schema completo da API publicamente, incluindo endpoints que exigem autenticação.

## Decisão
**Habilitar Swagger em desenvolvimento e produção**, sem restrição de acesso:

```csharp
// Program.cs:120-128
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "DebugMe API v1");
        options.RoutePrefix = "swagger";
    });
}
```

O Swagger documenta todos os endpoints (User, Emotion, EventLog), incluindo os protegidos por `[Authorize]`. O esquema de segurança JWT está configurado (`AddSecurityDefinition("Bearer", ...)`) — o Swagger UI permite colar um token no campo "Authorize" e testar endpoints autenticados.

A condição `IsDevelopment() || IsProduction()` garante que o Swagger funcione tanto localmente quanto no Render, mas estaria desabilitado em um hipotético ambiente "Staging" (se `ASPNETCORE_ENVIRONMENT` fosse diferente). Na prática, como o Render usa `Production`, funciona.

## Alternativas consideradas

| Alternativa | Por que foi descartada |
|---|---|
| Swagger só em desenvolvimento (`IsDevelopment()`) | O projeto perderia a capacidade de demonstração remota. A pessoa avaliadora precisaria clonar, instalar .NET SDK e rodar localmente — fricção desnecessária para um portfólio. |
| Swagger em produção mas protegido por autenticação básica (usuário/senha) | Exigiria implementar middleware de autenticação adicional ou configurar o Render com autenticação no proxy — complexidade extra para um ganho marginal de segurança em um projeto sem dados reais de terceiros. |
| Remover Swagger e usar apenas um arquivo estático de documentação (OpenAPI spec) | Perde a interatividade — o Swagger UI permite testar endpoints sem ferramenta externa, o que é o principal valor para demonstração. |
| Manter Swagger mas restringir os esquemas expostos (só endpoints públicos) | O Swashbuckle não tem suporte nativo para isso sem configuração complexa de `IDocumentFilter`. O esforço não se justifica para o estágio atual. |

## Consequências

**Positivas**
- API totalmente testável via navegador em `https://debugme-601s.onrender.com/swagger` — zero ferramentas externas necessárias.
- O `Authorize` button no Swagger UI aceita o token JWT retornado pelo endpoint de login — fluxo completo de autenticação demonstrável em 30 segundos.
- A documentação fica sempre sincronizada com o código (gerada a partir dos controllers e DTOs), sem risco de divergência entre spec e implementação.

**Negativas / trade-offs aceitos (com consciência explícita)**

1. **Exposição do schema completo da API** — qualquer pessoa com a URL pode ver todos os endpoints, parâmetros e modelos. Em um sistema com dados reais de usuários, isso facilitaria reconhecimento e ataques direcionados. **Se o projeto evoluísse para produção multi-tenant com dados sensíveis, esta decisão seria reavaliada** — as opções seriam: (a) desabilitar Swagger em produção, (b) proteger `/swagger` com autenticação básica no proxy reverso (Render), (c) restringir por IP, ou (d) manter apenas o `swagger.json` acessível e remover a UI.

2. **Swagger não exige autenticação para ser acessado** — o endpoint `/swagger` em si não passa pelo middleware `[Authorize]`, então qualquer pessoa pode ver a documentação mesmo sem token. Isso é intencional para demonstração, mas seria inaceitável em produção com dados reais.

3. **Potencial de uso abusivo** — a UI permite enviar requisições reais para a API. Com a connection string do PostgreSQL do Render, um atacante poderia criar usuários e poluir o banco. O risco é mitigado pelo fato de que: (a) o banco é gratuito e não contém dados reais, (b) o tier gratuito do Render já impõe limitações, (c) um reset do banco é trivial (`EnsureCreated()` recria o schema no próximo deploy).

## Notas de implementação
- `Program.cs:53-79` — configuração do Swagger (`AddSwaggerGen` com `AddSecurityDefinition` e `AddSecurityRequirement`)
- `Program.cs:120-128` — condição `IsDevelopment() || IsProduction()` para habilitar Swagger UI
- `Swashbuckle.AspNetCore` 10.1.7
- O Swagger documenta o esquema de segurança JWT (`Bearer`) para que o UI tenha o botão "Authorize"
