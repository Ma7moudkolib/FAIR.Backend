using System.Text;
using FAIR.Application.Mapping;
using FAIR.Application.Services.Implementations;
using FAIR.Application.Services.Interfaces;
using FAIR.Application.Services.Interfaces.Logging;
using FAIR.Application.Services.Interfaces.Managers;
using FAIR.Application.Services.Managers;
using FAIR.Application.Validations;
using FAIR.Application.Validations.Authentication;
using FAIR.Domain.Entities.Identity;
using FAIR.Domain.Interfaces;

using FAIR.Infrastructure.Context;
using FAIR.Infrastructure.Options;
using FAIR.Infrastructure.Repository;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FAIR.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services) => services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            const string connectionString = "FAIRconnection";
            services.AddDbContext<dbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString(connectionString), sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(dbContext).Assembly.FullName);
                        sqlOptions.EnableRetryOnFailure();
                    }),
                ServiceLifetime.Scoped);
        }

        public static void ConfigureRepositoryManager(this IServiceCollection services) =>
            services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureServiceManager(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();
            services.AddAutoMapper(typeof(MappingConfig));

            services.AddScoped<IValidationService, ValidationService>();

            services.AddScoped<IServiceManager, ServiceManager>();

            services.AddScoped(typeof(IAppLogger<>), typeof(SerilogLoggerAdapter<>));

            services.AddScoped<IAiVideoService, AiVideoService>();
            services.AddSingleton<IConnectionMappingService, ConnectionMappingService>();


            services.Configure<AiVideoIntegrationOptions>(configuration.GetSection("AiVideoIntegration"));

            services.AddHttpClient("AiVideoIntegration");

        }

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDefaultIdentity<AppUser>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredUniqueChars = 1;
            }).AddRoles<IdentityRole>().AddEntityFrameworkStores<dbContext>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
