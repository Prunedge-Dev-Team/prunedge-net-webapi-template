using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasData
        (
            new Company
            {
                Name = "Prunedge Deevelopment Technologies",
                Address = "32, Ade Ajayi Street, Ogudu GRA, Lagos",
                Country = "Nigeria"
            },
            new Company
            {
                Name = "Elsavia",
                Address = "7b, Omo Ighodalo Street, Ogudu GRA, Lagos",
                Country = "Nigeria"
            }
        );
    }
}