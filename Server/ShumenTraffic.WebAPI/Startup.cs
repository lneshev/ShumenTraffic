using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MoravianStar.WebAPI.Transformers;
using ShumenTraffic.Data.Context;
using ShumenTraffic.WebAPI.Configuration;
using ShumenTraffic.WebAPI.Filters;
using ShumenTraffic.WebAPI.Middleware;
using ShumenTraffic.WebAPI.Services.Common;
using System;
using System.Threading.Tasks;

namespace ShumenTraffic.WebAPI
{

    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure options
            services.Configure<ConsumersConfiguration>(Configuration.GetSection(nameof(ConsumersConfiguration)));
            services.Configure<UsersConfiguration>(Configuration.GetSection(nameof(UsersConfiguration)));

            // Add Controllers with validation filter
            services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
                options.Filters.Add<ValidationFilter>();
            })
            .AddControllersAsServices();

            // Add CORS
            var consumersConfig = Configuration.GetSection(nameof(ConsumersConfiguration)).Get<ConsumersConfiguration>();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.WithOrigins(
                            consumersConfig.WebApp.ApplicationUrl)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            // Add DbContext
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ShumenTrafficDbContext>(options =>
            {
                options
                    .UseSqlServer(connectionString, x =>
                    {
                        x.MigrationsAssembly("ShumenTraffic.WebAPI");
                    })
                    .UseAsyncSeeding(async (appDbContext, storeOperationPerformed, ct) =>
                    {
                        await DbSeeder.SeedAppDbAsync((ShumenTrafficDbContext)appDbContext);
                    });
            });

            // Add ASP.NET Core Identity
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                // Password requirements
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ShumenTrafficDbContext>()
            .AddDefaultTokenProviders();

            // Configure authentication cookies for cross-origin requests
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.HttpOnly = true;
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = 403;
                    return Task.CompletedTask;
                };
            });

            // Add Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Add Application Services
            services.AddTransient<IDbUpdater, DbUpdater>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Add exception handling middleware
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            InitDb();

            void InitDb()
            {
                using (var scope = app.ApplicationServices.CreateAsyncScope())
                {
                    var serviceProvider = scope.ServiceProvider;
                    var dbUpdaterService = serviceProvider.GetRequiredService<IDbUpdater>();
                    dbUpdaterService.CreateAndUpdateAsync().GetAwaiter().GetResult();
                }
            }
        }
    }
}