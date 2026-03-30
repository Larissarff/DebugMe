# Modelagem Inicial do Banco de Dados

## Entidades

### User
Representa o usuário do sistema, atualmente simplificado.

### Emotion
Representa categorias emocionais associadas aos eventos registrados.

### EventLog
Registro de eventos emocionais do usuário.

## Relacionamentos

- Um User possui vários EventLogs
- Um EventLog pertence a um User
- Um EventLog possui uma Emotion
