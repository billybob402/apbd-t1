using apbd_t1.Exceptions;
using apbd_t1.Models.DTOs;
using apbd_t1.Services;
using Microsoft.AspNetCore.Mvc;

namespace apbd_t1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VisitsController : ControllerBase
{
    private readonly IDbService _dbService;

    public VisitsController(IDbService dbService)
    {
        _dbService = dbService;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetVisitById(int id)
    {
        try
        {
            return Ok(await _dbService.GetVisitById(id));
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddVisit([FromBody] VisitRequestDto requestDto)
    {
        try
        {
            await _dbService.AddVisitByRequestDto(requestDto);
            return Ok();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ConflictException e)
        {
            return Conflict(e.Message);
        }
    }
}