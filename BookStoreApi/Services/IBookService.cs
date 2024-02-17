using BookStoreApi.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace BookStoreApi.Repositories
{
	public interface IBookService
	{
		Task<IEnumerable<BookModel>> GetAllAsync();
		Task<BookModel> GetByIdAsync(int id);
		Task<BookResultModel> AddBookAsync(BookModel model);
		Task<BookResultModel> UpdateBookAsync(int id, BookModel bookModel);
        //Task<int> UpdateBookStatusAsync(int id, bool status);
        //Task<int> UpdateBookPatchAsync(int id, JsonPatchDocument bookModel);
		Task<BookResultModel> DeleteBookAsync(int id);

		Task<BookResultModel> BorrowBookAsync(AddBorrowedBookModel model);
		Task<BookResultModel> ReturnBorrowedBookAsync(ReturnBorrowedBookModel model);
		Task<List<BorrowedBookModel>> GetUserBorrowedBooksAsync(string userId);
		Task<List<BorrowedBookModel>> GetBorrowedBooksForSpecificBookAsync(int bookId);

    }
}
