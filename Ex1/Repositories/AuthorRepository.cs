using Ex1.Common.Requests.Author;
using Ex1.Interfaces;
using Ex1.Models;
using Microsoft.EntityFrameworkCore;

namespace Ex1.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly EX1Context _context;

        public AuthorRepository(EX1Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
        {
            return await _context.Authors.FromSqlRaw("EXEC GetAllAuthors").ToListAsync();
        }

        public async Task<Author?> GetAuthorByIdAsync(int authorId)
        {
            var authors = await _context.Authors.FromSqlInterpolated($"EXEC GetAuthorById {authorId}").ToListAsync();
            return authors.FirstOrDefault();
        }

        public async Task<int> InsertAuthorAsync(Author newAuthor) 
        {
            var createdAuthorEntity = await _context.Authors.AddAsync(newAuthor);
            await _context.SaveChangesAsync();
            return createdAuthorEntity.Entity.AuthorId;
        }

        public async Task<bool> UpdateAuthorAsync(UpdateAuthorRequest updateAuthorRequest)
        {
            var author = await _context.Authors.FindAsync(updateAuthorRequest.AuthorId);
            if (author == null) return false;

            author.Name = updateAuthorRequest.Name;
            author.Bio = updateAuthorRequest.Bio;

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteAuthorAsync(int authorId)
        {
            var author = await _context.Authors.FindAsync(authorId);
            if (author == null) return false;

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
