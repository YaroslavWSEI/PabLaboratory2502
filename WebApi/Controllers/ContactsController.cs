using AppCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AppCore.Models;
using AppCore.Dto;

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
    public async Task<IActionResult> GetAllPersons(int page = 1, int size = 10)
    {
        var result = await _service.FindAllPeoplePaged(page, size);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPerson(Guid id)
    {
        var dto = await _service.GetById(id);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePersonDto dto)
    {
        var result = await _service.AddPerson(dto);

        if (result is not Person person)
            return BadRequest();

        return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, person);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdatePersonDto dto)
    {
        if (id != dto.Id)
            return BadRequest("Id w URL i DTO muszą być takie same.");

        var updated = await _service.UpdatePerson(dto);

        if (updated is null)
            return NotFound();

        return Ok(updated);
    }
}