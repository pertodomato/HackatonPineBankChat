# PineBank Hackaton – Assistente Virtual Bancário Integrado


## 📖 Descrição Geral

O **PineBank Hackaton** é uma solução inovadora desenvolvida para otimizar o atendimento ao cliente em ambientes bancários através de um assistente virtual inteligente. Este projeto integra um back-end robusto desenvolvido em .NET com um front-end dinâmico utilizando Nuxt 3 e Vuetify, proporcionando uma experiência de usuário fluida e eficiente. O sistema foi projetado para responder a consultas financeiras, fornecer cotações de câmbio em tempo real e facilitar a realização de contratos de câmbio, tudo isso enquanto mantém a capacidade de escalar o atendimento para um agente humano quando necessário.

## 🚀 Funcionalidades Principais

- **Chatbot Bancário Inteligente:** Permite aos usuários interagirem com um assistente virtual que responde a perguntas sobre produtos e serviços bancários, cotações de moedas e procedimentos internos.
  
- **Autenticação Segura:** Utiliza tokens JWT para garantir a segurança das interações, exigindo autenticação antes de permitir o acesso às funcionalidades protegidas da API.
  
- **Cotações de Moedas em Tempo Real:** Integra-se com a API [AwesomeAPI Economia](https://economia.awesomeapi.com.br/) para fornecer cotações atualizadas entre diferentes moedas.
  
- **Gestão de Conversas:** Armazena o histórico de conversas no MongoDB, permitindo manter o contexto e fornecer respostas mais precisas com base nas interações anteriores.
  
- **Escalonamento para Atendimento Humano:** Em casos de dúvidas complexas ou solicitações fora do escopo financeiro, o sistema encaminha a interação para um atendente humano, garantindo um atendimento completo e satisfatório.
  
- **Integração com Instagram Webhook:** Permite a recepção e envio de mensagens através do Instagram, ampliando os canais de atendimento ao cliente.

## 🛠 Tecnologias Utilizadas

### Back-end:

- **.NET 8.0 + C#**
- **ASP.NET Core Web API**
- **MediatR:** Implementação do padrão CQRS para separar responsabilidades de comandos e consultas.
- **MongoDB:** Banco de dados NoSQL para armazenamento eficiente das conversas e mensagens.
- **JWT (JSON Web Tokens):** Para autenticação e autorização seguras.
- **OpenAI:** Integração com modelos de linguagem para gerar respostas contextuais e inteligentes.
- **Redis:** Utilizado para caching, melhorando a performance das respostas.

### Front-end:

- **Nuxt 3 (Vue 3)**
- **Vuetify:** Framework de componentes de UI para uma interface moderna e responsiva.
- **SASS:** Para estilização personalizada e manutenção de variáveis de estilos.

### Outras Tecnologias:

- **Docker:** Facilita a configuração e execução de serviços como MongoDB e Redis.
- **PowerShell:** Scripts para testes rápidos da API.

## 📋 Pré-requisitos

Antes de iniciar, certifique-se de ter instalado em sua máquina:

- **.NET 8.0 SDK** ou superior. [Download .NET](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js** e **npm**. [Download Node.js](https://nodejs.org/)
- **Docker** (para rodar MongoDB e Redis). [Download Docker](https://www.docker.com/get-started)
- **Chave de API da OpenAI**.
- **Chave de API da Gemini** (se aplicável).

## 🏗️ Instalação e Configuração

### 1. Clonar o Repositório

Clone o repositório para a sua máquina local:

``bash

git clone https://github.com/pertodomato/PineBankHackaton.git

cd PineBankHackaton

2. Configurar Variáveis de Ambiente
   
Crie um arquivo .env na raiz do projeto com as seguintes variáveis:

JWT_SECRET=SeuSegredoJWTAqui

OPENAI_API_KEY=SuaChaveAPIOpenAIAqui

GEMINI_API_KEY=SuaChaveAPIGeminiAqui

MONGODB_URI=mongodb://localhost:27017/pinebankdb

REDIS_CONNECTION=localhost:6379

LLM_PROVIDER=openai

CURRENCY_API_BASE=https://economia.awesomeapi.com.br

IG_PAGE_ACCESS_TOKEN=SeuTokenDeAcessoInstagramAqui


3. Rodar os Serviços com Docker
Para facilitar a configuração do MongoDB e Redis, utilize o Docker Compose fornecido:

docker-compose -f docker-compose.local.yml up -d


4.1. Back-end (.NET API)
Navegue até o diretório do back-end e restaure as dependências:

cd src/PineBank.API
dotnet restore

4.2. Front-end (Nuxt 3)
Navegue até o diretório do front-end e instale as dependências:

cd ../../pinebank-web
npm install

Executando a Aplicação
   
5.1. Executando a API (.NET)

No terminal, navegue até o diretório da API e execute:

cd src/PineBank.API
dotnet build
dotnet run
A API estará acessível em http://localhost:5204.

5.2. Executando o Front-end (Nuxt 3)

Em outro terminal, navegue até o diretório do front-end e execute:
cd pinebank-web
npm run dev
6. Testando a API no terminal
Utilize o script PowerShell fornecido para autenticar e testar a API:
# Cria o corpo da requisição de autenticação
$authBody = @{
    username = "admin"
    password = "password"
} | ConvertTo-Json

Write-Host "1. Tentando autenticação..."
try {
    $authResponse = Invoke-RestMethod -Uri "http://localhost:5204/api/Auth/login" `
                                    -Method Post `
                                    -ContentType "application/json" `
                                    -Body $authBody `
                                    -ErrorVariable responseError

    Write-Host "Autenticação bem-sucedida!"
    Write-Host "Token recebido: $($authResponse.token.Substring(0, 20))..."

    # Configura os headers com o token
    $headers = @{
        "Authorization" = "Bearer $($authResponse.token)"
        "Content-Type" = "application/json"
    }

    Write-Host "`n2. Testando chat geral..."
    $chatBody = @{
        message = "Eu queria fazer uma viagem e quero comprar 100 euros, quantos reais eu preciso?"
        userId = "test-user"
    } | ConvertTo-Json

    $response = Invoke-RestMethod -Uri "http://localhost:5204/api/Chat" `
                                -Method Post `
                                -Headers $headers `
                                -Body $chatBody

    Write-Host "Resposta do chat recebida:"
    $response | ConvertTo-Json -Depth 10

    Write-Host "`n3. Testando consulta de câmbio..."
    $exchangeBody = @{
        message = "Eu queria fazer uma viagem e tenho 100 reais pra comprar euros, isso da quantos euros? Me passe um contrato para eu fazer essa transacao"
        userId = "test-user"
    } | ConvertTo-Json

    $exchangeResponse = Invoke-RestMethod -Uri "http://localhost:5204/api/Chat" `
                                        -Method Post `
                                        -Headers $headers `
                                        -Body $exchangeBody

    Write-Host "Resposta da consulta de câmbio recebida:"
    $exchangeResponse | ConvertTo-Json -Depth 10

} catch {
    Write-Host "Ocorreu um erro:"
    Write-Host $_.Exception.Message
    Write-Host "Código de Status da Resposta:" $_.Exception.Response.StatusCode.value__
    Write-Host "Descrição do Status da Resposta:" $_.Exception.Response.StatusDescription
    
    # Tenta obter mais detalhes do erro, se disponíveis
    try {
        $errorDetails = $_.ErrorDetails.Message | ConvertFrom-Json
        Write-Host "Detalhes do Erro:" $errorDetails
    } catch {
        Write-Host "Nenhum detalhe adicional do erro disponível"
    }
} 
7. Estrutura do Projeto
PineBankHackaton/
├── src/
│   ├── PineBank.API/
│   │   ├── Controllers/
│   │   ├── Models/
│   │   ├── Services/
│   │   ├── Program.cs
│   │   └── PineBank.API.csproj
│   ├── PineBank.Application/
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   ├── Features/
│   │   └── PineBank.Application.csproj
│   └── PineBank.Domain/
│       ├── Entities/
│       ├── Interfaces/
│       └── PineBank.Domain.csproj
├── pinebank-web/
│   ├── assets/
│   ├── components/
│   ├── pages/
│   ├── plugins/
│   ├── public/
│   ├── .nuxt/
│   ├── nuxt.config.ts
│   ├── package.json
│   ├── README.md
│   └── tsconfig.json
├── docker-compose.local.yml
├── .env
├── .gitignore
├── README.md
└── requirements.txt
