
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IMS.Webapp.Data
{
    public class AccountDbContext : IdentityDbContext
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
        {
        }
    }
}
