using System.Reflection;
using System.Text;
using eLib.Middleware;
using eLib.Providers;
using eLib.Security;
using eLib.Security.Handlers;
using eLib.Security.Requirements;
using eLib.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace eLib
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IBookService, BookService>();
        }

        public static void AddValidation(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddFluentValidationAutoValidation();
        }

        public static void AddMiddlewares(this IServiceCollection services)
        {
            services.AddTransient<ErrorHandlingMiddleware>();
        }

        public static void AddSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAccessTokenCreator, AccessTokenCreator>();

            var secretKey = configuration.GetValue<string>("AuthSettings:SecretKey");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(5)
            };

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = "Bearer";
                option.DefaultScheme = "Bearer";
                option.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = tokenValidationParameters;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.Requirements.Add(new AdminOnlyRequirement()));
                options.AddPolicy("AdminOrCurrentUser", policy => policy.Requirements.Add(new AdminOrCurrentUserRequirement()));

            });

            services.AddSingleton<IAuthorizationHandler, AdminOnlyHandler>();
            services.AddSingleton<IAuthorizationHandler, AdminOrCurrentUserHandler>();
            services.AddSingleton(tokenValidationParameters);
            services.AddHttpContextAccessor();
            services.AddScoped<IUserInfoProvider, UserInfoProvider>();
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "eLib API", Version = "v1" });

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Bearer token **_only_**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securityScheme, new string[] { } }
                });
            });
        }
    }
}