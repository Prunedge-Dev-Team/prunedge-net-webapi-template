namespace Shared.RequestFeatures;

public class EmployeeParameters : RequestParameters
{
    public EmployeeParameters() => OrderBy = "lastName";
    public uint MinAge { get; set; }
    public uint MaxAge { get; set; }

    public bool ValidAgeRange
    {
        get
        {
            if(MinAge > 0)
                return MaxAge > MinAge;
            return true;
        }
    }
}