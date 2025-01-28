using Microsoft.AspNetCore.Mvc;

namespace EmailAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("/send")]
    public async Task<IActionResult> SendEmailAsync([FromBody] EmailRequest emailRequest)
    {
        var result = await _emailService.SendEmailAsync(emailRequest);

        if (result)
            return Ok();

        return StatusCode(500);
    }
}
