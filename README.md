# Movie Search Service

A .NET 10 Web API for searching movies using Elasticsearch.

## Prerequisites

- Docker Desktop
- .NET 10 SDK

## Testing

You can use `curl` or any API client (like Postman) to test the search relevance.

### 1. Verify Indexing
Check if documents are indexed:
```bash
curl -X GET "http://localhost:9200/_cat/indices?v"
```
You should see a `movies` index with docs count around 4803.

### 2. Search Relevance Scenarios

**Scenario A: Exact Match (Highest Boost)**
Searching for "Avatar" should return the movie titled strictly "Avatar" as the top result (due to `title.keyword` boost).
```bash
curl "http://localhost:5000/search?query=Avatar"
```

**Scenario B: Phrase Match (Medium Boost)**
Searching for "Dark Knight" should prioritize "The Dark Knight" or "The Dark Knight Rises" over movies that just have "Dark" or "Knight" scattered in the title.
```bash
curl "http://localhost:5000/search?query=Dark%20Knight"
```

**Scenario C: Partial/Term Match (Lower Boost)**
Searching for "Action" might find movies with "Action" in the title.
```bash
curl "http://localhost:5000/search?query=Action"
```

## Architecture

- **Language**: C# 12 / .NET 10
- **Database**: Elasticsearch 8.15.0
- **Client**: Elastic.Clients.Elasticsearch 8.15.0
- **Flow**: CSV -> IndexingService -> Elasticsearch -> SearchService
- **Search Logic**:
    1. `Exact match` (Term on `title.keyword`) -> **Boost 10**
    2. `Phrase match` (MatchPhrase on `title`) -> **Boost 5**
    3. `Term match` (Match on `title`) -> **Boost 1**
    4. `Overview match` (Match on `overview`) -> **Boost 0.5**
