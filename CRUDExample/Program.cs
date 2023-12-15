using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

//add services into IoC Container
//builder.Services.AddSingleton<ICountriesService, CountriesService>();
//builder.Services.AddSingleton<IPersonsService, PersonsService>();

//After we change all the methods in service methods to perform operations with datastore (from in-memory collections to Database),
//we'll get an error saying can't consume scoped service( entities) from singleton service(services class).
//so, we'llconvert the lifetimes of Countries & Persons Services to Scoped Service Lifetime.

builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();

//adding DbContext as a service
builder.Services.AddDbContext<PersonsDbContext>( //by default scoped service.
    options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

var app = builder.Build();

if(app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();