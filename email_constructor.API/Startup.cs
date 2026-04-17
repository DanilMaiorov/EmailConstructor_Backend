using email_constructor.Api.Services;
using email_constructor.Extensions;
using email_constructor.Infrastructure.DatabaseInitializer;

namespace email_constructor;

public class Startup
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="configuration"></param>
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
        services.AddMapper();
        services.AddParser();
        services.AddDatabaseIndexes();
        services.AddGrpc(options =>
        {
            options.MaxReceiveMessageSize = 10 * 1024 * 1024;
            options.MaxSendMessageSize = 10 * 1024 * 1024;
        });
        services.AddControllers();
        services.AddCors();
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DatabaseInitializer databaseInitializer)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        databaseInitializer.InitializeAsync().Wait();

        app.UseRouting();
        app.UseCors(policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapCommunicationApiGrpcEndpoints();
        });
    }
}