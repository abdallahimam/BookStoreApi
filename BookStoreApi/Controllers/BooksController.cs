using BookStoreApi.Data;
using BookStoreApi.Errors;
using BookStoreApi.Models;
using BookStoreApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace BookStoreApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = "Admin,User")]
	public class BooksController : ControllerBase
	{
		public readonly IBookService _bookRepository;

		public BooksController(IBookService bookRepository)
		{
			_bookRepository = bookRepository;
		}
        
        [HttpGet("")]
		public async Task<IActionResult> GetBooks()
		{
			var books = await _bookRepository.GetAllAsync();
			return Ok(books);
		}
        
        [HttpGet("{id}")]
		public async Task<IActionResult> GetBookById([FromRoute]int id)
		{
			var book = await _bookRepository.GetByIdAsync(id);
			if (book == null) 
			{ 
				return BadRequest(new ApiExceptionError(404, "Book not exist")); 
			}
			return Ok(book);
		}

        [Authorize(Roles = "Admin")]
        [HttpPost("")]
		public async Task<IActionResult> AddBook([FromBody]BookModel book)
		{
            try
            {
                var result = await _bookRepository.AddBookAsync(book);
				if (!result.IsSuccessed)
					return BadRequest(new ApiExceptionError(400, result.Message));

                return Ok(result.Message);
            }
            catch (Exception)
            {
                return new ObjectResult(new ApiExceptionError(500, "Something went wrong"));
            }
            
		}

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
		public async Task<IActionResult> UpdateBook([FromRoute] int id, [FromBody] BookModel book)
		{
            try
            {
                var result = await _bookRepository.UpdateBookAsync(id, book);

                if (!result.IsSuccessed)
                    return BadRequest(new ApiExceptionError(400, result.Message));

                return Ok(result.Message);
            }
            catch (Exception)
            {
                return new ObjectResult(new ApiExceptionError(500, "Something went wrong"));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
		public async Task<IActionResult> DeleteBookAsync([FromRoute] int id)
		{
			try
			{
                var result = await _bookRepository.DeleteBookAsync(id);

                if (!result.IsSuccessed)
                    return BadRequest(new ApiExceptionError(400, result.Message));

                return Ok(result.Message);
            }
			catch (Exception)
			{
                return new ObjectResult(new ApiExceptionError(500, "Something went wrong"));
            }
		}

        [HttpPost("BorrowBook")]
        public async Task<IActionResult> BorrowBookAsync(AddBorrowedBookModel model)
		{
			try
			{
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var result = await _bookRepository.BorrowBookAsync(model);

                if (!result.IsSuccessed)
                    return BadRequest(new ApiExceptionError(400, result.Message));

                return Ok(result.Message);
            }
			catch (Exception)
			{
                return new ObjectResult(new ApiExceptionError(500, "Something went wrong"));
            }
			
		}

        [HttpPost("ReturnBook")]
		public async Task<IActionResult> ReturnBorrowedBookAsync(ReturnBorrowedBookModel model)
		{
			try
			{
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var result = await _bookRepository.ReturnBorrowedBookAsync(model);

                if (!result.IsSuccessed)
                    return BadRequest(new ApiExceptionError(400, result.Message));

                return Ok(result.Message);
            }
			catch (Exception)
			{
                return new ObjectResult(new ApiExceptionError(500, "Something went wrong"));
            }
        }

        [HttpGet("UserBorrowedHistory")]
		public async Task<IActionResult> GetUserBorrowedBooksAsync(string userId)
		{
			var result = await _bookRepository.GetUserBorrowedBooksAsync(userId);

			return Ok(result);
		}

        [HttpGet("BookBorrowedHistory")]
		public async Task<IActionResult> GetBorrowedBooksForSpecificBookAsync(int bookId)
		{
            var result = await _bookRepository.GetBorrowedBooksForSpecificBookAsync(bookId);

            return Ok(result);
        }

	}
}
