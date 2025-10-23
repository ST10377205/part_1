using System.ComponentModel.DataAnnotations;

namespace part_1.Models
{
    public class login
    {
        //property for  email 

        [Required]
        public string email { get; set; }
        //propert for  password 
        [Required]
        public string password {  get; set; }
        //property for  role 
        [Required]
        public string role { get; set; }



        

    }
}
