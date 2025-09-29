using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace PremiersoftChallenge.Security
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddSecurityBaseAuthentication(this IServiceCollection services, string secret, string? issuer, string? audience)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            return services;
        }

        public static IServiceCollection AddSecurityBaseAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization();

            return services;
        }

        public static IServiceCollection AddAuthenticatedSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(o =>
            {
                o.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));
                o.OperationFilter<AddHeaderParameter>();

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Autenticação JWT",
                    Description = "Insira seu token JWT neste campo",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT"
                };

                o.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        []
                    }
                };

                o.AddSecurityRequirement(securityRequirement);
            });

            return services;
        }
    }
}
