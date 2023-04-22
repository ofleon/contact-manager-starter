using ContactManagerStarter.Provider.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ContactManagerStarter.Extensions;

public static class ContactManagerStarterDbContextExtensions
{
    public static void AddContactManagerDb(this IServiceCollection services, IConfiguration configuration)
    {
        //var a = configuration.GetConnectionString("ContactDb");
        services.AddDbContext<ContactManagerStarterDbContext>(options =>
        {
            options.UseSqlServer("Data Source=localhost;Initial Catalog=ContactDb;User ID=sa;Password=Pa$$w0rd;Integrated Security=False;TrustServerCertificate=True", opts => opts.CommandTimeout(600));
        });
    }
    public static void EnsureContactDbIsCreated(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetService<ContactManagerStarterDbContext>();
        context.Database.EnsureCreated();
        context.Database.CloseConnection();
    }
}
