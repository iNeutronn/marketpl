using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IMapper _mapper;
        private readonly IReceiptRepository _receiptRepository;
        private readonly IReceiptDetailRepository _receiptDetailRepository;
        public StatisticService(IUnitOfWork uof, IMapper mapper)
        {
            _mapper = mapper;
            _receiptRepository = uof.ReceiptRepository;
            _receiptDetailRepository = uof.ReceiptDetailRepository;
        }
        public async Task<IEnumerable<ProductModel>> GetCustomersMostPopularProductsAsync(int productCount, int customerId)
        {
            var products = (await _receiptRepository.GetAllWithDetailsAsync())
                .Where(r => r.CustomerId == customerId)
                .SelectMany(r => r.ReceiptDetails)
                .GroupBy(rd => rd.ProductId)
                .OrderByDescending(g => g.Sum(rd => rd.Quantity))
                .Take(productCount)
                .Select(rd => rd.First().Product);
            return _mapper.Map<IEnumerable<ProductModel>>(products);
        }

        public async Task<decimal> GetIncomeOfCategoryInPeriod(int categoryId, DateTime startDate, DateTime endDate)
        {
            return (await _receiptRepository.GetAllWithDetailsAsync())
                .Where(r => r.OperationDate > startDate && r.OperationDate < endDate)
                .SelectMany(r => r.ReceiptDetails)
                .Where(rd => rd.Product.ProductCategoryId == categoryId)
                .Sum(rd => rd.Quantity * rd.DiscountUnitPrice);
        }

        public async Task<IEnumerable<ProductModel>> GetMostPopularProductsAsync(int productCount)
        {
            var products = (await _receiptDetailRepository.GetAllWithDetailsAsync())
                .GroupBy(rd => rd.ProductId)
                .OrderByDescending(g => g.Sum(rd => rd.Quantity))
                .Take(productCount)
                .Select(g=> g.First().Product);
            return _mapper.Map<IEnumerable<ProductModel>>(products);


        }

        public async Task<IEnumerable<CustomerActivityModel>> GetMostValuableCustomersAsync(int customerCount, DateTime startDate, DateTime endDate)
        {
            return (await _receiptRepository.GetAllWithDetailsAsync())
                .Where(r => r.OperationDate > startDate && r.OperationDate < endDate)
                .GroupBy(r => r.CustomerId)
                .Select(g => new CustomerActivityModel()
                {
                    CustomerId = g.First().CustomerId,
                    CustomerName = g.First().Customer.Person.Name + " " + g.First().Customer.Person.Surname,
                    ReceiptSum = g.Sum(r => r.ReceiptDetails.Sum(rd => rd.Quantity * rd.DiscountUnitPrice))
                })
                .OrderByDescending(cam => cam.ReceiptSum)
                .Take(customerCount);
        }
    }
}
