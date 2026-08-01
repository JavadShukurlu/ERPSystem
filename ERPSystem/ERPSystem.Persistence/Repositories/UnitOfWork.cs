using ERPSystem.Application.Interfaces;
using ERPSystem.Application.Interfaces.Repositories;
using ERPSystem.Domain.Entities;
using ERPSystem.Persistence.Context;

namespace ERPSystem.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IGenericRepository<Product> Products { get; }

        public IGenericRepository<Category> Categories { get; }

        public IGenericRepository<Customer> Customers { get; }

        public IGenericRepository<Supplier> Suppliers { get; }

        public IGenericRepository<Employee> Employees { get; }

        public IGenericRepository<Department> Departments { get; }

        public IGenericRepository<Warehouse> Warehouses { get; }

        public IGenericRepository<Stock> Stocks { get; }

        public IGenericRepository<Sale> Sales { get; }

        public IGenericRepository<Purchase> Purchases { get; }

        public IGenericRepository<Invoice> Invoices { get; }

        public IGenericRepository<Payment> Payments { get; }

        public IGenericRepository<AuditLog> AuditLogs { get; }

        public IGenericRepository<SystemSetting> SystemSettings { get; }
        public IGenericRepository<ModulePermission> ModulePermissions { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            Products = new GenericRepository<Product>(_context);
            Categories = new GenericRepository<Category>(_context);
            Customers = new GenericRepository<Customer>(_context);
            Suppliers = new GenericRepository<Supplier>(_context);
            Employees = new GenericRepository<Employee>(_context);
            Departments = new GenericRepository<Department>(_context);
            Warehouses = new GenericRepository<Warehouse>(_context);
            Stocks = new GenericRepository<Stock>(_context);
            Sales = new GenericRepository<Sale>(_context);
            Purchases = new GenericRepository<Purchase>(_context);
            Invoices = new GenericRepository<Invoice>(_context);
            Payments = new GenericRepository<Payment>(_context);
            AuditLogs = new GenericRepository<AuditLog>(_context);
            SystemSettings = new GenericRepository<SystemSetting>(_context);
            ModulePermissions = new GenericRepository<ModulePermission>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}