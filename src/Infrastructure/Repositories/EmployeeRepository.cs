using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories;

public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
    
    
}