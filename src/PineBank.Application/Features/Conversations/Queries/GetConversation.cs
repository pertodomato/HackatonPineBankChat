//./src/PineBank.Application/Features/Conversations/Queries/GetConversation.cs
namespace PineBank.Application.Features.Conversations.Queries
{
    using MediatR;
    using PineBank.Application.DTOs;
    using PineBank.Domain.Interfaces;
    using AutoMapper;

    public class GetConversation
    {
        public record Query(Guid ConversationId) : IRequest<ConversationDto>;

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
                var conversation = await _uow.Conversations.GetByIdAsync(request.ConversationId);
                if (conversation == null)
                    return null;
                return _mapper.Map<ConversationDto>(conversation);
            }
        }
    }
}
