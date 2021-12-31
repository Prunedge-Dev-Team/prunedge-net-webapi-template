using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;
using Infrastructure.Extensions;
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
            await FindByCondition(
                    e => e.CompanyId.Equals(companyId), trackChanges)
                .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
                .Search(employeeParameters.SearchTerm)
                .Sort(employeeParameters.OrderBy)
                .ToListAsync();
        // var count = await FindByCondition(
        //     e => e.CompanyId.Equals(companyId), trackChanges: false).CountAsync();
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