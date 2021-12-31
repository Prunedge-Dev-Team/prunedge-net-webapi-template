using Application.Contracts;
using Application.DataTransferObjects;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/v1/companies/{companyId}/employees")]
public class EmployeesController : ControllerBase
{
    private readonly IServiceManager _service;

    public EmployeesController(IServiceManager service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetEmployeesForCompany(Guid companyId)
    {
        var employees = _service.EmployeeService.GetEmployees(companyId, trackChanges: false);
        return Ok(employees);
    }

    [HttpGet("{id:guid}", Name = "GetEmployeesForCompany")]
    public IActionResult GetEmployeeForCompany(Guid companyId, Guid id)
    {
        var employee = _service.EmployeeService.GetEmployee(companyId, id, trackChanges: false);
        return Ok(employee);
    }

    [HttpPost]
    public IActionResult CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var employeeToReturn =
            _service.EmployeeService.CreateEmployeeForCompany(companyId, employee, trackChanges: false);
        return CreatedAtRoute("GetEmployeesForCompany", 
            new {companyId, id = employeeToReturn.Id}, employeeToReturn);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteEmployeeForCompany(Guid companyId, Guid id)
    {
        _service.EmployeeService.DeleteEmployeeForCompany(companyId, id, trackingChanges: false);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    public IActionResult UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employee)
    {
        _service.EmployeeService.UpdateEmployeeForCompany(companyId, id, employee, compTrackChanges: false,
            empTrackChanges: true);
        return NoContent();
    }

    [HttpPatch("{id:guid}")]
    public IActionResult PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id,
        [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
    {
        var result =
            _service.EmployeeService.GetEmployeeForPatch(companyId, id, compTrackChanges: false, empTrackChanges: true);
        patchDoc.ApplyTo(result.employeeToPatch, ModelState);
        TryValidateModel(result.employeeToPatch);
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        _service.EmployeeService.SaveChangesForPatch(result.employeeToPatch, result.employeeEntity);
        return NoContent();
    }
}