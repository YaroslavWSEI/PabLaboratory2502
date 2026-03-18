using AppCore.Models;
using System;
using System.Threading.Tasks;

namespace AppCore.Interfaces
{
    public interface ICompanyRepository
    {
        /// <summary>
        /// Znajduje firmę po Id. Zwraca null, jeśli firma nie istnieje.
        /// </summary>
        Task<Company?> FindByIdAsync(Guid id);

        /// <summary>
        /// Dodaje nową firmę (opcjonalne do testów)
        /// </summary>
        Task<Company> AddAsync(Company company);
    }
}