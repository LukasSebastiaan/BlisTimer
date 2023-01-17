using System.Globalization;
using BlisTimer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TimerDbContext>(
        x => x.UseNpgsql(builder.Configuration.GetConnectionString("TimerDb"))
        );
builder.Services.AddScoped<ApiDatabaseHandler>();

builder.WebHost.UseStaticWebAssets();

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = new PathString("/Login");
        options.AccessDeniedPath = new PathString("/Login?error=999");
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(3);
    });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Login?error=100");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}


app.Use(async (context, next) =>
{
    // Console.WriteLine(context.Request.Path.Value);
    
    await next.Invoke();
});

app.UseStatusCodePagesWithRedirects("/Login?error={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

var nlCulture = new CultureInfo("nl-NL");
Thread.CurrentThread.CurrentCulture = nlCulture;
Thread.CurrentThread.CurrentUICulture = nlCulture;

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Timer}/{action=Index}/{id?}");

app.Run();
