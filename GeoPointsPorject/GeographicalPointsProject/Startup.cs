using AspNet.Security.OpenIdConnect.Primitives;
using GP.Lib.Base.DataLayer;
using GP.Lib.Common;
using GP.Lib.Common.Constants;
using GP.Lib.Repo;
using GP.Lib.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GeographicalPointsProject
{
    public class Startup
    {
        #region Fields
        private IConfiguration Configuration { get; }
        private readonly bool _forceSsl = false;
        private string _baseUrl;
        #endregion
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration["Data:ConnectionString"]);
                options.UseOpenIddict();
            });

            services.AddIdentity<DbUser, DbRole>(options =>
            {
                // options for user and password can be set here
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredUniqueChars = 0;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });

            services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore().UseDbContext<ApplicationDbContext>();
            })
            .AddServer(options =>
            {
                options.UseMvc();

                options.EnableAuthorizationEndpoint("/connect/authorize")
                    .EnableTokenEndpoint("/connect/token");

                options.AllowPasswordFlow().AllowRefreshTokenFlow();

                options.SetAccessTokenLifetime(TimeSpan.FromHours(10))
                    .SetIdentityTokenLifetime(TimeSpan.FromHours(10))
                    .SetRefreshTokenLifetime(TimeSpan.FromHours(10));

                if (!_forceSsl)
                    options.DisableHttpsRequirement();

                options.DisableScopeValidation();
            });
            ///////////////
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddOAuthValidation(options =>
            {
                options.Events.OnRetrieveToken = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        (path.StartsWithSegments("/hub")))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                };
            });

            services.AddMvc(options =>
            {
                options.RequireHttpsPermanent = _forceSsl;
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddHttpContextAccessor();
            services.AddScoped<ApplicationDbContext>();
            services.AddRepositories();
            services.AddServices();
            services.AddCommonServices();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _baseUrl = app.ServerFeatures.Get<IServerAddressesFeature>().Addresses.FirstOrDefault();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.UseCookiePolicy();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            InitializeAsync(app.ApplicationServices).GetAwaiter().GetResult();
        }

        private async Task InitializeAsync(IServiceProvider services)
        {
            // Create a new service scope to ensure the database context is correctly disposed when this methods returns.
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var manager = scope.ServiceProvider.GetRequiredService<OpenIddictApplicationManager<OpenIddictApplication>>();

                ////await context.Database.EnsureCreatedAsync();
                await context.Database.MigrateAsync();
                #region Update Identity
                if (await manager.FindByClientIdAsync(ConstantApp.ApiClientId) == null)
                {
                    var descriptor = new OpenIddictApplicationDescriptor
                    {
                        ClientId = ConstantApp.ApiClientId,
                        ClientSecret = ConstantApp.ApiClientIdSecret,
                        DisplayName = ConstantApp.ApiDisplayName,
                        PostLogoutRedirectUris = { new Uri(_baseUrl + "/signout-oidc", UriKind.Absolute) },
                        RedirectUris = { new Uri(_baseUrl + "/", UriKind.Absolute) },
                        Permissions =
                        {
                            OpenIddictConstants.Permissions.Endpoints.Authorization,
                            OpenIddictConstants.Permissions.Endpoints.Token,
                            OpenIddictConstants.Permissions.GrantTypes.Implicit,
                            OpenIddictConstants.Permissions.GrantTypes.Password,
                            OpenIddictConstants.Permissions.GrantTypes.RefreshToken
                        }
                    };
                    await manager.CreateAsync(descriptor);
                }
                #endregion

                #region Update SQL
                var assembly = typeof(ApplicationDbContext).GetTypeInfo().Assembly;
                var resourcesNames = assembly.GetManifestResourceNames().OrderBy(n => n).ToArray();
                {
                    foreach (var res in resourcesNames)
                    {
                        var stream = assembly.GetManifestResourceStream(res);
                        using (var reader = new StreamReader(stream))
                        {
                            var commands = reader.ReadToEnd().Split("\nGO");
                            foreach (var command in commands)
                            {
                                var cmd = command.Trim('\r', '\n');
                                try
                                {
                                    context.Database.ExecuteSqlCommand(cmd);
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception($"Error occurred reading command{cmd}", ex);
                                }
                            }
                        }
                    }
                }
                #endregion
            }
        }
    }
}
