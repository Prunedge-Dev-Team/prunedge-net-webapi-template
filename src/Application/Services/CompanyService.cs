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

    public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(bool trackChanges)
    {
        var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges);
        return _mapper.Map<IEnumerable<CompanyDto>>(companies);
    }

    public async Task<CompanyDto> GetCompanyAsync(Guid id, bool trackChanges)
    {
        var company = await _repository.Company.GetCompanyAsync(id, trackChanges);
        if(company is null) 
            throw new CompanyNotFoundException(id);
        return _mapper.Map<CompanyDto>(company);
    }

    public async Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company)
    {
        var companyEntity = _mapper.Map<Company>(company);
        _repository.Company.CreateCompany(companyEntity);
        await _repository.SaveAsync();

        return _mapper.Map<CompanyDto>(companyEntity);
    }

    public async Task<IEnumerable<CompanyDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
    {
        if (ids is null)
            throw new IdsParametersBadRequestException();

        var idsList = ids as Guid[] ?? ids.ToArray();
        var companyEntities = await _repository.Company.GetByIdsAsync(idsList, trackChanges);
        if (idsList.Count() != companyEntities.Count())
            throw new CollectionByIdsBadRequestException();
        return _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
    }

    public async Task<(IEnumerable<CompanyDto> companies, string ids)> CreateCompanyCollectionAsync(IEnumerable<CompanyForCreationDto> companyCollection)
    {
        if (companyCollection is null)
            throw new CompanyCollectionBadRequest();

        var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
        foreach (var company in companyEntities)
        {
            _repository.Company.CreateCompany(company);
        }
        await _repository.SaveAsync();

        var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
        var collectionToReturn = companyCollectionToReturn as CompanyDto[] ?? companyCollectionToReturn.ToArray();
        var ids = string.Join(",", collectionToReturn.Select(c => c.Id));
        return (companies: collectionToReturn, ids: ids);
    }

    public async Task DeleteCompanyAsync(Guid companyId, bool trackChanges)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
        if (company is null)
            throw new CompanyNotFoundException(companyId);
        _repository.Company.DeleteCompany(company);
        await _repository.SaveAsync();
    }

    public async Task UpdateCompanyAsync(Guid id, CompanyForUpdateDto companyForUpdate, bool trackChanges)
    {
        var companyEntity = await _repository.Company.GetCompanyAsync(id, trackChanges);
        if (companyEntity is null)
            throw new CompanyNotFoundException(id);
        _mapper.Map(companyForUpdate, companyEntity);
        await _repository.SaveAsync();
    }
}