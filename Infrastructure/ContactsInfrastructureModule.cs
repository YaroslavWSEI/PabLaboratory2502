using AppCore.Interfaces;
using AppCore.Repositories;
using AppCore.Services;
using Infrastructure.Context;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using AppCore.Authorization;
using AppCore.Enums;
using Infrastructure.Seeders;

namespace Infrastructure;

public static class ContactsInfrastructureModule
{
    public static IServiceCollection AddContactsEfModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ContactsDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("CrmDb")));

        // Identity Configuration
        services.AddIdentity<CrmUser, CrmRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ContactsDbContext>()
            .AddDefaultTokenProviders();

        // Repositories
        services.AddScoped<IPersonRepository, EfPersonRepository>();
        services.AddScoped<ICompanyRepository, EfCompanyRepository>();
        services.AddScoped<IOrganizationRepository, EfOrganizationRepository>();

        // Unit of Work
        services.AddScoped<IContactUnitOfWork, EfContactsUnitOfWork>();

        // Services
        services.AddScoped<IPersonService, PersonService>();

        // AuthService
        services.AddScoped<IAuthService, AuthService>();

        // JWT settings
        var jwtSection = configuration.GetSection("Jwt");
        var jwtOptions = jwtSection.Get<JwtSettings>()
            ?? throw new InvalidOperationException("Jwt settings missing");
        services.AddScoped<IDataSeeder, IdentityDbSeeder>();
        services.AddScoped<IDataSeeder, ContactSeeder>();
        services.AddSingleton(jwtOptions);

        services.AddJwt(jwtOptions);

        return services;
    }

    // ─────────────────────────────────────────────
    // JWT CONFIG
    // ─────────────────────────────────────────────
    public static IServiceCollection AddJwt(this IServiceCollection services, JwtSettings jwtOptions)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = jwtOptions.GetSymmetricKey(),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(CrmPolicies.AdminOnly.ToString(), policy =>
                policy.RequireRole(UserRole.Administrator.ToString()));

            options.AddPolicy(CrmPolicies.SalesAccess.ToString(), policy =>
                policy.RequireRole(
                    UserRole.Administrator.ToString(),
                    UserRole.SalesManager.ToString(),
                    UserRole.Salesperson.ToString()));

            options.AddPolicy(CrmPolicies.SalesManagerAccess.ToString(), policy =>
                policy.RequireRole(
                    UserRole.Administrator.ToString(),
                    UserRole.SalesManager.ToString()));

            options.AddPolicy(CrmPolicies.SupportAccess.ToString(), policy =>
                policy.RequireRole(
                    UserRole.Administrator.ToString(),
                    UserRole.SupportAgent.ToString()));

            options.AddPolicy(CrmPolicies.ReadOnlyAccess.ToString(), policy =>
                policy.RequireAuthenticatedUser());

            options.AddPolicy(CrmPolicies.ActiveUser.ToString(), policy =>
                policy
                    .RequireAuthenticatedUser()
                    .RequireClaim("status", SystemUserStatus.Active.ToString()));

            options.AddPolicy(CrmPolicies.SalesDepartment.ToString(), policy =>
                policy.RequireClaim("department", "Sales"));

            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }
}