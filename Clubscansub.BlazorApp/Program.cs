using Clubscansub.BlazorApp;
using Clubscansub.BlazorApp.Areas.Identity;
using ClubScansub.Data;
using ClubScansub.Models;
using ClubScansub.Service;
using ClubScansub.Service.Communication;
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

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{

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
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultUI()
.AddDefaultTokenProviders()
; // UIFramework.Bootstrap4)

builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();

builder.Services.AddAuthentication();

builder.Services.Configure<ServiceOptions>(options =>
{
    options.ConnectionString = connectionString;
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents();
builder.Services.AddRadzenComponents();

builder.Services.AddServerSideBlazor().AddHubOptions(o =>
{
    o.MaximumReceiveMessageSize = 10 * 1024 * 1024;
});
//builder.Services.AddScoped<ThemeService>();
//builder.Services.AddScoped<DialogService>();
//builder.Services.AddScoped<NotificationService>();
//builder.Services.AddScoped<TooltipService>();
//builder.Services.AddScoped<ContextMenuService>();

builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddSingleton<ISmsSender, SmsSender>();

builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<MemberService>();
builder.Services.AddTransient<SiteService>();
builder.Services.AddTransient<EventService>();
builder.Services.AddTransient<EventUserService>();
builder.Services.AddTransient<CourseService>();

builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));
builder.Services.Configure<SmsOptions>(builder.Configuration.GetSection("SMS"));

var app = builder.Build();

// Apply database migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    if (context.Database.GetPendingMigrations().ToList().Count() > 0)
    {
        context.Database.Migrate();
    }
}

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