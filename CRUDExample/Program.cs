using CRUDExample;
using CRUDExample.Middleware;
using Rotativa.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Configuring Logging with Serilog
builder.Host.UseSerilog(
    (HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
    {
        loggerConfiguration
        .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
        .ReadFrom.Services(services); //read current application services and make them available to serilog
    });
builder.Services.ConfigureServices(builder.Configuration); //seperated the added services into a startupextensions file

var app = builder.Build();

app.UseSerilogRequestLogging(); //enables endpoint completion log (HTTP GET Response success type log) i.e., adds extra log message as soon as request & resonse is completed

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
{
    app.UseExceptionHandler("/Error"); //builtin exceptional handler
    app.UseExceptionHandlingMiddleware();
}
    

app.UseHttpLogging(); //added HttpLogging to the middleware pipeline

//Configuring wkhtmltopdf file path here to identify the PDF file
if (app.Environment.IsEnvironment("Test") == false)
    RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program { } //we can access automatic generated program class programatically anywhere in the application