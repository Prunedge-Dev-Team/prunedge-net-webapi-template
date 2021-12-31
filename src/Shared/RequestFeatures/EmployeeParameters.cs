namespace Shared.RequestFeatures;

public class EmployeeParameters : RequestParameters
{
    public EmployeeParameters() => OrderBy = "lastName";
    public uint MinAge { get; set; }
    public uint MaxAge { get; set; }
    public bool ValidAgeRange => MaxAge > MinAge;
}