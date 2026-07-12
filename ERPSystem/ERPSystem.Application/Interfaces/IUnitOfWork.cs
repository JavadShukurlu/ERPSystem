using ERPSystem.Application.Interfaces.Repositories;
using ERPSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Department> Departments { get; }
        IGenericRepository<Employee> Employees { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Product> Products { get; }
        IGenericRepository<Warehouse> Warehouses { get; }
        IGenericRepository<Stock> Stocks { get; }
        IGenericRepository<Customer> Customers { get; }
        IGenericRepository<Supplier> Suppliers { get; }
        IGenericRepository<Purchase> Purchases { get; }
        IGenericRepository<PurchaseItem> PurchaseItems { get; }
        IGenericRepository<Sale> Sales { get; }
        IGenericRepository<SaleItem> SaleItems { get; }
        IGenericRepository<Invoice> Invoices { get; }
        IGenericRepository<Payment> Payments { get; }
        IGenericRepository<AuditLog> AuditLogs { get; }

        Task<int> SaveChangesAsync();
    }
}
