using CarStatsApp.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarStatsApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarStatsController : ControllerBase
    {
        private readonly ICarStatService _service;

        public CarStatsController(ICarStatService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetCarStats(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int? speedFrom = null, [FromQuery] int? speedTo = null,
        [FromQuery] DateTime? dateFrom = null, [FromQuery] DateTime? dateTo = null)
        {
            var cancellationToken = HttpContext.RequestAborted;
            var (carStats, totalRecords) = await _service.GetFilteredCarStatsAsync(pageNumber, speedFrom, speedTo, dateFrom, dateTo, cancellationToken);
            var totalPages = (int)Math.Ceiling((double)totalRecords / _service.GetRecordsPerPage());

            var response = new
            {
                Data = carStats,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                RecordsPerPage = _service.GetRecordsPerPage(),
                TotalRecords = totalRecords
            };

            return Ok(response);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file.Length == 0)
            {
                return BadRequest("File is empty.");
            }

            try
            {
                await _service.AddRangeAsync(file, HttpContext.RequestAborted);

                return Ok("Data successfully uploaded and parsed.");
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, "Operation cancelled by the user.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("clear")]
        public async Task<IActionResult> ClearData()
        {
            try
            {
                await _service.ClearAllAsync(HttpContext.RequestAborted);
                return Ok("All records have been deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("average-speed/{date}")]
        public async Task<IActionResult> GetAverageSpeedByDay(DateTime date)
        {
            try
            {
                var averageSpeedData = await _service.GetAverageSpeedsByDayAsync(date, HttpContext.RequestAborted);
                if (averageSpeedData == null || !averageSpeedData.Any())
                {
                    return NotFound("No data available for the selected date.");
                }

                return Ok(averageSpeedData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}