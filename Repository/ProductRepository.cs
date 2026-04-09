using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;
using ProductAPI.Models;

namespace ProductAPI.Repository
{

    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync(int page, int pageSize, string? search);
        Task<int> GetTotalCountAsync(string? search);
        Task<Product?> GetByIdAsync(int id);
        Task<Product> CreateAsync(Product product);
        Task<Product?> UpdateAsync(int id, Product product);
        Task<bool> DeleteAsync(int id);
    }

    
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

       
        public async Task<List<Product>> GetAllAsync(int page, int pageSize, string? search)
        {
            var query = _context.Products.AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
            }

            return await query
                .OrderByDescending(p => p.CreatedDate)
                .Skip((page - 1) * pageSize)  
                .Take(pageSize)               
                .ToListAsync();
        }

        
        public async Task<int> GetTotalCountAsync(string? search)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
            }

            return await query.CountAsync();
        }

        
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }


        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        
        public async Task<Product?> UpdateAsync(int id, Product updatedProduct)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
                return null;

            existingProduct.Name = updatedProduct.Name;
            existingProduct.Description = updatedProduct.Description;
            existingProduct.Price = updatedProduct.Price;

            await _context.SaveChangesAsync();
            return existingProduct;
        }

       
        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
