using apbd_t1.Services;
using Microsoft.AspNetCore.Mvc;

namespace apbd_t1.Controllers;

[ApiController]
[Route("[controller]")]
public class TempController : ControllerBase
{
    private readonly ITempService _tempService;

    public TempController(ITempService tempService)
    {
        _tempService = tempService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTemps() => Ok(await _tempService.GetTemps());
}