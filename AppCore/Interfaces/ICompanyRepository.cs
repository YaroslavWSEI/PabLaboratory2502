using AppCore.Models;
using System;
using System.Threading.Tasks;

namespace AppCore.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company?> FindByIdAsync(Guid id);
        Task<Company> AddAsync(Company company);
    }
}