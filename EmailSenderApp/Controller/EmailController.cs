using EmailSenderApp.Domain;
using EmailSenderApp.Service;
using Microsoft.AspNetCore.Mvc;

namespace EmailSenderApp.Controller;

[Route("api/[controller]")]
[ApiController]
public class EmailController : ControllerBase
{
    private readonly IEmailService _service;

    public EmailController(IEmailService service)
    {
        _service = service;
    }


    [HttpPost]
    public async Task<ActionResult<string>> Send([FromForm] Email email)
    {
        var result = await _service.Send(email);

        if (result)
        {
            return Ok("Xabar muvaffaqqiyatli jo'natildi");
        }

        return BadRequest("Xabar jo'natishda xato");
    }
}