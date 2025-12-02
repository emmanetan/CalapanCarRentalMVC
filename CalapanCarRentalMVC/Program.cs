using Microsoft.EntityFrameworkCore;
using CalapanCarRentalMVC.Data;
using CalapanCarRentalMVC.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add MySQL Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CarRentalContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Register Email Service
builder.Services.AddScoped<IEmailService, EmailService>();

// Bind Traccar options and register HttpClient
var traccarSection = builder.Configuration.GetSection("Traccar");
var traccarOptions = traccarSection.Get<TraccarOptions>() ?? new TraccarOptions();
builder.Services.AddSingleton(traccarOptions);
builder.Services.AddHttpClient<ITraccarService, TraccarService>(client =>
{
    if (!string.IsNullOrWhiteSpace(traccarOptions.BaseUrl))
    {
        client.BaseAddress = new Uri(traccarOptions.BaseUrl.TrimEnd('/') + "/");
    }
    if (!string.IsNullOrWhiteSpace(traccarOptions.ApiKey))
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", traccarOptions.ApiKey);
    }
});

// Configure Authentication with Cookie and Google
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "Google";
})
.AddCookie(options =>
{
    options.Cookie.Name = ".CalapanCarRental.Auth";
    options.Cookie.HttpOnly = false;
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(14); //2 weeks
    options.SlidingExpiration = true;
    options.Cookie.IsEssential = true;
})
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    googleOptions.CallbackPath = "/signin-google";
    googleOptions.SaveTokens = true;

    // Handle authentication failures gracefully with logging
    googleOptions.Events.OnRemoteFailure = context =>
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger("GoogleAuth");
        logger.LogError(context.Failure, "Google authentication failed: {Message}", context.Failure?.Message);

        // Check if user cancelled the authentication
        if (context.Failure?.Message.Contains("access_denied") == true ||
            context.Failure?.Message.Contains("denied") == true)
        {
            context.Response.Redirect("/Account/Login?cancelled=true");
        }
        else
        {
            context.Response.Redirect("/Account/Login?error=true");
        }

        context.HandleResponse();
        return Task.CompletedTask;
    };
});


builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
    options.OnAppendCookie = cookieContext =>
    {
        // Force SameSite=None and Secure for Google correlation cookies
        if (cookieContext.CookieName.StartsWith(".AspNetCore.Correlation"))
        {
            cookieContext.CookieOptions.SameSite = SameSiteMode.None;
            cookieContext.CookieOptions.Secure = true;
        }
    };
});


// Add Session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();
app.UseCookiePolicy(); //for google fix
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
