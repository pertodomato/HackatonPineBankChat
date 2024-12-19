// PineBank/src/PineBank.Infrastructure/DependencyInjection.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using PineBank.Domain.Interfaces;
using PineBank.Infrastructure.Persistence;
using PineBank.Application.Interfaces;
using PineBank.Infrastructure.Services;
using System.Reflection;
using MediatR;
using PineBank.Application.Mappings;
using OpenAI.Extensions;

namespace PineBank.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            // Register OpenAI service
            services.AddOpenAIService(settings => { 
                settings.ApiKey = config["OPENAI_API_KEY"];
            });

            // Register AutoMapper
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // Register MongoDB context and repositories
            services.AddSingleton<MongoDbContext>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register ChatService
            services.AddScoped<IChatService, ChatService>();

            // Register InstagramService
            services.AddScoped<IInstagramService, InstagramService>();

            // Register MediatR
            services.AddMediatR(Assembly.GetExecutingAssembly(), typeof(IChatService).Assembly);

            // Add Redis cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config["REDIS_CONNECTION"] ?? "localhost:6379";
            });

            return services;
        }
    }
}
