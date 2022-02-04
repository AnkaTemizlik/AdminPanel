using System;
using System.IO;
using System.Threading.Tasks;
using DNA.API.BackgroundServices.Hangfire;
using Hangfire;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.SqlServer;
using Hangfire.States;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using DNA.API.Infrastructure;
using DNA.API.Services;
using DNA.Domain.Repositories;
using DNA.Domain.Services;
using DNA.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using DNA.API.BackgroundServices;
using System.Runtime.Loader;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using DNA.Domain.Extentions;
using DNA.Domain.Models;

namespace DNA.API.Extensions {
    public static class MiddlewareExtensions {

        public static TokenValidationParameters GetTokenValidationParameters(IConfiguration configuration) {
            return new TokenValidationParameters {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration.GetSection("HttpsRedirection").GetValue<bool>("Enabled") ? configuration["Origins:Https"] : configuration["Origins:WEB"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"])),
                ClockSkew = TimeSpan.Zero // Override the default clock skew of 5 mins
            };
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration) {

            services.AddSingleton<IJwtFactory, JwtFactory>();
            
            // api user claim policy
            services.AddAuthorization(options => {
                options.AddPolicy(Policies.ReadOnly, builder => builder.RequireRole(Roles.Admin, Roles.Writer, Roles.Reader));
                options.AddPolicy(Policies.WriteOnly, builder => builder.RequireRole(Roles.Admin, Roles.Writer));
                options.AddPolicy(Policies.AdminOnly, builder => builder.RequireRole(Roles.Admin));
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = GetTokenValidationParameters(configuration);
                    //services.AddCors();
                });

            return services;
        }

        public static IServiceCollection AddHangfireServices(this IServiceCollection services, IConfiguration configuration) {

            services.TryAddSingleton<SqlServerStorageOptions>(new SqlServerStorageOptions {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.FromTicks(1),
                UseRecommendedIsolationLevel = true,
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(1)
            });

            services.TryAddSingleton<IBackgroundJobFactory>(x => new CustomBackgroundJobFactory(new BackgroundJobFactory(x.GetRequiredService<IJobFilterProvider>())));
            services.TryAddSingleton<IBackgroundJobPerformer>(x => new CustomBackgroundJobPerformer(new BackgroundJobPerformer(
                x.GetRequiredService<IJobFilterProvider>(),
                x.GetRequiredService<JobActivator>(),
                TaskScheduler.Default)));
            services.TryAddSingleton<IBackgroundJobStateChanger>(x => new CustomBackgroundJobStateChanger(new BackgroundJobStateChanger(x.GetRequiredService<IJobFilterProvider>())));

            services.AddHangfire((provider, config) => config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseSqlServerStorage(configuration.GetConnectionString("Default"), provider.GetRequiredService<SqlServerStorageOptions>()));

            services.AddHangfireServer(options => {
                options.StopTimeout = TimeSpan.FromSeconds(15);
                options.ShutdownTimeout = TimeSpan.FromSeconds(30);
            });

            return services;
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration) {

            services.AddClassesWithServiceAttribute("DNA.API");
            services.AddClassesWithServiceAttribute(AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DNA.Domain.dll")));
            services.AddClassesWithServiceAttribute(AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DNA.Persistence.dll")));

            var workerSection = configuration.GetSection("Worker");
            var workerEnabled = workerSection.GetValue<bool>("Enabled");

            if (workerEnabled) {
                var pluginLocation = workerSection.GetValue<string>("Assembly");
                if (string.IsNullOrWhiteSpace(Path.GetDirectoryName(pluginLocation))) {
                    pluginLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pluginLocation);
                    services.AddClassesWithServiceAttribute(AssemblyLoadContext.Default.LoadFromAssemblyPath(pluginLocation));
                }
                else
                    services.AddClassesWithServiceAttribute(AssemblyLoadContext.Default.LoadFromAssemblyPath(pluginLocation));
            }

            var modules = configuration.GetSection("Modules");
            var names = modules.GetSection("Names").Get<List<string>>();
            if (names != null)
                foreach (var name in names) {
                    var moduleSection = modules.GetSection(name);
                    var module = new {
                        Assembly = moduleSection.GetValue<string>("Assembly")
                    };
                    if (!string.IsNullOrWhiteSpace(module.Assembly)) {
                        var assembly = module.Assembly.ToLower().EndsWith(".dll") ? module.Assembly : module.Assembly + ".dll";
                        if (string.IsNullOrWhiteSpace(Path.GetDirectoryName(assembly))) {
                            var pluginLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assembly);
                            services.AddClassesWithServiceAttribute(AssemblyLoadContext.Default.LoadFromAssemblyPath(pluginLocation));
                        }
                        else
                            services.AddClassesWithServiceAttribute(AssemblyLoadContext.Default.LoadFromAssemblyPath(assembly));
                    }
                }
            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration) {

            services.AddOpenApiDocument(document => {

                document.Title = $"{configuration["Config:Company:CompanyName"]} - {configuration["Config:Company:ProgramName"]} - Open API Documentation";

                document.AddSecurity("JWT", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme {
                    Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = NSwag.OpenApiSecurityApiKeyLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}."
                });

                document.OperationProcessors.Add(new NSwag.Generation.Processors.Security.AspNetCoreOperationSecurityScopeProcessor("JWT"));

                document.DocumentName = "open-api-document";

                document.SchemaNameGenerator = new SwaggerCustomSchemaNameGenerator();
                document.TypeNameGenerator = new SwaggerCustomTypeNameGenerator();

            });

            //services.AddSwaggerDocument(config => {
            //    config.PostProcess = document => {
            //        document.Info.Version = "v1";
            //        document.Info.Title = "DNA.API";
            //        //document.Info.Description = "DNA API";
            //        document.Info.TermsOfService = "None";
            //        document.Info.Contact = new NSwag.OpenApiContact {
            //            Name = "DNA",
            //            Email = string.Empty,
            //            Url = "https://twitter.com/dna"
            //        };
            //        document.Info.License = new NSwag.OpenApiLicense {
            //            Name = "Use under LICX",
            //            Url = "https://example.com/license"
            //        };
            //    };
            //    config.SchemaType = NJsonSchema.SchemaType.OpenApi3;

            //});


            //services.AddSwaggerGen(config => {
            //    config.SwaggerDoc("v1", new OpenApiInfo {
            //        Title = "API",
            //        Version = "v1",
            //        Contact = new OpenApiContact {
            //            Name = "DNA.API",
            //            Url = new Uri("https://dna.com.tr/")
            //        },
            //        License = new OpenApiLicense {
            //            Name = "MIT",
            //        },
            //    });

            //    config.SchemaFilter<EnumSchemaFilter>();

            //    /*
            //    config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
            //        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer'[space] and then your token in the text input below. Example: 'Bearer {token}'",
            //        Name = "Authorization",
            //        In = ParameterLocation.Header,
            //        Type = SecuritySchemeType.ApiKey,
            //        BearerFormat = "JWT",
            //        Flows = new OpenApiOAuthFlows {
            //            Implicit = new OpenApiOAuthFlow {
            //                AuthorizationUrl = new Uri("/api/auth/login", UriKind.Relative),
            //                Scopes = new Dictionary<string, string> {
            //                    { "readAccess", "Access read operations" },
            //                    { "writeAccess", "Access write operations" }
            //                 }
            //            }
            //        }
            //    });

            //    config.OperationFilter<SecurityRequirementsOperationFilter>();

            //    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //    config.IncludeXmlComments(xmlPath);
            //    config.CustomSchemaIds(t => t.ToString());
            //    */
            //});

            return services;
        }

        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app) {
            app.UseOpenApi();
            app.UseSwaggerUi3();
            // app.UseReDoc(); // serve ReDoc UI
            // app.UseSwagger();
            // app.UseSwaggerUI();
            return app;
        }

        public static void ConfigureWritable(this IServiceCollection services, IConfiguration configuration) {
            services.AddSingleton<IWritableOptions>(provider => {
                var configuration = (IConfigurationRoot)provider.GetService<IConfiguration>();
                var menuService = (IMenuService)provider.GetService<IMenuService>();
                var serviceProvider = provider.GetService<IServiceProvider>();
                var logger = provider.GetService<ILogger<IWritableOptions>>();
                var writableOptions = new WritableOptions(configuration, serviceProvider, menuService);
                try {
                    writableOptions.GenerateConfigs();
                    logger.LogInformation("GenerateConfigs successfuly.");
                }
                catch (Exception ex) {
                    logger.LogError("GenerateConfigs failed. " + ex.Message);
                    logger.LogError("GenerateConfigs failed. " + ex.StackTrace);
                }
                return writableOptions;
            });
        }
    }
}
