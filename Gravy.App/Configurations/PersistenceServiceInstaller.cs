using Gravy.Persistence.Interceptors;
using Gravy.Persistence;
using Microsoft.EntityFrameworkCore;
using Gravy.Domain.Repositories;
using Gravy.Persistence.Repositories;

namespace Gravy.App.Configurations;

public class PersistenceServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();
        services.AddSingleton<UpdateAuditableEntitiesInterceptor>();
        services.AddDbContext<ApplicationDbContext>(
            options => options.UseSqlServer(
                    configuration.GetConnectionString("Database")));
    }
}