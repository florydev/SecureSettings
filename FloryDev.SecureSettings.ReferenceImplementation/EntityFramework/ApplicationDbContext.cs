using Microsoft.EntityFrameworkCore;

namespace FloryDev.SecureSettings.ReferenceImplementation.EntityFramework
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
    }
}
