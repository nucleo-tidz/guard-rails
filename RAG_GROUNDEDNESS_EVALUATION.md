# RAG Results Exposure for Groundedness Evaluation

## Overview
The agent now exposes RAG (Retrieval Augmented Generation) search results alongside the AI response, allowing you to evaluate the groundedness of the AI's response against the retrieved documents.

## What Changed

### 1. **New Response Structure**
The `/api/agent/chat` endpoint now returns an `AgentResponse` object containing:
- `Message`: The AI agent's response
- `RetrievedDocuments`: List of documents retrieved by the RAG system

### 2. **New DTOs** (`application.Dtos`)
```csharp
public class AgentResponse
{
    public string Message { get; set; }
    public List<RagResult> RetrievedDocuments { get; set; }
}

public class RagResult
{
    public string SourceName { get; set; }
    public string SourceLink { get; set; }
    public string Text { get; set; }
}
```

### 3. **TextSearchAdapter Enhancement**
The `TextSearchAdapter` now tracks the last search results in `LastSearchResults` property, which are captured after each RAG query.

## API Response Example

### Request
```bash
curl -X GET "http://localhost:5000/api/agent/chat/What%20are%20your%20container%20types/user123"
```

### Response
```json
{
  "message": "We offer 20ft and 40ft standard containers. Based on your shipment size...",
  "retrievedDocuments": [
    {
      "sourceName": "CompanyDocument.txt",
      "sourceLink": "CompanyDocument.txt",
      "text": "Container Types: We provide 20ft and 40ft containers for standard shipping..."
    },
    {
      "sourceName": "CompanyDocument.txt",
      "sourceLink": "CompanyDocument.txt",
      "text": "40ft containers are ideal for large shipments weighing up to 30 tons..."
    },
    {
      "sourceName": "CompanyDocument.txt",
      "sourceLink": "CompanyDocument.txt",
      "text": "For smaller shipments, our 20ft containers are cost-effective options..."
    }
  ]
}
```

## How to Use for Groundedness Evaluation

### Step 1: Get the Response with RAG Results
Make a request to the chat endpoint and capture the response:

```bash
curl -X GET "http://localhost:5000/api/agent/chat/your%20question/username"
```

### Step 2: Analyze Groundedness
Compare the `message` against `retrievedDocuments`:

1. **Check Source Coverage**: Does the message cite information from the retrieved documents?
2. **Verify Factual Accuracy**: Is the information in the message supported by the retrieved documents?
3. **Identify Hallucinations**: Are there claims in the message not supported by any retrieved document?
4. **Assess Relevance**: Do the retrieved documents actually address the user's question?

### Example Evaluation Script (Pseudocode)
```csharp
public class GroundednessEvaluator
{
    public GroundednessScore Evaluate(AgentResponse response)
    {
        var documentTexts = response.RetrievedDocuments
            .Select(d => d.Text)
            .ToList();

        // Check if key claims in the message are supported by documents
        var score = new GroundednessScore
        {
            TotalClaims = ExtractClaims(response.Message).Count,
            GroundedClaims = CountGroundedClaims(response.Message, documentTexts),
            HallucinatedClaims = CountHallucinatedClaims(response.Message, documentTexts),
            GroundednessPercentage = CalculatePercentage(...)
        };

        return score;
    }
}
```

## Metrics to Track

1. **Groundedness Score**: % of claims in the response that are supported by retrieved documents
2. **Hallucination Rate**: % of claims that are not supported by any document
3. **Document Relevance**: % of retrieved documents that are relevant to the query
4. **Source Diversity**: Number of unique sources cited

## Benefits

? **Transparency**: See exactly what documents informed the AI's response  
? **Traceability**: Every statement can be traced back to source documents  
? **Quality Assurance**: Evaluate response quality before returning to users  
? **Debugging**: Identify if RAG is retrieving relevant documents  
? **Compliance**: Document which sources were used for regulatory requirements  
? **Trust**: Build user confidence with source citations

## Implementation Details

### Flow
1. User sends a question via `/api/agent/chat`
2. `NucleotidzAgent.Start()` is called
3. `TextSearchAdapter.SearchAdapter()` performs RAG search and stores results in `LastSearchResults`
4. `TextSearchProvider` uses the search results to augment the AI's context
5. AI generates response based on augmented context
6. `LastSearchResults` are captured and returned in `AgentResponse`
7. Full response (message + documents) is returned to the client

### Architecture
```
Client Request
    ?
AgentController.Chat()
    ?
NucleotidzAgent.Start()
    ?
TextSearchAdapter.SearchAdapter() ? Stores LastSearchResults
    ?
TextSearchProvider (uses search results)
    ?
ChatClient (AI model generates response)
    ?
AgentResponse (message + LastSearchResults)
    ?
Client Response
```

## Future Enhancements

- **Scoring**: Add confidence scores to retrieved documents
- **Ranking**: Return top-k most relevant documents
- **Filtering**: Filter by document type or source
- **Batch Evaluation**: Run groundedness checks on multiple responses
- **Metrics Dashboard**: Visualize groundedness trends over time
