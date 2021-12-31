using Application.Contracts;
using Application.DataTransferObjects;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Contracts;
using Shared.RequestFeatures;

namespace Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper;
    }
    
    private async Task CheckIfCompanyExists(Guid companyId, bool trackChanges)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
        if(company is null) 
            throw new CompanyNotFoundException(companyId);
    }

    private async Task<Employee> GetEmployeeForCompanyAndCheckIfItExists(Guid companyId, Guid id, bool trackChanges)
    {
        var employee = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);
        if (employee is null)
            throw new EmployeeNotFoundException(id);
        return employee;
    }

    public async Task<(IEnumerable<EmployeeDto> employees, MetaData metaData)> GetEmployeesAsync(Guid companyId, EmployeeParameters parameters, 
        bool trackChanges)
    {
        if (!parameters.ValidAgeRange)
            throw new MaxAgeRangeBadRequestException();
        
        await CheckIfCompanyExists(companyId, trackChanges);
        var employees = await _repository.Employee.GetEmployeesAsync(
            companyId, parameters, trackChanges);
        var employeesDto =  _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        return (employees: employeesDto, metaData: employees.MetaData);
    }

    public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
    {
        await CheckIfCompanyExists(companyId, trackChanges);
        var employee = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);
        if (employee is null)
            throw new EmployeeNotFoundException(id);

        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, 
        EmployeeForCreationDto employeeForCreation, bool trackChanges)
    {
        await CheckIfCompanyExists(companyId, trackChanges);
        var employeeEntity = _mapper.Map<Employee>(employeeForCreation);
        _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
        await _repository.SaveAsync();

        return _mapper.Map<EmployeeDto>(employeeEntity);
    }

    public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges)
    {
        await CheckIfCompanyExists(companyId, trackChanges);

        var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, trackChanges);
        _repository.Employee.DeleteEmployee(employee);
        await _repository.SaveAsync();
    }

    public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id,  EmployeeForUpdateDto employeeForUpdateDto, 
        bool compTrackChanges, bool empTrackChanges)
    {
        await CheckIfCompanyExists(companyId, compTrackChanges);
        var employeeEntity = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);
        _mapper.Map(employeeForUpdateDto, employeeEntity);
        await _repository.SaveAsync();
    }

    public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(
        Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
    {
        await CheckIfCompanyExists(companyId, compTrackChanges);
        var employeeEntity = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);
        var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);
        return (employeeToPatch, employeeEntity);
    }

    public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
    {
        _mapper.Map(employeeToPatch, employeeEntity);
        await _repository.SaveAsync();
    }
}