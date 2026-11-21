# Sistema de Inscrição em Cursos — Blazor Web App

- **Controle de estado assíncrono** com `Task.Delay(2000)` e feedback visual no botão

---

## Sumário

1. [Pré-requisitos](#1-pré-requisitos)
2. [Estrutura do projeto](#2-estrutura-do-projeto)
3. [Passo a passo da resolução](#3-passo-a-passo-da-resolução)
4. [Como executar](#4-como-executar)
5. [Roteiro de testes](#5-roteiro-de-testes)
6. [Como criar o repositório e enviar para o GitHub](#6-como-criar-o-repositório-e-enviar-para-o-github)
7. [Observações técnicas](#7-observações-técnicas)

---

## 1. Pré-requisitos

| Ferramenta | Versão | Como conferir |
|---|---|---|
| .NET SDK | 8.0 ou superior (projeto criado no 9.0) | `dotnet --version` |
| Git | qualquer versão recente | `git --version` |
| Conta no GitHub | — | https://github.com |
| Editor | VS Code (com extensão C# Dev Kit) ou Visual Studio 2022 | — |

> Se você tiver só o .NET 8 instalado, abra o arquivo `SistemaInscricaoCursosBlazor.csproj` e troque `net9.0` por `net8.0`.

---

## 2. Estrutura do projeto

```
SistemaInscricaoCursosBlazor/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor          ← link "Inscrição" adicionado no menu
│   ├── Models/
│   │   └── InscricaoModel.cs      ← NOVO: modelo com DataAnnotations
│   ├── Pages/
│   │   ├── Home.razor
│   │   ├── Counter.razor
│   │   ├── Weather.razor
│   │   ├── Error.razor
│   │   └── Inscricao.razor        ← NOVO: página /inscricao
│   ├── App.razor
│   ├── Routes.razor
│   └── _Imports.razor
├── Properties/launchSettings.json
├── wwwroot/                        (CSS, Bootstrap, favicon)
├── .gitignore                      (gerado com `dotnet new gitignore`)
├── appsettings.json
├── Program.cs
├── SistemaInscricaoCursosBlazor.csproj
└── README.md
```

---

## 3. Passo a passo da resolução

### Passo 0 — Criar o projeto

```bash
dotnet new blazor -n SistemaInscricaoCursosBlazor
cd SistemaInscricaoCursosBlazor
```

### Passo 1 — Modelo `InscricaoModel`

Crie a pasta `Components/Models` e o arquivo `InscricaoModel.cs`:

```csharp
using System.ComponentModel.DataAnnotations;

namespace SistemaInscricaoCursosBlazor.Components.Models;

public class InscricaoModel
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    public string? Nome { get; set; }

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail em formato válido.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "A idade é obrigatória.")]
    [Range(16, 100, ErrorMessage = "A idade deve estar entre 16 e 100 anos.")]
    public int Idade { get; set; }

    [Required(ErrorMessage = "Selecione um curso.")]
    public string? CursoId { get; set; }
}
```

| Propriedade | Tipo | Validações |
|---|---|---|
| `Nome` | `string` | `[Required]` |
| `Email` | `string` | `[Required]` + `[EmailAddress]` |
| `Idade` | `int` | `[Required]` + `[Range(16, 100)]` |
| `CursoId` | `string` | `[Required]` |

### Passo 2 — Página `Inscricao.razor`

Arquivo `Components/Pages/Inscricao.razor`. Pontos exigidos pelo enunciado:

- Rota: `@page "/inscricao"`
- `<EditForm Model="modelo" FormName="InscricaoForm" OnValidSubmit="EnviarAsync">`
- Propriedade pública com `[SupplyParameterFromForm]` no bloco `@code`
- `<DataAnnotationsValidator />`, `<ValidationSummary />` e um `<ValidationMessage>` para cada campo
- `<InputSelect>` com uma opção vazia (`-- Selecione um curso --`) e 4 cursos:
  - Desenvolvimento Web com .NET
  - Desenvolvimento Mobile
  - Usabilidade e UX Design
  - Desenvolvimento de Jogos

Trecho principal:

```razor
<EditForm Model="modelo" FormName="InscricaoForm" OnValidSubmit="EnviarAsync">
    <DataAnnotationsValidator />
    <ValidationSummary class="text-danger" />

    <InputSelect id="curso" class="form-select" @bind-Value="modelo.CursoId">
        <option value="">-- Selecione um curso --</option>
        @foreach (var curso in cursos)
        {
            <option value="@curso.Key">@curso.Value</option>
        }
    </InputSelect>
    <ValidationMessage For="@(() => modelo.CursoId)" class="text-danger" />
    ...
</EditForm>

@code {
    [SupplyParameterFromForm]
    public InscricaoModel modelo { get; set; } = new();
}
```

### Passo 3 — Envio assíncrono com `Task.Delay`

```csharp
private bool enviando = false;
private bool sucesso = false;

private async Task EnviarAsync()
{
    enviando = true;          // trava o botão e mostra "Processando..."
    sucesso = false;

    await Task.Delay(2000);   // simula uma requisição de 2 segundos

    enviando = false;
    sucesso = true;           // exibe a mensagem de confirmação
    modelo = new InscricaoModel(); // limpa o formulário
}
```

Botão com feedback visual:

```razor
<button type="submit" class="btn btn-primary" disabled="@enviando">
    @(enviando ? "Processando..." : "Confirmar Inscrição")
</button>
```

### Passo extra — Link no menu

Em `Components/Layout/NavMenu.razor` foi adicionado um item **Inscrição** apontando para `/inscricao`, para facilitar o acesso.

---

## 4. Como executar

Na raiz do projeto (pasta onde está o `.csproj`):

```bash
dotnet watch run
```

O terminal mostra o endereço, por exemplo `http://localhost:5xxx`. Abra no navegador e acesse:

```
http://localhost:5xxx/inscricao
```

> Com `dotnet watch`, toda alteração salva nos arquivos recarrega a página sozinha. Para parar, use `Ctrl + C`.

---

## 5. Roteiro de testes

| # | Cenário | O que fazer | Resultado esperado |
|---|---|---|---|
| 1 | Submissão vazia | Clique em **Confirmar Inscrição** sem preencher nada | Resumo de erros no topo + mensagens em vermelho em Nome, E-mail, Idade e Curso |
| 2a | Idade fora da faixa | Preencha tudo e use idade `15` ou `101` | "A idade deve estar entre 16 e 100 anos." |
| 2b | E-mail inválido | Preencha tudo e use e-mail `joao.com` (sem @) | "Informe um e-mail em formato válido." |
| 3 | Preenchimento correto | Nome, e-mail válido, idade entre 16 e 100 e um curso | Botão fica desabilitado com **"Processando..."** por 2 s, depois aparece a mensagem verde de inscrição confirmada e o formulário é limpo |

---

## 6. Como criar o repositório e enviar para o GitHub

### 6.1 Configurar o Git (só na primeira vez no computador)

```bash
git config --global user.name "Seu Nome"
git config --global user.email "seu-email@exemplo.com"
```

### 6.2 Criar o repositório no GitHub

1. Entre em https://github.com e faça login.
2. Clique no **+** (canto superior direito) → **New repository**.
3. Em **Repository name**, digite exatamente: `sistema-inscricao-cursos-blazor`
4. Escolha **Public** (para o professor conseguir acessar).
5. **Não** marque "Add a README file", ".gitignore" nem "license" — o projeto já tem esses arquivos.
6. Clique em **Create repository** e copie a URL que aparece (ex.: `https://github.com/SEU-USUARIO/sistema-inscricao-cursos-blazor.git`).

### 6.3 Versionar e enviar o projeto

No terminal, dentro da pasta `SistemaInscricaoCursosBlazor`:

```bash
# 1. Inicia o repositório local
git init

# 2. Adiciona todos os arquivos (bin/ e obj/ são ignorados pelo .gitignore)
git add .

# 3. Primeiro commit
git commit -m "Sistema de inscrição em cursos com Blazor"

# 4. Define a branch principal como main
git branch -M main

# 5. Liga o repositório local ao GitHub (troque SEU-USUARIO)
git remote add origin https://github.com/SEU-USUARIO/sistema-inscricao-cursos-blazor.git

# 6. Envia para o GitHub
git push -u origin main
```

> **Login no push:** o GitHub não aceita mais senha da conta no terminal. Se pedir senha, use um **Personal Access Token** (GitHub → Settings → Developer settings → Personal access tokens → Generate new token, com permissão `repo`) ou faça login pelo **GitHub Desktop** / **VS Code** (botão "Publish to GitHub"), que já cuidam da autenticação.

> **Se você baixou este projeto já com a pasta `.git`**, pule os passos 1 a 4. Antes do passo 5, coloque o commit no seu nome (depois de fazer o 6.1):
> ```bash
> git commit --amend --reset-author --no-edit
> ```
> Depois rode os passos 5 e 6.

### 6.4 Alternativa com GitHub CLI

```bash
gh auth login
gh repo create sistema-inscricao-cursos-blazor --public --source=. --remote=origin --push
```

### 6.5 Conferir

Abra `https://github.com/SEU-USUARIO/sistema-inscricao-cursos-blazor` e confira se aparecem as pastas `Components`, `wwwroot`, o `Program.cs` e este `README.md`. Envie esse link na entrega.

### 6.6 Enviar alterações depois

```bash
git add .
git commit -m "Descreva o que mudou"
git push
```

---

## 7. Observações técnicas

- **Por que `@rendermode InteractiveServer`?** Sem modo interativo, a página roda em SSR estático: o formulário é enviado, o servidor espera os 2 segundos e só então devolve a página. Nesse caso o texto "Processando..." nunca chega a aparecer. Com `InteractiveServer`, o Blazor atualiza a tela pelo SignalR antes e depois do `await Task.Delay(2000)`, então o botão desabilitado e o texto dinâmico ficam visíveis.
- **`[SupplyParameterFromForm]` + `FormName`:** o `FormName="InscricaoForm"` identifica o formulário no POST, e o atributo `[SupplyParameterFromForm]` faz o Blazor preencher a propriedade `modelo` com os dados enviados quando a página roda em SSR. A propriedade é inicializada com `new()` (e reforçada com `??=` no `OnInitialized`) para nunca ser nula.
- **`Idade` como `int`:** como `int` nunca é nulo, o `[Required]` não dispara para esse campo; um campo de idade vazio vira `0` e quem barra é o `[Range(16, 100)]`. O comportamento atende ao enunciado.
- **Mensagens em português:** todas as DataAnnotations usam `ErrorMessage` personalizado.

---

**Tecnologias:** C# · .NET 9 · Blazor Web App · Bootstrap 5
