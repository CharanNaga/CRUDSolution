using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CRUDTests
{
    //Class is made available for the integration tests but not for the clients(browsers)
    //All the services from the Program.cs will be loaded while compiled and we will try to use EFCore InMemoryCollection as an ApplicationDbContext
    public class CustomWebApplicationFactory: WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                //checking for a service having DbContextOptions<ApplicationDbContext>
                var descriptor = services.SingleOrDefault(
                    temp=> temp.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)
                    );

                //if found, then we will remove that service
                if(descriptor != null )
                    services.Remove(descriptor);

                //and then AddDbContext of ApplicationDbContext to use in memory collection of EFCore for test environment.
                services.AddDbContext<ApplicationDbContext>(
                    options =>
                    {
                        options.UseInMemoryDatabase("DatabaseForIntegrationTesting");
                    });
            });
        }
    }
}