using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{
    public class ProductService : GenericService<Product,ProductModel>, IProductService
    {
        private readonly IProductRepository productRepository;
        private readonly IProductCategoryRepository productCategoryRepository;
        public ProductService(IUnitOfWork uof, IMapper mapper) : base(uof, mapper)
        {
            productRepository = uof.ProductRepository;
            productCategoryRepository = uof.ProductCategoryRepository;

        }

        protected override IRepository<Product> _repository => productRepository;

        public Task AddCategoryAsync(ProductCategoryModel categoryModel)
        {
            Validate(categoryModel);
            return productCategoryRepository.AddAsync(_mapper.Map<ProductCategory>(categoryModel))
                .ContinueWith(t => _uof.SaveAsync());
        }

        public Task<IEnumerable<ProductCategoryModel>> GetAllProductCategoriesAsync()
        {
            return productCategoryRepository.GetAllAsync()
                .ContinueWith(task => _mapper.Map<IEnumerable<ProductCategoryModel>>(task.Result));
        }

        public async Task<IEnumerable<ProductModel>> GetByFilterAsync(FilterSearchModel filterSearch)
        {
            var products = await productRepository.GetAllWithDetailsAsync();

            if (filterSearch.CategoryId.HasValue)
                products = products.Where(p => p.ProductCategoryId == filterSearch.CategoryId);

            if (filterSearch.MinPrice.HasValue)
                products = products.Where(p => p.Price >= filterSearch.MinPrice);

            if (filterSearch.MaxPrice.HasValue)
                products = products.Where(p => p.Price <= filterSearch.MaxPrice);

            return _mapper.Map<IEnumerable<ProductModel>>(products);
        }

        public Task RemoveCategoryAsync(int categoryId)
        {
            return productCategoryRepository.DeleteByIdAsync(categoryId)
                .ContinueWith(t=> _uof.SaveAsync());
        }

        public Task UpdateCategoryAsync(ProductCategoryModel categoryModel)
        {
            Validate(categoryModel);
            var entity = _mapper.Map<ProductCategory>(categoryModel);
            productCategoryRepository.Update(entity);
            return _uof.SaveAsync();
        }

        public override Task<IEnumerable<ProductModel>> GetAllAsync()
        {
            return productRepository.GetAllWithDetailsAsync()
                .ContinueWith(task => _mapper.Map<IEnumerable<ProductModel>>(task.Result));
        }
        public override Task<ProductModel> GetByIdAsync(int id)
        {
            return productRepository.GetByIdWithDetailsAsync(id)
                .ContinueWith(task => _mapper.Map<ProductModel>(task.Result));
        }
    }
}
