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
        // The UnitOfWork now calls the EF Repository under the hood
        var pagedEntities = await _unitOfWork.Persons.FindPagedAsync(page, size);

        var dtos = pagedEntities.Items
            .Select(PersonDto.FromEntity)
            .ToList();

        return new PagedResult<PersonDto>(
            dtos,
            pagedEntities.TotalCount,
            pagedEntities.Page,
            pagedEntities.PageSize
        );
    }

    public async Task<object?> AddPerson(CreatePersonDto personDto)
    {
        var entity = new Person
        {
            // Note: EF Core/Sqlite will handle Guid generation if configured, 
            // but manual assignment is fine for specific business logic.
            Id = Guid.NewGuid(), 
            FirstName = personDto.FirstName,
            LastName = personDto.LastName,
            Email = personDto.Email,
            Phone = personDto.Phone,
            BirthDate = personDto.BirthDate,
            Gender = personDto.Gender,
            EmployerId = personDto.EmployerId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Status = ContactStatus.Active, // Default status
            Address = personDto.Address is not null
                ? new Address
                {
                    Street = personDto.Address.Street,
                    City = personDto.Address.City,
                    ZipCode = personDto.Address.PostalCode,
                    Country = Enum.Parse<Country>(personDto.Address.Country)
                }
                : null
        };

        await _unitOfWork.Persons.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return PersonDto.FromEntity(entity);
    }

    public async Task<object?> UpdatePerson(UpdatePersonDto personDto)
    {
        var entity = await _unitOfWork.Persons.FindByIdAsync(personDto.Id);

        if (entity == null)
            return null;

        // Update properties
        entity.FirstName = personDto.FirstName ?? entity.FirstName;
        entity.LastName = personDto.LastName ?? entity.LastName;
        entity.Email = personDto.Email ?? entity.Email;
        entity.Phone = personDto.Phone ?? entity.Phone;
        
        if (personDto.BirthDate.HasValue) entity.BirthDate = personDto.BirthDate;
        if (personDto.Gender.HasValue) entity.Gender = personDto.Gender.Value;
        if (personDto.EmployerId.HasValue) entity.EmployerId = personDto.EmployerId;

        if (personDto.Address != null)
        {
            entity.Address = new Address
            {
                Street = personDto.Address.Street,
                City = personDto.Address.City,
                ZipCode = personDto.Address.PostalCode,
                Country = Enum.Parse<Country>(personDto.Address.Country)
            };
        }

        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Persons.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return PersonDto.FromEntity(entity);
    }

    public async Task<object?> GetById(Guid id)
    {
        var entity = await _unitOfWork.Persons.FindByIdAsync(id);
        return entity == null ? null : PersonDto.FromEntity(entity);
    }

    public async Task<Note> AddNoteToPerson(Guid personId, CreateNoteDto noteDto)
    {
        var person = await _unitOfWork.Persons.FindByIdAsync(personId);
    
        if (person == null)
            throw new ContactNotFoundException($"Person with id={personId} not found!");
    
        var note = new Note
        {
            Id = Guid.NewGuid(),
            Content = noteDto.Content,
            CreatedAt = DateTime.UtcNow
        };
    
        // In EF Core, if Notes is a collection, ensure it's loaded 
        // or just add to the collection if using lazy loading/include
        person.Notes ??= new List<Note>();
        person.Notes.Add(note);
    
        await _unitOfWork.Persons.UpdateAsync(person);
        await _unitOfWork.SaveChangesAsync();
    
        return note;
    }

    public async Task<PersonDto> GetPerson(Guid personId)
    {
        var person = await _unitOfWork.Persons.FindByIdAsync(personId);
    
        if (person == null)
            throw new ContactNotFoundException($"Person with id={personId} not found!");
    
        return PersonDto.FromEntity(person);
    }
}