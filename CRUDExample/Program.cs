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

builder.Services.AddControllersWithViews();

//After we change all the methods in service methods to perform operations with datastore (from in-memory collections to Database),
//we'll get an error saying can't consume scoped service( entities) from singleton service(services class).
//so, we'llconvert the lifetimes of Countries & Persons Services to Scoped Service Lifetime.
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

//Creating Logs
//LogLevel => Debug, Information, Warning, Error, & Critical. If min loglevel is set to Information, we can access all logs apart from Debug.

//app.Logger.LogDebug("debug-message");
//app.Logger.LogInformation("information-message"); //Minimum log level is set to information, when we run application only we can see logs apart from Debug in Kestrel window. which we can set in appsettings.json
//app.Logger.LogWarning("warning-message");
//app.Logger.LogError("error-message");
//app.Logger.LogCritical("critical-message");

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