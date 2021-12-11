using Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/v1/companies")]
[ApiController]
public class CompaniesController : ControllerBase
{
    private readonly IServiceManager _service;

    public CompaniesController(IServiceManager service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetCompanies()
    {
        var companies = _service.CompanyService.GetAllCompanies(trackChanges: false);
        return Ok(companies);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetCompany(Guid id)
    {
        var company = _service.CompanyService.GetCompany(id, trackChanges: false);
        return Ok(company);
    }
}