using CsvHelper.Configuration.Attributes;

namespace Server.Data
{
    public class BookCSV
    {
        [Name("Title")]
        public string Title { get; set; } = string.Empty;

        [Name("Authors")]
        public string Authors { get; set; } = string.Empty;

        [Name("Description")]
        public string? Description { get; set; }

        [Name("Category")]
        public string? Category { get; set; }

        [Name("Publisher")]
        public string? Publisher { get; set; }

        [Name("Price Starting With ($)")]
        public decimal? Price { get; set; }

        [Name("Publish Date (Month)")]
        public string? PublishMonth { get; set; }

        [Name("Publish Date (Year)")]
        public int? PublishYear { get; set; }
    }
}