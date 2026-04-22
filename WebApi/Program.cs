using AppCore.Interfaces;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =========================
// MODULES
// =========================
builder.Services.AddContactsEfModule(builder.Configuration);

// =========================
// CONTROLLERS
// =========================
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// =========================
// JWT AUTH (TO BYŁO BRAKUJĄCE)
// =========================

// =========================
// ERRORS
// =========================

builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// =========================
// SEEDERS
// =========================
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var seeders = scope.ServiceProvider.GetServices<IDataSeeder>();

    foreach (var seeder in seeders.OrderBy(x => x.Order))
        await seeder.SeedAsync();
}
// =========================
// PIPELINE
// =========================
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();