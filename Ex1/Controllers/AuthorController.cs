using Ex1.Common.Requests.Author;
using Ex1.Common.Responses;
using Ex1.Filters;
using Ex1.Interfaces;
using Ex1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ex1.Controllers
{
    [Route("api/authors")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        [HttpGet("fetch")]
        public async Task<ActionResult> GetAuthors()
        {
            var authors = await _authorRepository.GetAllAuthorsAsync();
            var response = new DataResponse<IEnumerable<Author>>
            {
                IsSuccess = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "All authors",
                Data = authors
            };
            return Ok(response);
        }

        [HttpGet("fetch/{AuthorId}")]
        public async Task<ActionResult> GetAuthorById([FromRoute] int AuthorId)
        {
            var author = await _authorRepository.GetAuthorByIdAsync(AuthorId);
            if (author == null)
            {
                return NotFound(new BaseResponse
                {
                    IsSuccess = false,
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    Message = "Author not found"
                });
            }

            var response = new DataResponse<Author>
            {
                IsSuccess = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "Author by id " + AuthorId,
                Data = author
            };
            return Ok(response);
        }

        [HttpPost("insert")]
        [ServiceFilter(typeof(InputValidationFilter))]
        public async Task<ActionResult> CreateAuthor([FromBody] CreateAuthorRequest request)
        {
            var newAuthor = new Author
            {
                Name = request.Name,
                Bio = request.Bio
            };

            var authorId = await _authorRepository.InsertAuthorAsync(newAuthor);

            return CreatedAtAction(nameof(GetAuthorById), new { authorId }, new BaseResponse
            {
                IsSuccess = true,
                StatusCode = System.Net.HttpStatusCode.Created,
                Message = "Book created successfully"
            });
        }

        [HttpPut("update")]
        [ServiceFilter(typeof(InputValidationFilter))]
        public async Task<ActionResult> UpdateAuthor([FromBody] UpdateAuthorRequest request)
        {
            var result = await _authorRepository.UpdateAuthorAsync(request);
            if (!result)
            {
                return NotFound(new BaseResponse
                {
                    IsSuccess = false,
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    Message = "Author not found"
                });
            }

            return Ok(new BaseResponse
            {
                IsSuccess = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "Author updated successfully"
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuthor([FromRoute] int id)
        {
            var result = await _authorRepository.DeleteAuthorAsync(id);
            if (!result)
            {
                return NotFound(new BaseResponse
                {
                    IsSuccess = false,
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    Message = "Author not found"
                });
            }

            return Ok(new BaseResponse
            {
                IsSuccess = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = "Author deleted successfully"
            });
        }
    }
}
