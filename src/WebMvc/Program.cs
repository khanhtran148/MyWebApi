using System.Reflection;
using Mapster;
using MapsterMapper;
using MyWebApi.Application;
using MyWebApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add mapster
TypeAdapterConfig config = TypeAdapterConfig.GlobalSettings;
config.Scan(typeof(Program).Assembly,
    Assembly.Load("MyWebApi.Application"),
    Assembly.Load("MyWebApi.Infrastructure"));
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();

// Options
builder.Services.AddOptions();

// DI
builder.Services.RegisterDatabase(configuration);
builder.Services.RegisterApplicationDependencies(configuration);
builder.Services.RegisterInfrastructureDependencies(configuration);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
