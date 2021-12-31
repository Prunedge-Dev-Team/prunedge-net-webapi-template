using Application.DataTransferObjects;
using Domain.Entities;
using Shared.RequestFeatures;

namespace Application.Contracts;

public interface IEmployeeService
{
    Task<(IEnumerable<EmployeeDto> employees, MetaData metaData)> GetEmployeesAsync(Guid companyId, EmployeeParameters parameters, bool trackChanges);
    Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges);
    Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges);
    Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackingChanges);
    Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdateDto, 
        bool compTrackChanges, bool empTrackChanges);

    Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(Guid companyId, Guid id,
        bool compTrackChanges, bool empTrackChanges);
    Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity);
}