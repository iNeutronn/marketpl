using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(DbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Customer>> GetAllWithDetailsAsync()
        {
            return await _dbSet.Include(c => c.Receipts).ThenInclude(r => r.ReceiptDetails).Include(c=>c.Person).ToListAsync();
        }

        public Task<Customer> GetByIdWithDetailsAsync(int id)
        {
            return _dbSet.Include(c => c.Receipts).ThenInclude(r => r.ReceiptDetails).Include(c=>c.Person).FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
