using BlisTimer.Data;
using Microsoft.EntityFrameworkCore;
using BlisTimer.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<UserDataHolder>();
builder.Services.AddScoped<HomeController>();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<TimerDbContext>(
        x => x.UseNpgsql(builder.Configuration.GetConnectionString("TimerDb"))
        );

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
