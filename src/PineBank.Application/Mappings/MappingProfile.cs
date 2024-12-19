//PineBank\src\PineBank.Application\Mappings\MappingProfile.cs
using AutoMapper;
using PineBank.Application.DTOs;
using PineBank.Domain.Entities;

namespace PineBank.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Conversation, ConversationDto>();
            CreateMap<Message, MessageDto>();
            CreateMap<MessageMetadata, MessageMetadataDto>();
            CreateMap<ChatResponse, ChatbotResponseDto>()
                .ForMember(dest => dest.Metadata, opt => opt.MapFrom(src => src.Metadata));
            // Adicione mapeamentos adicionais conforme necessário
        }
    }
}
