using LeaveManagementSystem.Web.Data.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace LeaveManagementSystem.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

#if true // Simpler way to apply configurations
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

#else
            builder.ApplyConfiguration(new LeaveRequestStatusConfiguration());
            builder.ApplyConfiguration(new IdentityRoleConfiguration());
            builder.ApplyConfiguration(new ApplicationUserConfiguration());
            builder.ApplyConfiguration(new IdentityUserRoleConfiguration());
#endif
        }

        public DbSet<LeaveType> LeaveTypes { get; set; } = default!;
        public DbSet<LeaveAllocation> LeaveAllocations { get; set; } = default!;
        public DbSet<Period> Periods { get; set; } = default!;
        public DbSet<LeaveRequestStatus> LeaveRequestStatuses { get; set; } = default!;
        public DbSet<LeaveRequest> LeaveRequests { get; set; } = default!;
    }
}
