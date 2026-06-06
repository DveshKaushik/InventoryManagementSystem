using DotNetEnv;
using InventoryManagementSystem.Models;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DatabaseHelper>(new DatabaseHelper(
    builder.Configuration.GetConnectionString("DefaultConnection")
));

builder.Services.AddSingleton<GroqService>(new GroqService(
    Environment.GetEnvironmentVariable("GROQ_API_KEY")
));

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();