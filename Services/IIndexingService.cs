namespace MovieSearchService.Services;

public interface IIndexingService
{
    Task<int> IndexMoviesAsync(string csvPath);
}
