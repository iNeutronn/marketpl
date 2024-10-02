using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(DbContext context) : base(context)

        {
        }

        public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
        {
            return await _dbSet.Include(p => p.Category).Include(p=>p.ReceiptDetails).ToListAsync();
        }

        public Task<Product> GetByIdWithDetailsAsync(int id)
        {
            return _dbSet.Include(p => p.Category).Include(p => p.ReceiptDetails).FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
