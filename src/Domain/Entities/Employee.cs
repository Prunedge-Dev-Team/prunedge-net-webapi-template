using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Employee
{
    [Column("CompanyId")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "FirstName is required")]
    [MaxLength(30, ErrorMessage = "Maximum length for FirstName is 30 characters")]
    public string? FirstName { get; set; }
    
    [Required(ErrorMessage = "LastName is required")]
    [MaxLength(30, ErrorMessage = "Maximum length for LastName is 30 characters")]
    public string? LastName { get; set; }
    
    [Required(ErrorMessage = "Position is required")]
    [MaxLength(30, ErrorMessage = "Maximum length for Position is 30 characters")]
    public string? Position { get; set; }
    
    [ForeignKey(nameof(Company))]
    public Guid CompanyId { get; set; }
    public Company? Company { get; set; }
    
}