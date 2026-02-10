using System.Globalization;
using CsvHelper;
using Elastic.Clients.Elasticsearch;
using MovieSearchService.Models;

namespace MovieSearchService.Services;

public class IndexingService : IIndexingService
{
    private readonly ElasticsearchClient _esClient;

    public IndexingService(ElasticsearchClient esClient)
    {
        _esClient = esClient;
    }

    public async Task<int> IndexMoviesAsync(string csvPath)
    {
        if (!File.Exists(csvPath)) return -1; // Or throw exception

        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<Movie>().ToList();

        if (!records.Any()) return 0;

        // Ensure index exists with mappings
        var indexResponse = await _esClient.Indices.ExistsAsync("movies");
        if (!indexResponse.Exists)
        {
            await _esClient.Indices.CreateAsync("movies", c => c
                .Mappings(m => m
                    .Properties<Movie>(p => p
                        .Text(t => t.Title, t => t.Fields(f => f.Keyword("keyword"))) // Title.keyword
                        .Text(t => t.Overview)
                        .Text(t => t.Keywords)
                        .Keyword(t => t.Genres)
                    )
                )
            );
        }

        // Bulk Index in chunks
        var chunks = records.Chunk(1000);
        foreach (var chunk in chunks)
        {
            var bulkResponse = await _esClient.BulkAsync(b => b
                .Index("movies")
                .IndexMany(chunk)
            );
            
            if (bulkResponse.Errors)
            {
                // In production, handle errors
                Console.WriteLine("Errors occurred during bulk indexing");
            }
        }

        return records.Count;
    }
}
