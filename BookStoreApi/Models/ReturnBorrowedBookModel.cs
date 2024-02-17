using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreApi.Models
{
    public class ReturnBorrowedBookModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public int BookId { get; set; }

        public string? Notes { get; set; }
    }
}
