using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ICollectedDataRepository
    {
        Task AddRangeAsync(List<CollectedData> items, CancellationToken cancellationToken = default);
        Task<List<CollectedData>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<CollectedData?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string source, string title, string url, CancellationToken cancellationToken = default);
    }
}
