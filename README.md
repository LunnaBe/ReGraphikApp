
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
- [Frontend Desktop (WPF)](#frontend-desktop-wpf)
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
