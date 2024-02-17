using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BookStoreApi.Models
{
    public class BorrowedBookModel
    {
        
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        
        public string? UserName { get; set; } = null;

        [Required]
        public int BookId { get; set; }

        
        public string? BookTitle { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        
        public DateTime? ReturnDate { get; set; } = null;

        public string? Notes { get; set; }
    }
}
