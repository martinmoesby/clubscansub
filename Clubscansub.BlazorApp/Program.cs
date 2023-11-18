using Clubscansub.BlazorApp;
using Clubscansub.BlazorApp.Areas.Identity;
using ClubScansub.Data;
using ClubScansub.Models;
using ClubScansub.Service;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString); 
}, ServiceLifetime.Transient);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {

    options.SignIn = new SignInOptions
    {
        RequireConfirmedEmail = false,
        RequireConfirmedPhoneNumber = false,
    };

    options.Password = new PasswordOptions
    {
        RequireDigit = false,
        RequiredLength = 3,
        //RequiredUniqueChars = 3,
        RequireLowercase = false,
        RequireNonAlphanumeric = false,
        RequireUppercase = false
    };

    options.Lockout = new LockoutOptions
    {
        AllowedForNewUsers = true,
        DefaultLockoutTimeSpan = new TimeSpan(0, 15, 0),
        MaxFailedAccessAttempts = 3
    };

})
.AddDefaultTokenProviders()
.AddDefaultUI() // UIFramework.Bootstrap4)
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();

builder.Services.AddAuthentication();

builder.Services.Configure<ServiceOptions>(options =>
{
    options.ConnectionString = connectionString;
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddHubOptions(o =>
{
    o.MaximumReceiveMessageSize = 10 * 1024 * 1024;
});
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

builder.Services.AddTransient<UserService>();

var app = builder.Build();

var supportedCultures = new[]
{
    new CultureInfo("da-DK"),
    new CultureInfo("en-US")
};
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("da-DK"),
    SupportedUICultures = supportedCultures,
    SupportedCultures = supportedCultures
});


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<BlazorLoginMiddleware<ApplicationUser>>();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();