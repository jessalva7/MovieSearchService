using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using MovieSearchService.Models;

namespace MovieSearchService.Services;

public class SearchService : ISearchService
{
    private readonly ElasticsearchClient _esClient;

    public SearchService(ElasticsearchClient esClient)
    {
        _esClient = esClient;
    }

    public async Task<IReadOnlyCollection<Movie>> SearchAsync(string query)
    {
        var response = await _esClient.SearchAsync<Movie>(s => s
            .Index("movies")
            .Query(q => q
                .Bool(b => b
                    .Should(should => should
                        // 1. Exact match query (highest score)
                        // Matches exact string in title.keyword. Boost 10.
                        .Term(t => t.Field(f => f.Title.Suffix("keyword")).Value(query).Boost(10.0f))

                        // 2. Phrase match query (second highest score)
                        // Matches the phrase in the title. Boost 5.
                        .MatchPhrase(mp => mp.Field(f => f.Title).Query(query).Boost(5.0f))

                        // 3. Term match query (third highest score)
                        // Standard text match (terms) in Title. Boost 1.
                        .Match(m => m.Field(f => f.Title).Query(query).Boost(1.0f))
                    )
                )
            )
        );

        if (!response.IsValidResponse)
        {
            // Log error or throw
            throw new Exception($"Elasticsearch error: {response.DebugInformation}");
        }

        return response.Documents;
    }
}
