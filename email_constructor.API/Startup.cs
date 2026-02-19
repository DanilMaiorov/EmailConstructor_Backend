using email_constructor.Extensions;
using email_constructor.Infrastructure.DatabaseInitializer;
using email_constructor.Services;

namespace email_constructor;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    
    public IConfiguration Configuration { get; }
    
    public void ConfigureServices(IServiceCollection services)
    {
        services.RegisterMongo(Configuration);
        services.AddRepositories();
        services.AddBlockServices();
        services.AddDatabaseIndexes();
        services.AddGrpc(options =>
        {
            options.MaxReceiveMessageSize = 10 * 1024 * 1024;
            options.MaxSendMessageSize = 10 * 1024 * 1024;
        });
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DatabaseInitializer databaseInitializer)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        databaseInitializer.InitializeAsync().Wait();

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<EmailContentBlockService>();
        });
    }
}