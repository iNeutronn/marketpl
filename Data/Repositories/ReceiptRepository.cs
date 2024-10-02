using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class ReceiptRepository : Repository<Receipt> , IReceiptRepository
    {
        public ReceiptRepository(DbContext context) : base(context)
        { }

        public async Task<IEnumerable<Receipt>> GetAllWithDetailsAsync()
        {
            return await _dbSet.Include(r => r.ReceiptDetails).ThenInclude(rd=>rd.Product).ThenInclude(p=>p.Category).Include(r => r.Customer).ThenInclude(c=>c.Person).ToListAsync();
        }

        public Task<Receipt> GetByIdWithDetailsAsync(int id)
        {
            return _dbSet.Include(r => r.ReceiptDetails).ThenInclude(rd => rd.Product).ThenInclude(p => p.Category).Include(r => r.Customer).FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
