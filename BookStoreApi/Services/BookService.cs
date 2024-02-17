using AutoMapper;
using BookStoreApi.Data;
using BookStoreApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApi.Repositories
{
	public class BookService : IBookService
	{
		private readonly BookStoreDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
		private readonly IMapper _mapper;

		public BookService(BookStoreDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager) {
			_context = context;
			_mapper = mapper;
            _userManager = userManager;
		}

		public async Task<IEnumerable<BookModel>> GetAllAsync()
		{
			var records = await _context.Books.AsNoTracking().ToListAsync();
			return _mapper.Map<List<BookModel>>(records);
		}

		public async Task<BookModel> GetByIdAsync(int id)
		{
			var book = await _context.Books.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
			return _mapper.Map<BookModel>(book);
		}

		public async Task<BookResultModel> AddBookAsync(BookModel model)
		{
            var bookResult = new BookResultModel();
            try
            {
                var book = _mapper.Map<Book>(model);

                await _context.Books.AddAsync(book);
                await _context.SaveChangesAsync();

                bookResult.IsSuccessed = true;
                bookResult.Message = "Book added successfully";
                return bookResult;
            }
            catch (Exception)
            {
                bookResult.IsSuccessed = false;
                bookResult.Message = "Failed to add book";
                return bookResult;
            }
            
		}

		public async Task<BookResultModel> UpdateBookAsync(int id, BookModel bookModel)
		{
            var bookResult = new BookResultModel();
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    bookResult.IsSuccessed = false;
                    bookResult.Message = "Failed, Book not exist";
                    return bookResult;
                }

				// Update properities
                book.Title = bookModel.Title != null ? bookModel.Title : book.Title;
                book.Description = bookModel.Description != null ? bookModel.Description : book.Description;
                book.AuthorName = bookModel.AuthorName != null ? bookModel.AuthorName : book.AuthorName;
                book.Pages = bookModel.Pages != null ? bookModel.Pages : book.Pages;
                book.Edition = bookModel.Edition != null ? bookModel.Edition : book.Edition;
                book.IsBorrowed = bookModel.IsBorrowed != null ? bookModel.IsBorrowed : book.IsBorrowed;
                
                await _context.SaveChangesAsync();

                bookResult.IsSuccessed = true;
                bookResult.Message = "Book updated successfully";
                return bookResult;
            }
            catch (Exception)
            {
                bookResult.IsSuccessed = false;
                bookResult.Message = "Failed to add book";
                return bookResult;
            }
		}

        //private async Task<int> UpdateBookStatusAsync(int id, bool status)
        //{
        //    try
        //    {
        //        var book = await _context.Books.FindAsync(id);
        //        if (book == null) return 0; // book not exist

        //        book.IsBorrowed = status;
        //        await _context.SaveChangesAsync();

        //        return 1;
        //    }
        //    catch (Exception)
        //    {
        //        // internal server error
        //        return 0;
        //    }
        //}

        //public async Task<int> UpdateBookPatchAsync(int id, JsonPatchDocument bookModel)
        //{
        //    try
        //    {
        //        var book = await _context.Books.FindAsync(id);
        //        if (book == null) return 0; // book not exist

        //        bookModel.ApplyTo(book);
        //        await _context.SaveChangesAsync();
        //        return 1;
        //    }
        //    catch (Exception)
        //    {
        //        // internal server error
        //        return 0;
        //    }
        //}

        public async Task<BookResultModel> DeleteBookAsync(int id)
		{
            var bookResult = new BookResultModel();
            try
			{
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    bookResult.IsSuccessed = false;
                    bookResult.Message = "Failed, Book not exist";
                    return bookResult;
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                bookResult.IsSuccessed = true;
                bookResult.Message = "Book deleted successfully";
                return bookResult;
            }
			catch (Exception ex)
			{
                bookResult.IsSuccessed = false;
                bookResult.Message = "Failed to delete book";
                return bookResult;
            }
            
		}

        public async Task<BookResultModel> BorrowBookAsync(AddBorrowedBookModel model)
        {
            var bookResult = new BookResultModel();
            try
            {
                var book = await _context.Books.FindAsync(model.BookId);
                if (book == null)
                {
                    bookResult.IsSuccessed = false;
                    bookResult.Message = "Failed, Book not exist";
                    return bookResult;
                }

                if (book.IsBorrowed == true)
                {
                    bookResult.IsSuccessed = false;
                    bookResult.Message = "Failed, Book already borrowed by someone";
                    return bookResult;
                }

                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    bookResult.IsSuccessed = false;
                    bookResult.Message = "Failed, User not exist";
                    return bookResult;
                }

                var borrowedBookOld = await _context.BorrowedBooks.OrderByDescending(x => x.FromDate)
                    .FirstOrDefaultAsync(
                        b => b.UserId == model.UserId &&
                        b.BookId == model.BookId);

                if (borrowedBookOld != null)
                {
                    if (model.FromDate < borrowedBookOld.ReturnDate)
                    {
                        bookResult.IsSuccessed = false;
                        bookResult.Message = "Failed, Invalid Start Date";
                        return bookResult;
                    }
                }

                var borrowedBook = _mapper.Map<BorrowedBook>(model);

                await _context.BorrowedBooks.AddAsync(borrowedBook);

                // Update status of the book to borrowed
                book.IsBorrowed = true;

                await _context.SaveChangesAsync();

                bookResult.IsSuccessed = true;
                bookResult.Message = "Book borrowed successfully";
                return bookResult;
            }
            catch (Exception ex)
            {
                bookResult.IsSuccessed = true;
                bookResult.Message = "Failed, Something went wrong";
                return bookResult;
            }
        }

        public async Task<BookResultModel> ReturnBorrowedBookAsync(ReturnBorrowedBookModel model)
        {
            var bookResult = new BookResultModel();
            try
            {
                var book = await _context.Books.FindAsync(model.BookId);
                if (book == null)
                {
                    bookResult.IsSuccessed = false;
                    bookResult.Message = "Failed, Book not exist";
                    return bookResult;
                }

                if (book.IsBorrowed == false)
                {
                    bookResult.IsSuccessed = false;
                    bookResult.Message = "Failed, Book not borrowed";
                    return bookResult;
                }

                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    bookResult.IsSuccessed = false;
                    bookResult.Message = "Failed, User not exist";
                    return bookResult;
                }

                var borrowedBook = await _context.BorrowedBooks.OrderByDescending(x => x.FromDate)
                    .FirstOrDefaultAsync(
                        b => b.UserId == model.UserId && 
                        b.BookId == model.BookId);
                
                if (borrowedBook == null)
                {
                    bookResult.IsSuccessed = false;
                    bookResult.Message = "Failed, Book not borrowed by this user";
                    return bookResult;
                }

                var returnDate = DateTime.UtcNow;

                if (returnDate < borrowedBook.FromDate)
                {
                    bookResult.IsSuccessed = false;
                    bookResult.Message = "Failed, Return date not valid";
                    return bookResult;
                }

                borrowedBook.ReturnDate = returnDate;

                // Update status of the book to borrowed
                book.IsBorrowed = false;

                await _context.SaveChangesAsync();

                bookResult.IsSuccessed = true;
                bookResult.Message = "Book returned successfully";
                return bookResult;
            }
            catch (Exception ex)
            {
                bookResult.IsSuccessed = true;
                bookResult.Message = "Failed, Something went wrong";
                return bookResult;
            }
        }

        public async Task<List<BorrowedBookModel>> GetUserBorrowedBooksAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var borrowedBooks = await _context.BorrowedBooks.Where(x => x.UserId == userId).Include(b => b.Book).Include(u => u.User).ToListAsync();

            return _mapper.Map<List<BorrowedBookModel>>(borrowedBooks);
        }

        public async Task<List<BorrowedBookModel>> GetBorrowedBooksForSpecificBookAsync(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) return null;

            var borrowedBooks = await _context.BorrowedBooks.Where(x => x.BookId == bookId).Include(b => b.Book).Include(u => u.User).ToListAsync();

            return _mapper.Map<List<BorrowedBookModel>>(borrowedBooks);
        }
    }
}
