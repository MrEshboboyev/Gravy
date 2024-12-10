using FluentValidation;
using Gravy.Application.Behaviors;
using Gravy.Application.Services.Deliveries;
using Gravy.Application.Services.Orders;
using Gravy.Infrastructure.Idempotence;
using MediatR;

namespace Gravy.App.Configurations;

public class ApplicationServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(Application.AssemblyReference.Assembly);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));

        services.Decorate(typeof(INotificationHandler<>), typeof(IdempotentDomainEventHandler<>));

        services.AddValidatorsFromAssembly(
            Application.AssemblyReference.Assembly,
            includeInternalTypes: true);

        services.AddScoped<IDeliveryPersonSelector, DeliveryPersonSelector>();
        services.AddScoped<IPricingService, PricingService>();
    }
}