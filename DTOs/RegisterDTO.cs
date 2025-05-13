using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SupplyChain.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Name can't be blank")]
        public string PersonName { get; set; }


        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email should be in a proper email adddess format")]
        [Remote(action: "IsEmailAlreadyRegistered", controller: "Account", ErrorMessage = "Email is already in use")]  //just after enterning mail in filed it will show error if exited mail or not before submitng
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number can't be blank")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number should contain only numbers")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Password can't be blank")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "ConfirmPassword can't be blank")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirm password don not match ")]

        public string ConfirmPassword { get; set; }
    }
}
