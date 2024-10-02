using Data.Data;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TradeMarketDbContext _context;

        private readonly  IReceiptDetailRepository _receiptDetailRepository;
        public IReceiptDetailRepository ReceiptDetailRepository => _receiptDetailRepository;
        private readonly ICustomerRepository _customerRepository;
        public ICustomerRepository CustomerRepository => _customerRepository;
        private readonly IPersonRepository _personRepository;
        public IPersonRepository PersonRepository => _personRepository;
        private readonly IProductRepository _productRepository;
        public IProductRepository ProductRepository => _productRepository;
        private readonly IProductCategoryRepository _productCategoryRepository;
        public IProductCategoryRepository ProductCategoryRepository => _productCategoryRepository;
        private readonly IReceiptRepository _receiptRepository;
        public IReceiptRepository ReceiptRepository => _receiptRepository;

        public UnitOfWork(TradeMarketDbContext context)
        {
            _context = context;
            _receiptDetailRepository = new ReceiptDetailRepository(_context);
            _customerRepository = new CustomerRepository(_context);
            _personRepository = new PersonRepository(_context);
            _productRepository = new ProductRepository(_context);
            _productCategoryRepository = new ProductCategoryRepository(_context);
            _receiptRepository = new ReceiptRepository(_context);
        }
        public Task SaveAsync()
        {
            return _context.SaveChangesAsync();
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity
        {
            return typeof(TEntity).Name switch
            {
                nameof(ReceiptDetail) => (IRepository<TEntity>)_receiptDetailRepository,
                nameof(Customer) => (IRepository<TEntity>)_customerRepository,
                nameof(Person) => (IRepository<TEntity>)_personRepository,
                nameof(Product) => (IRepository<TEntity>)_productRepository,
                nameof(ProductCategory) => (IRepository<TEntity>)_productCategoryRepository,
                nameof(Receipt) => (IRepository<TEntity>)_receiptRepository,
                _ => throw new InvalidOperationException($"Repository for type {typeof(TEntity)} not found"),
            };
        }

    }
}
