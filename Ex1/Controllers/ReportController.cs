using Ex1.Common.Responses;
using Ex1.Interfaces;
using Ex1.Models;
using Ex1.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ex1.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportRepository reportRepository;

        public ReportController(IReportRepository reportRepository)
        {
            this.reportRepository = reportRepository;
        }   

        [HttpGet("book")]
        public async Task<ActionResult> GetBooksByFilter(
            [FromQuery] string? searchKey,
            [FromQuery] int? authorId,
            [FromQuery] DateTime? fromPublishedDate,
            [FromQuery] DateTime? toPublishedDate,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 1)
        {
            var response = await reportRepository.GetBooksByFilter(searchKey, authorId, fromPublishedDate, toPublishedDate, pageSize, pageIndex);
            
            return Ok(response);
        }
    }
}
