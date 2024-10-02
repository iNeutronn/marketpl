using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Business.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using Business.Models;
using System;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticService _statisticService;
        public StatisticsController(IStatisticService IStatisticService)
        {
            this._statisticService = IStatisticService;
        }

        [HttpGet("popularProducts")]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetPopularProducts([FromQuery] int productCount)
        {
            if (productCount <= 0)
                return BadRequest("Product count must be greater than 0");
            return Ok(await _statisticService.GetMostPopularProductsAsync(productCount));
        }


        [HttpGet("customer/{id}/{productCount}")]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetCustomerFavoriteProducts(int id, int productCount)
        {
            if (productCount <= 0)
                return BadRequest("Product count must be greater than 0");
            return Ok(await _statisticService.GetCustomersMostPopularProductsAsync(id, productCount));
        }

        [HttpGet("activity/{customerCount}")]
        public async Task<ActionResult<IEnumerable<CustomerModel>>> GetMostActiveCustomers(int customerCount, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {

            if (customerCount <= 0)
                return BadRequest("Customer count must be greater than 0");
            return Ok(await _statisticService.GetMostValuableCustomersAsync(customerCount,startDate,endDate));
        }
        [HttpGet("income/{categoryId}")]
        public async Task<ActionResult<decimal>> GetIncomeByCategory(int categoryId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            return Ok(await _statisticService.GetIncomeOfCategoryInPeriod(categoryId, startDate, endDate));
        }

    }
    

    
}
