using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Company
{
    [Column("CompanyId")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(60, ErrorMessage = "Maximum length for Name is 60 characters")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Address is required")]
    [MaxLength(200, ErrorMessage = "Maximum length for Address is 200 characters")]
    public string? Address { get; set; }

    public string? Country { get; set; }
    
    public ICollection<Employee>? Employees { get; set; }

}