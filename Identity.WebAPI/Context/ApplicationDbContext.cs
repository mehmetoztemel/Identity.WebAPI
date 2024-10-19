using Identity.WebAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.WebAPI.Context
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
