using ClubScansub.Data;
using ClubScansub.Service;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
//using Microsoft.AspNetCore.Authentication.Facebook;
using System.Globalization;

namespace ClubScansub
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                //options.UseMySql(Configuration.GetConnectionString("MySQLConnection"));
            });

            services.AddIdentity<IdentityUser, IdentityRole>(options => {

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


            services.AddScoped<IDbInitializer, DbInitializer>();

            services.Configure<FacebookOptions>(Configuration.GetSection("facebook"));

            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddSingleton<ISmsSender, SmsSender>();

            services.Configure<EmailOptions>(Configuration.GetSection("Email"));
            services.Configure<SmsOptions>(Configuration.GetSection("SMS"));


            services.AddAuthentication()
                .AddFacebook(options =>
                {
                    options.AppId = Configuration.GetSection("Facebook").GetValue<string>("AppId");
                    options.AppSecret = Configuration.GetSection("Facebook").GetValue<string>("AppSecret");

                })
                ;

            services.AddMvc(options =>
            {
                // MvcOptions.EnableEndpointRouting = false
                options.EnableEndpointRouting = false;
            });
            services.AddApplicationInsightsTelemetry();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IDbInitializer dbInit)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                //The default HSTS value is 30 days.You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            dbInit.Initialize();

            var defaultCulture = new CultureInfo("da-DK");
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(defaultCulture),
                SupportedCultures = new List<CultureInfo> { defaultCulture },
                SupportedUICultures = new List<CultureInfo> { defaultCulture }
            };

            app.UseRequestLocalization(localizationOptions);

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "areas",
                  template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

            });
        }
    }
}
