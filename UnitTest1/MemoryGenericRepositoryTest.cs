using Xunit;
using AppCore.Models;
using AppCore.Repositories;
using Infrastructure.Memory;

public class MemoryGenericRepositoryTests
{
    private readonly IGenericRepositoryAsync<Person> _repo;

    public MemoryGenericRepositoryTests()
    {
        _repo = new MemoryGenericRepository<Person>();
    }
    private Person CreatePerson(string firstName = "Adam")
        => new Person
        {
            FirstName = firstName,
            LastName = "Test",
            Email = $"{firstName.ToLower()}@test.pl",
            Phone = "123456789"
        };
    [Fact]
    public async Task Add_Should_Add_Entity()
    {
        var person = CreatePerson();

        await _repo.AddAsync(person);

        var result = await _repo.FindByIdAsync(person.Id);

        Assert.NotNull(result);
        Assert.Equal(person.Id, result!.Id);
    }
    [Fact]
    public async Task Add_Should_Throw_When_Entity_Already_Exists()
    {
        var person = CreatePerson();

        await _repo.AddAsync(person);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _repo.AddAsync(person));
    }
    [Fact]
    public async Task FindAll_Should_Return_All_Entities()
    {
        await _repo.AddAsync(CreatePerson("A"));
        await _repo.AddAsync(CreatePerson("B"));

        var all = await _repo.FindAllAsync();

        Assert.Equal(2, all.Count());
    }
    [Fact]
    public async Task Update_Should_Modify_Existing_Entity()
    {
        var person = CreatePerson();
        await _repo.AddAsync(person);

        person.FirstName = "Updated";

        await _repo.UpdateAsync(person);

        var updated = await _repo.FindByIdAsync(person.Id);

        Assert.Equal("Updated", updated!.FirstName);
    }

    [Fact]
    public async Task Update_Should_Throw_When_Entity_Not_Exists()
    {
        var person = CreatePerson();

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _repo.UpdateAsync(person));
    }
    [Fact]
    public async Task Remove_Should_Delete_Entity()
    {
        var person = CreatePerson();
        await _repo.AddAsync(person);

        await _repo.RemoveByIdAsync(person.Id);

        var result = await _repo.FindByIdAsync(person.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task Remove_Should_Throw_When_Not_Exists()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _repo.RemoveByIdAsync(Guid.NewGuid()));
    }
    [Fact]
    public async Task FindPaged_Should_Return_Correct_Page()
    {
        for (int i = 0; i < 25; i++)
        {
            await _repo.AddAsync(CreatePerson($"Person{i}"));
        }

        var page = await _repo.FindPagedAsync(2, 10);

        Assert.Equal(10, page.Items.Count);
        Assert.Equal(25, page.TotalCount);
        Assert.Equal(2, page.Page);
        Assert.Equal(10, page.PageSize);
        Assert.True(page.HasPrevious);
        Assert.True(page.HasNext);
    }
}