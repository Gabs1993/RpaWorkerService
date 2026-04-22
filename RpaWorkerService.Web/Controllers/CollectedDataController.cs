using Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace RpaWorkerService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CollectedDataController : ControllerBase
    {
        private readonly GetCollectedDataUseCase _getCollectedDataUseCase;
        private readonly GetCollectedDataByIdUseCase _getCollectedDataByIdUseCase;

        public CollectedDataController(
            GetCollectedDataUseCase getCollectedDataUseCase,
            GetCollectedDataByIdUseCase getCollectedDataByIdUseCase)
        {
            _getCollectedDataUseCase = getCollectedDataUseCase;
            _getCollectedDataByIdUseCase = getCollectedDataByIdUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _getCollectedDataUseCase.ExecuteAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _getCollectedDataByIdUseCase.ExecuteAsync(id, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }
    }
}
