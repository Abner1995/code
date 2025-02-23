using Contact.Application.Contacts.Commands.CreateContact;
using Contact.Application.Contacts.Commands.DeleteContact;
using Contact.Application.Contacts.Commands.UpdateContact;
using Contact.Application.Contacts.Queries.GetAllContacts;
using Contact.Application.Contacts.Queries.GetContactById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Contact.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ContactsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllContactsQuery query)
    {
        var contacts = await mediator.Send(query);
        return Ok(contacts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetId([FromRoute] int id)
    {
        var contact = await mediator.Send(new GetContactByIdQuery(id));
        return Ok(contact);
    }

    [HttpPost]
    public async Task<IActionResult> CreateContact([FromBody] CreateContactCommand command)
    {
        int id = await mediator.Send(command);
        return CreatedAtAction(nameof(GetId), new { id }, null);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateContact([FromRoute] int id, UpdateContactCommand command)
    {
        command.Id = id;
        await mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRestaurant([FromRoute] int id)
    {
        await mediator.Send(new DeleteContactCommand(id));
        return NoContent();
    }
}