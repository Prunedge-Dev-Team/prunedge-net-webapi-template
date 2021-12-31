using Domain.Entities;
using Shared.RequestFeatures;

namespace Infrastructure.Contracts;

public interface IEmployeeRepository
{
    Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeesParameters, 
        bool trackChanges);
    Task<Employee?> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges);
    void CreateEmployeeForCompany(Guid companyId, Employee employee);
    void DeleteEmployee(Employee employee);
}