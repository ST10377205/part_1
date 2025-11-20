using System.ComponentModel.DataAnnotations;

namespace part_1.Models
{                                                                      
                                        /// Model for user registration with validation attributes
                                       
    public class register_users      
    {
        [Required(ErrorMessage = "Name is required")]     
        public string name { get; set; }     /// User's first name

        [Required(ErrorMessage = "Surname is required")]
        public string surname { get; set; }    // User's surname

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string email { get; set; }
        public string gender { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string role { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

    }
}
