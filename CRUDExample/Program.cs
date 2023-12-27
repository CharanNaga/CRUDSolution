using CRUDExample.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using Rotativa.AspNetCore;
using Serilog;
using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);

//Configuring Logging with Serilog
builder.Host.UseSerilog(
    (HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
    {
        loggerConfiguration
        .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
        .ReadFrom.Services(services); //read current application services and make them available to serilog
    });

//adds controllers & views as services
builder.Services.AddControllersWithViews(options =>
{
    //options.Filters.Add<ResponseHeaderActionFilter>(); //set as global filter but it won't accept parameters
    var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();
    options.Filters.Add(new ResponseHeaderActionFilter(logger,"CustomKey-FromGlobal","CustomValue-FromGlobal"));
});

builder.Services.AddScoped<ICountriesRepository,CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository,PersonsRepository>();

builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();

//adding DbContext as a service
builder.Services.AddDbContext<ApplicationDbContext>( //by default scoped service.
    options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

//adding HttpLogging as a service
builder.Services.AddHttpLogging(options=>
{
    options.LoggingFields = HttpLoggingFields.RequestProperties 
    | HttpLoggingFields.ResponsePropertiesAndHeaders;
});

var app = builder.Build();

app.UseSerilogRequestLogging(); //enables endpoint completion log (HTTP GET Response success type log) i.e., adds extra log message as soon as request & resonse is completed

if(app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseHttpLogging(); //added HttpLogging to the middleware pipeline

//Configuring wkhtmltopdf file path here to identify the PDF file
if (app.Environment.IsEnvironment("Test") == false)
    RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program { } //we can access automatic generated program class programatically anywhere in the application