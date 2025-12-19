namespace Server.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Publisher { get; set; }
        public decimal? Price { get; set; }
        public string? PublishMonth { get; set; }
        public int? PublishYear { get; set; }
        public int AuthorId { get; set; }
        public string? AuthorName { get; set; }
    }
}