using Havit.Blazor.Components.Web;
using LinkSharp.Configuration;
using LinkSharp.Database;
using MediatR;
using Serilog;

internal class Program
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

            builder.Services.AddHxServices();
            
            builder.Services.AddMediatR(typeof(Program).Assembly);

            builder.Services.AddLinkSharpIdentity(builder.Configuration);

            builder.Services.AddAuthentication()
                .ConfigureOpenIdConnectAuth(builder.Configuration);
            builder.Services.AddAuthorization();
        
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

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