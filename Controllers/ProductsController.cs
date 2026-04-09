using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.DTOs;
using ProductAPI.Services;

namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/products")]
    [Authorize] 
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var result = await _productService.GetAllProductsAsync(page, pageSize, search);
            return Ok(result);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
                return NotFound(new { message = $"Product with ID {id} was not found." });

            return Ok(product);
        }

       
        [HttpPost]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> Create([FromBody] ProductRequestDto request)
        {
            var product = await _productService.CreateProductAsync(request);

           
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

       
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> Update(int id, [FromBody] ProductRequestDto request)
        {
            var product = await _productService.UpdateProductAsync(id, request);

            if (product == null)
                return NotFound(new { message = $"Product with ID {id} was not found." });

            return Ok(product);
        }

        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteProductAsync(id);

            if (!deleted)
                return NotFound(new { message = $"Product with ID {id} was not found." });

            return NoContent(); 
        }
    }
}
