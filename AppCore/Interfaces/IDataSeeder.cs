namespace AppCore.Interfaces;

public interface IDataSeeder
{
    int Order { get; }
    Task SeedAsync();
}