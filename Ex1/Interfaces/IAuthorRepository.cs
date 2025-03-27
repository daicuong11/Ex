using Ex1.Common.Requests.Author;
using Ex1.Models;

namespace Ex1.Interfaces
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<Author>> GetAllAuthorsAsync();
        Task<Author?> GetAuthorByIdAsync(int authorId);
        Task<int> InsertAuthorAsync(Author newAuthor);
        Task<bool> UpdateAuthorAsync(UpdateAuthorRequest updateAuthorRequest);
        Task<bool> DeleteAuthorAsync(int authorId);
    }
}
