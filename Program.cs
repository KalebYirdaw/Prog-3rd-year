using GLMS.Services;
using GLMS.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// ============================================
// REGISTER ALL SERVICES
// ============================================

// Email Service
builder.Services.AddScoped<IEmailService, EmailService>();

// Currency Strategy and Service
builder.Services.AddScoped<ICurrencyStrategy, ExchangeRateApiStrategy>();
builder.Services.AddScoped<CurrencyService>();

// API Service
builder.Services.AddScoped<IApiService, ApiService>();

// HTTP Client for API (read from configuration)
builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    // Read API URL from appsettings.json or environment variable
    var apiUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5095/";
    client.BaseAddress = new Uri(apiUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// HTTP Client for external APIs (Exchange Rate)
builder.Services.AddHttpClient();

// HTTP Context and Cache
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();

// Session Configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Cookie Authentication (no database required)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// ============================================
// CONFIGURE HTTP PIPELINE
// ============================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Default route - Login page first
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();