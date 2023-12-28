using CRUDExample.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;

namespace CRUDExample
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services,IConfiguration configuration)
        {
            //Use ctrl + H key and replace all "builder.Services" names with "services"

            //add ResponseHeaderActionFilter as a service
            services.AddTransient<ResponseHeaderActionFilter>();

            //adds controllers & views as services
            services.AddControllersWithViews(options =>
            {
                //options.Filters.Add<ResponseHeaderActionFilter>(5); //set as global filter but it won't accept parameters other than order
                var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();
                //options.Filters.Add(new ResponseHeaderActionFilter("CustomKey-FromGlobal","CustomValue-FromGlobal",2));

                options.Filters.Add(new ResponseHeaderActionFilter(logger)
                {
                    Key = "CustomKey-FromGlobal",
                    Value = "CustomValue-FromGlobal",
                    Order = 2
                });
            });

            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<IPersonsRepository, PersonsRepository>();

            services.AddScoped<ICountriesService, CountriesService>();
            services.AddScoped<IPersonsService, PersonsService>();

            //adding DbContext as a service
            services.AddDbContext<ApplicationDbContext>( //by default scoped service.
                options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                });

            //adding PersonsListActionFilter as a service
            services.AddTransient<PersonsListActionFilter>();

            //adding HttpLogging as a service
            services.AddHttpLogging(options =>
            {
                options.LoggingFields = HttpLoggingFields.RequestProperties
                | HttpLoggingFields.ResponsePropertiesAndHeaders;
            });
            return services;
        }
    }
}
