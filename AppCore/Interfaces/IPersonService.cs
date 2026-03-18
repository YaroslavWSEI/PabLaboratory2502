using AppCore.Dto;

namespace AppCore.Interfaces;

public interface IPersonService
{
    Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int size);
    Task<object?> AddPerson(CreatePersonDto dto);
    Task<object?> GetById(Guid id);
    Task<object?> UpdatePerson(UpdatePersonDto dto);
}