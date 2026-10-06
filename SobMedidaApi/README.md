# SobMedidaApi

API REST do **Sob Medida**. Responsável por autenticação, armazenamento do perfil profissional, importação de currículos em PDF e geração de currículos personalizados com o Google Gemini.

Para a visão geral do projeto, veja o [README principal](../README.md).

## Tecnologias

- **.NET 8** / ASP.NET Core (controllers)
- **Entity Framework Core 8** + **PostgreSQL** (Npgsql)
- **ASP.NET Core Identity** para usuários e senhas
- **JWT Bearer** para autenticação
- **PdfPig** para extrair texto de PDFs
- **Google Gemini** (API REST `generateContent`) para a IA
- **Swagger** (somente em ambiente de desenvolvimento)

## Estrutura

```
SobMedidaApi/
├── Controllers/   # Endpoints HTTP (auth, perfil, importação, currículos)
├── DTOs/          # Objetos de entrada e saída da API
├── Data/          # AppDbContext (EF Core)
├── Migrations/    # Migrations do banco (aplicadas automaticamente ao iniciar)
├── Models/        # Entidades do banco e modelos das respostas da IA
├── Services/      # Regras de negócio e integração com o Gemini
└── Program.cs     # Configuração: banco, Identity, JWT, CORS, rate limiting
```

### Serviços

| Serviço | Responsabilidade |
|---|---|
| `TokenService` | Gera o JWT após o login |
| `ResumeGeneratorService` | Cliente HTTP do Gemini — todas as chamadas à IA passam por ele |
| `JobExtractionService` | Extrai da descrição da vaga: requisitos, responsabilidades, palavras-chave ATS, nível e formação |
| `PromptBuilderService` | Monta os prompts de extração da vaga, geração e validação do currículo |
| `ResumeValidationService` | Confere se o currículo gerado não inventou dados que não existem no perfil |
| `PdfExtractionService` | Extrai o texto de um PDF com PdfPig |
| `ImportPromptBuilderService` | Monta o prompt que transforma o texto do PDF em dados de perfil |
| `ImportService` | Prévia e confirmação da importação, evitando duplicar itens |
| `PortugueseIdentityErrorDescriber` | Traduz as mensagens de erro do Identity para português |

## Como funciona

### Geração de currículo (`POST /resume/generate`)

1. **Extração da vaga** — o Gemini transforma a descrição da vaga em dados estruturados (`ExtractedJobInfo`). Se nenhum requisito for identificado, a API responde `400` pedindo uma descrição mais completa.
2. **Montagem do prompt** — o perfil completo do usuário é combinado com os dados da vaga.
3. **Geração** — o Gemini devolve o currículo em JSON.
4. **Validação** — uma segunda chamada ao Gemini atua como auditor e verifica se há empresas, cargos, habilidades, cursos, projetos, certificações ou datas inventadas. Reformulações de texto são permitidas; fatos novos, não.
5. Se o JSON for inválido ou a validação falhar, a geração é repetida **uma vez**. Se falhar de novo, a API responde `500` (JSON inválido) ou `422` com a lista de inconsistências.
6. O currículo validado é salvo no histórico do usuário.

### Importação de PDF (`/import`)

1. `POST /import/preview` recebe o PDF (somente `.pdf`, até 5 MB), extrai o texto (limitado a 8.000 caracteres) e pede ao Gemini que o estruture.
2. A resposta é uma **prévia**: os dados extraídos e a contagem do que é novo ou será atualizado. Nada é gravado nesta etapa.
3. `POST /import/confirm` recebe os dados revisados pelo usuário e grava no perfil. Experiências da mesma empresa são atualizadas; formações, habilidades, projetos e certificações já existentes (comparação pelo nome, sem diferenciar maiúsculas) são ignorados.

## Endpoints

Todos os endpoints, exceto `register` e `login`, exigem o header `Authorization: Bearer <token>`. Todas as consultas são filtradas pelo usuário do token: um usuário nunca acessa dados de outro.

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/auth/register` | Cria a conta (nome, e-mail, senha) e um perfil pessoal inicial |
| `POST` | `/auth/login` | Retorna o JWT, e-mail e nome |
| `GET` | `/auth/me` | Dados do usuário logado |
| `GET` `POST` `PUT` | `/profile/personal-info` | Dados pessoais e endereço |
| `GET` `POST` | `/profile/experiences` | Lista / cria experiências |
| `PUT` `DELETE` | `/profile/experiences/{id}` | Atualiza / remove uma experiência |
| `GET` `POST` | `/profile/educations` | Lista / cria formações |
| `PUT` `DELETE` | `/profile/educations/{id}` | Atualiza / remove uma formação |
| `GET` `POST` | `/profile/skills` | Lista / cria habilidades |
| `PUT` `DELETE` | `/profile/skills/{id}` | Atualiza / remove uma habilidade |
| `GET` `POST` | `/profile/projects` | Lista / cria projetos |
| `PUT` `DELETE` | `/profile/projects/{id}` | Atualiza / remove um projeto |
| `GET` `POST` | `/profile/certifications` | Lista / cria certificações |
| `PUT` `DELETE` | `/profile/certifications/{id}` | Atualiza / remove uma certificação |
| `POST` | `/import/preview` | Envia um PDF e recebe a prévia da importação (`multipart/form-data`, campo `file`) |
| `POST` | `/import/confirm` | Grava os dados importados no perfil |
| `POST` | `/resume/generate` | Gera um currículo para a vaga (`jobTitle` até 200 e `jobDescription` até 15.000 caracteres) |
| `GET` | `/resume/history` | Lista os currículos gerados |
| `GET` `DELETE` | `/resume/history/{id}` | Detalha / remove um currículo do histórico |

Com a API rodando em desenvolvimento, a documentação interativa fica em `http://localhost:5076/swagger`.

## Segurança

| Proteção | Configuração |
|---|---|
| Senha | Mínimo de 8 caracteres, com pelo menos um número |
| Bloqueio de conta | 15 minutos após 5 senhas incorretas |
| Rate limit — `login` e `register` | 10 requisições por minuto, por IP |
| Rate limit — `resume/generate` e `import/preview` | 20 requisições por hora, por usuário |
| CORS | Somente `https://sob-medida.vercel.app` |
| Swagger | Desativado fora do ambiente `Development` |

Ao exceder um limite, a API responde `429` com uma mensagem em português. Os contadores ficam em memória e são reiniciados quando a API reinicia. Erros internos são registrados no log e nunca devolvidos em detalhe ao cliente.

## Configuração

Nenhum segredo fica no repositório. O `appsettings.json` traz apenas valores vazios, que devem ser preenchidos por **variáveis de ambiente** (produção) ou **user-secrets** (desenvolvimento):

| Chave | Variável de ambiente | Descrição |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | `ConnectionStrings__DefaultConnection` | Connection string do PostgreSQL |
| `JwtSettings:SecretKey` | `JwtSettings__SecretKey` | Chave de assinatura do JWT — **mínimo de 32 bytes**; a API não inicia sem ela |
| `GeminiSettings:ApiKey` | `GeminiSettings__ApiKey` | Chave da API do Google Gemini |

Também configuráveis no `appsettings.json` (sem segredos): `JwtSettings:ExpirationHours` (validade do token), `GeminiSettings:Model` (modelo utilizado) e `GeminiSettings:ApiUrl`.

## Rodando localmente

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- PostgreSQL 14+ (local ou via Docker)
- Uma chave da API do Gemini: https://aistudio.google.com/apikey

### Passo a passo

1. **Suba um PostgreSQL** (exemplo com Docker):
   ```bash
   docker run -d --name sobmedida-db -e POSTGRES_PASSWORD=postgres -p 5432:5432 postgres:16
   ```

2. **Configure os segredos** dentro da pasta `SobMedidaApi/`:
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=SobMedida_db;Username=postgres;Password=postgres"
   dotnet user-secrets set "JwtSettings:SecretKey" "$(openssl rand -base64 64)"
   dotnet user-secrets set "GeminiSettings:ApiKey" "SUA_CHAVE_DO_GEMINI"
   ```
   Os user-secrets ficam fora do repositório, no seu diretório de usuário, e só são carregados em ambiente `Development`.

3. **Rode a API**:
   ```bash
   dotnet run
   ```
   As migrations são aplicadas automaticamente na inicialização, criando as tabelas. A API sobe em `http://localhost:5076` e o Swagger em `http://localhost:5076/swagger`.

### Usando com o App local

O CORS libera apenas o domínio de produção. Para usar a API com o App rodando em `http://localhost:4200`, adicione essa origem temporariamente em `Program.cs`:

```csharp
policy.WithOrigins("https://sob-medida.vercel.app", "http://localhost:4200")
```

E aponte o App para a API local (veja o [README do App](../SobMedidaApp/README.md)). Não faça commit dessas alterações.

### Migrations

Para alterar o modelo de dados, crie uma nova migration (requer a ferramenta `dotnet-ef`):

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add NomeDaMigration
```

Ela será aplicada automaticamente na próxima inicialização, inclusive em produção.

## Deploy

A API é publicada no **Railway** automaticamente a cada push na branch `main`. As três variáveis da tabela de [Configuração](#configuração) precisam estar definidas no serviço do Railway. Como `ASPNETCORE_ENVIRONMENT` não é definido, a API roda como `Production` (Swagger desativado).
