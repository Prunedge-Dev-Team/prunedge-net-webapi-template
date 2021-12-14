namespace Application.DataTransferObjects;

public record EmployeeDto(Guid Id, string FirstName, string LastName, string Position);
public record EmployeeForCreation(string FirstName, string LastName, int Age, string Position);
