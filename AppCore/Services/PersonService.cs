using AppCore.Dto;
using AppCore.Enums;
using AppCore.Interfaces;
using AppCore.Models;
using AppCore.ValueObjects;
using AppCore.Exceptions;

namespace AppCore.Services;

public class PersonService : IPersonService
{
    private readonly IContactUnitOfWork _unitOfWork;

    public PersonService(IContactUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int size)
    {
        var paged = await _unitOfWork.Persons.FindPagedAsync(page, size);

        return new PagedResult<PersonDto>(
            paged.Items.Select(PersonDto.FromEntity).ToList(),
            paged.TotalCount,
            paged.Page,
            paged.PageSize
        );
    }

    public async Task<PersonDto> AddPerson(CreatePersonDto dto)
    {
        var entity = new Person
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Pesel = !string.IsNullOrEmpty(dto.Pesel) ? new Pesel(dto.Pesel) : null,
            Phone = dto.Phone,
            BirthDate = dto.BirthDate,
            Gender = dto.Gender,
            EmployerId = dto.EmployerId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Status = ContactStatus.Active,
            Notes = new List<Note>()
        };

        if (dto.Address != null)
        {
            entity.Address = new Address
            {
                Street = dto.Address.Street,
                City = dto.Address.City,
                ZipCode = dto.Address.PostalCode,
                Country = Enum.TryParse<Country>(dto.Address.Country, true, out var c)
                    ? c
                    : Country.Unknown
            };
        }

        await _unitOfWork.Persons.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return PersonDto.FromEntity(entity);
    }

    public async Task<PersonDto?> UpdatePerson(UpdatePersonDto dto)
    {
        var entity = await _unitOfWork.Persons.FindByIdAsync(dto.Id);
        if (entity == null) return null;

        entity.FirstName = dto.FirstName ?? entity.FirstName;
        entity.LastName = dto.LastName ?? entity.LastName;
        entity.Email = dto.Email ?? entity.Email;
        entity.Phone = dto.Phone ?? entity.Phone;

        if (dto.BirthDate.HasValue)
            entity.BirthDate = dto.BirthDate;

        if (dto.Gender.HasValue)
            entity.Gender = dto.Gender.Value;

        if (dto.EmployerId.HasValue)
            entity.EmployerId = dto.EmployerId;

        if (dto.Address != null)
        {
            entity.Address = new Address
            {
                Street = dto.Address.Street,
                City = dto.Address.City,
                ZipCode = dto.Address.PostalCode,
                Country = Enum.TryParse<Country>(dto.Address.Country, true, out var c)
                    ? c
                    : Country.Unknown
            };
        }

        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Persons.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return PersonDto.FromEntity(entity);
    }

    public async Task<PersonDto?> GetById(Guid id)
    {
        var entity = await _unitOfWork.Persons.FindByIdAsync(id);
        return entity == null ? null : PersonDto.FromEntity(entity);
    }
    
    public async Task<PersonDto> GetPerson(Guid personId)
    {
        var entity = await _unitOfWork.Persons.FindByIdAsync(personId);

        if (entity == null)
            throw new ContactNotFoundException($"Person {personId} not found");

        return PersonDto.FromEntity(entity);
    }

    public async Task<Note> AddNoteToPerson(Guid personId, CreateNoteDto noteDto)
    {
        var person = await _unitOfWork.Persons.FindByIdAsync(personId);

        if (person == null)
            throw new ContactNotFoundException("Not found");

        person.Notes ??= new List<Note>();

        var note = new Note
        {
            Id = Guid.NewGuid(),
            Content = noteDto.Content,
            CreatedAt = DateTime.UtcNow
        };

        person.Notes.Add(note);

        await _unitOfWork.Persons.UpdateAsync(person);
        await _unitOfWork.SaveChangesAsync();

        return note;
    }
    public async Task<List<PersonDto>> SearchPeople(string? emailDomain, Guid? organizationId)
    {
        // В Unit of Work обычно есть доступ к репозиториям. 
        // Используем FindAllAsync или создаем специальный метод в репозитории.
        // Для простоты выгрузим список и отфильтруем (или добавь фильтрацию в репозиторий)
        var allPersons = await _unitOfWork.Persons.FindAllAsync();
        var query = allPersons.AsQueryable();

        // 1. Фильтр по домену почты
        if (!string.IsNullOrEmpty(emailDomain))
        {
            query = query.Where(p => p.Email.EndsWith("@" + emailDomain, StringComparison.OrdinalIgnoreCase));
        }

        // 2. Фильтр по организации
        if (organizationId.HasValue && organizationId.Value != Guid.Empty)
        {
            query = query.Where(p => p.OrganizationId == organizationId);
        }

        // 3. Маппинг через твой существующий метод FromEntity
        return query.Select(PersonDto.FromEntity).ToList();
    }
    public async Task<bool> AssignToOrganization(Guid personId, Guid organizationId)
    {
        var person = await _unitOfWork.Persons.FindByIdAsync(personId);
        // Проверяем, существует ли организация (опционально, зависит от репозитория)
        // var org = await _unitOfWork.Organizations.FindByIdAsync(organizationId); 

        if (person == null) return false;

        person.OrganizationId = organizationId;
        person.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Persons.UpdateAsync(person);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task DeleteContact(Guid id)
    {
        var contact = await _unitOfWork.Persons.FindByIdAsync(id);
        if (contact != null)
        {
            await _unitOfWork.Persons.RemoveByIdAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}