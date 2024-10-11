using LaTiQ.Core.DTO.Request.Room;
using LaTiQ.Core.Entities.Room;
using LaTiQ.Core.Entities.Topic;
using LaTiQ.Core.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LaTiQ.Infrastructure.DatabaseContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Room>? Rooms { get; set; }
    }
}
