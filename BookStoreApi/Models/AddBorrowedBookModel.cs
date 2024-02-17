using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BookStoreApi.Models
{
    public class AddBorrowedBookModel : IValidatableObject
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required, DataType(DataType.DateTime)]
        public DateTime FromDate { get; set; }

        [Required, DataType(DataType.DateTime)]
        public DateTime DueDate { get; set; }

        public string? Notes { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            if (DueDate < FromDate)
            {
                errors.Add(new ValidationResult($"{nameof(DueDate)} needs to be greater than From date.", new List<string> { nameof(DueDate) }));
            }
            return errors;
        }
    }
}
