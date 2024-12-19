// ./src/PineBank.Application/ApplicationServicesRegistration.cs
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using FluentValidation;
using FluentValidation.AspNetCore;
using AutoMapper;
using PineBank.Domain.Interfaces;
using System.Reflection;

namespace PineBank.Application
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            // Adicione outros serviços conforme necessário
            return services;
        }
    }
}
