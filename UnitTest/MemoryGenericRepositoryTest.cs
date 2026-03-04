using Xunit;
using AppCore.Models;
using AppCore.Repositories;
using Infrastructure.Memory;

public class MemoryGenericRepositoryTest
{
    private readonly IGenericRepositoryAsync<Person> _repo;

    public MemoryGenericRepositoryTest()
    {
        _repo = new MemoryGenericRepository<Person>();
    }

    [Fact]
    public async Task AddPersonTestAsync()
    {
        // Arrange
        var expected = new Person
        {
            FirstName = "Adam",
            LastName = "Nowak",
            Email = "adam@test.pl",
            Phone = "123456789"
        };

        // Act
        await _repo.AddAsync(expected);

        // Assert
        var actual = await _repo.FindByIdAsync(expected.Id);

        Assert.NotNull(actual);
        Assert.Equal(expected.Id, actual!.Id);
        Assert.Equal(expected.FirstName, actual.FirstName);
        Assert.Equal(expected.LastName, actual.LastName);
    }
	
}