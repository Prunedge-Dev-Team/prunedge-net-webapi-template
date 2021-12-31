namespace Application.DataTransferObjects;

// public record CompanyDto(Guid Id, string Name, string FullAddress);
public record CompanyDto
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? FullAddress { get; init; }
}

public record CompanyForCreationDto
{
    public string? Name { get; init; }
    public string? Address { get; init; }
    public string? Country { get; init; }
    public IEnumerable<EmployeeForCreationDto>? Employees { get; init; }
}

public record CompanyForUpdateDto(string Name, string Address, string Country, 
    IEnumerable<EmployeeForCreationDto> Employees);