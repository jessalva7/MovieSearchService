using MovieSearchService.Models;

namespace MovieSearchService.Services;

public interface ISearchService
{
    Task<IReadOnlyCollection<Movie>> SearchAsync(string query);
}
