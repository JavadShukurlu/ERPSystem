using ERPSystem.Application.DTOs.Common;
using ERPSystem.Application.DTOs.Purchases;
using ERPSystem.Application.Interfaces;
using ERPSystem.Application.Interfaces.Services;
using ERPSystem.Domain.Entities;
using ERPSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERPSystem.Application.Services
{
    public class PurchaseService : IPurchaseService
    {
        private const string ModuleName = "Purchases";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IModulePermissionService _modulePermissionService;
        private readonly ICurrentUserService _currentUserService;

        public PurchaseService(
            IUnitOfWork unitOfWork,
            IModulePermissionService modulePermissionService,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _modulePermissionService = modulePermissionService;
            _currentUserService = currentUserService;
        }

        public async Task<ResultDto<List<PurchaseDto>>> GetAllAsync()
        {
            var purchasesQuery = _unitOfWork.Purchases
    .GetQueryable()
    .Include(purchase => purchase.Supplier)
    .Include(purchase => purchase.PurchaseItems)
        .ThenInclude(item => item.Product)
    .Include(purchase => purchase.PurchaseItems)
        .ThenInclude(item => item.Warehouse)
    .AsQueryable();

            var permission = await _modulePermissionService.CheckPermissionAsync(
                _currentUserService.UserId!,
                ModuleName,
                "View");

            if (!permission.IsSuccess || permission.Data!.AccessLevel == PermissionAccessLevel.None)
            {
                return ResultDto<List<PurchaseDto>>.Success(new List<PurchaseDto>());
            }

            if (permission.Data!.AccessLevel == PermissionAccessLevel.Own)
            {
                purchasesQuery = purchasesQuery
                    .Where(purchase => purchase.CreatedByUserId == _currentUserService.UserId);
            }

            var purchases = await purchasesQuery
                .Select(purchase => new PurchaseDto
                {
                    Id = purchase.Id,
                    PurchaseDate = purchase.PurchaseDate,
                    SupplierId = purchase.SupplierId,
                    SupplierName = purchase.Supplier.CompanyName,
                    TotalAmount = purchase.TotalAmount,
                    CreatedByUserId = purchase.CreatedByUserId,
                    CreatedDate = purchase.CreatedDate,
                    UpdatedDate = purchase.UpdatedDate,
                    Items = purchase.PurchaseItems.Select(item => new PurchaseItemDto
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

            return ResultDto<List<PurchaseDto>>.Success(purchases);
        }

        public async Task<ResultDto<PurchaseDto>> GetByIdAsync(int id)
        {
            var purchase = await _unitOfWork.Purchases
                .GetQueryable()
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(i => i.Product)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(i => i.Warehouse)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (purchase is null)
            {
                return ResultDto<PurchaseDto>.Failure("Purchase not found.");
            }

            var hasViewPermission = await _modulePermissionService.HasPermissionAsync(
    _currentUserService.UserId!,
    ModuleName,
    "View",
    purchase);

            if (!hasViewPermission)
            {
                return ResultDto<PurchaseDto>.Failure("You do not have permission to view this purchase.");
            }

            return ResultDto<PurchaseDto>.Success(MapToDto(purchase));
        }

        public async Task<ResultDto<PurchaseDto>> CreateAsync(CreatePurchaseDto dto)
        {
            var hasCreatePermission = await _modulePermissionService.HasPermissionAsync(
    _currentUserService.UserId!,
    ModuleName,
    "Create");

            if (!hasCreatePermission)
            {
                return ResultDto<PurchaseDto>.Failure("You do not have permission to create purchases.");
            }

            if (dto.Items.Count == 0)
            {
                return ResultDto<PurchaseDto>.Failure("Purchase must contain at least one item.");
            }

            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(dto.SupplierId);

            if (supplier is null)
            {
                return ResultDto<PurchaseDto>.Failure("Supplier not found.");
            }

            var purchase = new Purchase
            {
                SupplierId = dto.SupplierId,
                PurchaseDate = dto.PurchaseDate,
                TotalAmount = 0
            };

            foreach (var itemDto in dto.Items)
            {
                if (itemDto.Quantity <= 0)
                {
                    return ResultDto<PurchaseDto>.Failure("Item quantity must be greater than zero.");
                }

                if (itemDto.UnitPrice <= 0)
                {
                    return ResultDto<PurchaseDto>.Failure("Item unit price must be greater than zero.");
                }

                var product = await _unitOfWork.Products.GetByIdAsync(itemDto.ProductId);

                if (product is null)
                {
                    return ResultDto<PurchaseDto>.Failure($"Product with id {itemDto.ProductId} not found.");
                }

                var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(itemDto.WarehouseId);

                if (warehouse is null)
                {
                    return ResultDto<PurchaseDto>.Failure($"Warehouse with id {itemDto.WarehouseId} not found.");
                }

                var totalPrice = itemDto.Quantity * itemDto.UnitPrice;

                var purchaseItem = new PurchaseItem
                {
                    ProductId = itemDto.ProductId,
                    WarehouseId = itemDto.WarehouseId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    TotalPrice = totalPrice
                };

                purchase.PurchaseItems.Add(purchaseItem);
                purchase.TotalAmount += totalPrice;

                var stock = await _unitOfWork.Stocks.GetAsync(stock =>
                    stock.ProductId == itemDto.ProductId &&
                    stock.WarehouseId == itemDto.WarehouseId);

                if (stock is null)
                {
                    stock = new Stock
                    {
                        ProductId = itemDto.ProductId,
                        WarehouseId = itemDto.WarehouseId,
                        Quantity = itemDto.Quantity,
                        MinimumQuantity = 0
                    };

                    await _unitOfWork.Stocks.AddAsync(stock);
                }
                else
                {
                    stock.Quantity += itemDto.Quantity;
                    _unitOfWork.Stocks.Update(stock);
                }
            }

            await _unitOfWork.Purchases.AddAsync(purchase);
            await _unitOfWork.SaveChangesAsync();

            var createdPurchase = await _unitOfWork.Purchases
                .GetQueryable()
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(i => i.Product)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(i => i.Warehouse)
                .FirstAsync(p => p.Id == purchase.Id);

            return ResultDto<PurchaseDto>.Success(
                MapToDto(createdPurchase),
                "Purchase created successfully and stock increased.");
        }

        public async Task<ResultDto<PurchaseDto>> UpdateAsync(UpdatePurchaseDto dto)
        {
            var purchase = await _unitOfWork.Purchases
                .GetQueryable()
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(i => i.Product)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(i => i.Warehouse)
                .FirstOrDefaultAsync(p => p.Id == dto.Id);

            if (purchase is null)
            {
                return ResultDto<PurchaseDto>.Failure("Purchase not found.");
            }

            var hasUpdatePermission = await _modulePermissionService.HasPermissionAsync(
                _currentUserService.UserId!,
                ModuleName,
                "Update",
                purchase);

            if (!hasUpdatePermission)
            {
                return ResultDto<PurchaseDto>.Failure("You do not have permission to update this purchase.");
            }

            if (dto.Items.Count == 0)
            {
                return ResultDto<PurchaseDto>.Failure("Purchase must contain at least one item.");
            }

            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(dto.SupplierId);

            if (supplier is null)
            {
                return ResultDto<PurchaseDto>.Failure("Supplier not found.");
            }

            foreach (var itemDto in dto.Items)
            {
                if (itemDto.Quantity <= 0)
                {
                    return ResultDto<PurchaseDto>.Failure("Item quantity must be greater than zero.");
                }

                if (itemDto.UnitPrice <= 0)
                {
                    return ResultDto<PurchaseDto>.Failure("Item unit price must be greater than zero.");
                }

                var product = await _unitOfWork.Products.GetByIdAsync(itemDto.ProductId);

                if (product is null)
                {
                    return ResultDto<PurchaseDto>.Failure($"Product with id {itemDto.ProductId} not found.");
                }

                var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(itemDto.WarehouseId);

                if (warehouse is null)
                {
                    return ResultDto<PurchaseDto>.Failure($"Warehouse with id {itemDto.WarehouseId} not found.");
                }
            }

            foreach (var oldItem in purchase.PurchaseItems.ToList())
            {
                var stock = await _unitOfWork.Stocks.GetAsync(stock =>
                    stock.ProductId == oldItem.ProductId &&
                    stock.WarehouseId == oldItem.WarehouseId);

                if (stock is not null)
                {
                    stock.Quantity -= oldItem.Quantity;

                    if (stock.Quantity < 0)
                    {
                        stock.Quantity = 0;
                    }

                    _unitOfWork.Stocks.Update(stock);
                }
            }

            purchase.SupplierId = dto.SupplierId;
            purchase.PurchaseDate = dto.PurchaseDate;
            purchase.TotalAmount = 0;
            purchase.PurchaseItems.Clear();

            foreach (var itemDto in dto.Items)
            {
                var totalPrice = itemDto.Quantity * itemDto.UnitPrice;

                var purchaseItem = new PurchaseItem
                {
                    ProductId = itemDto.ProductId,
                    WarehouseId = itemDto.WarehouseId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    TotalPrice = totalPrice
                };

                purchase.PurchaseItems.Add(purchaseItem);
                purchase.TotalAmount += totalPrice;

                var stock = await _unitOfWork.Stocks.GetAsync(stock =>
                    stock.ProductId == itemDto.ProductId &&
                    stock.WarehouseId == itemDto.WarehouseId);

                if (stock is null)
                {
                    stock = new Stock
                    {
                        ProductId = itemDto.ProductId,
                        WarehouseId = itemDto.WarehouseId,
                        Quantity = itemDto.Quantity,
                        MinimumQuantity = 0
                    };

                    await _unitOfWork.Stocks.AddAsync(stock);
                }
                else
                {
                    stock.Quantity += itemDto.Quantity;
                    _unitOfWork.Stocks.Update(stock);
                }
            }

            _unitOfWork.Purchases.Update(purchase);
            await _unitOfWork.SaveChangesAsync();

            var updatedPurchase = await _unitOfWork.Purchases
                .GetQueryable()
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(i => i.Product)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(i => i.Warehouse)
                .FirstAsync(p => p.Id == purchase.Id);

            return ResultDto<PurchaseDto>.Success(
                MapToDto(updatedPurchase),
                "Purchase updated successfully and stock recalculated.");
        }

        public async Task<ResultDto<bool>> DeleteAsync(int id)
        {
            var purchase = await _unitOfWork.Purchases.GetByIdAsync(id);

            if (purchase is null)
            {
                return ResultDto<bool>.Failure("Purchase not found.");
            }

            var hasDeletePermission = await _modulePermissionService.HasPermissionAsync(
    _currentUserService.UserId!,
    ModuleName,
    "Delete",
    purchase);

            if (!hasDeletePermission)
            {
                return ResultDto<bool>.Failure("You do not have permission to delete this purchase.");
            }

            _unitOfWork.Purchases.Delete(purchase);
            await _unitOfWork.SaveChangesAsync();

            return ResultDto<bool>.Success(true, "Purchase deleted successfully.");
        }

        private static PurchaseDto MapToDto(Purchase purchase)
        {
            return new PurchaseDto
            {
                Id = purchase.Id,
                PurchaseDate = purchase.PurchaseDate,
                SupplierId = purchase.SupplierId,
                SupplierName = purchase.Supplier?.CompanyName,
                TotalAmount = purchase.TotalAmount,
                CreatedByUserId = purchase.CreatedByUserId,
                CreatedDate = purchase.CreatedDate,
                UpdatedDate = purchase.UpdatedDate,
                Items = purchase.PurchaseItems.Select(item => new PurchaseItemDto
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