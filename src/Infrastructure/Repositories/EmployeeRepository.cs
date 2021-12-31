using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;

namespace Infrastructure.Repositories;

public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters,
        bool trackChanges)
    {
        var employees = 
            await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
            .OrderBy(e => e.LastName)
            .Skip((employeeParameters.PageNumber -1 )*employeeParameters.PageSize)
            .Take(employeeParameters.PageSize)
            .ToListAsync();
        var count = await FindByCondition(
            e => e.CompanyId.Equals(companyId), trackChanges: false).CountAsync();
        return PagedList<Employee>.ToPagedList(employees, employeeParameters.PageNumber, employeeParameters.PageSize);
    }

    public async Task<Employee?> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges) =>
        await FindByCondition(e => 
            e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges).SingleOrDefaultAsync();
    public void CreateEmployeeForCompany(Guid companyId, Employee employee)
    {
        employee.CompanyId = companyId;
        Create(employee);
    }
    public void DeleteEmployee(Employee employee) => Delete(employee);
}