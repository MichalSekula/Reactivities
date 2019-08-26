using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Zmodyfikowalismy klse ktora odpowiada za uruchomienie calej aplikacji
            // zmiana polega na tym, iz za kazdym razem kiedy zmienimy cos w naszym Domain
            // i potrzebujemy zrobic update database, nie musismy tego wykonywac poprzez terminal 
            // lecz kod ponizej podczas uruchomienia utworzy baze danych jesli nie ma lub zrobi jej update 
            // o ostatnia migracje.
            var host = CreateWebHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<DataContext>();
                    context.Database.Migrate();
                    //inicjalizujemy baze danych o dane z Seed
                    Seed.SeedData(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occured during migration");
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
