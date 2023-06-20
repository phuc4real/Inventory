﻿using Inventory.Repository.DbContext;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Repositories
{
    public class ReceiptRepository : Repository<Receipt>, IReceiptRepository
    {
        private readonly AppDbContext _context;

        public ReceiptRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private IQueryable<Receipt> GetAllWithProperty => _context.Receipts
            .Include(x => x.Details)!
            .ThenInclude(d => d.Item);

        public async Task<IEnumerable<Receipt>> GetAllAsync()
        {
            return await GetAllWithProperty.ToListAsync();
        }

        public async Task<Receipt> GetById(int id)
        {
            var query = GetAllWithProperty
                .Where(x => x.Id == id);

#pragma warning disable CS8603 // Possible null reference return.
            return await query.FirstOrDefaultAsync();
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<IEnumerable<Receipt>> ReceiptByItem(Item item)
        {
            var query = GetAllWithProperty
                .Where(x=>x.Items!.Contains(item));

            return await query.ToListAsync();
        }
    }
}
