
using AppCore.Interfaces;
using AppCore.Repositories;
using Infrastructure;
using Infrastructure.Memory;
using Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;
namespace WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddContactsEfModule(builder.Configuration);
        //builder.Services.AddSingleton<IPersonRepository, MemoryPersonRepository>();
        //builder.Services.AddSingleton<IContactUnitOfWork, MemoryContactUnitOfWork>();
        //builder.Services.AddSingleton<IPersonService, MemoryPersonService>();
        builder.Services.AddSingleton<JwtSettings>();
        builder.Services.AddJwt(new JwtSettings(builder.Configuration));
        builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();
        builder.Services.AddProblemDetails();

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();

            var seeders = scope.ServiceProvider
                .GetServices<IDataSeeder>()
                .OrderBy(s => s.Order);

            foreach (var seeder in seeders)
            {
                await seeder.SeedAsync();
            }
        }

        app.UseHttpsRedirection();
        app.UseExceptionHandler(); // ДО MapControllers
        app.UseAuthorization();
        app.MapControllers();
        
        

        app.MapGet("/api/customers", ([FromServices] ICustomerService service, HttpContext httpContext) =>
            {
                return service.GetCustomers();
            })
            .WithName("GetCustomers");

        app.Run();
    }
}
