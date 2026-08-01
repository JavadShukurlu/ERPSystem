using ERPSystem.Application.Interfaces.Repositories;
using ERPSystem.Domain.Entities;

namespace ERPSystem.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<Product> Products { get; }

        IGenericRepository<Category> Categories { get; }

        IGenericRepository<Customer> Customers { get; }

        IGenericRepository<Supplier> Suppliers { get; }

        IGenericRepository<Employee> Employees { get; }

        IGenericRepository<Department> Departments { get; }

        IGenericRepository<Warehouse> Warehouses { get; }

        IGenericRepository<Stock> Stocks { get; }

        IGenericRepository<Sale> Sales { get; }

        IGenericRepository<Purchase> Purchases { get; }

        IGenericRepository<Invoice> Invoices { get; }

        IGenericRepository<Payment> Payments { get; }

        IGenericRepository<AuditLog> AuditLogs { get; }

        IGenericRepository<SystemSetting> SystemSettings { get; }
        IGenericRepository<ModulePermission> ModulePermissions { get; }

        Task<int> SaveChangesAsync();
    }
}