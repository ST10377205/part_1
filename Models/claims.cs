using System.ComponentModel.DataAnnotations;

namespace part_1.Models
{
    public class claims
    {

        [Required(ErrorMessage = "Faculty name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Module name is required")]
        public string NameModule { get; set; }

        [Required(ErrorMessage = "Number of sessions is required")]
        public int Sessions { get; set; }

        [Required(ErrorMessage = "Hours are required")]
        public int Hours { get; set; }

        [Required(ErrorMessage = "Rate is required")]
        [Range(0, 500, ErrorMessage = "Rate must be between 0 and 500")]
        public decimal Rate { get; set; }
        public string? SupportingDocumentPath { get; set; } 
        public string? ClaimStatus { get; set; }
        public DateTime CreatingDate { get; set; }
        public int LecturerID { get; set; }
        public int ClaimID { get; set; }

        public decimal TotalAmount { get; set; }

        // Optional file upload
        public IFormFile? SupportingDocument { get; set; }

    }
}
