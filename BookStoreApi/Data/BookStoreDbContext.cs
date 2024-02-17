using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BookStoreApi.Data
{
    public class BookStoreDbContext : IdentityDbContext<ApplicationUser>
	{
		public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options) : base(options) { }
		
		public DbSet<Book> Books { get; set;}
		public DbSet<BorrowedBook> BorrowedBooks { get; set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

  //      protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
		//{
		//	configurationBuilder.Properties<DateOnly>().HaveConversion<DateOnlyConverter>().HaveColumnType("date");
		//}

		//public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
		//{
		//	/// <summary>
		//	/// Creates a new instance of this converter.
		//	/// </summary>
		//	public DateOnlyConverter() : base(
		//			d => d.ToDateTime(TimeOnly.MinValue),
		//			d => DateOnly.FromDateTime(d))
		//	{ }
		//}
	}
}
