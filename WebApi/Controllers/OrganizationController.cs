using AppCore.Authorization;
using AppCore.Dto;
using AppCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/organizations")]
public class OrganizationsController : ControllerBase
{
    private readonly IOrganizationService _service;

    public OrganizationsController(IOrganizationService service)
    {
        _service = service;
    }
    [HttpGet]
    [Authorize(Policy = nameof(CrmPolicies.ReadOnlyAccess))]
    public async Task<IActionResult> GetAll(int page = 1, int size = 10)
    {
        var result = await _service.GetAllPaged(page, size);
        return Ok(result);
    }
    [HttpGet("{id:guid}")]
    [Authorize(Policy = nameof(CrmPolicies.ReadOnlyAccess))]
    public async Task<IActionResult> GetById(Guid id)
    {
        var org = await _service.GetById(id);
        return org is null ? NotFound() : Ok(org);
    }
    [HttpGet("search")]
    [Authorize(Policy = nameof(CrmPolicies.ReadOnlyAccess))]
    public async Task<IActionResult> Search(
        [FromQuery] string? name,
        [FromQuery] string? emailDomain)
    {
        var result = await _service.Search(name, emailDomain);
        return Ok(result);
    }
    [HttpGet("{id:guid}/members")]
    [Authorize(Policy = nameof(CrmPolicies.ReadOnlyAccess))]
    public async Task<IActionResult> GetMembers(Guid id)
    {
        var org = await _service.GetById(id);
        if (org is null) return NotFound();

        var members = await _service.GetMembers(id);
        return Ok(members);
    }
    [HttpPost]
    [Authorize(Policy = nameof(CrmPolicies.SalesAccess))]
    public async Task<IActionResult> Create(CreateOrganizationDto dto)
    {
        var result = await _service.AddOrganization(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
    [HttpPut("{id:guid}")]
    [Authorize(Policy = nameof(CrmPolicies.SalesAccess))]
    public async Task<IActionResult> Update(Guid id, UpdateOrganizationDto dto)
    {
        if (id != dto.Id) return BadRequest("Id w URL i DTO muszą być takie same.");
        var updated = await _service.UpdateOrganization(dto);
        return updated is null ? NotFound() : Ok(updated);
    }
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = nameof(CrmPolicies.SalesAccess))]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteOrganization(id);
        return NoContent();
    }
    [HttpPatch("{orgId:guid}/members/{personId:guid}")]
    [Authorize(Policy = nameof(CrmPolicies.SalesAccess))]
    public async Task<IActionResult> AddMember(Guid orgId, Guid personId)
    {
        var success = await _service.AddMember(orgId, personId);
        return success ? NoContent() : NotFound("Nie znaleziono organizacji lub osoby.");
    }
    [HttpDelete("{orgId:guid}/members/{personId:guid}")]
    [Authorize(Policy = nameof(CrmPolicies.SalesAccess))]
    public async Task<IActionResult> RemoveMember(Guid orgId, Guid personId)
    {
        var success = await _service.RemoveMember(orgId, personId);
        return success ? NoContent() : NotFound("Nie znaleziono osoby w tej organizacji.");
    }
}