using System.Configuration;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SalesWebMvc.Data;
using SalesWebMvc.Services;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SalesWebMvcContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("SalesWebMvcContext"), 
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("SalesWebMvcContext")),
            mySqlOptions => mySqlOptions.MigrationsAssembly("SalesWebMvc")));

// Add services to the container.
builder.Services.AddScoped<SeedingService>();
builder.Services.AddScoped<SellerService>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();
var scope = app.Services.CreateScope();
var seedingService = scope.ServiceProvider.GetRequiredService<SeedingService>();

var defaultCulture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
seedingService.Seed();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
