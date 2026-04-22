using AppCore.Dto;
using AppCore.Models;
namespace AppCore.Interfaces;

public interface IPersonService
{
    Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int size);

    Task<PersonDto> AddPerson(CreatePersonDto dto);

    Task<PersonDto?> GetById(Guid id);

    Task<PersonDto?> UpdatePerson(UpdatePersonDto dto);

    Task<Note> AddNoteToPerson(Guid personId, CreateNoteDto noteDto);

    Task<PersonDto> GetPerson(Guid personId);
}