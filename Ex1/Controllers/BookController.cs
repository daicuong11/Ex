using Ex1.Common.Requests.Book;
using Ex1.Common.Responses;
using Ex1.Filters;
using Ex1.Interfaces;
using Ex1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ex1.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookRepository bookRepository;

        public BookController(IBookRepository bookRepository)
        {
            this.bookRepository = bookRepository;
        }

        [HttpGet("fetch")]
        public async Task<ActionResult> GetBooks()
        {
            var books = await bookRepository.GetAllBooksAsync();
            var response = new DataResponse<IEnumerable<Book>>
            {
                IsSuccess = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "All books",
                Data = books
            };
            return Ok(response);
        }

        [HttpGet("fetch/{BookId}")]
        public async Task<ActionResult> GetBookById([FromRoute] int BookId)
        {
            var book = await bookRepository.GetBookByIdAsync(BookId);
            if (book == null)
            {
                return NotFound(new BaseResponse
                {
                    IsSuccess = false,
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    Message = "Book not found"
                });
            }
            var response = new DataResponse<Book>
            {
                IsSuccess = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = $"Book with ID {BookId}",
                Data = book
            };
            return Ok(response);
        }

        [HttpPost("insert")]
        [ServiceFilter(typeof(InputValidationFilter))]
        public async Task<ActionResult> CreateBook([FromBody] CreateBookRequest bookReq)
        {
            var newBook = new Book
            {
                Title = bookReq.Title,
                Price = bookReq.Price,
                PublishedDate = bookReq.PublishedDate,
                AuthorId = bookReq.AuthorId,
            };

            var bookId = await bookRepository.InsertBookAsync(newBook);
            if (bookId > 0)
            {
                return CreatedAtAction(nameof(GetBookById), new { bookId }, new BaseResponse
                {
                    IsSuccess = true,
                    StatusCode = System.Net.HttpStatusCode.Created,
                    Message = "Book created successfully"
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
            {
                IsSuccess = false,
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Message = "Error creating book"
            });
        }

        [HttpPut("update")]
        [ServiceFilter(typeof(InputValidationFilter))]
        public async Task<ActionResult> UpdateBook(
            [FromBody] UpdateBookRequest updateBookReq)
        {

            var result = await bookRepository.UpdateBookAsync(updateBookReq);
            if (result)
            {
                return Ok(new BaseResponse
                {
                    IsSuccess = true,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Message = "Book updated successfully"
                });
            }

            return NotFound(new BaseResponse
            {
                IsSuccess = false,
                StatusCode = System.Net.HttpStatusCode.NotFound,
                Message = "Book not found or update failed"
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBook([FromRoute] int id)
        {
            var result = await bookRepository.DeleteBookAsync(id);
            if (result)
            {
                return Ok(new BaseResponse
                {
                    IsSuccess = true,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Message = "Book deleted successfully"
                });
            }

            return NotFound(new BaseResponse
            {
                IsSuccess = false,
                StatusCode = System.Net.HttpStatusCode.NotFound,
                Message = "Book not found or delete failed"
            });
        }
    }
}
