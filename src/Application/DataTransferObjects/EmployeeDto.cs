namespace Application.DataTransferObjects;

public record EmployeeDto(Guid Id, string FirstName, string LastName, string Position);
public record EmployeeForCreationDto(string FirstName, string LastName, int Age, string Position);
public record EmployeeForUpdateDto(string FirstName, string LastName, int Age, string Position);
