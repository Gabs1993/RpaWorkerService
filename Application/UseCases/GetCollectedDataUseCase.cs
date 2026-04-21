using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public class GetCollectedDataUseCase
    {
        private readonly ICollectedDataRepository _repository;

        public GetCollectedDataUseCase(ICollectedDataRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CollectedData>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.GetAllAsync(cancellationToken);
        }
    }
}
