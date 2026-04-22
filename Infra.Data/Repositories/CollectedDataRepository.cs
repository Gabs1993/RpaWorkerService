using Domain.Entities;
using Domain.Interfaces;
using Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Data.Repositories
{
    public class CollectedDataRepository : ICollectedDataRepository
    {
        private readonly AppDbContext _context;

        public CollectedDataRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(List<CollectedData> items, CancellationToken cancellationToken = default)
        {
            await _context.CollectedData.AddRangeAsync(items, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<CollectedData>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.CollectedData
                .OrderByDescending(x => x.CollectedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<CollectedData?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.CollectedData
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsAsync(string source, string title, string description, string url, CancellationToken cancellationToken = default)
        {
            return await _context.CollectedData.AnyAsync(x =>
                x.Source == source &&
                x.Title == title &&
                x.Description == description &&
                x.Url == url,
                cancellationToken);
        }
    }
}
