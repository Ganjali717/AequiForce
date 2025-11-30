using AequiForce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AequiForce.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Policy> Policies => Set<Policy>();
    public DbSet<PolicyRule> PolicyRules => Set<PolicyRule>();
    public DbSet<PolicyAllocation> PolicyAllocations => Set<PolicyAllocation>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<CalculationSnapshot> CalculationSnapshots => Set<CalculationSnapshot>();
    public DbSet<CalculationAllocationSnapshot> CalculationAllocationSnapshots => Set<CalculationAllocationSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Организация
        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasIndex(o => o.Name);
        });

        // Policy
        modelBuilder.Entity<Policy>(entity =>
        {
            entity.HasIndex(p => new { p.OrganizationId, p.IsDefault })
                  .HasFilter("\"IsDefault\" = TRUE");

            entity.HasMany(p => p.Rules)
                  .WithOne(r => r.Policy)
                  .HasForeignKey(r => r.PolicyId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.Allocations)
                  .WithOne(a => a.Policy)
                  .HasForeignKey(a => a.PolicyId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PolicyRule>(entity =>
        {
            entity.HasIndex(r => new { r.PolicyId, r.Priority });
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasIndex(p => new { p.OrganizationId, p.Status });
        });

        modelBuilder.Entity<CalculationSnapshot>(entity =>
        {
            entity.HasIndex(c => new { c.ProjectId, c.CalculatedAt });
        });

        // Можно явно задать precision для decimal, если нужно:
        modelBuilder.Entity<Project>(entity =>
        {
            entity.Property(p => p.BaselineAnnualCost).HasColumnType("numeric(18,2)");
            entity.Property(p => p.PostAiAnnualCost).HasColumnType("numeric(18,2)");
            entity.Property(p => p.BaselineFte).HasColumnType("numeric(10,2)");
            entity.Property(p => p.PostAiFte).HasColumnType("numeric(10,2)");
        });

        modelBuilder.Entity<CalculationSnapshot>(entity =>
        {
            entity.Property(c => c.AnnualSavings).HasColumnType("numeric(18,2)");
            entity.Property(c => c.RequiredReinvestmentAmount).HasColumnType("numeric(18,2)");
            entity.Property(c => c.DisplacedFte).HasColumnType("numeric(10,2)");
            entity.Property(c => c.AppliedReinvestmentPercent).HasColumnType("numeric(5,2)");
            entity.Property(c => c.ImplementationOneOffCost).HasColumnType("numeric(18,2)");
        });

        modelBuilder.Entity<CalculationAllocationSnapshot>(entity =>
        {
            entity.Property(a => a.Amount).HasColumnType("numeric(18,2)");
            entity.Property(a => a.BucketPercent).HasColumnType("numeric(5,2)");
        });
    }
}
