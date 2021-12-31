using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;

namespace Infrastructure.Repositories;

public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
{
    public CompanyRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public async Task<PagedList<Company>> GetAllCompaniesAsync(CompanyParameters parameters, bool trackChanges)
    {
        var companies = await FindAll(trackChanges)
            .Search(parameters.SearchTerm)
            .Sort(parameters.OrderBy)
            .ToListAsync();
        return PagedList<Company>.ToPagedList(companies, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<Company?> GetCompanyAsync(Guid id, bool trackChanges) =>
        await FindByCondition(c => c.Id.Equals(id), trackChanges).SingleOrDefaultAsync();

    public void CreateCompany(Company company) => Create(company);

    public async  Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges) =>
        await FindByCondition(x => ids.Contains(x.Id), trackChanges).ToListAsync();

    public void DeleteCompany(Company company) => Delete(company);
}