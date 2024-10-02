using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class CustomerService : GenericService<Customer,CustomerModel>, ICustomerService
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IPersonRepository personRepository;

        public CustomerService(IUnitOfWork uof, IMapper mapper) : base(uof, mapper)
        {
            customerRepository = uof.CustomerRepository;
            personRepository = uof.PersonRepository;
        }

        protected override IRepository<Customer> _repository { get => customerRepository; }

        public async Task<IEnumerable<CustomerModel>> GetCustomersByProductIdAsync(int productId)
        {
            var customers = await customerRepository.GetAllWithDetailsAsync();

            var customersWithProduct = customers
                .Where(customer => customer.Receipts
                    .SelectMany(receipt => receipt.ReceiptDetails)
                    .Any(rd => rd.ProductId == productId));

            return _mapper.Map<IEnumerable<CustomerModel>>(customersWithProduct);
        }


        public override async Task<IEnumerable<CustomerModel>> GetAllAsync()
        {
            return await customerRepository.GetAllWithDetailsAsync()
               .ContinueWith(t =>
               _mapper.Map<IEnumerable<CustomerModel>>(t.Result));
        }
        public override Task<CustomerModel> GetByIdAsync(int id)
        {
            return customerRepository.GetByIdWithDetailsAsync(id)
                .ContinueWith(t => _mapper.Map<CustomerModel>(t.Result));
        }

        public override Task UpdateAsync(CustomerModel model)
        {
            Validate(model);
            var entity = _mapper.Map<Customer>(model);
            entity.PersonId = model.Id;
            entity.Person.Id = model.Id;
            personRepository.Update(entity.Person);
            customerRepository.Update(entity);
            return _uof.SaveAsync();
        }

    }
}
