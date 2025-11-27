using ElmahCore.Mvc;
using ElmahCore.Sql;
using Giserver.NetTopologySuite.Serialize;
using Giserver.NetTopologySuite.Swagger.Swashbuckle;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MoravianStar.Dao;
using MoravianStar.Extensions;
using MoravianStar.Settings;
using MoravianStar.WebAPI.Attributes;
using MoravianStar.WebAPI.Extensions;
using MoravianStar.WebAPI.JsonConverters;
using MoravianStar.WebAPI.Swagger;
using MoravianStar.WebAPI.Transformers;
using NetTopologySuite.IO;
using ShumenTraffic.Common.Core.Configuration;
using ShumenTraffic.Common.Core.Constants.Security;
using ShumenTraffic.Common.Core.Entities.BusLines;
using ShumenTraffic.Common.Core.Entities.BusStops;
using ShumenTraffic.Common.Core.Entities.Routes;
using ShumenTraffic.Common.Core.Entities.TransportationCompanies;
using ShumenTraffic.Common.Core.Entities.Zones;
using ShumenTraffic.Common.Core.Enums.Maintenance;
using ShumenTraffic.Common.Core.Enums.Routes;
using ShumenTraffic.Common.Core.Resources;
using ShumenTraffic.Common.DataAccess.DbContexts;
using ShumenTraffic.Common.Services.Interfaces;
using ShumenTraffic.Common.Services.Services.BusLines;
using ShumenTraffic.Common.Services.Services.BusStops;
using ShumenTraffic.Common.Services.Services.Maintenance;
using ShumenTraffic.Common.Services.Services.Routes;
using ShumenTraffic.Common.Services.Services.Schedules;
using ShumenTraffic.Common.Services.Services.TransportationCompanies;
using ShumenTraffic.Common.Services.Services.Zones;
using ShumenTraffic.Web.Core.Models.BusLines;
using ShumenTraffic.Web.Core.Models.BusStops;
using ShumenTraffic.Web.Core.Models.Routes;
using ShumenTraffic.Web.Core.Models.TransportationCompanies;
using ShumenTraffic.Web.Core.Models.Zones;
using ShumenTraffic.Web.Services.Interfaces;
using ShumenTraffic.Web.Services.Services.BusLines;
using ShumenTraffic.Web.Services.Services.BusStops;
using ShumenTraffic.Web.Services.Services.Routes;
using ShumenTraffic.Web.Services.Services.Schedules;
using ShumenTraffic.Web.Services.Services.TransportationCompanies;
using ShumenTraffic.Web.Services.Services.Zones;
using ShumenTraffic.Web.WebAPI.Infrastructure.Constants;
using ShumenTraffic.Web.WebAPI.Infrastructure.Middlewares;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShumenTraffic.Web.WebAPI
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure options
            services.Configure<ConsumersConfiguration>(configuration.GetSection(nameof(ConsumersConfiguration)));
            services.Configure<UsersConfiguration>(configuration.GetSection(nameof(UsersConfiguration)));

            // Add Controllers with validation filter
            services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
                options.Filters.Add<ValidateModelStateAttribute>();
            })
            .AddControllersAsServices()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new CustomStringTypeJsonConverter());
                foreach (var geoJsonConverter in GeoJsonSerializer.CreateDefault().Converters)
                {
                    options.SerializerSettings.Converters.Add(geoJsonConverter);
                }
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                // This is needed, because the logic in ValidateModelStateAttribute 
                // won't be triggered for controllers marked with ApiControllerAttribute
                options.SuppressModelStateInvalidFilter = true;
            });

            // Add CORS
            var consumersConfig = configuration.GetSection(nameof(ConsumersConfiguration)).Get<ConsumersConfiguration>();
            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyConstants.Default, policy =>
                {
                    policy.WithOrigins(
                            consumersConfig.WebApp.ApplicationUrl)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            services
                .AddDbContextPool<LogDbContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("Log"));
                });

            // Add DbContext
            var connectionString = configuration.GetConnectionString("App");
            services.AddDbContext<AppDbContext>(options =>
            {
                options
                    .UseSqlServer(connectionString, x =>
                    {
                        x.MigrationsAssembly(typeof(AppDbContext).Assembly);
                        x.UseNetTopologySuite();
                    })
                    .UseAsyncSeeding(async (appDbContext, storeOperationPerformed, ct) =>
                    {
                        await DbSeeder.SeedAppDbAsync((AppDbContext)appDbContext);
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
            .AddEntityFrameworkStores<AppDbContext>()
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

            // Add Elmah for error logging
            services.AddElmah<SqlErrorLog>(options =>
            {
                options.OnError = async (httpContext, error) =>
                {
                    if (error.Exception != null)
                    {
                        error.StatusCode = error.Exception.GetHttpStatusCode();
                    }
                    await Task.CompletedTask;
                };
                options.ConnectionString = configuration.GetConnectionString("Log");
                options.SqlServerDatabaseSchemaName = "dbo";
                options.SqlServerDatabaseTableName = "Elmah";
                options.OnPermissionCheck = (context) => context.User.Identity.IsAuthenticated && context.User.IsInRole(RoleConstants.SuperAdminRoleName);
            });

            // Add Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.DocumentFilter<HideInDocsFilter>();
                options.AddGeometry(GeoSerializeType.Geojson);
            });
            services.AddSwaggerGenNewtonsoftSupport();

            // Add Moravian Star services
            services.AddScoped<IDbTransaction<AppDbContext>, DbTransaction<AppDbContext>>();

            // Add Application Services
            services.AddTransient<IDbUpdater, DbUpdater>();

            // Add Domain Services (Common Layer)
            services.AddTransient<IEntityValidated<TransportationCompany>, TransportationCompanyEntityValidated>();
            services.AddTransient<IEntityDeleting<TransportationCompany>, TransportationCompanyEntityDeleting>();
            services.AddTransient<IEntityValidated<BusLine>, BusLineEntityValidated>();
            services.AddTransient<IEntityDeleting<BusLine>, BusLineEntityDeleting>();
            services.AddTransient<IEntityValidated<Zone>, ZoneEntityValidated>();
            services.AddTransient<IEntityDeleting<Zone>, ZoneEntityDeleting>();
            services.AddTransient<IEntityValidated<BusStop>, BusStopEntityValidated>();
            services.AddTransient<IEntityDeleting<BusStop>, BusStopEntityDeleting>();
            services.AddTransient<IEntityValidated<Route>, RouteEntityValidated>();
            services.AddTransient<IEntitySaving<Route>, RouteEntitySaving>();
            services.AddTransient<IEntityDeleting<Route>, RouteEntityDeleting>();
            services.AddScoped<IRouteService, RouteService>();
            services.AddScoped<IScheduleService, ScheduleService>();

            // Add Application Services (Web Layer)
            services.AddTransient<IModelsMappingService<TransportationCompanyModel, TransportationCompany>, TransportationCompanyModelsMappingService>();
            services.AddTransient<IModelsMappingService<BusLineModel, BusLine>, BusLineModelsMappingService>();
            services.AddTransient<IModelsMappingService<BusLineLightModel, BusLine>, BusLineLightModelsMappingService>();
            services.AddTransient<IModelsMappingService<ZoneModel, Zone>, ZoneModelMappingService>();
            services.AddTransient<IModelsMappingService<BusStopModel, BusStop>, BusStopModelsMappingService>();
            services.AddTransient<IModelsMappingService<RouteModel, Route>, RouteModelsMappingService>();
            services.AddTransient<IModelsMappingService<RouteOverviewModel, Route>, RouteOverviewModelsMappingService>();
            services.AddScoped<IRouteModelService, RouteModelService>();
            services.AddScoped<IScheduleModelService, ScheduleModelService>();
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

            app.UseMoravianStar(env, () =>
            {
                Settings.RegisterDefaultExceptionMiddleware = false;
                Settings.DefaultDbContextType = typeof(AppDbContext);
                Settings.StringResourceTypeForEnums = typeof(Strings);
                Settings.AssemblyForEnums = typeof(RouteDirection).Assembly;
            });

            // Add exception handling middleware
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseRouting();
            app.UseCors(CorsPolicyConstants.Default);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseElmah();

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
                    var result = dbUpdaterService.CreateAndUpdateAllAsync().GetAwaiter().GetResult();
                    if (result.State != DbsUpdateState.Success)
                    {
                        throw new AggregateException(string.Format(Strings.OneOrMoreDatabasesCouldNotBeUpdated, result.State), result.Results.Where(x => x.Exception != null).Select(x => x.Exception));
                    }
                }
            }
        }
    }
}