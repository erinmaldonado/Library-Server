namespace LibraryModel
{
    public enum LoanStatus
    {
        Active,
        Returned,
        Overdue
    }

    public class Loan
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }
        public string UserId { get; set; } = string.Empty;
        public LibraryModelUser? User { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public LoanStatus Status { get; set; } = LoanStatus.Active;
    }
}