//./src/PineBank.Application/Features/Conversations/Commands/AddMessage.cs
namespace PineBank.Application.Features.Conversations.Commands
{
    using MediatR;
    using PineBank.Application.DTOs;
    using PineBank.Domain.Interfaces;
    using PineBank.Domain.Entities;
    using AutoMapper;
    using Microsoft.Extensions.Logging;
    using FluentValidation;
    using PineBank.Domain.Exceptions;


    
    public class AddMessage
    {
        public record Command(
            Guid ConversationId,
            string Content,
            string Role,
            double Confidence = 1.0,
            bool NeedsEmployee = false,
            string Intent = null,
            string Topic = null,
            string EscalationReason = null,
            string EmployeeSpec = null
        ) : IRequest<MessageDto>;

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.ConversationId).NotEmpty().WithMessage("ConversationId is required.");
                RuleFor(x => x.Content).NotEmpty().WithMessage("Content is required.");
                RuleFor(x => x.Role)
                    .NotEmpty()
                    .Must(r => r == "user" || r == "assistant")
                    .WithMessage("Role must be either 'user' or 'assistant'.");
            }
        }

        public class Handler : IRequestHandler<Command, MessageDto>
        {
            private readonly IUnitOfWork _uow;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;

            public Handler(IUnitOfWork uow, IMapper mapper, ILogger<Handler> logger)
            {
                _uow = uow;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<MessageDto> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var conversation = await _uow.Conversations.GetByIdAsync(request.ConversationId);
                    if (conversation == null)
                        throw new DomainNotFoundException($"Conversation {request.ConversationId} not found.");

                    var message = new Message(request.Content, request.Role, request.Confidence, request.NeedsEmployee);
                    if (request.NeedsEmployee)
                    {
                        message.MarkNeedsEmployee(request.EscalationReason, request.EmployeeSpec);
                    }

                    conversation.AddMessage(message);
                    await _uow.Conversations.UpdateAsync(conversation);
                    await _uow.SaveChangesAsync();

                    return _mapper.Map<MessageDto>(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding message to conversation {ConversationId}", request.ConversationId);
                    throw;
                }
            }
        }
    }
}
