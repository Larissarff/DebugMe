# Contexto Diário - 08/06/2026

## Resumo do que foi feito

### 1. Correção do erro de login (Failed to fetch)

**Problema:** O frontend Angular estava chamando `https://localhost:5001/api/user/login` (URL de produção) em vez de `http://localhost:5165` (URL de desenvolvimento), resultando em `TypeError: Failed to fetch`.

**Causa raiz:** O Angular estava usando o arquivo `environment.ts` (produção) que tinha `apiUrl: 'https://localhost:5001'`. O `ng serve` usa a configuração `development` por padrão, que faz o fileReplacements trocar `environment.development.ts` por `environment.ts`. Como `environment.ts` tinha a URL errada, o frontend chamava o endpoint errado.

**Correções aplicadas:**

#### a) Arquivos de Environment
- **`src/debugme-frontend/src/environments/environment.ts`** - Alterado de `https://localhost:5001` para `http://localhost:5165`
- **`src/debugme-frontend/src/environments/environment.development.ts`** - Já estava com `http://localhost:5165` (confirmado correto)

#### b) CORS no Backend
- **`src/DebugMeBackend/Program.cs`** - Adicionado CORS policy `AllowFrontend` permitindo requisições de `http://localhost:4200` com qualquer header/method

#### c) Angular.json
- **`src/debugme-frontend/angular.json`** - Adicionado `fileReplacements` na configuração `production` para substituir `environment.development.ts` por `environment.ts`

### 2. Ações tomadas para corrigir

1. ✅ Verificado que os environments estão com a URL correta (`http://localhost:5165`)
2. ✅ Verificado que o `Program.cs` já tem a configuração de CORS
3. ✅ Verificado que o `angular.json` já tem o `fileReplacements`
4. ✅ Matado o processo do backend (PID 17576) com `taskkill /PID 17576 /F`
5. ✅ Reiniciado o backend com `dotnet run` - agora ouvindo em `http://localhost:5165`

### 3. Estado atual dos serviços

| Serviço | URL | Status |
|---------|-----|--------|
| Frontend (Angular) | `http://localhost:4200` | Rodando |
| Backend (.NET) | `http://localhost:5165` | Rodando |
| Swagger | `http://localhost:5165/swagger` | Rodando |

### 4. Fluxo de Login (corrigido)

1. Usuário acessa `http://localhost:4200` → redirecionado para `/login`
2. Usuário preenche email e senha → clica em "Entrar"
3. Frontend chama `POST http://localhost:5165/api/user/login` com `{ email, password }`
4. Backend valida com BCrypt → retorna `UserResponseDto` (id, name, email, createdAt)
5. Frontend salva usuário no `localStorage` (chave: `debugme_user`)
6. Redireciona para `/home`

### 5. Pendências

- [ ] Testar o fluxo de login completo após as correções
- [ ] Commitar e fazer push das alterações (environment.ts, Program.cs, angular.json)
- [ ] Implementar tela Home
- [ ] Implementar CRUD de EventLog no frontend

### 6. Branch atual

As alterações estão na branch `feature/frontend-initial-structure` (ou branch de login). Verificar com `git branch` e `git status` antes de commitar.

### 7. Comandos úteis

```bash
# Matar processo do backend (se precisar)
taskkill /F /IM dotnet.exe

# Iniciar backend
cd src/DebugMeBackend && dotnet run

# Iniciar frontend
cd src/debugme-frontend && npx ng serve

# Build frontend
cd src/debugme-frontend && npx ng build

# Rodar testes
cd tests/DebugMeBackend.Tests && dotnet test
```
