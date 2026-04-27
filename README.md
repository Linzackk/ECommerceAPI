# ECommerce API

API RESTful desenvolvida para uso interno de um sistema de E-Commerce, com foco em segurança, organização e boas práticas de desenvolvimento. O sistema permite o gerenciamento completo de **Usuários**, **Itens** e **Pedidos**, com autenticação e autorização baseadas em JWT.

---

## Tecnologias Utilizadas

| Tecnologia | Descrição |
|---|---|
| **C# / .NET 8** | Linguagem e framework principal |
| **SQL Server** | Banco de dados relacional |
| **JWT (JSON Web Token)** | Autenticação e autorização |
| **BCrypt** | Hash seguro de senhas |
| **xUnit / Integration Tests** | Testes unitários e de integração |

---

## Arquitetura

O projeto segue uma **arquitetura em camadas**, garantindo separação de responsabilidades e facilidade de manutenção:

```
ECommerceAPI/
├── Controllers/      # Recebe as requisições HTTP e delega para os Services
├── Services/         # Contém as regras de negócio
├── Repositories/     # Comunicação com o banco de dados
├── Models/           # Entidades do domínio
├── DTOs/             # Objetos de transferência de dados
├── Policies/         # Políticas customizadas de autorização
└── ECommerce.Tests/  # Testes unitários e de integração
```

---

## Segurança

- **Autenticação via JWT**: Todos os endpoints protegidos exigem um token válido no header `Authorization: Bearer <token>`.
- **Autorização baseada em Claims**: Cada endpoint valida os claims do token, garantindo que apenas o próprio usuário (ou um administrador) possa acessar seus próprios dados.
- **Policies customizadas**: Foram criadas políticas de autorização específicas para cada recurso, garantindo controle granular de acesso.
- **Senhas protegidas com BCrypt**: As senhas nunca são armazenadas em texto puro — são sempre hasheadas antes de persistidas no banco.

---

## Endpoints

### Login (`/api/Login`)

| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| `POST` | `/api/Login` | Público | Realiza login e retorna o token JWT |
| `GET` | `/api/Login` | Admin | Lista todos os registros de login |

---

### Usuarios (`/api/Usuarios`)

| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| `POST` | `/api/Usuarios` | Público | Cria um novo usuário |
| `GET` | `/api/Usuarios` | Admin | Lista todos os usuários |
| `GET` | `/api/Usuarios/{id}` | Próprio usuário / Admin | Retorna dados de um usuário |
| `PATCH` | `/api/Usuarios/{id}` | Próprio usuário / Admin | Atualiza dados de um usuário |
| `DELETE` | `/api/Usuarios/{id}` | Próprio usuário / Admin | Remove um usuário |

> Para criar uma conta administrador, cadastre um usuario utilizando o e-mail `admin@admin.com`. Contas criadas com esse e-mail recebem automaticamente a role de administrador.

---

### Itens (`/api/Itens`)

| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| `GET` | `/api/Itens` | Público | Lista todos os itens disponíveis |
| `GET` | `/api/Itens/{id}` | Público | Retorna um item específico |
| `POST` | `/api/Itens` | Admin | Cria um novo item |
| `PATCH` | `/api/Itens/{id}` | Admin | Atualiza um item |
| `DELETE` | `/api/Itens/{id}` | Admin | Remove um item |

---

### Pedidos (`/api/Pedidos`)

| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| `POST` | `/api/Pedidos` | Usuário autenticado | Cria um novo pedido |
| `GET` | `/api/Pedidos/{usuarioId}` | Usuário autenticado | Lista todos os pedidos de um usuário |
| `PATCH` | `/api/Pedidos/{pedidoId}` | Dono do pedido / Admin | Finaliza um pedido |
| `DELETE` | `/api/Pedidos/{pedidoId}` | Dono do pedido / Admin | Remove um pedido |
| `POST` | `/api/Pedidos/{pedidoId}/Itens` | Dono do pedido / Admin | Adiciona um item ao pedido |
| `GET` | `/api/Pedidos/{pedidoId}/Itens` | Dono do pedido / Admin | Retorna os itens de um pedido |
| `PATCH` | `/api/Pedidos/{pedidoId}/Itens` | Dono do pedido / Admin | Atualiza um item no pedido |
| `DELETE` | `/api/Pedidos/{pedidoId}/Itens/{pedidoItemId}` | Dono do pedido / Admin | Remove um item do pedido |

---

## Testes

O projeto foi desenvolvido com uma abordagem orientada a testes, garantindo qualidade e confiabilidade em cada funcionalidade:

- **Testes Unitários**: Cada serviço foi testado de forma isolada, com mocks dos repositórios.
- **Testes de Integração**: Os endpoints foram validados end-to-end, simulando o fluxo real da aplicação.
- **Isolamento**: Os testes são independentes entre si, sem compartilhamento de estado.

---

## Como Executar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads)

### Configuração

1. Clone o repositório:
   ```bash
   git clone https://github.com/Linzackk/ECommerceAPI.git
   cd ECommerceAPI
   ```

2. Configure a string de conexão no arquivo `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=SEU_SERVIDOR;Database=ECommerceDB;Trusted_Connection=True;"
     },
     "Jwt": {
       "Key": "SUA_CHAVE_SECRETA",
       "Issuer": "ECommerceAPI"
     }
   }
   ```

3. Aplique as migrations e inicie a aplicação:
   ```bash
   dotnet ef database update
   dotnet run --project ECommerce
   ```

### Executando os Testes

```bash
dotnet test
```

---

## Versionamento

O projeto foi desenvolvido com versionamento estruturado no Git:

- **Commits semânticos**: mensagens claras e padronizadas (feat, fix, test, refactor...)
- **Branches por funcionalidade**: cada módulo foi desenvolvido em sua própria branch antes de ser integrado à `main`
- **Histórico limpo**: 220+ commits documentando a evolução do projeto

---

## Licença

Este projeto está licenciado sob a [MIT License](LICENSE).

---

## Autor

Desenvolvido por **[Linzackk](https://github.com/Linzackk)** — estudante de desenvolvimento de software em busca da primeira oportunidade como desenvolvedor júnior, estagiário ou freelancer.

[![GitHub](https://img.shields.io/badge/GitHub-Linzackk-181717?style=flat&logo=github)](https://github.com/Linzackk)
