using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{
    public class ReceiptService : GenericService<Receipt,ReceiptModel>, IReceiptService
    {
        private readonly IReceiptRepository receiptRepository;
        private readonly IProductRepository productRepository;
        private readonly IReceiptDetailRepository receiptDetailRepository;
        public ReceiptService(IUnitOfWork uof, IMapper mapper) : base(uof, mapper)
        {
            receiptRepository = uof.ReceiptRepository;
            productRepository = uof.ProductRepository;
            receiptDetailRepository = uof.ReceiptDetailRepository;
        }

        protected override IRepository<Receipt> _repository => receiptRepository;

        public async Task AddProductAsync(int productId, int receiptId, int quantity)
        {
            var receipt = await receiptRepository.GetByIdWithDetailsAsync(receiptId) ?? throw new MarketException($"There is no receipt with id={receiptId} in repository");
            var receiptDetail = receipt.ReceiptDetails?.SingleOrDefault(rd => rd.ProductId == productId);
            if (receiptDetail is null)
            {
                var product = await productRepository.GetByIdAsync(productId) ?? throw new MarketException($"There is no product with id={productId} in repository");
                var NewReceipt = new ReceiptDetail()
                {
                    ProductId = productId,
                    ReceiptId = receiptId,
                    Quantity = quantity,
                    UnitPrice = product.Price,
                    DiscountUnitPrice = (1 - receipt.Customer.DiscountValue/100m) * product.Price,
                };
                await receiptDetailRepository.AddAsync(NewReceipt);
            }
            else
            {
                receiptDetail.Quantity += quantity;
            }
            await _uof.SaveAsync();
        }

        public Task CheckOutAsync(int receiptId)
        {
            return receiptRepository.GetByIdAsync(receiptId)
                .ContinueWith(t =>
                {
                    var receipt = t.Result;
                    if (receipt is null)
                        throw new MarketException($"There is no receipt with id={receiptId} in repository");
                    if (receipt.IsCheckedOut)
                        throw new MarketException($"Receipt with id={receiptId} is already checked out");
                    receipt.IsCheckedOut = true;
                    return _uof.SaveAsync();
                });
        }

        public Task<IEnumerable<ReceiptDetailModel>> GetReceiptDetailsAsync(int receiptId)
        {
            return receiptRepository.GetByIdWithDetailsAsync(receiptId)
                .ContinueWith(t => _mapper.Map<IEnumerable<ReceiptDetailModel>>(t.Result.ReceiptDetails));
        }

        public Task<IEnumerable<ReceiptModel>> GetReceiptsByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            return receiptRepository.GetAllWithDetailsAsync()
                .ContinueWith(t => _mapper.Map<IEnumerable<ReceiptModel>>(t.Result.Where(x=> x.OperationDate >startDate && x.OperationDate < endDate)));
        }

        public async Task RemoveProductAsync(int productId, int receiptId, int quantity)
        {
            var receipt = await receiptRepository.GetByIdWithDetailsAsync(receiptId);
            if (receipt is null)
                throw new MarketException($"There is no receipt with id={receiptId} in repository");

            var receiptDetail = receipt.ReceiptDetails.SingleOrDefault(rd => rd.ProductId == productId);
            if (receiptDetail is null)
                throw new MarketException($"There is no product with id={productId} in receipt with id={receiptId}");

            if (receiptDetail.Quantity < quantity)
                throw new MarketException($"There is not enough quantity of product with id={productId} in receipt with id={receiptId}");

            if(receiptDetail.Quantity - quantity == 0)
            {
                receipt.ReceiptDetails.Remove(receiptDetail);
                receiptDetailRepository.Delete(receiptDetail);
            }
            else
            {
                receiptDetail.Quantity -= quantity;
            }
            await _uof.SaveAsync();
        }

        public async Task<decimal> ToPayAsync(int receiptId)
        {
            var receipt = await receiptRepository.GetByIdWithDetailsAsync(receiptId);
            if (receipt is null)
                throw new MarketException($"There is no receipt with id={receiptId} in repository");
            return receipt.ReceiptDetails.Sum(rd => rd.Quantity * rd.DiscountUnitPrice);
        }

        public override Task<IEnumerable<ReceiptModel>> GetAllAsync()
        {
            return receiptRepository.GetAllWithDetailsAsync()
                .ContinueWith(t => _mapper.Map<IEnumerable<ReceiptModel>>(t.Result));
        }
        public override Task<ReceiptModel> GetByIdAsync(int id)
        {
            return receiptRepository.GetByIdWithDetailsAsync(id)
                .ContinueWith(t => _mapper.Map<ReceiptModel>(t.Result));
        }
        public override async Task DeleteAsync(int modelId)
        {
            var receipt = await receiptRepository.GetByIdWithDetailsAsync(modelId);
            if (receipt is null)
                return;
            var receiptDetails = receipt.ReceiptDetails.ToList();
            foreach (var rd in receiptDetails)
                receiptDetailRepository.Delete(rd);
            
            await receiptRepository.DeleteByIdAsync(modelId);
            await _uof.SaveAsync();
        }
    }
}
