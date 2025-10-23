using System.ComponentModel.DataAnnotations;

namespace part_1.Models
{
    public class login
    {
        //property for  emails

        [Required]
        public string email { get; set; }
        //propert for  passwords 
        [Required]
        public string password {  get; set; }
        //property for  roles 
        [Required]
        public string role { get; set; }



        

    }
}
