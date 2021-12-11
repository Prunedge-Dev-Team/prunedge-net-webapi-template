using Application.Contracts;
using Application.DataTransferObjects;
using AutoMapper;
using Domain.Exceptions;
using Infrastructure.Contracts;


namespace Application.Services;

internal sealed class CompanyService : ICompanyService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public CompanyService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper;
    }

    public IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges)
    {
        var companies = _repository.Company.GetAllCompanies(trackChanges);
        return _mapper.Map<IEnumerable<CompanyDto>>(companies);
    }

    public CompanyDto GetCompany(Guid id, bool trackChanges)
    {
        var company = _repository.Company.GetCompany(id, trackChanges);
        if(company is null) 
            throw new CompanyNotFoundException(id);
        return _mapper.Map<CompanyDto>(company);
    }
}