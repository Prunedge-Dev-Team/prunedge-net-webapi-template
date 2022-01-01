using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using Domain.Entities;
using Infrastructure.Extensions.Utility;

namespace Infrastructure.Extensions;

public static class RepositoryEmployeeExtensions
{
    public static IQueryable<Employee> FilterEmployees(this IQueryable<Employee> employees, uint minAge, uint maxAge)
    {
        return minAge > 0 ? employees.Where(e => (e.Age >= minAge && e.Age <= maxAge)) : employees;
    }

    public static IQueryable<Employee> Search(this IQueryable<Employee> employees, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return employees;
        
        var lowerCaseTerm = searchTerm.Trim().ToLower();
        return employees.Where(e =>
            (e.LastName!.ToLower().Contains(lowerCaseTerm) || e.FirstName!.ToLower().Contains(lowerCaseTerm)));
    }

    public static IQueryable<Employee> Sort(this IQueryable<Employee> employees, string? orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return employees.OrderBy(e => e.LastName);

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<Employee>(orderByQueryString);
        
        if (string.IsNullOrWhiteSpace(orderQuery))
            return employees.OrderBy(e => e.LastName);

        return employees.OrderBy(orderQuery);
    }
}