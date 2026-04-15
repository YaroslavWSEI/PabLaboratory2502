using AppCore.Interfaces;
using Infrastructure.Context;
using AppCore.Models;
using AppCore.Enums;

namespace Infrastructure.Seeders;

public class ContactSeeder : IDataSeeder
{
    public int Order => 2;

    private readonly ContactsDbContext _context;

    public ContactSeeder(ContactsDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        if (_context.People.Any())
            return;

        var people = new List<Person>
        {
            new Person
            {
                Id = Guid.NewGuid(),
                FirstName = "Adam",
                LastName = "Nowak",
                Email = "adam@test.pl",
                Phone = "123456789",
                Gender = Gender.Male,
                Status = ContactStatus.Active,
                CreatedAt = DateTime.UtcNow
            },
            new Person
            {
                Id = Guid.NewGuid(),
                FirstName = "Anna",
                LastName = "Kowalska",
                Email = "anna@test.pl",
                Phone = "987654321",
                Gender = Gender.Female,
                Status = ContactStatus.Active,
                CreatedAt = DateTime.UtcNow
            }
        };

        await _context.People.AddRangeAsync(people);
        await _context.SaveChangesAsync();
    }
}