using AppCore.Interfaces;
using AppCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Memory
{
    public class MemoryCompanyRepository : ICompanyRepository
    {
        private readonly List<Company> _companies = new();

        public Task<Company?> FindByIdAsync(Guid id)
        {
            var company = _companies.FirstOrDefault(c => c.Id == id);
            return Task.FromResult(company);
        }

        public Task<Company> AddAsync(Company company)
        {
            company.Id = Guid.NewGuid();
            _companies.Add(company);
            return Task.FromResult(company);
        }
    }
}