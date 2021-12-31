using Domain.Entities;

namespace Infrastructure.Extensions;

public static class RepositoryCompanyExtensions
{
    public static IQueryable<Company> Search(this IQueryable<Company> companies, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return companies;
        
        var lowerCaseTerm = searchTerm!.Trim().ToLower();
        
        return companies.Where(e =>
            (e.Name!.ToLower().Contains(lowerCaseTerm) || e.Address!.ToLower().Contains(lowerCaseTerm)));
    }
}