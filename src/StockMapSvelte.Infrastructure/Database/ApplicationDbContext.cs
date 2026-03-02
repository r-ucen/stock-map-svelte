using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockMapSvelte.Domain.Entities;
using StockMapSvelte.Infrastructure.Database.Seeding;
using StockMapSvelte.Infrastructure.Identity;

namespace StockMapSvelte.Infrastructure.Database;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, Role, string>
{
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Portfolio> Portfolios { get; set; }
    public DbSet<StockProfile> StockProfiles { get; set; }
    public DbSet<UserSetting> UserSettings { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // map relationships
            modelBuilder.Entity<Portfolio>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<UserSetting>()
                .HasOne<ApplicationUser>()
                .WithOne(au => au.UserSetting)
                .HasForeignKey<UserSetting>(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Portfolio>()
                .HasMany(p => p.Stocks)
                .WithMany(s => s.Portfolios);
            
            modelBuilder.Entity<Stock>()
                .HasOne(s => s.StockProfile)
                .WithOne(sp => sp.Stock)
                .HasForeignKey<StockProfile>(sp => sp.StockId)
                .OnDelete(DeleteBehavior.Cascade);
                

            // SEEDING ENTITIES
            StockInit stockInit = new StockInit();
            modelBuilder.Entity<Stock>().HasData(stockInit.GetStocks());

            // SEEDING IDENTITY

            // Init roles
            RolesInit rolesInit = new RolesInit();
            modelBuilder.Entity<Role>().HasData(rolesInit.GetRolesAmc());

            // init users
            UserInit userInit = new UserInit();
            ApplicationUser admin = userInit.GetAdmin();
            ApplicationUser manager = userInit.GetManager();
            ApplicationUser demoUser = userInit.GetDemoUser();

            // add users to the table
            modelBuilder.Entity<ApplicationUser>().HasData(admin, manager, demoUser);

            // assign roles to users
            UserRolesInit userRolesInit = new UserRolesInit();
            List<IdentityUserRole<string>> adminUserRoles = userRolesInit.GetRolesForAdmin();
            List<IdentityUserRole<string>> managerUserRoles = userRolesInit.GetRolesForManager();
            List<IdentityUserRole<string>> demoUserRoles = userRolesInit.GetRolesForDemoUser();
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(adminUserRoles);
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(managerUserRoles);
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(demoUserRoles);
        }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var newUsers = ChangeTracker.Entries<ApplicationUser>()
            .Where(e => e.State == EntityState.Added)
            .Select(e => e.Entity)
            .ToList();
        
        foreach (var user in newUsers)
        {
            var portfolio = new Portfolio
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = "Default portfolio"
            };
            
            Portfolios.Add(portfolio);
        
            UserSettings.Add(new UserSetting
            {
                UserId = user.Id,
                DefaultPortfolioId = portfolio.Id,
                ToastAutoHide = true,
                ToastAutoHideDelayMs = 3000
            });
        }
        
        return await base.SaveChangesAsync(cancellationToken);
    }
}