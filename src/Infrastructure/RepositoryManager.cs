using Contracts;
using Infrastructure.Data.DbContext;
using Infrastructure.Repositories;

namespace Infrastructure;

public class RepositoryManager : IRepositoryManager
{
    private readonly AppDbContext _appDbContext;
    private readonly Lazy<ICompanyRepository> _companyRepository;
    private readonly Lazy<IEmployeeRepository> _employeeRepository;

    public RepositoryManager(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
        _companyRepository = new Lazy<ICompanyRepository>(() => new CompanyRepository(appDbContext));
        _employeeRepository = new Lazy<IEmployeeRepository>(() => new EmployeeRepository(appDbContext));
    }

    public ICompanyRepository Company => _companyRepository.Value;
    public IEmployeeRepository Employee => _employeeRepository.Value;

    public void Save() => _appDbContext.SaveChanges();
}