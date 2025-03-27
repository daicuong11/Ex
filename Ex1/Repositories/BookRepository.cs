using Ex1.Common.Requests.Book;
using Ex1.Interfaces;
using Ex1.Models;
using Microsoft.EntityFrameworkCore;

namespace Ex1.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly EX1Context _context;

        public BookRepository(EX1Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            var bookDtos = await _context.BookDTOs
                .FromSqlRaw("EXEC GetAllBooks")
                .AsNoTracking() 
                .ToListAsync(); 

            return bookDtos.Select(dto => new Book
            {
                BookId = dto.BookId,
                Title = dto.Title,
                Price = dto.Price,
                PublishedDate = dto.PublishedDate,
                AuthorId = dto.AuthorId,
                Author = dto.AuthorId.HasValue ? new Author
                {
                    AuthorId = dto.AuthorId.Value,
                    Name = dto.AuthorName,
                    Bio = dto.AuthorBio
                } : null
            }).ToList();
        }


        public async Task<Book?> GetBookByIdAsync(int bookId)
        {
            var books = await _context.BookDTOs
                .FromSqlInterpolated($"EXEC GetBookById {bookId}")
                .AsNoTracking() 
                .ToListAsync(); 

            var dto = books.FirstOrDefault(); 

            if (dto == null) return null;

            return new Book
            {
                BookId = bookId,
                Title = dto.Title,
                Price = dto.Price,
                PublishedDate = dto.PublishedDate,
                AuthorId = dto.AuthorId,
                Author = dto.AuthorId.HasValue ? new Author
                {
                    AuthorId = dto.AuthorId.Value,
                    Name = dto.AuthorName,
                    Bio = dto.AuthorBio
                } : null
            };
        }


        public async Task<int> InsertBookAsync(Book newBook)
        {
            var createdBookEntity = await _context.Books.AddAsync(newBook);
            await _context.SaveChangesAsync();
            return createdBookEntity.Entity.BookId;
        }

        public async Task<bool> UpdateBookAsync(UpdateBookRequest updateBookReq)
        {
            var book = await _context.Books.FindAsync(updateBookReq.BookId);
            if (book == null) return false;

            var authorExists = await _context.Authors.AnyAsync(a => a.AuthorId == updateBookReq.AuthorId);
            if (!authorExists) return false;

            book.Title = updateBookReq.Title;
            book.Price = updateBookReq.Price;
            book.AuthorId = updateBookReq.AuthorId;
            book.PublishedDate = updateBookReq.PublishedDate;

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteBookAsync(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
