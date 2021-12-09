using Domain.Entities;

namespace Infrastructure.Contracts;

public interface ICompanyRepository
{
    IEnumerable<Company> GetAllCompanies(bool trackChanges);
}