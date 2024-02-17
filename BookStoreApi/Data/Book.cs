using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreApi.Data
{
	public class Book
	{
		public int Id { get; set; }
		public string Title { get; set; }

		[StringLength(20)]
		public string AuthorName { get; set; }
		public int? Pages { get; set; }
		public int? Edition { get; set; }
		public int? Publication { get; set; }
		public string? Description { get; set; }
		public bool? IsBorrowed { get; set; } = false;

    }
}
