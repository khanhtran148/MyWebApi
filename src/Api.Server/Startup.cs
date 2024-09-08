using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MyWebApi.Api.Server.Configuration;
using MyWebApi.Api.Server.Configuration.Exception;
using MyWebApi.Api.Server.Formatters;
using MyWebApi.Application;
using MyWebApi.Domain.Configurations;
using MyWebApi.Infrastructure;
using Newtonsoft.Json.Converters;

namespace MyWebApi.Api.Server
{
    public class Startup
    {
        public Startup(IConfiguration config, IWebHostEnvironment env)
        {
            Env = env;
            Configuration = config;
        }

        public IWebHostEnvironment Env { get; }
        public IConfiguration Configuration { get; }

        private string LocalAppDataPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private string HealthCheckDBName { get; } = "MyApi-api-healthcheck.db";
        public string HealthCheckDBFullPath => Path.Combine(LocalAppDataPath, HealthCheckDBName);

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowDefault",
                    builder => builder
                        .WithOrigins("*")
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .WithExposedHeaders("Content-Disposition")
                        .WithExposedHeaders("Digest")
                );
            });
            services.AddHttpContextAccessor();

            services
                .AddControllers(o =>
                {
                    // AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                    //     .RequireAuthenticatedUser()
                    //     .Build();
                    //o.Filters.Add(new AuthorizeFilter(policy));
                    o.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
                    o.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
                    o.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
                    o.OutputFormatters.Add(new TextHtmlOutputFormatter());
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            services.AddProblemDetails(options =>
                options.CustomizeProblemDetails = ctx =>
                {
                    ctx.ProblemDetails.Extensions.Add("trace-id", ctx.HttpContext.TraceIdentifier);
                    ctx.ProblemDetails.Extensions.Add("instance", $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}");
                });
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddOpenApiConfig();

            // HealthChecks
            services.AddHealthChecks()
                //.AddNpgSql(Configuration.GetConnectionString("MyWebPostgres") ?? throw new InvalidOperationException(), name: "MyWebPostgres", tags: new[] { "db", "postgres" });
                // .AddApiDependencyHealthCheck(Configuration)
                .AddSqlServer(Configuration.GetConnectionString("MyWeb") ?? string.Empty, name: "MyWebDb", tags: new[] { "sql" });

            // Options
            services.AddOptions();
            services.Configure<MessageBrokerOptions>(Configuration.GetSection(MessageBrokerOptions.BindLocator));

            // Logging
            services.AddLogging();

            // Add HealthCheckUI
            if (!Env.IsDevelopment())
            {
                services
                    .AddHealthChecksUI(setupSettings: setup =>
                    {
                        setup.SetEvaluationTimeInSeconds(15);
                        setup.MaximumHistoryEntriesPerEndpoint(25);
                    })
                    .AddSqliteStorage($"Data Source={HealthCheckDBFullPath}");
            }
            else
            {
                services
                    .AddHealthChecksUI(setupSettings: setup =>
                    {
                        setup.SetEvaluationTimeInSeconds(15);
                        setup.MaximumHistoryEntriesPerEndpoint(25);
                    })
                    .AddInMemoryStorage();
            }

            // register dependencies
            services.RegisterDatabase(Configuration);
            services.RegisterApplicationDependencies(Configuration);
            services.RegisterInfrastructureDependencies(Configuration);
            services.RegisterActiveMqPublisher(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHttpContextAccessor accessor)
        {
            if (Env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //This method should be called before other middleware pipelines.
            app.UseCors("AllowDefault");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseHealthChecks("/health", new HealthCheckOptions { Predicate = _ => true, ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
            app.UseHealthChecksUI(x => x.UIPath = "/health-ui");
            app.UseStatusCodePages();
            app.UseExceptionHandler();
            app.UseEndpoints(endpoints =>
            {
                var descriptions = endpoints.DescribeApiVersions();
                app.UseOpenApiConfig(descriptions);
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health",
                    new HealthCheckOptions
                    {
                        Predicate = _ => true,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    });
                //HealthCheck UI is facing with open issues: https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/issues/2130
                endpoints.MapHealthChecksUI(x => x.UIPath = "/health-ui");
            });
        }

        private static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
        {
            var builder = new ServiceCollection()
                .AddMvc()
                .AddNewtonsoftJson()
                .Services.BuildServiceProvider();

            return builder
                .GetRequiredService<IOptions<MvcOptions>>()
                .Value
                .InputFormatters
                .OfType<NewtonsoftJsonPatchInputFormatter>()
                .First();
        }
    }
}
