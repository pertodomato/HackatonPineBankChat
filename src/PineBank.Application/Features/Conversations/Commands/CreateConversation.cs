//./src/PineBank.Application/Features/Conversations/Commands/CreateConversation.cs
namespace PineBank.Application.Features.Conversations.Commands
{
    using MediatR;
    using PineBank.Application.DTOs;
    using PineBank.Domain.Interfaces;
    using PineBank.Domain.Entities;
    using AutoMapper;
    using Microsoft.Extensions.Logging;
    using FluentValidation;

    public class CreateConversation
    {
        public record Command(string UserId) : IRequest<ConversationDto>;

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
            }
        }

        public class Handler : IRequestHandler<Command, ConversationDto>
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

            public async Task<ConversationDto> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var conversation = new Conversation(request.UserId);
                    await _uow.Conversations.AddAsync(conversation);
                    await _uow.SaveChangesAsync();
                    return _mapper.Map<ConversationDto>(conversation);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating conversation for user {UserId}", request.UserId);
                    throw;
                }
            }
        }
    }
}
