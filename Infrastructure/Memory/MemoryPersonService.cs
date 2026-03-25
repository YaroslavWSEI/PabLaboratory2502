using AppCore.Dto;
using AppCore.Interfaces;
using AppCore.Models;
using AppCore.ValueObjects;
using AppCore.Exceptions;
namespace Infrastructure.Memory;

public class MemoryPersonService : IPersonService
{
    private readonly IContactUnitOfWork _unitOfWork;

    public MemoryPersonService(IContactUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int size)
    {
        var people = await _unitOfWork.Persons.FindPagedAsync(page, size);

        var items = people.Items
            .Select(p => PersonDto.FromEntity(p))
            .ToList();

        return new PagedResult<PersonDto>(
            items,
            people.TotalCount,
            people.Page,
            people.PageSize
        );
    }

    public async Task<object?> AddPerson(CreatePersonDto personDto)
    {
        var entity = new Person
        {
            Id = Guid.NewGuid(),
            FirstName = personDto.FirstName,
            LastName = personDto.LastName,
            Email = personDto.Email,
            Phone = personDto.Phone,
            BirthDate = personDto.BirthDate,
            Gender = personDto.Gender,
            EmployerId = personDto.EmployerId,
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

        entity = await _unitOfWork.Persons.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity;
    }

    public async Task<object?> UpdatePerson(UpdatePersonDto personDto)
    {
        var entity = await _unitOfWork.Persons.FindByIdAsync(personDto.Id);

        if (entity == null)
            return null;

        if (personDto.FirstName != null)
            entity.FirstName = personDto.FirstName;

        if (personDto.LastName != null)
            entity.LastName = personDto.LastName;

        if (personDto.Email != null)
            entity.Email = personDto.Email;

        if (personDto.Phone != null)
            entity.Phone = personDto.Phone;

        if (personDto.BirthDate.HasValue)
            entity.BirthDate = personDto.BirthDate;

        if (personDto.Gender.HasValue)
            entity.Gender = personDto.Gender.Value;

        if (personDto.EmployerId.HasValue)
            entity.EmployerId = personDto.EmployerId;

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

        entity = await _unitOfWork.Persons.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return entity;
    }

    public async Task<object?> GetById(Guid id)
    {
        var entity = await _unitOfWork.Persons.FindByIdAsync(id);

        if (entity == null)
            return null;

        return PersonDto.FromEntity(entity);
    }
    public async Task<Note> AddNoteToPerson(Guid personId, CreateNoteDto noteDto)
    {
        var person = await _unitOfWork.Persons.FindByIdAsync(personId);
    
        if (person == null)
            throw new ContactNotFoundException($"Person with id={personId} not found!");
    
        if (person.Notes == null)
            person.Notes = new List<Note>();
    
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
    
    public async Task<PersonDto> GetPerson(Guid personId)
    {
        var person = await _unitOfWork.Persons.FindByIdAsync(personId);
    
        if (person == null)
            throw new ContactNotFoundException($"Person with id={personId} not found!");
    
        return PersonDto.FromEntity(person);
    }
}