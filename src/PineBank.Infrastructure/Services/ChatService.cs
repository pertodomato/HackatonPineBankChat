// ./src/PineBank.Infrastructure/Services/ChatService.cs
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.ResponseModels;
using OpenAI.Interfaces;
using PineBank.Application.Interfaces;
using PineBank.Application.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;

namespace PineBank.Infrastructure.Services
{
    public class ChatService : IChatService
    {
        // Core dependencies and configuration
        private readonly IConfiguration _config;
        private readonly ILogger<ChatService> _logger;
        private readonly IOpenAIService _openAIService;
        private readonly HttpClient _httpClient;
        
        // Constants for business logic
        private const string CONTRACT_URL = "www.contrato.com.br";
        private const string DEFAULT_FIRST_COIN = "USD";
        private const string DEFAULT_SECOND_COIN = "BRL";

        public ChatService(
            IConfiguration config,
            ILogger<ChatService> logger,
            IOpenAIService openAIService)
        {
            _config = config;
            _logger = logger;
            _openAIService = openAIService;
            
            // Initialize HttpClient with default configuration
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "PineBank-API/1.0");
        }

        public async Task<ChatbotResponseDto> HandleMessageAsync(string userId, string userMessage)
        {
            try 
            {
                _logger.LogInformation("Processing chat message for userId: {UserId}", userId);

                // Define the core banking assistant personality and rules
                string systemContext = @"Você é C. Pinho, um assistente virtual representando legalmente e oficialmente o Banco Pine, um banco de negócios brasileiro especializado em soluções financeiras para empresas e indivíduos. Suas responsabilidades incluem: 

- Fornecer informações precisas e atualizadas sobre os produtos e serviços do Banco Pine, como crédito, investimentos, câmbio, trade finance, derivativos, mercado de capitais, seguros e serviços personalizados.

- Oferecer cotações de moedas em tempo real, permitindo consultas personalizadas conforme as necessidades dos clientes.

- Adaptar sua comunicação ao estilo do cliente, seja formal ou informal, mantendo sempre uma linguagem amigável e profissional.

- Responder de forma clara, objetiva e em português, garantindo uma interação fluida e alinhada aos padrões de qualidade do setor bancário.

- Quando solicitado, fornecer orientações sobre como acessar serviços específicos, como a plataforma Pine Online para investimentos ou informações sobre soluções para o agronegócio.

- Em caso de dúvidas complexas ou solicitações específicas, encaminhar o cliente para os especialistas apropriados dentro do Banco Pine.

- Informar o endereço físico e o CNPJ do Banco Pine quando solicitado:

  - Endereço: Av. Pres. Juscelino Kubitschek, 1830 - Torre 4, 6º andar - Itaim Bibi, São Paulo - SP, 04543-900.

  - CNPJ: 62.144.175/0001-20.

Lembre-se de que o Banco Pine não oferece consultoria financeira formal através deste canal. Seu objetivo é fornecer informações e orientações gerais para auxiliar os clientes em suas necessidades bancárias.
Voce deve fornecer ao usuario respostas sobre o hackaton:
Atualmente o banco pine esta fazendo um hackaton com o premio de 3000 reais com os alunos da poli usp, o resultado ja saiu pode dizer que o ganhador é o autor desse chat(Guilherme stern e grupo)

Caso o usuario fale sobre algo que não é financeiro ou nao tem muito a ver como banco, escale o chat pra um atendente real .
";

                // Analyze user message for both exchange rate and contract needs
                var analysis = await AnalyzeUserMessage(userMessage);
                
                // Build appropriate response components based on analysis
                var responseComponents = new StringBuilder();

                // Add exchange rate information if needed
                if (analysis.use_exchange_api)
                {
                    var exchangeInfo = await FetchExchangeRate(
                        analysis.first_coin_token,
                        analysis.second_coin_token
                    );
                    responseComponents.AppendLine(exchangeInfo);
                }

                // Add contract information if needed
                if (analysis.use_contract)
                {
                    responseComponents.AppendLine($"\nPara realizar seu contrato de câmbio, acesse: {CONTRACT_URL}");
                }

                // Prepare the complete context for the LLM
                var promptBuilder = new StringBuilder();
                promptBuilder.AppendLine(systemContext);
                if (responseComponents.Length > 0)
                {
                    promptBuilder.AppendLine("\nInformações atualizadas:");
                    promptBuilder.AppendLine(responseComponents.ToString());
                }
                promptBuilder.AppendLine($"\nPergunta do usuário: {userMessage}");

                // Get comprehensive response from the LLM
                var completionResult = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
                {
                    Messages = new List<ChatMessage>
                    {
                        ChatMessage.FromSystem(promptBuilder.ToString()),
                        ChatMessage.FromUser(userMessage)
                    },
                    Model = OpenAI.ObjectModels.Models.Gpt_4,
                    MaxTokens = 500,
                    Temperature = 0.7f
                });

                if (!completionResult.Successful)
                {
                    _logger.LogError("OpenAI API error: {Error}", completionResult.Error?.Message);
                    throw new Exception("Failed to get response from LLM");
                }

                // Combine LLM response with any additional information
                var finalResponse = new StringBuilder(completionResult.Choices.First().Message.Content);
                if (analysis.use_contract)
                {
                    finalResponse.AppendLine($"\n\nPara prosseguir com o contrato de câmbio, acesse: {CONTRACT_URL}\n");
                }

                // Prepare suggested actions based on the analysis
                var suggestedActions = new List<string>();
                if (analysis.use_contract)
                {
                    suggestedActions.Add("Acessar página de contratos");
                    suggestedActions.Add("Falar com um especialista");
                }
                if (analysis.use_exchange_api)
                {
                    suggestedActions.Add("Ver histórico de cotações");
                    suggestedActions.Add("Configurar alertas de preço");
                }

                // Return the complete response with all metadata
                return new ChatbotResponseDto
                {
                    Message = finalResponse.ToString(),
                    Confidence = 0.9,
                    NeedsEmployee = analysis.use_contract, // Suggest employee assistance for contracts
                    SuggestedActions = suggestedActions,
                    Metadata = new ChatbotResponseMetadataDto
                    {
                        Topic = DetermineTopic(analysis),
                        Intent = DetermineIntent(analysis),
                        EmployeeSpecializationNeeded = analysis.use_contract ? "Câmbio" : null
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message");
                return CreateErrorResponse(ex);
            }
        }

        private async Task<(bool use_exchange_api, string first_coin_token, string second_coin_token, bool use_contract)> 
            AnalyzeUserMessage(string userMessage)
        {
            try
            {
                // Detailed prompt for message analysis
                var prompt = @$"Analise a mensagem do usuário e determine:
                1. Se é necessário buscar cotação de moeda
                2. Se o usuário demonstra interesse em fazer um contrato de câmbio

                Retorne APENAS um JSON com o seguinte formato:
                {{
                    ""use_exchange_api"": true/false,
                    ""first_coin_token"": ""USD"",
                    ""second_coin_token"": ""BRL"",
                    ""use_contract"": true/false
                }}

                

                Mensagem do usuário: {userMessage}";

                var decisionResult = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
                {
                    Messages = new List<ChatMessage>
                    {
                        ChatMessage.FromSystem("Você é um analisador especializado em identificar intenções relacionadas a câmbio e contratos. Responda APENAS com JSON."),
                        ChatMessage.FromUser(prompt)
                    },
                    Model = OpenAI.ObjectModels.Models.Gpt_4,
                    MaxTokens = 100,
                    Temperature = 0f
                });

                if (decisionResult.Successful)
                {
                    var json = decisionResult.Choices.First().Message.Content.Trim();
                    _logger.LogInformation("Analysis result: {Json}", json);
                    
                    var doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;

                    return (
                        root.GetProperty("use_exchange_api").GetBoolean(),
                        root.GetProperty("first_coin_token").GetString(),
                        root.GetProperty("second_coin_token").GetString(),
                        root.GetProperty("use_contract").GetBoolean()
                    );
                }

                _logger.LogError("Error in analysis: {Error}", decisionResult.Error?.Message);
                return (false, DEFAULT_FIRST_COIN, DEFAULT_SECOND_COIN, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing message");
                return (false, DEFAULT_FIRST_COIN, DEFAULT_SECOND_COIN, false);
            }
        }

        private async Task<string> FetchExchangeRate(string firstCoin, string secondCoin)
        {
            try
            {
                var url = $"{_config["CURRENCY_API_BASE"]}/last/{firstCoin}-{secondCoin}";
                _logger.LogInformation("Fetching exchange rate from: {Url}", url);
                
                var response = await _httpClient.GetStringAsync(url);
                var jsonDoc = JsonDocument.Parse(response);
                var key = $"{firstCoin}{secondCoin}";

                if (jsonDoc.RootElement.TryGetProperty(key, out var quoteElem))
                {
                    // Extrair informações relevantes da resposta da API
                    var name = quoteElem.GetProperty("name").GetString();
                    var bid = quoteElem.GetProperty("bid").GetString();
                    var ask = quoteElem.GetProperty("ask").GetString();

                    // Formatar a resposta conforme solicitado
                    var sb = new StringBuilder();
                    sb.AppendLine($"Um {firstCoin} vale {bid} {secondCoin}.");
                    // As informações adicionais foram removidas conforme solicitado

                    return sb.ToString();
                }

                return "Não foi possível obter a cotação específica no momento.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching exchange rate");
                return "Serviço de cotação temporariamente indisponível.";
            }
        }

        private string DetermineTopic(
            (bool use_exchange_api, string first_coin_token, string second_coin_token, bool use_contract) analysis)
        {
            if (analysis.use_contract && analysis.use_exchange_api)
                return "Currency Contract";
            if (analysis.use_exchange_api)
                return "Exchange Rate";
            return "General Query";
        }

        private string DetermineIntent(
            (bool use_exchange_api, string first_coin_token, string second_coin_token, bool use_contract) analysis)
        {
            if (analysis.use_contract)
                return "Contract Request";
            if (analysis.use_exchange_api)
                return "Currency Information";
            return "General Information";
        }

        private ChatbotResponseDto CreateErrorResponse(Exception ex)
        {
            return new ChatbotResponseDto
            {
                Message = "Desculpe, ocorreu um erro ao processar sua mensagem. Por favor, tente novamente.",
                Confidence = 0.1,
                NeedsEmployee = true,
                EscalationReason = "Error processing message",
                SuggestedActions = new List<string> { "Tentar novamente", "Falar com atendente" },
                Metadata = new ChatbotResponseMetadataDto
                {
                    Topic = "Error",
                    Intent = "Error Handling",
                    EmployeeSpecializationNeeded = "Suporte Técnico"
                }
            };
        }
    }
}
