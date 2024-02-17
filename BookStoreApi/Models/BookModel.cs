using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BookStoreApi.Models
{
	public class BookModel
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
