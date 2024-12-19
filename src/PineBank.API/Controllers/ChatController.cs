// ./src/PineBank.API/Controllers/ChatController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PineBank.API.Models.Requests;
using PineBank.API.Models.Responses;
using PineBank.Application.Interfaces;
using PineBank.Application.Features.Conversations.Queries;
using PineBank.Application.Features.Conversations.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace PineBank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IMediator mediator, IChatService chatService, ILogger<ChatController> logger)
        {
            _mediator = mediator;
            _chatService = chatService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ChatResponse>> SendMessage([FromBody] ChatRequest request)
        {
            try
            {
                _logger.LogInformation("Processing chat request for user: {UserId}", request.UserId);

                // Validate request
                if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.Message))
                {
                    _logger.LogWarning("Invalid request received - missing required fields");
                    return BadRequest(new { error = "UserId and Message are required fields" });
                }

                // Get or create conversation with detailed logging
                _logger.LogInformation("Attempting to get or create conversation");
                var conversation = await _mediator.Send(new GetOrCreateConversation.Query(request.UserId));
                _logger.LogInformation("Conversation retrieved/created with ID: {ConversationId}", conversation.Id);

                // Process message with detailed logging
                _logger.LogInformation("Processing message through chat service");
                var responseDto = await _chatService.HandleMessageAsync(request.UserId, request.Message);
                _logger.LogInformation("Chat service response received");

                // Add messages to conversation
                _logger.LogInformation("Adding messages to conversation");
                await _mediator.Send(new AddMessage.Command(
                    conversation.Id,
                    request.Message,
                    "user"
                ));

                await _mediator.Send(new AddMessage.Command(
                    conversation.Id,
                    responseDto.Message,
                    "assistant",
                    responseDto.Confidence,
                    responseDto.NeedsEmployee,
                    responseDto.Metadata?.Intent,
                    responseDto.Metadata?.Topic,
                    responseDto.EscalationReason,
                    responseDto.Metadata?.EmployeeSpecializationNeeded
                ));

                // Prepare and return response
                var response = new ChatResponse
                {
                    Message = responseDto.Message,
                    Confidence = responseDto.Confidence,
                    NeedsEmployee = responseDto.NeedsEmployee,
                    EscalationReason = responseDto.EscalationReason,
                    SuggestedActions = responseDto.SuggestedActions ?? new List<string>(),
                    Metadata = new ChatResponseMetadata
                    {
                        Topic = responseDto.Metadata?.Topic,
                        Intent = responseDto.Metadata?.Intent,
                        EmployeeSpecializationNeeded = responseDto.Metadata?.EmployeeSpecializationNeeded
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message");
                return StatusCode(500, new
                {
                    error = "An error occurred processing your request",
                    message = ex.Message,
                    details = ex.InnerException?.Message,
                    shouldEscalate = true
                });
            }
        }
    }
}