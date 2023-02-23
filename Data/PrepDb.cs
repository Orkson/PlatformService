using Microsoft.EntityFrameworkCore;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProd)
        {
            using(var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>()!, isProd);
                
            }

        }
        private static void SeedData(AppDbContext context, bool isProd)
        {
            if(isProd)
            {
                Console.WriteLine("--> Attempting to apply migrations..");
                try
                {
                    context.Database.Migrate();
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"--> Could not run migrations: {ex.Message}");
                }
            }
            if(!context.Platforms.Any())
            {
                Console.WriteLine("seeding data...");

                context.Platforms.AddRange(
                    new Models.Platform(){Name="Dot Net", Publisher="Microsoft", Cost="Free"},
                    new Models.Platform(){Name="SQL Server", Publisher="Micosoft", Cost="Free"},
                    new Models.Platform(){Name="Kubernetes", Publisher="CNCF", Cost="Free"}
                );

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("we have data");
            }
        }
    }
}