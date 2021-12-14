using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories;

public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
{
    public CompanyRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public IEnumerable<Company> GetAllCompanies(bool trackChanges) =>
        FindAll(trackChanges).OrderBy(c => c.Name).ToList();

    public Company? GetCompany(Guid id, bool trackChanges) =>
        FindByCondition(c => c.Id.Equals(id), trackChanges).SingleOrDefault();

    public void CreateCompany(Company company) => Create(company);

    public IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges) =>
        FindByCondition(x => ids.Contains(x.Id), trackChanges);

    public void DeleteCompany(Company company) => Delete(company);
}