using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Sales;
using ERPSystem.Application.Interfaces;
using ERPSystem.Application.Interfaces.Services;
using ERPSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERPSystem.Application.Services
{
    public class SaleService : ISaleService
    {
        private const string ModuleName = "Sales";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IModulePermissionService _modulePermissionService;
        private readonly ICurrentUserService _currentUserService;

        public SaleService(
            IUnitOfWork unitOfWork,
            IModulePermissionService modulePermissionService,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _modulePermissionService = modulePermissionService;
            _currentUserService = currentUserService;
        }

        public async Task<ResultDto<List<SaleDto>>> GetAllAsync()
        {
            var salesQuery = _unitOfWork.Sales
                .GetQueryable()
                .Include(sale => sale.Customer)
                .Include(sale => sale.SaleItems)
                    .ThenInclude(item => item.Product)
                .Include(sale => sale.SaleItems)
                    .ThenInclude(item => item.Warehouse)
                .AsQueryable();

            if (!IsAdmin())
            {
                var accessLevel = await GetAccessLevelAsync("View");

                if (accessLevel == 0)
                {
                    return ResultDto<List<SaleDto>>.Success(new List<SaleDto>());
                }

                if (accessLevel == 1)
                {
                    salesQuery = salesQuery.Where(sale =>
                        sale.CreatedByUserId == _currentUserService.UserId);
                }
            }

            var sales = await salesQuery
                .OrderByDescending(sale => sale.SaleDate)
                .Select(sale => new SaleDto
                {
                    Id = sale.Id,
                    SaleDate = sale.SaleDate,
                    CustomerId = sale.CustomerId,
                    CustomerName = sale.Customer.FullName,
                    TotalAmount = sale.TotalAmount,
                    Items = sale.SaleItems.Select(item => new SaleItemDto
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        ProductName = item.Product.Name,
                        WarehouseId = item.WarehouseId,
                        WarehouseName = item.Warehouse.Name,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    }).ToList()
                })
                .ToListAsync();

            return ResultDto<List<SaleDto>>.Success(sales);
        }

        public async Task<ResultDto<SaleDto>> GetByIdAsync(int id)
        {
            var sale = await _unitOfWork.Sales
                .GetQueryable()
                .Include(sale => sale.Customer)
                .Include(sale => sale.SaleItems)
                    .ThenInclude(item => item.Product)
                .Include(sale => sale.SaleItems)
                    .ThenInclude(item => item.Warehouse)
                .FirstOrDefaultAsync(sale => sale.Id == id);

            if (sale is null)
            {
                return ResultDto<SaleDto>.Failure("Sale not found.");
            }

            if (!IsAdmin())
            {
                var hasPermission = await HasPermissionAsync("View", sale);

                if (!hasPermission)
                {
                    return ResultDto<SaleDto>.Failure("You do not have permission to view this sale.");
                }
            }

            return ResultDto<SaleDto>.Success(MapToDto(sale));
        }

        public async Task<ResultDto<SaleDto>> CreateAsync(CreateSaleDto dto)
        {
            if (!IsAdmin())
            {
                var hasPermission = await HasPermissionAsync("Create");

                if (!hasPermission)
                {
                    return ResultDto<SaleDto>.Failure("You do not have permission to create sale records.");
                }
            }

            if (dto.Items.Count == 0)
            {
                return ResultDto<SaleDto>.Failure("Sale must contain at least one item.");
            }

            var customer = await _unitOfWork.Customers.GetByIdAsync(dto.CustomerId);

            if (customer is null)
            {
                return ResultDto<SaleDto>.Failure("Customer not found.");
            }

            var sale = new Sale
            {
                CustomerId = dto.CustomerId,
                SaleDate = dto.SaleDate,
                TotalAmount = 0,
                CreatedByUserId = _currentUserService.UserId
            };

            foreach (var itemDto in dto.Items)
            {
                if (itemDto.Quantity <= 0)
                {
                    return ResultDto<SaleDto>.Failure("Item quantity must be greater than zero.");
                }

                if (itemDto.UnitPrice <= 0)
                {
                    return ResultDto<SaleDto>.Failure("Item unit price must be greater than zero.");
                }

                var product = await _unitOfWork.Products.GetByIdAsync(itemDto.ProductId);

                if (product is null)
                {
                    return ResultDto<SaleDto>.Failure($"Product with id {itemDto.ProductId} not found.");
                }

                var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(itemDto.WarehouseId);

                if (warehouse is null)
                {
                    return ResultDto<SaleDto>.Failure($"Warehouse with id {itemDto.WarehouseId} not found.");
                }

                var stock = await _unitOfWork.Stocks.GetAsync(stock =>
                    stock.ProductId == itemDto.ProductId &&
                    stock.WarehouseId == itemDto.WarehouseId);

                if (stock is null)
                {
                    return ResultDto<SaleDto>.Failure($"No stock found for product id {itemDto.ProductId} in warehouse id {itemDto.WarehouseId}.");
                }

                if (stock.Quantity < itemDto.Quantity)
                {
                    return ResultDto<SaleDto>.Failure($"Insufficient stock for product id {itemDto.ProductId}. Available quantity: {stock.Quantity}.");
                }

                var totalPrice = itemDto.Quantity * itemDto.UnitPrice;

                sale.SaleItems.Add(new SaleItem
                {
                    ProductId = itemDto.ProductId,
                    WarehouseId = itemDto.WarehouseId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    TotalPrice = totalPrice
                });

                sale.TotalAmount += totalPrice;

                stock.Quantity -= itemDto.Quantity;
                _unitOfWork.Stocks.Update(stock);
            }

            await _unitOfWork.Sales.AddAsync(sale);
            await _unitOfWork.SaveChangesAsync();

            var createdSale = await _unitOfWork.Sales
                .GetQueryable()
                .Include(sale => sale.Customer)
                .Include(sale => sale.SaleItems)
                    .ThenInclude(item => item.Product)
                .Include(sale => sale.SaleItems)
                    .ThenInclude(item => item.Warehouse)
                .FirstAsync(sale => sale.Id == sale.Id);

            return ResultDto<SaleDto>.Success(MapToDto(createdSale), "Sale created successfully and stock decreased.");
        }

        public async Task<ResultDto<bool>> DeleteAsync(int id)
        {
            var sale = await _unitOfWork.Sales.GetByIdAsync(id);

            if (sale is null)
            {
                return ResultDto<bool>.Failure("Sale not found.");
            }

            if (!IsAdmin())
            {
                var hasPermission = await HasPermissionAsync("Delete", sale);

                if (!hasPermission)
                {
                    return ResultDto<bool>.Failure("You do not have permission to delete this sale.");
                }
            }

            _unitOfWork.Sales.Delete(sale);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<bool>.Success(true, "Sale deleted successfully.");
        }

        private async Task<int> GetAccessLevelAsync(string actionName)
        {
            if (string.IsNullOrWhiteSpace(_currentUserService.UserId))
            {
                return 0;
            }

            var permissions = await _modulePermissionService.GetUserModulePermissionsAsync(
                _currentUserService.UserId,
                ModuleName);

            var permission = permissions.Data?
                .FirstOrDefault(permission => permission.ActionName == actionName);

            return permission is null ? 0 : (int)permission.AccessLevel;
        }

        private async Task<bool> HasPermissionAsync(string actionName, Sale? sale = null)
        {
            if (string.IsNullOrWhiteSpace(_currentUserService.UserId))
            {
                return false;
            }

            return await _modulePermissionService.HasPermissionAsync(
                _currentUserService.UserId,
                ModuleName,
                actionName,
                sale);
        }

        private bool IsAdmin()
        {
            return string.Equals(_currentUserService.UserName, "admin", StringComparison.OrdinalIgnoreCase);
        }

        private static SaleDto MapToDto(Sale sale)
        {
            return new SaleDto
            {
                Id = sale.Id,
                SaleDate = sale.SaleDate,
                CustomerId = sale.CustomerId,
                CustomerName = sale.Customer?.FullName,
                TotalAmount = sale.TotalAmount,
                Items = sale.SaleItems.Select(item => new SaleItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name,
                    WarehouseId = item.WarehouseId,
                    WarehouseName = item.Warehouse?.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                }).ToList()
            };
        }
    }
}