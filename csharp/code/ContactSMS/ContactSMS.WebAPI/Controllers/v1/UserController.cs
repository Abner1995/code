using Asp.Versioning;
using ContactSMS.WebAPI.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ContactSMS.WebAPI.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion(1.0)]
public class UserController : ControllerBase
{
    // GET: api/<UserController>
    [HttpGet]
    [AllowAnonymous]
    public IEnumerable<string> Get()
    {
        return new string[] { "1", "1" };
    }

    // GET api/<UserController>/5
    [HttpGet("{id}")]
    [Authorize(Policy = PolicyContstants.MustHaveEmployeeId)]
    [Authorize(Policy = PolicyContstants.MustBeTheOwner)]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/<UserController>
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/<UserController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<UserController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
