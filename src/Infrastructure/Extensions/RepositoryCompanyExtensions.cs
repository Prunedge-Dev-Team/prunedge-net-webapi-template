using System.Linq.Dynamic.Core;
using Domain.Entities;
using Infrastructure.Extensions.Utility;

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
    
    public static IQueryable<Company> Sort(this IQueryable<Company> companies, string? orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return companies.OrderBy(e => e.Name);

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<Employee>(orderByQueryString);
        
        if (string.IsNullOrWhiteSpace(orderQuery))
            return companies.OrderBy(e => e.Name);

        return companies.OrderBy(orderQuery);
    }
}