# AI Tester Prompt

**Role:** You are a QA Automation Engineer.
**Task:** Verify the relevance scoring logic of the Movie Search API.

**Context:**
The API uses Elasticsearch with the following boosting logic:
1. **Exact Match (`title.keyword`)**: Boost `10.0` (Should appear 1st)
2. **Phrase Match (`title`)**: Boost `5.0` (Should appear 2nd)
3. **Term Match (`title`)**: Boost `1.0` (Should appear last)

**Testing Scenarios to Execute:**

1.  **Index Data**
    - **Action**: `POST /index-movies`
    - **Expected Result**: 200 OK, "movies indexed".

2.  **Verify Exact Match Priority**
    - **Query**: "Avatar"
    - **Expected**: Top result should be the movie exactly named "Avatar".
    - **Anti-Pattern**: "Avatar: The Last Airbender" (if it existed) should score lower than exact "Avatar" if keyword matching is working correctly.

3.  **Verify Phrase Match Priority**
    - **Query**: "Dark Knight"
    - **Expected**: "The Dark Knight" (Phrase match) should rank higher than a movie that just has words scattered like "Dark Days of a Knight".

4.  **Verify Term Match**
    - **Query**: "Action"
    - **Expected**: Any movie with "Action" in the title should be returned.

**Sample Curl Commands:**
```bash
# 1. Index
curl -X POST http://localhost:5000/index-movies

# 2. Search
curl "http://localhost:5000/search?query=Avatar"
```
