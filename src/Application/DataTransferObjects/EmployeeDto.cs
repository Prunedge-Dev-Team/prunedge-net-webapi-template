using System.ComponentModel.DataAnnotations;

namespace Application.DataTransferObjects;

public record EmployeeDto(Guid Id, string FirstName, string LastName, string Position);

public record EmployeeForMutationDto
{
    [Required(ErrorMessage = "FirstName is a required field")]
    [MaxLength(50, ErrorMessage = "Maximum length for FirstName is 50 characters")]
    public  string? FirstName { get; init; }

    [Required(ErrorMessage = "LastName is a required field")]
    [MaxLength(50, ErrorMessage = "Maximum length for LastName is 50 characters")]
    public  string? LastName { get; init; }

    [Required(ErrorMessage = "Age is a required field")]
    public  int Age { get; init; }

    [Required(ErrorMessage = "Position is a required field")]
    [MaxLength(50, ErrorMessage = "Maximum length for Position is 50 characters")]
    public  string? Position { get; init; }
}

public record EmployeeForCreationDto : EmployeeForMutationDto;

public record EmployeeForUpdateDto : EmployeeForMutationDto;
