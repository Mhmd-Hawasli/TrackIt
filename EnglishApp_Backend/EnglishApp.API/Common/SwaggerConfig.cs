using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EnglishApp.API.Common
{
    public static class SwaggerConfig
    {
        public class FixedDeviceIdHeaderFilter : IOperationFilter
        {
            private const string FixedDeviceId = "Device_01";

            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                if (operation.Parameters == null)
                    operation.Parameters = new List<OpenApiParameter>();

                if (!operation.Parameters.Any(p => p.Name == "Device-Id"))
                {
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = "Device-Id",
                        In = ParameterLocation.Header,
                        Required = true,
                        Description = "Fixed Device Id (hidden from user)",
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Default = new Microsoft.OpenApi.Any.OpenApiString(FixedDeviceId)
                        },
                        Style = ParameterStyle.Simple,
                        Explode = false
                    });
                }
            }
        }
        public static void AddSwaggerWithAuth(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                // Swagger doc
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                // JWT Bearer
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[] {}
                    }
                });

                c.OperationFilter<FixedDeviceIdHeaderFilter>();
            });
        }
    }


}
