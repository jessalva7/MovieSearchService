using System.Globalization;
using CsvHelper;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using MovieSearchService.Models;
using MovieSearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Elasticsearch Configuration
var settings = new ElasticsearchClientSettings(new Uri("http://localhost:9200"))
    .DefaultIndex("movies")
    .DisableDirectStreaming(); 

var client = new ElasticsearchClient(settings);
builder.Services.AddSingleton(client);

// Register Services
builder.Services.AddScoped<IIndexingService, IndexingService>();
builder.Services.AddScoped<ISearchService, SearchService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Indexing Endpoint
app.MapPost("/index-movies", async (IIndexingService indexingService) =>
{
    var csvPath = Path.Combine(app.Environment.ContentRootPath, "dataset", "movies.csv");
    var count = await indexingService.IndexMoviesAsync(csvPath);

    if (count == -1) return Results.NotFound("movies.csv not found");
    return Results.Ok($"{count} movies indexed.");
});

// Search Endpoint
app.MapGet("/search", async (string query, ISearchService searchService) =>
{
    if (string.IsNullOrWhiteSpace(query)) return Results.BadRequest("Query is empty");

    try 
    {
        var movies = await searchService.SearchAsync(query);
        return Results.Ok(movies);
    }
    catch (Exception ex)
    {
        return Results.Json(new { error = ex.Message }, statusCode: 500);
    }
});

app.Run();
