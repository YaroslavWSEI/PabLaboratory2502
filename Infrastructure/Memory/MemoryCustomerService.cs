using AppCore.Interfaces;
using AppCore.Models;
namespace Infrastructure.Memory;

public class MemoryCustomerService : ICustomerService
{
    public IEnumerable<Customer> GetCustomers()
    {
        return
        [
            new Customer()
            {
                id = 1,
                FirstName = "Jan",
                LastName = "Kow",
                Email = "yyaeadasasdaas@wsei.edu.pl",
                Phone = "123456789",
                AddressId = 1,
            },
            

        ];
    }

    public Task<IEnumerable<Customer>> GetCustomersAsync()
    {
        throw new NotImplementedException();
    }
}