using Domain.Entities;

namespace Infrastructure.Contracts;

public interface ICompanyRepository
{
    IEnumerable<Company> GetAllCompanies(bool trackChanges);
    Company? GetCompany(Guid id, bool trackChanges);
    void CreateCompany(Company company);
}