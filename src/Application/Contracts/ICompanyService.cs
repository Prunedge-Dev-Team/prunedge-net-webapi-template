using Application.DataTransferObjects;
using Shared.RequestFeatures;

namespace Application.Contracts;

public interface ICompanyService
{
    Task<(IEnumerable<CompanyDto> companies, MetaData metaData)> GetAllCompaniesAsync(CompanyParameters parameters, bool trackChanges);
    Task<CompanyDto> GetCompanyAsync(Guid id, bool trackChanges);
    Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company);
    Task<IEnumerable<CompanyDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);
    Task<(IEnumerable<CompanyDto> companies, string ids)> CreateCompanyCollectionAsync(
        IEnumerable<CompanyForCreationDto> companyCollection);
    Task DeleteCompanyAsync(Guid companyId, bool trackChanges);
    Task UpdateCompanyAsync(Guid id, CompanyForUpdateDto companyForUpdate, bool trackChanges);
}