
# ReGraphik — Gestão de Estoque Reverso
 
> Sistema de gestão e automação focado em sustentabilidade e eficiência operacional para o setor gráfico.
 
![C#](https://img.shields.io/badge/C%23-.NET-512BD4?style=flat-square&logo=dotnet)
![WPF](https://img.shields.io/badge/UI-WPF-0078D4?style=flat-square&logo=windows)
![Firebase](https://img.shields.io/badge/DB-Firebase-FFCA28?style=flat-square&logo=firebase&logoColor=black)
![Swagger](https://img.shields.io/badge/Docs-Swagger-85EA2D?style=flat-square&logo=swagger&logoColor=black)
![Google Maps](https://img.shields.io/badge/API-Google%20Maps-4285F4?style=flat-square&logo=googlemaps&logoColor=white)
 
---

## Sumário
 
- [Sobre o Projeto](#sobre-o-projeto)
- [O Desafio](#o-desafio)
- [Nossa Solução](#nossa-solução)
- [Arquitetura](#arquitetura)
- [Tecnologias](#tecnologias)
- [Estrutura do Repositório](#estrutura-do-repositório)
- [API REST — Endpoints](#api-rest--endpoints)
- [Modelos de Dados](#modelos-de-dados)
- [Integrações Externas](#integrações-externas)
- [Como Executar](#como-executar)
- [Documentação](#documentação)
- [Integrantes](#integrantes)
---
## Sobre o Projeto
 
O **ReGraphik** é um software desenvolvido para resolver um problema real do setor gráfico: o descarte inadequado de resíduos como papel, cartão e vinil. O sistema transforma esses materiais descartados em valor através de um ciclo completo de gestão — do cadastro do resíduo à localização de pontos de coleta e sugestão de reaproveitamento.
 
O projeto é composto por uma **API REST** em ASP.NET Core integrada ao **Firebase Realtime Database** e um **cliente desktop** desenvolvido em WPF seguindo o padrão MVVM.
 
---

## O Desafio
 
Empresas do setor gráfico geram diariamente resíduos como papel A4, cartões, vinil e outros materiais que são descartados sem critério, gerando:
 
- Custos desnecessários de descarte
- Alto impacto ambiental
- Perda de matéria-prima que poderia ser reaproveitada
---

## Nossa Solução
 
O ReGraphik atua em três pilares:
 
**1. Gestão de Estoque Reverso**
Organização inteligente dos resíduos gerados dentro das próprias gráficas, com controle de tipo, quantidade, condição, dimensões e status de cada material.
 
**2. Economia Circular**
Transformação de resíduos em matéria-prima para personalização de novos produtos como camisetas, canecas e brindes, integrando sustentabilidade ao processo produtivo.
 
**3. Sugestões de Reaproveitamento**
Algoritmos de sugestão que relacionam cada tipo de resíduo cadastrado à melhor forma de reaproveitamento, reduzindo desperdício de forma inteligente.
 
---

## Arquitetura
 
O projeto segue rigorosamente o padrão **MVVM (Model-View-ViewModel)** na camada de apresentação e uma arquitetura de **serviços desacoplados** na API REST.
 
```
ReGraphikApp/
├── ApiRestReGraphik/          # Backend — ASP.NET Core REST API
│   ├── Controllers/           # Endpoints HTTP (CRUD completo)
│   ├── Services/              # Regras de negócio e acesso ao Firebase
│   ├── Models/                # Entidades do domínio
│   ├── Data/                  # Configuração do Firebase Client
│   └── Program.cs             # Configuração da aplicação, DI, Swagger, CORS
│
├── ReGraphik/                 # Frontend — WPF Desktop (MVVM)
│   ├── Views/                 # Janelas e páginas XAML
│   │   └── Pages/             # Dashboard, Resíduos, Pontos de Coleta, Mapa, Relatórios
│   ├── ViewModels/            # Lógica de apresentação (BaseViewModel, ResiduoViewModel)
│   ├── Models/                # Espelho das entidades do domínio
│   ├── Services/              # GooglePlacesService (integração com Maps)
│   └── Commands/              # RelayCommand (padrão Command do MVVM)
│
├── Modelagem/                 # Documentação técnica (PDFs)
└── Banco de Dados/            # Scripts e documentação do banco
```
**Fluxo da aplicação:**
 
```
Cliente WPF  →  API REST (ASP.NET Core)  →  Firebase Realtime Database
                        ↓
               Google Maps Places API  (busca de pontos de coleta)
```
 
--- 

## Tecnologias
 
| Camada | Tecnologia |
|---|---|
| Linguagem | C# (.NET) |
| Frontend | WPF — Windows Presentation Foundation |
| Padrão de Projeto | MVVM |
| Backend | ASP.NET Core Web API |
| Banco de Dados | Firebase Realtime Database |
| Autenticação Firebase | Google Credential (Service Account JSON) |
| Mapa | Google Maps Places API + Leaflet.js (WebBrowser/WebView2) |
| Documentação API | Swagger / OpenAPI |
| CORS | Aberto para qualquer origem (configurável para produção) |
 
---

## Estrutura do Repositório
 
```
ReGraphikApp/
├── ApiRestReGraphik/
│   ├── Controllers/
│   │   ├── UsuarioController.cs
│   │   ├── ResiduoController.cs
│   │   ├── PontosColetaController.cs
│   │   ├── SugestaoController.cs
│   │   └── SugestaoResiduosController.cs
│   ├── Services/
│   │   ├── UsuarioService.cs
│   │   ├── ResiduoService.cs
│   │   ├── PontosColetaService.cs
│   │   ├── SugestaoService.cs
│   │   └── SugestaoResiduosService.cs
│   ├── Models/
│   │   ├── Usuario.cs
│   │   ├── Residuo.cs
│   │   ├── PontosColeta.cs
│   │   ├── Sugestao.cs
│   │   └── SugestaoResiduo.cs
│   ├── Data/
│   │   └── DbReGraphik.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── appsettings.Development.json
│
├── ReGraphik/
│   ├── Views/
│   │   ├── MainWindow.xaml
│   │   └── Pages/
│   │       ├── DashboardPage.xaml
│   │       ├── ResiduosPage.xaml
│   │       ├── PontosColetaPage.xaml
│   │       ├── MapaPage.xaml
│   │       └── RelatoriosPage.xaml
│   ├── ViewModels/
│   │   ├── BaseViewModel.cs
│   │   └── ResiduoViewModel.cs
│   ├── Models/
│   ├── Services/
│   │   └── GooglePlacesService.cs
│   └── Commands/
│       └── RelayCommand.cs
│
├── Modelagem/
│   ├── MiniMundo Demanda.pdf
│   ├── Modelo Conceitual.pdf
│   ├── Modelo Lógico.pdf
│   └── Modelo Físico.pdf
│
└── Banco de Dados/
    └── Documentação Criação Modelagem.pdf
```
 
---
### RelayCommand — Padrão de Binding
 
O padrão `ICommand` garante que a View nunca acessa a ViewModel diretamente:
 
```csharp
// RelayCommand.cs
public class RelayCommand : ICommand
{
    Action<object> _execute;
    Func<object, bool> _canExecute;
 
    public bool CanExecute(object p) => _canExecute?.Invoke(p) ?? true;
    public void Execute(object p)    => _execute(p);
}
```
 
```xml
<!-- Binding na View (XAML) — zero code-behind -->
<Button Command="{Binding SalvarCommand}"
        CommandParameter="{Binding Residuo}"/>
```
 
### GooglePlacesService — Async/Await
 
Service completamente desacoplada da ViewModel, garantindo que a UI nunca trava durante chamadas HTTP externas:
 
```csharp
// GooglePlacesService.cs
public class GooglePlacesService
{
    private readonly HttpClient _http;
 
    public async Task<List<PontosColeta>> BuscarPontosAsync(double lat, double lng)
    {
        var url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json" +
                  $"?location={lat},{lng}&radius=2000&key={_key}";
 
        var response = await _http.GetAsync(url);   // não bloqueia a UI thread
        var json     = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<PontosColeta>>(json);
    }
}
```
 
- **Async/Await** garante UI responsiva durante chamadas HTTP externas
- **Service desacoplada** — ViewModel não toca no HttpClient
- **Injeção via construtor** habilita testes unitários com mocks
---


## API REST — Endpoints
 
A API expõe **5 controllers** com operações CRUD completas. A documentação interativa fica disponível via **Swagger** na raiz da aplicação ao rodar o projeto.
 
### Usuário — `api/Usuario`
 
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/Usuario` | Lista todos os usuários |
| GET | `/api/Usuario/{id}` | Obtém usuário por ID |
| POST | `/api/Usuario` | Cria novo usuário |
| PUT | `/api/Usuario/{id}` | Atualiza usuário existente |
| DELETE | `/api/Usuario/{id}` | Remove usuário |

### Pontos de Coleta — `api/PontosColeta`
 
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/PontosColeta` | Lista todos os pontos cadastrados no Firebase |
| GET | `/api/PontosColeta/{id}` | Obtém ponto por ID |
| POST | `/api/PontosColeta/google?cidade=...` | Cadastra pontos via Google Maps Places API e salva no Firebase |
| POST | `/api/PontosColeta` | Cadastra novo ponto de coleta |
| PUT | `/api/PontosColeta/{id}` | Atualiza ponto existente |
| DELETE | `/api/PontosColeta/{id}` | Remove ponto de coleta |
 
> O endpoint `/google` valida se a cidade já está cadastrada no Firebase antes de consultar o Google Maps. Se autorizada, busca os pontos, salva automaticamente e retorna a lista com os IDs gerados.
>
> ### Sugestão — `api/Sugestao`
 
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/Sugestao` | Lista todas as sugestões |
| GET | `/api/Sugestao/{id}` | Obtém sugestão por ID |
| POST | `/api/Sugestao` | Cria nova sugestão |
| PUT | `/api/Sugestao/{id}` | Atualiza sugestão existente |
| DELETE | `/api/Sugestao/{id}` | Remove sugestão |
 
### Sugestão de Resíduos — `api/SugestaoResiduos`
 
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/SugestaoResiduos` | Lista todas as sugestões aplicadas a resíduos |
| GET | `/api/SugestaoResiduos/{id}` | Obtém por ID |
| POST | `/api/SugestaoResiduos` | Registra aplicação de sugestão a um resíduo |
| PUT | `/api/SugestaoResiduos/{id}` | Atualiza registro |
| DELETE | `/api/SugestaoResiduos/{id}` | Remove registro |
 
---

 ## Modelos de Dados
 
### Usuario
```json
{
  "id": "string",
  "name": "string",
  "cpf": "string",
  "email": "string",
  "login": "string",
  "senha": "string",
  "data_cadastro": "datetime"
}
```
 
### Residuo
```json
{
  "id": "string",
  "id_usuario": "string",
  "tipo_residuo": "string",
  "origem": "string",
  "especificacao": "string",
  "projeto": "string",
  "quantidade": "double",
  "data_cadastro": "datetime",
  "condicao": "string",
  "dimensoes_cm": "double?",
  "dimensoes_lm": "double?",
  "observacao": "string",
  "anexo": "string",
  "status": "string"
}
```
 
### PontosColeta
```json
{
  "id": "string",
  "nome_ponto": "string",
  "cidade": "string",
  "estado": "string",
  "cep": "string",
  "residuos_aceitos": "string",
  "latitude": "double",
  "longitude": "double"
}
```
 
### Sugestao
```json
{
  "id": "string",
  "tipo_residuo_aceito": "string",
  "descricao_sugestao": "string"
}
```
 
### SugestaoResiduo
```json
{
  "id": "string",
  "id_cadastro_residuo": "int",
  "id_sugestao": "int",
  "data_aplicacao": "datetime?"
}
```
 
---## Frontend Desktop (WPF)
 
O cliente desktop foi desenvolvido em **WPF** com o padrão **MVVM**, garantindo separação entre interface e lógica de negócio.
 
### Navegação
 
A janela principal (`MainWindow`) possui uma barra lateral de navegação com 5 seções:
 
| Seção | Status | Descrição |
|---|---|---|
| Dashboard | Em desenvolvimento | Painel com visão geral do sistema |
| Cadastrar Resíduos | Em desenvolvimento | Formulário de cadastro de resíduos |
| Mapa/Ponto de Coleta | Funcional | Mapa interativo com busca por cidade |
| Relatórios | Em desenvolvimento | Geração de relatórios |

## Integrações Externas
 
### Firebase Realtime Database
 
Toda a persistência de dados é feita no **Firebase Realtime Database**. A autenticação é realizada via **Google Service Account** (arquivo `.json` de credenciais), com escopos:
 
- `https://www.googleapis.com/auth/userinfo.email`
- `https://www.googleapis.com/auth/firebase.database`
Os nós do banco de dados são:
 
| Nó | Entidade |
|---|---|
| `usuarios` | Usuários do sistema |
| `residuos` | Resíduos cadastrados |
| `pontos_coleta` | Pontos de coleta |
| `sugestoes` | Sugestões de reaproveitamento |
| `sugestoes_residuos` | Aplicação de sugestões a resíduos |

### Google Maps Places API
 
Utilizada em dois pontos do sistema:
 
- **API REST** (`PontosColetaController`) — busca pontos de coleta por cidade, valida se a cidade está pré-autorizada no Firebase, salva os resultados e retorna com coordenadas de latitude/longitude
- **WPF Client** (`GooglePlacesService`) — busca postos de coleta por cidade e material para exibição no mapa
---
 
## Como Executar
 
### Pré-requisitos
 
- .NET 8 SDK ou superior
- Conta no Firebase com Realtime Database configurado
- Chave de API do Google Maps habilitada para Places API
- Visual Studio 2022 ou VS Code com extensões C#
### Configuração da API
 
1. Clone o repositório:
```bash
git clone https://github.com/BrunoMaiaSenai/ReGraphikApp.git
cd ReGraphikApp
```
 
2. Adicione o arquivo de credenciais do Firebase na raiz da API:
```
ApiRestReGraphik/ReGraphikFirebaseKey.json
```
 
3. Configure o `appsettings.json`:
```json
{
  "Firebase": {
    "RealtimeDatabaseUrl": "https://seu-projeto-default-rtdb.firebaseio.com/",
    "CredentialFilePath": "ReGraphikFirebaseKey.json"
  },
  "GoogleMaps": {
    "ApiKey": "SUA_CHAVE_AQUI"
  }
}
```
 
4. Execute a API:
```bash
cd ApiRestReGraphik
dotnet run
```
 
5. Acesse o Swagger em: `http://localhost:PORT/`
### Executando o Cliente WPF
 
1. Abra a solution `ReGraphik.slnx` no Visual Studio
2. Defina o projeto `ReGraphik` como projeto de inicialização
3. Certifique-se que a API está em execução
4. Pressione `F5` para rodar
---
## Arquitetura do Projeto (MVVM)

Este projeto foi desenvolvido utilizando o padrão **Model-View-ViewModel (MVVM)**, garantindo a separação clara entre a interface do usuário, a lógica de apresentação e as regras de negócio/dados. 

Abaixo está o fluxo de comunicação entre as camadas da nossa aplicação:

```mermaid
flowchart LR
    classDef view fill:#e1f5fe,stroke:#0288d1,stroke-width:2px,color:#000
    classDef vm fill:#e8f5e9,stroke:#388e3c,stroke-width:2px,color:#000
    classDef model fill:#fff3e0,stroke:#f57c00,stroke-width:2px,color:#000

    subgraph View ["📱 View (XAML)"]
        UI["Páginas e Controles UI\n(ex: ResiduosPage.xaml)"]:::view
    end

    subgraph ViewModel ["🧠 ViewModel (C#)"]
        CMD["RelayCommand\n(Ações)"]:::vm
        VM["ViewModel\n(ex: ResiduoViewModel)"]:::vm
        BVM["BaseViewModel\n(INotifyPropertyChanged)"]:::vm
        
        CMD --> VM
        VM --> BVM
    end

    subgraph Model ["💾 Model & Services (C#)"]
        SVC["Services\n(ex: GooglePlacesService)"]:::model
        MOD["Models\n(ex: Residuo, Pontos)"]:::model
        
        SVC --> MOD
    end

    UI -- "1. Interação" --> CMD
    VM -- "2. Solicita Dados" --> SVC
    MOD -- "3. Alimenta Dados" --> VM
    BVM -. "4. Atualiza XAML" .-> UI
```

### Como funciona o fluxo?

1. **Ação do Usuário:** O usuário interage com a **View** (ex: clica num botão em `ResiduosPage.xaml`). Essa ação aciona um `RelayCommand`.
2. **Processamento:** O comando avisa a **ViewModel** correspondente (`ResiduoViewModel`), que processa a lógica de tela.
3. **Busca de Dados:** Se necessário, a ViewModel chama um **Service** (como o `GooglePlacesService`), que consome APIs externas ou banco de dados, retornando objetos do tipo **Model** (`Residuo`, `PontosColeta`).
4. **Atualização Reativa:** A ViewModel atualiza suas propriedades. Através da `BaseViewModel` (que implementa `INotifyPropertyChanged`), o XAML é notificado e atualiza a tela automaticamente via *Data Binding*.
## Documentação
 
A estrutura de dados e o planejamento técnico completo estão disponíveis nas pastas do repositório:
 
- [MiniMundo e Demanda](./Modelagem/MiniMundo%20Demanda.pdf) — descrição do problema e contexto do negócio
- [Modelo Conceitual](./Modelagem/Modelo%20Conceitual.pdf) — diagrama entidade-relacionamento conceitual
- [Modelo Lógico](./Modelagem/Modelo%20L%C3%B3gico.pdf) — estrutura lógica do banco de dados
- [Modelo Físico](./Modelagem/Modelo%20F%C3%ADsico.pdf) — script e estrutura física do banco
- [Documentação do Banco](./Banco%20de%20Dados/Documenta%C3%A7%C3%A3o%20Cria%C3%A7%C3%A3o%20Modelagem.pdf) — documentação de criação e modelagem
A documentação interativa da API está disponível via **Swagger** ao rodar o projeto — é a página inicial da aplicação.
 
---
 
## Integrantes
 
Projeto desenvolvido por alunos do SENAI:
 
| Nome |
|---|
| Lucas Aquino Guedes |
| Otavio Henrique Barbosa Soares |
| Bruno Maia Santos |
| Luna Beatriz Alves |
| Kaio Alves Gonzaga Silva |
 
---
 

  Desenvolvido com foco em sustentabilidade e economia circular para o setor gráfico

