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

            services.AddScoped<ICountriesGetterService, CountriesGetterService>();
            services.AddScoped<ICountriesAdderService, CountriesAdderService>();
            services.AddScoped<ICountriesUploaderService, CountriesUploaderService>();

            //services.AddScoped<IPersonsGetterService, PersonsGetterService>();

            //1. Open closed principle using Interfaces.
            services.AddScoped<IPersonsGetterService, PersonsGetterServiceWithFewExcelFields>(); //Added GetterService with the new functionality into IoC container. So in the client classes, FewExcelFields implementation would be available

            //2. Open closed principle using Inheritance.
            //services.AddScoped<IPersonsGetterService, PersonsGetterServiceChild>(); //Added GetterService with the new functionality into IoC container. So in the client classes, PersonsGetterServiceChild implementation would be available

            services.AddScoped<PersonsGetterService, PersonsGetterService>(); //when any class asks object for PersonsGetterService, provide object of PersonsGetterService as an argument.
            services.AddScoped<IPersonsAdderService, PersonsAdderService>();
            services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
            services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
            services.AddScoped<IPersonsSorterService, PersonsSorterService>();

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
