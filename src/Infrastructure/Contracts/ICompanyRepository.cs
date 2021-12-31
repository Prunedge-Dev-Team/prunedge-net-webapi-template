using Domain.Entities;
using Shared.RequestFeatures;

namespace Infrastructure.Contracts;

public interface ICompanyRepository
{
    Task<PagedList<Company>> GetAllCompaniesAsync(CompanyParameters parameters, bool trackChanges);
    Task<Company?> GetCompanyAsync(Guid id, bool trackChanges);
    void CreateCompany(Company company);
    Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);
    void DeleteCompany(Company company);
}