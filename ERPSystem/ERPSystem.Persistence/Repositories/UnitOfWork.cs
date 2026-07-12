using ERPSystem.Application.Interfaces;
using ERPSystem.Application.Interfaces.Repositories;
using ERPSystem.Domain.Entities;
using ERPSystem.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            Departments = new GenericRepository<Department>(_context);
            Employees = new GenericRepository<Employee>(_context);
            Categories = new GenericRepository<Category>(_context);
            Products = new GenericRepository<Product>(_context);
            Warehouses = new GenericRepository<Warehouse>(_context);
            Stocks = new GenericRepository<Stock>(_context);
            Customers = new GenericRepository<Customer>(_context);
            Suppliers = new GenericRepository<Supplier>(_context);
            Purchases = new GenericRepository<Purchase>(_context);
            PurchaseItems = new GenericRepository<PurchaseItem>(_context);
            Sales = new GenericRepository<Sale>(_context);
            SaleItems = new GenericRepository<SaleItem>(_context);
            Invoices = new GenericRepository<Invoice>(_context);
            Payments = new GenericRepository<Payment>(_context);
            AuditLogs = new GenericRepository<AuditLog>(_context);
        }

        public IGenericRepository<Department> Departments { get; }
        public IGenericRepository<Employee> Employees { get; }
        public IGenericRepository<Category> Categories { get; }
        public IGenericRepository<Product> Products { get; }
        public IGenericRepository<Warehouse> Warehouses { get; }
        public IGenericRepository<Stock> Stocks { get; }
        public IGenericRepository<Customer> Customers { get; }
        public IGenericRepository<Supplier> Suppliers { get; }
        public IGenericRepository<Purchase> Purchases { get; }
        public IGenericRepository<PurchaseItem> PurchaseItems { get; }
        public IGenericRepository<Sale> Sales { get; }
        public IGenericRepository<SaleItem> SaleItems { get; }
        public IGenericRepository<Invoice> Invoices { get; }
        public IGenericRepository<Payment> Payments { get; }
        public IGenericRepository<AuditLog> AuditLogs { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
