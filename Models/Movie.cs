using CsvHelper.Configuration.Attributes;

namespace MovieSearchService.Models
{
    public class Movie
    {
        [Name("id")]
        public string? Id { get; set; }

        [Name("title")]
        public string? Title { get; set; }

        [Name("overview")]
        public string? Overview { get; set; }

        [Name("keywords")]
        public string? Keywords { get; set; }
        
        [Name("genres")]
        public string? Genres { get; set; }
    }
}
