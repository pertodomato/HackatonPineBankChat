//./src/PineBank.Application/Features/Conversations/Queries/GetOrCreateConversation.cs
namespace PineBank.Application.Features.Conversations.Queries
{
    using MediatR;
    using PineBank.Application.DTOs;
    using PineBank.Domain.Interfaces;
    using PineBank.Domain.Entities;
    using AutoMapper;

    public class GetOrCreateConversation
    {
        public record Query(string UserId) : IRequest<ConversationDto>;

        public class Handler : IRequestHandler<Query, ConversationDto>
        {
            private readonly IUnitOfWork _uow;
            private readonly IMapper _mapper;

            public Handler(IUnitOfWork uow, IMapper mapper)
            {
                _uow = uow;
                _mapper = mapper;
            }

            public async Task<ConversationDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var conversations = await _uow.Conversations.GetByUserIdAsync(request.UserId);
                var conversation = conversations.FirstOrDefault(c => c.IsActive);

                if (conversation == null)
                {
                    conversation = new Conversation(request.UserId);
                    await _uow.Conversations.AddAsync(conversation);
                    await _uow.SaveChangesAsync();
                }

                return _mapper.Map<ConversationDto>(conversation);
            }
        }
    }
}
