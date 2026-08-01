using ERPSystem.Application.Interfaces;
using ERPSystem.Domain.Common;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ERPSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Persistence.Context
{
    public class AppDbContext: IdentityDbContext<AppUser, AppRole, string>
    {
        private readonly ICurrentUserService? _currentUserService;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            ICurrentUserService? currentUserService = null) : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<ModulePermission> ModulePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .HasOne(user => user.Employee)
                .WithOne(employee => employee.AppUser)
                .HasForeignKey<Employee>(employee => employee.AppUserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ModulePermission>()
                .HasOne(permission => permission.User)
                .WithMany(user => user.ModulePermissions)
                .HasForeignKey(permission => permission.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ModulePermission>()
                .HasIndex(permission => new
                {
                    permission.UserId,
                    permission.ModuleName,
                    permission.ActionName
                })
                .IsUnique();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetAuditFields();

            var auditEntries = CreateAuditEntries();

            var result = await base.SaveChangesAsync(cancellationToken);

            if (auditEntries.Count > 0)
            {
                await AuditLogs.AddRangeAsync(auditEntries, cancellationToken);
                await base.SaveChangesAsync(cancellationToken);
            }

            return result;
        }
        private void SetAuditFields()
        {
            var userId = _currentUserService?.UserId;

            var entries = ChangeTracker.Entries<BaseEntity>()
                .Where(entry =>
                    entry.Entity is not AuditLog &&
                    entry.State is EntityState.Added or EntityState.Modified)
                .ToList();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    entry.Entity.CreatedByUserId = userId;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedDate = DateTime.UtcNow;
                    entry.Entity.UpdatedByUserId = userId;
                }
            }
        }

        private List<AuditLog> CreateAuditEntries()
        {
            var auditLogs = new List<AuditLog>();

            var entries = ChangeTracker.Entries<BaseEntity>()
                .Where(entry =>
                    entry.Entity is not AuditLog &&
                    entry.State is EntityState.Added or EntityState.Modified)
                .ToList();

            foreach (var entry in entries)
            {
                var action = GetAction(entry);

                auditLogs.Add(new AuditLog
                {
                    UserId = _currentUserService?.UserName ?? "System",
                    Action = action,
                    EntityName = entry.Entity.GetType().Name,
                    EntityId = entry.Entity.Id == 0 ? null : entry.Entity.Id,
                    ActionDate = DateTime.UtcNow,
                    Details = BuildDetails(entry)
                });
            }

            return auditLogs;
        }

        private static string GetAction(EntityEntry<BaseEntity> entry)
        {
            if (entry.State == EntityState.Added)
            {
                return "Created";
            }

            if (entry.State == EntityState.Modified && entry.Entity.IsDeleted)
            {
                return "Deleted";
            }

            return "Updated";
        }

        private static string BuildDetails(EntityEntry<BaseEntity> entry)
        {
            if (entry.State == EntityState.Added)
            {
                return $"{entry.Entity.GetType().Name} was created.";
            }

            if (entry.State == EntityState.Modified && entry.Entity.IsDeleted)
            {
                return $"{entry.Entity.GetType().Name} was deleted.";
            }

            var changedProperties = entry.Properties
                .Where(property => property.IsModified)
                .Select(property => property.Metadata.Name)
                .ToList();

            if (changedProperties.Count == 0)
            {
                return $"{entry.Entity.GetType().Name} was updated.";
            }

            return $"{entry.Entity.GetType().Name} updated fields: {string.Join(", ", changedProperties)}.";
        }
    }
}
