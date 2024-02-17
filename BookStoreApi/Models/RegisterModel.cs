using System.ComponentModel.DataAnnotations;

namespace BookStoreApi.Models
{
	public class RegisterModel
	{

		[Required]
		public string Fullname { get; set; }

		[Required, EmailAddress]
		public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required, DataType(DataType.Password)]
		public string Password { get; set; }
		[Required, DataType(DataType.Password), Compare(nameof(Password))]
		public string ConfirmPassword { get; set; }
	}
}
