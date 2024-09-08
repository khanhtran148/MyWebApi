using System;
using System.Collections.Generic;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyWebApi.Api.Server.Configuration;

public static class OpenApiExtension
{
    /// <summary>
    /// Add OpenAPI configuration
    /// </summary>
    /// <param name="services"></param>
    public static void AddOpenApiConfig(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddApiVersioning((Action<ApiVersioningOptions>)(c =>
            {
                c.AssumeDefaultVersionWhenUnspecified = true;
                c.DefaultApiVersion = new ApiVersion(1, 0);
                c.ReportApiVersions = true;
                c.ApiVersionReader = new MediaTypeApiVersionReader();
                c.AssumeDefaultVersionWhenUnspecified = true;
                c.ApiVersionSelector = new CurrentImplementationApiVersionSelector(c);
            }))
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        services.AddSwaggerGen(
            options =>
            {
                // add a custom operation filter which sets default values
                //options.OperationFilter<SwaggerDefaultValues>();
            });

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
    }

    /// <summary>
    /// Use OpenAPI configuration
    /// </summary>
    /// <param name="app"></param>
    /// <param name="apiVersionDescriptions"></param>
    public static void UseOpenApiConfig(this IApplicationBuilder app, IReadOnlyList<ApiVersionDescription> apiVersionDescriptions)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            // build a swagger endpoint for each discovered API version
            foreach (var description in apiVersionDescriptions)
            {
                string url = $"/swagger/{description.GroupName}/swagger.json";
                string name = description.GroupName.ToUpperInvariant();
                c.SwaggerEndpoint(url, name);
            }

            c.DisplayRequestDuration();
            c.DisplayOperationId();
        });
    }
}
