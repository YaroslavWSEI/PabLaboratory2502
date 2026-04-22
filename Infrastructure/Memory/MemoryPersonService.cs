using AppCore.Dto;
using AppCore.Enums;
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

        return new PagedResult<PersonDto>(
            people.Items.Select(PersonDto.FromEntity).ToList(),
            people.TotalCount,
            people.Page,
            people.PageSize
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

        if (dto.FirstName != null) entity.FirstName = dto.FirstName;
        if (dto.LastName != null) entity.LastName = dto.LastName;
        if (dto.Email != null) entity.Email = dto.Email;
        if (dto.Phone != null) entity.Phone = dto.Phone;
        if (dto.BirthDate.HasValue) entity.BirthDate = dto.BirthDate;
        if (dto.Gender.HasValue) entity.Gender = dto.Gender.Value;
        if (dto.EmployerId.HasValue) entity.EmployerId = dto.EmployerId;

        if (dto.Address != null)
        {
            entity.Address = new Address
            {
                Street = dto.Address.Street,
                City = dto.Address.City,
                ZipCode = dto.Address.PostalCode,
                Country = Enum.Parse<Country>(dto.Address.Country)
            };
        }

        await _unitOfWork.Persons.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return PersonDto.FromEntity(entity);
    }

    public async Task<PersonDto?> GetById(Guid id)
    {
        var entity = await _unitOfWork.Persons.FindByIdAsync(id);
        return entity == null ? null : PersonDto.FromEntity(entity);
    }

    public async Task<Note> AddNoteToPerson(Guid personId, CreateNoteDto noteDto)
    {
        var person = await _unitOfWork.Persons.FindByIdAsync(personId);
        if (person == null)
            throw new ContactNotFoundException($"Person with id={personId} not found!");

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

    public async Task<PersonDto> GetPerson(Guid personId)
    {
        var person = await _unitOfWork.Persons.FindByIdAsync(personId);

        if (person == null)
            throw new ContactNotFoundException($"Person with id={personId} not found!");

        return PersonDto.FromEntity(person);
    }
}