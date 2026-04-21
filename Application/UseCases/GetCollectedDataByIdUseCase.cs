using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public class GetCollectedDataByIdUseCase
    {
        private readonly ICollectedDataRepository _repository;

        public GetCollectedDataByIdUseCase(ICollectedDataRepository repository)
        {
            _repository = repository;
        }

        public async Task<CollectedData?> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _repository.GetByIdAsync(id, cancellationToken);
        }
    }
}
