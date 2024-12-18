﻿using Asp.Versioning;
using ContactSMS.WebAPI.Constants;
using ContactSMS.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ContactSMS.WebAPI.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion(1.0)]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> logger;

    public UserController(ILogger<UserController> logger)
    {
        this.logger = logger;
    }

    // GET: api/<UserController>
    [HttpGet]
    [AllowAnonymous]
    [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, NoStore = false)]
    public IEnumerable<string> Get()
    {
        logger.LogInformation("日志");
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
    [AllowAnonymous]
    public IActionResult Post([FromBody] UserModel user)
    {
        if (ModelState.IsValid)
        {
            return Ok("Ok");
        }
        else
        {
            return BadRequest(ModelState);
        }
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
