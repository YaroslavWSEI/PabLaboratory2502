using AppCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AppCore.Models;
using AppCore.Dto;
using AppCore.Exceptions;
using Microsoft.AspNetCore.Authorization;
using AppCore.Authorization;

namespace WebApi.Controllers;

[ApiController]
[Route("api/contacts")]
public class ContactsController : ControllerBase
{
    private readonly IPersonService _service;

    public ContactsController(IPersonService service)
    {
        _service = service;
    }
    
    [HttpGet]
    [Authorize(Policy = nameof(CrmPolicies.ReadOnlyAccess))]
    public async Task<IActionResult> GetAllPersons(int page = 1, int size = 10)
    {
        var result = await _service.FindAllPeoplePaged(page, size);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = nameof(CrmPolicies.ReadOnlyAccess))]
    public async Task<IActionResult> GetPerson(Guid id)
    {
        var dto = await _service.GetById(id);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    [HttpPost]
    [Authorize(Policy = nameof(CrmPolicies.SalesAccess))]
    public async Task<IActionResult> Create(CreatePersonDto dto)
    {
        var result = await _service.AddPerson(dto);

        if (result == null)
            return BadRequest("Nie udało się utworzyć osoby – sprawdź logikę serwisu.");

        return CreatedAtAction(
            nameof(GetPerson),
            new { id = result.Id },
            result
        );
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = nameof(CrmPolicies.SalesAccess))]
    public async Task<IActionResult> Update(Guid id, UpdatePersonDto dto)
    {
        if (id != dto.Id)
            return BadRequest("Id w URL i DTO muszą być takie same.");

        var updated = await _service.UpdatePerson(dto);

        if (updated is null)
            return NotFound();

        return Ok(updated);
    }

    [HttpPost("{contactId:guid}/notes")]
    [Authorize(Policy = nameof(CrmPolicies.SupportAccess))]
    public async Task<IActionResult> AddNote(
        [FromRoute] Guid contactId,
        [FromBody] CreateNoteDto dto)
    {
        var note = await _service.AddNoteToPerson(contactId, dto);

        return CreatedAtAction(
            nameof(GetNotes),
            new { contactId },
            note);
    }

    [HttpGet("{contactId:guid}/notes")]
    [Authorize(Policy = nameof(CrmPolicies.ReadOnlyAccess))]
    public async Task<IActionResult> GetNotes([FromRoute] Guid contactId)
    {
        var person = await _service.GetPerson(contactId);

        return Ok(person.Notes);
    }
}