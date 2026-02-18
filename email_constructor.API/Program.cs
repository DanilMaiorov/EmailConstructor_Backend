using email_constructor;
using email_constructor.Services;

internal class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("Starting up");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception)
        {
            throw new Exception("Application start-up failed");
        }
        finally
        {
            Console.WriteLine("Upalo");
        }
    }
    
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .UseEnvironment("Development")
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;
                    
                config
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
            });
}