using events_manager_api.Application.Dtos;
using events_manager_api.Application.Services;
using events_manager_api.Domain.Entities;
using FluentValidation;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;
using System.Net;

namespace events_manager_api.Application.Controllers;

[ApiController]
[Route("developer")]
public class DeveloperController : ControllerBase
{
    private readonly ILogger<DeveloperController> _logger;
    private readonly IDeveloperService _developerService;

    public DeveloperController(ILogger<DeveloperController> logger, IDeveloperService developerService)
    {
        _logger = logger;
        _developerService = developerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDevelopers()
    {
        var developers = await _developerService.GetDevelopersAsync();
        return Ok(developers);
    }


    [ProducesResponseType(typeof(DeveloperDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [HttpGet("{email}")]
    public IActionResult Get(string email)
    {
        DeveloperDto developer = _developerService.GetDeveloperByEmail(email);

        return Ok(developer);
    }

    [ProducesResponseType(typeof(DeveloperDto), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] DeveloperDto developer)
    {
        DeveloperDto createdDeveloper = await _developerService.CreateDeveloperAsync(developer);

        return CreatedAtAction(
                    nameof(Get),
                    new { email = developer.Email },
                    createdDeveloper);
    }
}