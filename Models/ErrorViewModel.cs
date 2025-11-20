namespace part_1.Models
{
    public class ErrorViewModel   // Model for error view
    {
        public string? RequestId { get; set; }   // Unique identifier for the request

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);  // Determines if RequestId should be shown
    }
}
