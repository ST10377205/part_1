using System.ComponentModel.DataAnnotations;

namespace part_1.Models
{
    public class login
    {


        [Required]
        public string email { get; set; }

        [Required]
        public string password {  get; set; }

        [Required]
        public string role { get; set; }



        

    }
}
