using Application.Contracts;
using Application.DataTransferObjects;
using AutoMapper;
using Domain.Entities;
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

    public CompanyDto CreateCompany(CompanyForCreationDto company)
    {
        var companyEntity = _mapper.Map<Company>(company);
        _repository.Company.CreateCompany(companyEntity);
        _repository.Save();

        return _mapper.Map<CompanyDto>(companyEntity);
    }

    public IEnumerable<CompanyDto> GetByIds(IEnumerable<Guid> ids, bool trackChanges)
    {
        if (ids is null)
            throw new IdsParametersBadRequestException();

        var idsList = ids as Guid[] ?? ids.ToArray();
        var companyEntities = _repository.Company.GetByIds(idsList, trackChanges);
        if (idsList.Count() != companyEntities.Count())
            throw new CollectionByIdsBadRequestException();
        return _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
    }

    public (IEnumerable<CompanyDto> companies, string ids) CreateCompanyCollection(IEnumerable<CompanyForCreationDto> companyCollection)
    {
        if (companyCollection is null)
            throw new CompanyCollectionBadRequest();

        var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
        foreach (var company in companyEntities)
        {
            _repository.Company.CreateCompany(company);
        }
        _repository.Save();

        var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
        var collectionToReturn = companyCollectionToReturn as CompanyDto[] ?? companyCollectionToReturn.ToArray();
        var ids = string.Join(",", collectionToReturn.Select(c => c.Id));
        return (companies: collectionToReturn, ids: ids);
    }

    public void DeleteCompany(Guid companyId, bool trackChanges)
    {
        var company = _repository.Company.GetCompany(companyId, trackChanges);
        if (company is null)
            throw new CompanyNotFoundException(companyId);
        _repository.Company.DeleteCompany(company);
        _repository.Save();
    }
}