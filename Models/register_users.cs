using System.ComponentModel.DataAnnotations; // Provides validation attributes 

namespace part_1.Models
{         // Model class used for registering users in the system
    public class register_users
    {

        // User's first name (must be filled in)
        [Required(ErrorMessage = "Name is required")]
        public string name { get; set; }

        // User's last name (must be filled in)
        [Required(ErrorMessage = "Surname is required")]
        public string surname { get; set; }

        // User's email address (must be filled in and in a valid email format)
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string email { get; set; }
        // User's gender (optional field)
        public string gender { get; set; }
       
        // The role assigned to the user (e.g., Admin, Lecturer, Coordinator)
        [Required(ErrorMessage = "Role is required")]
        public string role { get; set; }

        // Confirmation of the password (must match the 'password' field 1)
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string password { get; set; }
        // Confirmation of the password (must match the 'password' field)

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

    }
}
