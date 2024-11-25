using Gravy.Infrastructure.BackgroundJobs;
using Quartz;

namespace Gravy.App.Configurations;

public class BackgroundJobsServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        // Register the job with DI
        services.AddScoped<IJob, ProcessOutboxMessagesJob>();

        // Configure Quartz
        services.AddQuartz(configure =>
        {
            var jobKey = new JobKey(nameof(ProcessOutboxMessagesJob));
            configure
                .AddJob<ProcessOutboxMessagesJob>(jobKey)  // Add the job
                .AddTrigger(                               // Add a trigger
                    trigger =>
                        trigger.ForJob(jobKey) 
                            .WithSimpleSchedule(
                                schedule =>
                                    schedule.WithIntervalInSeconds(10)
                                        .RepeatForever()));
            // Remove the obsolete method. The default DI job factory is now used automatically.
        });

        // Add Quartz as a hosted service
        services.AddQuartzHostedService();
    }
}