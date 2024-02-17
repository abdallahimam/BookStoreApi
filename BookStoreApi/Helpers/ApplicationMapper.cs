using AutoMapper;
using BookStoreApi.Data;
using BookStoreApi.Models;

namespace BookStoreApi.Helpers
{
	public class ApplicationMapper : Profile
	{
		public ApplicationMapper() 
		{ 
			CreateMap<Book, BookModel>().ReverseMap();

            CreateMap<AddBorrowedBookModel, BorrowedBook>();

            CreateMap<BorrowedBookModel, BorrowedBook>();
			CreateMap<BorrowedBook, BorrowedBookModel>()
				.ForMember(s => s.BookTitle, o => o.MapFrom(d => d.Book.Title))
				.ForMember(s => s.UserName, o => o.MapFrom(d => d.User.Fullname));
			
		}
	}
}
