using System.ComponentModel.DataAnnotations;

namespace SupplyChain.DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email can not be blank")]
        [EmailAddress(ErrorMessage = "email should be in proper email address format")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password can not be blank")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
