using Ex1.Common.Requests.Book;
using Ex1.Models;

namespace Ex1.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int bookId);
        Task<int> InsertBookAsync(Book newBook);
        Task<bool> UpdateBookAsync(UpdateBookRequest updateBookReq);
        Task<bool> DeleteBookAsync(int bookId);
    }
}
