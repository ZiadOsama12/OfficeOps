using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
               new IdentityRole
               {
                   Id = "a1b2c3d4-e5f6-7890-1234-56789abcdef0", // Hardcoded GUID
                   Name = "Manager",
                   NormalizedName = "MANAGER"
               },
               new IdentityRole
               {
                   Id = "f0e9d8c7-b6a5-4321-0987-654321fedcba", // Hardcoded GUID

                   Name = "Administrator",
                   NormalizedName = "ADMINISTRATOR"
               });
        }
    }
}
