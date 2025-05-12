using FamilyWallet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyWallet.API.Extensions
{
    public static class MigrationExtensions
    {
        public static IApplicationBuilder MigrateDatabase(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            using var db = serviceScope.ServiceProvider.GetRequiredService<FamilyWalletDbContext>();

            db.Database.Migrate();

            return app;
        }
    }
}
