namespace HoneyWebPlatform.Web
{
    using System.Reflection;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using HoneyWebPlatform.Services.Data.Models;
    using HoneyWebPlatform.Web.Areas.Hubs;
    using Hubs;
    using Data;
    using Data.Models;
    using Infrastructure.Extensions;
    using Infrastructure.ModelBinders;
    using Services.Data.Interfaces;
    using Services.Mapping;
    using ViewModels.Home;
    using static Common.GeneralApplicationConstants;

    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            string connectionString =
                builder.Configuration.GetConnectionString("DefaultConnection")
                    //builder.Configuration.GetConnectionString("AzureConnection") 
                    ?? throw new InvalidOperationException
                    ("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<HoneyWebPlatformDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount =
                    builder.Configuration.GetValue<bool>("Identity:SignIn:RequireConfirmedAccount");
                options.Password.RequireLowercase =
                    builder.Configuration.GetValue<bool>("Identity:Password:RequireLowercase");
                options.Password.RequireUppercase =
                    builder.Configuration.GetValue<bool>("Identity:Password:RequireUppercase");
                options.Password.RequireNonAlphanumeric =
                    builder.Configuration.GetValue<bool>("Identity:Password:RequireNonAlphanumeric");
                options.Password.RequiredLength =
                    builder.Configuration.GetValue<int>("Identity:Password:RequiredLength");
            })
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<HoneyWebPlatformDbContext>();

            // Add email settings configuration here
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

            builder.Services.AddApplicationServices(typeof(IHoneyService));
            // Print the list of added services
            //PrintAddedServicesForEntities(builder.Services);


            builder.Services.AddRecaptchaService();

            builder.Services.AddMemoryCache();
            builder.Services.AddResponseCaching();

            builder.Services.AddSignalR();

            builder.Services.ConfigureApplicationCookie(cfg =>
            {
                cfg.LoginPath = "/User/Login";
                cfg.AccessDeniedPath = "/Home/Error/401";
            });

            builder.Services
                .AddControllersWithViews()
                .AddMvcOptions(options =>
                {
                    options.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
                    options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
                });

            builder.Services.AddTransient<IEmailSender, EmailSender>();

            //todo
            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = "YOUR_GOOGLE_CLIENT_ID";
                    options.ClientSecret = "YOUR_GOOGLE_CLIENT_SECRET";
                })
                .AddFacebook(options =>
                {
                    options.AppId = "YOUR_FACEBOOK_APP_ID";
                    options.AppSecret = "YOUR_FACEBOOK_APP_SECRET";
                });


            WebApplication app = builder.Build();

            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error/500");
                app.UseStatusCodePagesWithRedirects("/Home/Error?statusCode={0}");

                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseResponseCaching();

            app.UseAuthentication();
            app.UseAuthorization();

            app.EnableOnlineUsersCheck();

            if (app.Environment.IsDevelopment())
            {
                app.SeedAdministrator(DevelopmentAdminEmail);
            }

            app.UseEndpoints(config =>
            {
                config.MapControllerRoute(
                    name: "areas",
                    pattern: "/{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                config.MapControllerRoute(
                    name: "ProtectingUrlRoute",
                    pattern: "/{controller}/{action}/{id}/{information}",
                    defaults: new { Controller = "Category", Action = "Details" });

                config.MapDefaultControllerRoute();
                config.MapRazorPages();
                config.MapHub<ChatHub>("/chatHub");
                config.MapHub<CartHub>("/cartHub");
                //for admin order panel
                config.MapHub<OrderHub>("/orderHub");
            });


            app.Run();
        }

        /// Checking if all the services are added for each entity
        private static void PrintAddedServicesForEntities(IServiceCollection services)
        {
            var entityNames = new[] { "Beekeeper", "Honey", "Post", "Flavour", "Category", "Propolis", "SubscribedEmail", "User" /* Add other entity names here */ };

            Console.WriteLine("Added Services for Entities:");
            foreach (var entityName in entityNames)
            {
                var matchingServices = services
                    .Where(service => service.ServiceType.Name.Contains(entityName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (matchingServices.Any())
                {
                    Console.WriteLine($"- {entityName} Services:");
                    foreach (var service in matchingServices)
                    {
                        Console.WriteLine($"  - {service.ServiceType.Name} | Implementation: {service.ImplementationType?.Name ?? "N/A"}");
                    }
                    Console.WriteLine();
                }
            }
        }

    }

}