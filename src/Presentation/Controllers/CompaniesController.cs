using System.Text.Json;
using Application.Contracts;
using Application.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Presentation.ModelBinders;
using Shared.RequestFeatures;

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
    public async Task<IActionResult> GetCompanies([FromQuery] CompanyParameters companyParameters)
    {
        var pagedResult = await _service.CompanyService.GetAllCompaniesAsync(
            companyParameters, trackChanges: false);
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));
        return Ok(pagedResult.companies);
    }

    [HttpGet("{id:guid}", Name = "CompanyById")]
    public async Task<IActionResult> GetCompany(Guid id)
    {
        var company = await _service.CompanyService.GetCompanyAsync(id, trackChanges: false);
        return Ok(company);
    }

    [HttpPost]
    // [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
    {
        var createdCompany = await _service.CompanyService.CreateCompanyAsync(company);
        return CreatedAtRoute("CompanyById", new {id = createdCompany.Id}, createdCompany);
    }

    [HttpGet("collections/({ids})", Name = "GetCompanyCollection")]
    public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
    {
        var companies = await _service.CompanyService.GetByIdsAsync(ids, trackChanges: false);
        return Ok(companies);
    }

    [HttpPost("collection")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
    {
        var result = await _service.CompanyService.CreateCompanyCollectionAsync(companyCollection);
        return CreatedAtRoute("GetCompanyCollection", new {result.ids}, result.companies);
    }

    [HttpDelete("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        await _service.CompanyService.DeleteCompanyAsync(id, trackChanges: false);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
    {
        await _service.CompanyService.UpdateCompanyAsync(id, company, trackChanges: true);
        return NoContent();
    }
}