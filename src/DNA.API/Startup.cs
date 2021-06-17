using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using AutoMapper;
using DNA.API.Controllers.Config;
using DNA.API.Extensions;
using DNA.API.Infrastructure;
using DNA.Domain;
using DNA.Persistence.Contexts;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using DNA.API.BackgroundServices.Hangfire;
using Hangfire.Dashboard;

namespace DNA.API {

    public class Startup {

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services) {

            services.AddControllersWithViews();

            services.AddCors(options => {
                options.AddDefaultPolicy(
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        );
            });

            services.ConfigureWritable(Configuration);

            services.AddCustomAuthentication(Configuration);

            // services.AddMemoryCache();

            services.AddCustomSwagger();

            services.AddControllers(o => {
                o.Conventions.Add(new ControllerHidingConvention(Configuration));
            })
                .AddNewtonsoftJson(options => {
                    options.SerializerSettings.ContractResolver = new JsonShouldSerializeContractResolver();
                    //options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                    options.UseMemberCasing();
                })
                .ConfigureApiBehaviorOptions(options => {
                    // Adds a custom error response factory when ModelState is invalid
                    options.InvalidModelStateResponseFactory = InvalidModelStateResponseFactory.ProduceErrorResponse;
                })
                ;

            services.AddSingleton<IAppDbContext>(o => {
                var appDbContext = new AppDbContext(Configuration);
                Dapper.Contrib.Extensions.SqlMapperExtensions.TableNameMapper = (Type type) => {
                    var table = type.GetCustomAttribute<Dapper.Contrib.Extensions.TableAttribute>();
                    return appDbContext.SetTablePrefix(table.Name);
                };
                return appDbContext;
            });

            services.AddHttpContextAccessor();

            //services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddHangfireServices(Configuration);

            services.AddCustomServices(Configuration);

            services.AddHostedService<BackgroundServices.TimedHostedService>();

            services.AddSubdomains();

            services.AddDirectoryBrowser();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddSpaStaticFiles(configuration => {
                configuration.RootPath = "ClientApp/build";
            });

            if (Configuration.GetSection("HttpsRedirection").GetValue<bool>("Enabled"))
                services.AddHttpsRedirection(options => {
                    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                    options.HttpsPort = Configuration.GetSection("HttpsRedirection").GetValue<int>("Port");
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {


            #region get client prefered language
            var section = Configuration.GetSection("MultiLanguage");
            var enabled = section.GetValue<bool?>("Enabled") ?? false;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(enabled ? section.GetValue<string>("Default") ?? "tr" : "tr");
            app.Use((context, next) => {
                var languages = section.GetSection("Languages").Get<string[]>();
                var firstLang = "tr";
                if (enabled) {
                    var def = section.GetValue<string>("Default") ?? firstLang;
                    var acceptLang = context.Request.Headers["Accept-Language"].ToString();
                    var clientLangs = string.IsNullOrWhiteSpace(acceptLang) ? def : acceptLang;
                    firstLang = clientLangs.Split(',').FirstOrDefault() ?? def;
                    if (firstLang.Contains("-"))
                        firstLang = firstLang.Split('-')[0];
                    if (!languages.Contains(firstLang))
                        firstLang = def ?? "tr";
                }
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(firstLang);

                return next();
            });
            #endregion

            if (!env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler(_ => {
                    _.Run(async context => {
                        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                        await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new Domain.Services.Communication.Response(exceptionHandlerPathFeature.Error.Message)));
                    });
                });

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                if (Configuration.GetSection("HttpsRedirection").GetValue<bool>("Enabled"))
                    app.UseHsts();
            }

            app.UseCors();

            app.UseCustomSwagger();

            if (Configuration.GetSection("HttpsRedirection").GetValue<bool>("Enabled"))
                app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions {
                Authorization = new[] {
                    new HangfireDashboardJwtAuthorizationFilter(MiddlewareExtensions.GetTokenValidationParameters(Configuration), Configuration.GetSection("Config:RecurringJobs:Roles").Get<List<string>>())
                },
                IsReadOnlyFunc = (DashboardContext context) => {

                    // Hangfire yetkili kiþi dýþýndakilere read only olacak

                    var allowedRoles = Configuration.GetSection("Config:RecurringJobs:Roles").Get<List<string>>();
                    var httpContext = context.GetHttpContext();
                    var user = httpContext.User;

                    // görevler için rol tanýmlanmamýþsa readonly, giriþ yapmamýþsa readonly
                    if (allowedRoles == null || allowedRoles.Count == 0 || user == null)
                        return true;

                    try {
                        var access_token = httpContext.Request.Cookies[HangfireDashboardJwtAuthorizationFilter.HangfireCookieName];
                        if (access_token == null)
                            return true;
                        Microsoft.IdentityModel.Tokens.SecurityToken validatedToken = null;
                        System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler hand = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                        var claims = hand.ValidateToken(access_token, MiddlewareExtensions.GetTokenValidationParameters(Configuration), out validatedToken);
                        var role = claims.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                        if (role == null)
                            return true;
                        if (allowedRoles.Contains(role))
                            return false;
                    }
                    catch {
                        return true;
                    }

                    return true;
                }
            });

            var fileServerPath = Configuration.GetSection("FileServerOptions").GetValue<string>("Dir");
            fileServerPath = string.IsNullOrWhiteSpace(fileServerPath)
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files")
                : fileServerPath;
            app.UseFileServer(new FileServerOptions {
                FileProvider = new PhysicalFileProvider(fileServerPath),
                RequestPath = "/" + Configuration.GetSection("FileServerOptions").GetValue<string>("RequestPath"),
                EnableDirectoryBrowsing = Configuration.GetSection("FileServerOptions").GetValue<bool>("Enabled")
            });

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

                endpoints.MapHangfireDashboard();

            });

            app.MapWhen(_ => !_.Request.Path.Value.StartsWith("/api"), builder => {
                builder.UseSpa(spa => {
                    spa.Options.SourcePath = "ClientApp";
                    if (env.IsDevelopment()) {
                        spa.Options.StartupTimeout = TimeSpan.FromSeconds(360);
                        //spa.UseReactDevelopmentServer(npmScript: "start");
                    }
                });
            });

            //app.UseSpa(spa => {
            //    spa.Options.SourcePath = "ClientApp";
            //    if (env.IsDevelopment()) {
            //        spa.Options.StartupTimeout = TimeSpan.FromSeconds(360);
            //        //spa.UseReactDevelopmentServer(npmScript: "start");
            //    }
            //});

        }
    }
}