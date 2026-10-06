# SobMedidaApp

Aplicação web do **Sob Medida**, feita em Angular. É a interface onde o usuário monta seu perfil profissional, importa um currículo em PDF, gera currículos adaptados a cada vaga e baixa o resultado em PDF.

Toda a lógica de IA e os dados ficam na [API](../SobMedidaApi/README.md); o App consome essa API. Para a visão geral do projeto, veja o [README principal](../README.md).

🔗 **Produção:** https://sob-medida.vercel.app

## Tecnologias

- **Angular 21** (componentes standalone)
- **Angular Material** para a interface
- **jsPDF** para gerar o PDF do currículo no navegador
- **ngx-mask** para a máscara do campo de telefone
- **ViaCEP** para preencher o endereço a partir do CEP
- **Vitest** para testes unitários

## Telas

| Rota | Tela | Acesso |
|---|---|---|
| `/login` | Login | Pública |
| `/register` | Cadastro | Pública |
| `/dashboard` | Painel com atalhos para perfil, geração de currículo e importação | Logado |
| `/profile` | Perfil em abas: Pessoal, Experiência, Educação, Habilidades, Projetos e Certificações | Logado |
| `/import` | Importação de currículo em PDF | Logado |
| `/resume` | Abas **Gerar currículo** e **Histórico** | Logado |

A rota raiz (`/`) redireciona para `/login`.

## Estrutura

```
src/app/
├── core/
│   ├── guards/         # authGuard: bloqueia rotas sem login válido
│   ├── interceptors/   # jwtInterceptor: envia o token em toda requisição
│   ├── models/         # Interfaces TypeScript (auth, perfil, importação, currículo)
│   ├── services/       # Comunicação com a API e geração do PDF
│   └── material.module.ts
├── features/
│   ├── auth/           # login e register
│   ├── dashboard/
│   ├── profile/        # uma aba (e um dialog de edição) por seção do perfil
│   ├── import/
│   └── resume/         # resume-generator e resume-history
├── app.config.ts       # Providers: router, HttpClient com interceptor, animações
└── app.routes.ts
```

### Serviços

| Serviço | Responsabilidade |
|---|---|
| `AuthService` | Login, cadastro, logout, leitura do token e do nome do usuário |
| `ProfileService` | CRUD de todas as seções do perfil |
| `ImportService` | Envia o PDF para a prévia e confirma a importação |
| `ResumeService` | Gera currículos e consulta/remove o histórico |
| `ResumeParserService` | Converte o JSON gerado pela IA no modelo exibido na tela |
| `PdfBuilderService` | Monta o PDF A4 do currículo com jsPDF e dispara o download |

## Como funciona

### Autenticação

1. No login, a API devolve um **JWT**, que é salvo no `localStorage`.
2. O `jwtInterceptor` adiciona `Authorization: Bearer <token>` em todas as requisições.
3. O `authGuard` lê a data de expiração do próprio token: se não houver token ou ele estiver vencido, redireciona para `/login`.

### Geração de currículo

1. O usuário informa o título da vaga e cola a descrição.
   - O formulário mostra um contador de caracteres. Se a descrição passar de **15.000 caracteres**, aparece um aviso indicando quanto remover e o botão de gerar fica desabilitado. O texto colado nunca é cortado sem aviso.
2. O App chama `POST /resume/generate`. A API extrai os requisitos da vaga, gera o currículo e valida que nada foi inventado. Isso leva alguns segundos.
3. O resultado é exibido como prévia e pode ser baixado em PDF. O PDF é gerado **no navegador**, sem nova chamada à API.
4. Os currículos gerados ficam na aba **Histórico**, de onde podem ser reabertos, baixados ou excluídos.

### Importação de PDF

A tela de importação tem três etapas:

1. **Upload** — o usuário escolhe um PDF (até 5 MB).
2. **Prévia** — a API devolve os dados extraídos e um resumo do que é novo ou será atualizado. O usuário revisa antes de confirmar.
3. **Concluído** — os dados confirmados são gravados no perfil.

### Mensagens de erro

As mensagens vêm da API em português, no campo `message` da resposta, e são exibidas em snackbars. Se o usuário atingir o limite de requisições, verá a mensagem de "Muitas requisições" devolvida pela API.

## Rodando localmente

### Pré-requisitos

- Node.js 20.19+, 22.12+ ou 24+ (exigência do Angular 21)
- npm

### Passo a passo

```bash
npm install
npm start
```

A aplicação abre em `http://localhost:4200` e recarrega automaticamente a cada alteração.

### Qual API o App usa?

A URL da API está fixa em cada serviço de `src/app/core/services/` (`auth`, `profile`, `import` e `resume`), apontando para a produção:

```ts
private apiUrl = 'https://sob-medida-production.up.railway.app/...';
```

A API de produção só aceita requisições vindas de `https://sob-medida.vercel.app` (CORS). Por isso, rodando o App em `localhost`, o navegador bloqueia as chamadas. Para desenvolver localmente:

1. Rode a [API localmente](../SobMedidaApi/README.md#rodando-localmente) e libere `http://localhost:4200` no CORS dela.
2. Troque o início da URL nos quatro serviços para `http://localhost:5076`.
3. **Não faça commit** dessas alterações, porque o deploy da `main` é automático.

## Scripts

| Comando | O que faz |
|---|---|
| `npm start` | Servidor de desenvolvimento em `http://localhost:4200` |
| `npm run build` | Build de produção em `dist/SobMedidaApp` |
| `npm run watch` | Build de desenvolvimento a cada alteração |
| `npm test` | Testes unitários com Vitest |

## Deploy

O App é publicado na **Vercel** automaticamente a cada push na branch `main`, usando o build de produção (`ng build`). Não há variáveis de ambiente nem segredos no front-end: tudo o que é sensível fica na API.
