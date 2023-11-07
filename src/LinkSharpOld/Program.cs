using FastEndpoints;
using FastEndpoints.Swagger;
using LinkSharp.Configuration;
using LinkSharp.Database;
using Serilog;

public class Program
{
    public static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCOR_ENVIRONMENT")}.json", true)
            .Build();

        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
        
        MainAsync(args).Wait();
    }

    private static async Task MainAsync(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();

            builder.Services.AddLinkSharpIdentity(builder.Configuration);

            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication()
                .ConfigureOpenIdConnectAuth(builder.Configuration);
            
            builder.Services.AddFastEndpoints();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseFastEndpoints();
            app.UseSwaggerGen();

            await app.RunAsync();
        }
        catch (HostAbortedException)
        {
            throw;
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Startup failed...");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}