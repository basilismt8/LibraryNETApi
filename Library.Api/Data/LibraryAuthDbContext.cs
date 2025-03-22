using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Data
{
    public class LibraryAuthDbContext: IdentityDbContext
    {
        public LibraryAuthDbContext(DbContextOptions<LibraryAuthDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var librarianRoleId = "64c44e2b-3942-4f0c-bd0d-6484e5fb778";
            var memberRoleId = "92acd083-da9a-44b2-b8d9-b7e81f2e96e5";

            var roles = new List<IdentityRole>
            {
                new IdentityRole {Id = librarianRoleId, ConcurrencyStamp = librarianRoleId, Name = "Librarian", NormalizedName = "Librarian".ToUpper()},
                new IdentityRole {Id = memberRoleId, ConcurrencyStamp = memberRoleId, Name = "Member", NormalizedName = "Member".ToUpper()}
            };

            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
