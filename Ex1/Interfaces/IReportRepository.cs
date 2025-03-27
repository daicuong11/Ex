using Ex1.Common.Responses;
using Ex1.Models;

namespace Ex1.Interfaces
{
    public interface IReportRepository
    {
        Task<PaginationResponse<IEnumerable<Book>>> GetBooksByFilter(
            string searchKey,
            int? authorId,
            DateTime? fromPublishedDate,
            DateTime? toPublishedDate,
            int pageSize = 10,
            int pageIndex = 1);
    }
}
