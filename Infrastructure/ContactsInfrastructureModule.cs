using AppCore.Authorization;
using AppCore.Enums;
using AppCore.Interfaces;
using AppCore.Repositories;
using AppCore.Services;
using Infrastructure.Context;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Infrastructure.Seeders;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class ContactsInfrastructureModule
{
    public static IServiceCollection AddContactsEfModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ContactsDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("CrmDb")));

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

        services.AddScoped<IContactUnitOfWork, EfContactsUnitOfWork>();
        services.AddScoped<IPersonService, PersonService>();

        services.AddScoped<IAuthService, AuthService>();
        
        var jwtSettings = new JwtSettings(configuration);

        services.AddSingleton(jwtSettings);

        services.AddJwt(jwtSettings);

        // Seeders
        services.AddScoped<IDataSeeder, IdentityDbSeeder>();
        services.AddScoped<IDataSeeder, ContactSeeder>();

        return services;
    }

    // =========================
    // JWT CONFIG
    // =========================
   public static IServiceCollection AddJwt(this IServiceCollection services, JwtSettings jwtSettings)
{
    services
        .AddAuthentication(options =>
        {
            // WAŻNE: jeden default scheme
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

                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = jwtSettings.GetSymmetricKey(),

                ClockSkew = TimeSpan.Zero
            };
        });

    services.AddAuthorization(options =>
    {
        options.AddPolicy(CrmPolicies.AdminOnly.ToString(),
            p => p.RequireRole(UserRole.Administrator.ToString()));

        options.AddPolicy(CrmPolicies.SalesAccess.ToString(),
            p => p.RequireRole(
                UserRole.Administrator.ToString(),
                UserRole.SalesManager.ToString(),
                UserRole.Salesperson.ToString()));

        options.AddPolicy(CrmPolicies.SalesManagerAccess.ToString(),
            p => p.RequireRole(
                UserRole.Administrator.ToString(),
                UserRole.SalesManager.ToString()));

        options.AddPolicy(CrmPolicies.SupportAccess.ToString(),
            p => p.RequireRole(
                UserRole.Administrator.ToString(),
                UserRole.SupportAgent.ToString()));

        options.AddPolicy(CrmPolicies.ReadOnlyAccess.ToString(),
            p => p.RequireAuthenticatedUser());

        options.AddPolicy(CrmPolicies.ActiveUser.ToString(),
            p => p.RequireAuthenticatedUser()
                  .RequireClaim("status", SystemUserStatus.Active.ToString()));

        options.AddPolicy(CrmPolicies.SalesDepartment.ToString(),
            p => p.RequireClaim("department", "Sales"));

        options.DefaultPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

        options.FallbackPolicy = options.DefaultPolicy;
    });

    return services;
}
}