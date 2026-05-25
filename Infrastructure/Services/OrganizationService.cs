using AppCore.Dto;
using AppCore.Enums;
using AppCore.Interfaces;
using AppCore.Models;
using AppCore.ValueObjects;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class OrganizationService : IOrganizationService
{
    private readonly ContactsDbContext _db;

    public OrganizationService(ContactsDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<OrganizationDto>> GetAllPaged(int page, int size)
    {
        var query = _db.Organizations
            .Include(o => o.Members)
            .AsNoTracking();

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return new PagedResult<OrganizationDto>(
            items.Select(OrganizationDto.FromEntity).ToList(),
            total, page, size);
    }

    public async Task<OrganizationDetailDto?> GetById(Guid id)
    {
        var org = await _db.Organizations
            .Include(o => o.Members)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);

        return org is null ? null : OrganizationDetailDto.FromEntity(org);
    }

    public async Task<OrganizationDto> AddOrganization(CreateOrganizationDto dto)
    {
        var org = new Organization
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            RegistrationNumber = dto.RegistrationNumber,
            Type = dto.Type,
            Status = ContactStatus.Active,
            CreatedAt = DateTime.UtcNow,
            Address = dto.Address is null ? null : new Address
            {
                Street = dto.Address.Street,
                City = dto.Address.City,
                ZipCode = dto.Address.PostalCode,
                Country = Enum.TryParse<Country>(dto.Address.Country, true, out var c)
                    ? c : Country.Unknown
            }
        };

        _db.Organizations.Add(org);
        await _db.SaveChangesAsync();
        return OrganizationDto.FromEntity(org);
    }

    public async Task<OrganizationDto?> UpdateOrganization(UpdateOrganizationDto dto)
    {
        var org = await _db.Organizations.FindAsync(dto.Id);
        if (org is null) return null;

        if (dto.Name is not null)               org.Name = dto.Name;
        if (dto.Email is not null)              org.Email = dto.Email;
        if (dto.Phone is not null)              org.Phone = dto.Phone;
        if (dto.RegistrationNumber is not null) org.RegistrationNumber = dto.RegistrationNumber;
        if (dto.Type is not null)               org.Type = dto.Type.Value;
        if (dto.Status is not null)             org.Status = dto.Status.Value;
        org.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return OrganizationDto.FromEntity(org);
    }

    public async Task DeleteOrganization(Guid id)
    {
        var org = await _db.Organizations.FindAsync(id);
        if (org is null) return;
        _db.Organizations.Remove(org);
        await _db.SaveChangesAsync();
    }

    public async Task<List<PersonDto>> GetMembers(Guid organizationId)
    {
        var members = await _db.People
            .Where(p => p.OrganizationId == organizationId)
            .Include(p => p.Notes)
            .AsNoTracking()
            .ToListAsync();

        return members.Select(PersonDto.FromEntity).ToList();
    }

    public async Task<bool> AddMember(Guid organizationId, Guid personId)
    {
        var org = await _db.Organizations.FindAsync(organizationId);
        var person = await _db.People.FindAsync(personId);
        if (org is null || person is null) return false;

        person.OrganizationId = organizationId;
        person.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveMember(Guid organizationId, Guid personId)
    {
        var person = await _db.People
            .FirstOrDefaultAsync(p => p.Id == personId && p.OrganizationId == organizationId);
        if (person is null) return false;

        person.OrganizationId = null;
        person.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<OrganizationDto>> Search(string? name, string? emailDomain)
    {
        var query = _db.Organizations
            .Include(o => o.Members)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(o => o.Name.Contains(name));

        if (!string.IsNullOrWhiteSpace(emailDomain))
            query = query.Where(o => o.Email.EndsWith("@" + emailDomain));

        var results = await query.ToListAsync();
        return results.Select(OrganizationDto.FromEntity).ToList();
    }
}