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
    
    // --- ПОИСК (Должен быть выше методов с {id}) ---
    [HttpGet("search")]
    [Authorize(Policy = nameof(CrmPolicies.ReadOnlyAccess))]
    public async Task<IActionResult> SearchPeople(
        [FromQuery] string? emailDomain, 
        [FromQuery] Guid? organizationId)
    {
        var result = await _service.SearchPeople(emailDomain, organizationId);
        return Ok(result);
    }

    // --- ОСТАЛЬНЫЕ GET МЕТОДЫ ---
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

    // --- МЕТОДЫ МОДИФИКАЦИИ (POST/PUT/PATCH/DELETE) ---
    [HttpPost]
    [Authorize(Policy = nameof(CrmPolicies.SalesAccess))]
    public async Task<IActionResult> Create(CreatePersonDto dto)
    {
        try 
        {
            var result = await _service.AddPerson(dto);
            if (result == null)
                return BadRequest("Nie udało się utworzyć osoby.");

            return CreatedAtAction(nameof(GetPerson), new { id = result.Id }, result);
        }
        catch (ArgumentException ex) // Ловим невалидный PESEL
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = nameof(CrmPolicies.SalesAccess))]
    public async Task<IActionResult> Update(Guid id, UpdatePersonDto dto)
    {
        if (id != dto.Id) return BadRequest("Id w URL i DTO muszą być takie same.");
        var updated = await _service.UpdatePerson(dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPatch("{personId:guid}/assign-organization/{orgId:guid}")]
    [Authorize(Policy = nameof(CrmPolicies.SalesAccess))]
    public async Task<IActionResult> AssignToOrganization(Guid personId, Guid orgId)
    {
        var success = await _service.AssignToOrganization(personId, orgId);
        if (!success) return NotFound("Nie znaleziono osoby lub organizacji.");
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = nameof(CrmPolicies.SalesAccess))]
    public async Task<IActionResult> DeleteContact(Guid id)
    {
        await _service.DeleteContact(id); 
        return NoContent();
    }

    // --- ЗАМЕТКИ ---
    [HttpPost("{contactId:guid}/notes")]
    [Authorize(Policy = nameof(CrmPolicies.SupportAccess))]
    public async Task<IActionResult> AddNote([FromRoute] Guid contactId, [FromBody] CreateNoteDto dto)
    {
        var note = await _service.AddNoteToPerson(contactId, dto);
        return CreatedAtAction(nameof(GetNotes), new { contactId }, note);
    }

    [HttpGet("{contactId:guid}/notes")]
    [Authorize(Policy = nameof(CrmPolicies.ReadOnlyAccess))]
    public async Task<IActionResult> GetNotes([FromRoute] Guid contactId)
    {
        var person = await _service.GetPerson(contactId);
        return Ok(person.Notes);
    }
}