using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;
using Services.PersonsServices;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// add services into IoC container
builder.Services.AddScoped<ICountriesService, CountriesService>();

//PersonsServices:
builder.Services.AddScoped<IPersonsGetterService, PersonsGetterServiceWithFewExcelFields>();

builder.Services.AddScoped<PersonsGetterService, PersonsGetterService>();


builder.Services.AddScoped<IPersonsAdderService, PersonsAdderService>();
builder.Services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
builder.Services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
builder.Services.AddScoped<IPersonsSorterService, PersonsSorterService>();
//

builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

if (!builder.Environment.IsEnvironment("Test"))
{
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program { }
//make the auto-generated Program accessible programmatically