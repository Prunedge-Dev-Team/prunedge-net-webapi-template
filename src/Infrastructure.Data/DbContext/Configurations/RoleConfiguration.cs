using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DbContext.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(new IdentityRole
        {
            Name = "Regular",
            NormalizedName = "REGULAR"
        }, new IdentityRole
        {
            Name = "Manager",
            NormalizedName = "MANAGER"
        }, new IdentityRole
        {
            Name = "Administrator",
            NormalizedName = "ADMINISTRATOR"
        });
    }
}