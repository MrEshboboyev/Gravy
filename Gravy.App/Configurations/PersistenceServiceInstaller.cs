using Gravy.Persistence.Interceptors;
using Gravy.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gravy.App.Configurations;

public class PersistenceServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();
        services.AddSingleton<UpdateAuditableEntitiesInterceptor>();
        services.AddDbContext<ApplicationDbContext>(
            options => options.UseNpgsql(
                    configuration.GetConnectionString("Database")));
    }
}