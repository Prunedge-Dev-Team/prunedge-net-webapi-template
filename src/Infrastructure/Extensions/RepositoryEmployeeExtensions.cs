using Domain.Entities;

namespace Infrastructure.Extensions;

public static class RepositoryEmployeeExtensions
{
    public static IQueryable<Employee> FilterEmployees(this IQueryable<Employee> employees, uint minAge, uint maxAge)
    {
        return employees.Where(e => (e.Age >= minAge && e.Age <= maxAge));
    }

    public static IQueryable<Employee> Search(this IQueryable<Employee> employees, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return employees;
        
        var lowerCaseTerm = searchTerm.Trim().ToLower();
        return employees.Where(e =>
            (e.LastName!.ToLower().Contains(lowerCaseTerm) || e.FirstName!.ToLower().Contains(lowerCaseTerm)));
    }
}