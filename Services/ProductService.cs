using ProductAPI.DTOs;
using ProductAPI.Models;
using ProductAPI.Repository;

namespace ProductAPI.Services
{
   
    public interface IProductService
    {
        Task<PagedResponseDto<ProductResponseDto>> GetAllProductsAsync(int page, int pageSize, string? search);
        Task<ProductResponseDto?> GetProductByIdAsync(int id);
        Task<ProductResponseDto> CreateProductAsync(ProductRequestDto request);
        Task<ProductResponseDto?> UpdateProductAsync(int id, ProductRequestDto request);
        Task<bool> DeleteProductAsync(int id);
    }

    
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

       
        public async Task<PagedResponseDto<ProductResponseDto>> GetAllProductsAsync(int page, int pageSize, string? search)
        {
            
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; 

            var products = await _productRepository.GetAllAsync(page, pageSize, search);
            var totalCount = await _productRepository.GetTotalCountAsync(search);

            return new PagedResponseDto<ProductResponseDto>
            {
                Data = products.Select(MapToResponseDto).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return null;

            return MapToResponseDto(product);
        }

       
        public async Task<ProductResponseDto> CreateProductAsync(ProductRequestDto request)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                CreatedDate = DateTime.UtcNow
            };

            var created = await _productRepository.CreateAsync(product);
            return MapToResponseDto(created);
        }

       
        public async Task<ProductResponseDto?> UpdateProductAsync(int id, ProductRequestDto request)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price
            };

            var updated = await _productRepository.UpdateAsync(id, product);
            if (updated == null)
                return null;

            return MapToResponseDto(updated);
        }

        
        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteAsync(id);
        }

        
        private static ProductResponseDto MapToResponseDto(Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CreatedDate = product.CreatedDate
            };
        }
    }
}
