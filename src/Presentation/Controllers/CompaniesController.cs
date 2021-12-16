using Application.Contracts;
using Application.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Presentation.ModelBinders;

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

    [HttpGet("{id:guid}", Name = "CompanyById")]
    public IActionResult GetCompany(Guid id)
    {
        var company = _service.CompanyService.GetCompany(id, trackChanges: false);
        return Ok(company);
    }

    [HttpPost]
    public IActionResult CreateCompany([FromBody] CompanyForCreationDto company)
    {
        var createdCompany = _service.CompanyService.CreateCompany(company);
        return CreatedAtRoute("CompanyById", new {id = createdCompany.Id}, createdCompany);
    }

    [HttpGet("collections/({ids})", Name = "GetCompanyCollection")]
    public IActionResult GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
    {
        var companies = _service.CompanyService.GetByIds(ids, trackChanges: false);
        return Ok(companies);
    }

    [HttpPost("collection")]
    public IActionResult CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
    {
        var result = _service.CompanyService.CreateCompanyCollection(companyCollection);
        return CreatedAtRoute("GetCompanyCollection", new {result.ids}, result.companies);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteCompany(Guid id)
    {
        _service.CompanyService.DeleteCompany(id, trackChanges: false);
        return NoContent();
    }

    public IActionResult UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
    {
        _service.CompanyService.UpdateCompany(id, company, trackChanges: true);
        return NoContent();
    }
}