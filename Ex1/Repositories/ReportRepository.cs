using Ex1.Common.Responses;
using Ex1.DTOs;
using Ex1.Interfaces;
using Ex1.Models;
using Microsoft.EntityFrameworkCore;

namespace Ex1.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly EX1Context _context;
        public ReportRepository(EX1Context context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<IEnumerable<Book>>> GetBooksByFilter(
            string? searchKey,
            int? authorId,
            DateTime? fromPublishedDate,
            DateTime? toPublishedDate,
            int pageSize = 10,
            int pageIndex = 1)
        {
            var booksWithAuthors = await _context.BookWithTotalCounts
                .FromSqlRaw(
                    "EXEC GetBooksByFilter @p0, @p1, @p2, @p3, @p4, @p5",
                    searchKey ?? (object)DBNull.Value,
                    authorId ?? (object)DBNull.Value,
                    fromPublishedDate ?? (object)DBNull.Value,
                    toPublishedDate ?? (object)DBNull.Value,
                    pageSize,
                    pageIndex
                )
                .ToListAsync();

            int totalCount = booksWithAuthors.FirstOrDefault()?.TotalCount ?? 0;
            int totalPages = Math.Max(1, (int)Math.Ceiling((double)totalCount / pageSize));

            var books = booksWithAuthors.Select(b => new Book
            {
                BookId = b.BookId,
                Title = b.Title,
                Price = b.Price,
                PublishedDate = b.PublishedDate,
                AuthorId = b.AuthorId,
                Author = !string.IsNullOrEmpty(b.AuthorName) ? new Author
                {
                    AuthorId = b.AuthorId ?? 0,
                    Name = b.AuthorName,
                    Bio = b.AuthorBio
                } : null
            });

            return new PaginationResponse<IEnumerable<Book>>
            {
                IsSuccess = true,
                Message = "All books with filters",
                StatusCode = System.Net.HttpStatusCode.OK,
                Data = books,
                Pagination = new Pagination
                {
                    Page = pageIndex,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                }
            };
        }

    }
}
