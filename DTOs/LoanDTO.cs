namespace Server.DTOs
{
    public class CreateLoanRequest
    {
        public int BookId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
    }

    public class ReturnLoanRequest
    {
        public DateTime ReturnDate { get; set; } = DateTime.UtcNow;
    }

    public class LoanResponse
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}