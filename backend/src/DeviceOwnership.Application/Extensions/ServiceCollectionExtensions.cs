using DeviceOwnership.Application.Services;
using DeviceOwnership.Core.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceOwnership.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Auto Mapper
        services.AddAutoMapper(typeof(ServiceCollectionExtensions).Assembly);

        // Fluent Validation
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

        // Application Services
        services.AddScoped<IDeviceService, DeviceService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPaymentService, PaymentService>();

        return services;
    }
}
