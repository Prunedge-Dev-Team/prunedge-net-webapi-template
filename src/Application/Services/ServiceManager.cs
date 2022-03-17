using Application.Contracts;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Identity;
using Infrastructure.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Application.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<ICompanyService> _companyService;
    private readonly Lazy<IEmployeeService> _employeeService;
    private readonly Lazy<IAuthenticationService> _authenticationService;

    public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper, 
        UserManager<User> userManager, IConfiguration configuration)
    {
        _companyService = 
            new Lazy<ICompanyService>(() => new CompanyService(repositoryManager, logger, mapper));
        _employeeService = 
            new Lazy<IEmployeeService>(() => new EmployeeService(repositoryManager, logger, mapper));
        _authenticationService =
            new Lazy<IAuthenticationService>(
                () => new AuthenticationService(logger, mapper, userManager, configuration));
    }
    
    public ICompanyService CompanyService => _companyService.Value;
    public IEmployeeService EmployeeService => _employeeService.Value;
    public IAuthenticationService AuthenticationService => _authenticationService.Value;
}