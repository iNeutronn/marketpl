using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetByFilter([FromQuery] int? categoryId, [FromQuery] int? minPrice, [FromQuery] int? maxPrice)
        {
            FilterSearchModel filter = new FilterSearchModel
            {
                CategoryId = categoryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };
            try
            {
                return Ok(await _productService.GetByFilterAsync(filter));
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        // POST api/<ProductsController>
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] ProductModel model)
        {
            try
            {
                await _productService.AddAsync(model);
                return Ok(model);
            }
            catch (MarketException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<ProductsController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] ProductModel value)
        {
            try
            {
                value.Id = id;
                await _productService.UpdateAsync(value);
                return Ok();
            }
            catch (MarketException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<ProductsController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return Ok();
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<ProductCategoryModel>>> GetAllCategoryes()
        {
            return Ok(await _productService.GetAllProductCategoriesAsync());
        }

        [HttpPost("categories")]
        public async Task<ActionResult> AddCategory([FromBody] ProductCategoryModel model)
        {
            try
            {
                await _productService.AddCategoryAsync(model);
                return Ok(model);
            }
            catch (MarketException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("categories/{id}")]
        public async Task<ActionResult> UpdateCategory(int id, [FromBody] ProductCategoryModel value)
        {
            try
            {
                value.Id = id;
                await _productService.UpdateCategoryAsync(value);
                return Ok();
            }
            catch (MarketException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("categories/{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            await _productService.RemoveCategoryAsync(id);
            return Ok();
        }

    }
}
