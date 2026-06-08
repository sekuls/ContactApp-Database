using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContactApp.DTOs;
using ContactApp.Services;

namespace ContactApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly ContactService _contactService;

    public ContactsController(ContactService contactService)
    {
        _contactService = contactService;
    }

    // GET /api/contacts - returns contact list
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var list = await _contactService.GetListAsync();
        return Ok(list);
    }

    // GET /api/contacts/{id} returns contact details
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var contact = await _contactService.GetByIdAsync(id);
        if (contact == null)
            return NotFound(new { message = "Contact not found" });

        return Ok(contact);
    }

    // POST /api/contacts add new contact - requires login
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Add([FromBody] ContactRequest request)
    {
        var (id, error) = await _contactService.AddAsync(request);

        if (error != null)
            return BadRequest(new { message = error });

        return Created($"/api/contacts/{id}", new { id });
    }

    // PUT /api/contacts/{id}
    // update contact - requires login
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] ContactRequest request)
    {
        var error = await _contactService.UpdateAsync(id, request);

        if (error == "Contact not found")
            return NotFound(new { message = error });

        if (error != null)
            return BadRequest(new { message = error });

        return Ok(new { message = "Updated successfully" });
    }

    // DELETE /api/contacts/{id}
    // delete contact - requires login
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _contactService.DeleteAsync(id);

        if (!success)
            return NotFound(new { message = "Contact not found" });

        return Ok(new { message = "Deleted successfully" });
    }
}
