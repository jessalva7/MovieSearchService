# Movie Search Service

A .NET 10 Web API for searching movies using Elasticsearch.

## Prerequisites

- Docker Desktop
- .NET 10 SDK

## Setup

1. **Start Elasticsearch and Kibana**:
   ```bash
   docker compose up -d
   ```
   Wait for a minute for the services to become healthy. You can check http://localhost:5601 for Kibana.

2. **Run the API**:
   ```bash
   dotnet run
   ```

3. **Index Data**:
   The `dataset/movies.csv` file needs to be indexed into Elasticsearch.
   ```bash
   curl -X POST http://localhost:5000/index-movies
   ```
   *Note: This might take a moment to index ~4,800 movies.*

4. **Search**:
   ```bash
   # Exact match boost example
   curl "http://localhost:5000/search?query=Avatar"
   ```

## Architecture

- **Language**: C# 12 / .NET 10
- **Database**: Elasticsearch 8.15.0
- **Client**: Elastic.Clients.Elasticsearch 8.15.0
- **Indexing**: Reads a CSV file and creates an index with `text` and `keyword` mappings.
- **Search Logic**:
    - `Exact match` (Term query on `title.keyword`) -> Boost 10
    - `Phrase match` (MatchPhrase query on `title`) -> Boost 5
    - `Term match` (Match query on `title`) -> Boost 1
    - `Overview match` (Match query on `overview`) -> Boost 0.5
