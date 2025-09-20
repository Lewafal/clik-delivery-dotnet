using ProductService.Data;
using ProductService.Models;
using ProductService.DTOs;
using ProductService.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Services
{
    public class SupplierService
    {
        private readonly AppDbContext _context;
        public SupplierService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SupplierDto>> GetAllAsync()
        {
            return await _context.Suppliers
                .Select(s => new SupplierDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    ContactEmail = s.ContactEmail,
                    PhoneNumber = s.PhoneNumber
                }).ToListAsync();
        }
        public async Task<SupplierDto> GetByIdAsync(Guid id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
                throw new NotFoundException($"Supplier with Id{id} not found");
            return new SupplierDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactEmail = supplier.ContactEmail,
                PhoneNumber = supplier.PhoneNumber
            };
        }

        public async Task<SupplierDto> CreateAsync(CreateSupplierDto dto)
        {
            var supplier = new Supplier
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                ContactEmail = dto.ContactEmail,
                PhoneNumber = dto.PhoneNumber
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return new SupplierDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactEmail = supplier.ContactEmail,
                PhoneNumber = supplier.PhoneNumber
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            
            if (supplier == null)
                throw new NotFoundException($"Supplier with Id {id} not found");

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductStockAsync(Guid supplierId, UpdateProductStockDto dto)
        {
            var supplier = await _context.Suppliers
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.Id == supplierId);

            if (supplier == null)
                throw new NotFoundException($"Supplier with Id {supplierId} not found");

            var product = supplier.Products.FirstOrDefault(p => p.Id == dto.ProductId);
            if (product == null)
                throw new NotFoundException($"Prodouct with Id {dto.ProductId} not found for this supplier");

            product.Stock += dto.QuantityToAdd;
            await _context.SaveChangesAsync();
        }
    }
}