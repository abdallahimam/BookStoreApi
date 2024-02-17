using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreApi.Data
{
    public class BorrowedBook
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Book Book { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; } = null;
        public string? Notes { get; set; }
    }
}
