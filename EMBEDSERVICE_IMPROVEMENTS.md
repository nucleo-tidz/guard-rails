# EmbedService Improvements

## Overview
The `EmbedService` has been refactored to support uploading text documents from the API. The API endpoint now accepts file uploads and reads the content server-side.

## Key Improvements

### 1. **File Upload Support**
- **Before**: Documents had to be passed as raw content strings
- **After**: Upload files directly via multipart/form-data

### 2. **Server-Side File Reading**
- Files are uploaded to the API and read on the server
- No need to pre-read files on the client side
- Content is extracted and passed to the embedding service

### 3. **Better Input Validation**
- Validates that a file is provided
- Validates that the file is not empty
- Validates that chunk size is positive

### 4. **Configurable Chunking**
- **Chunk Size**: Customize how large each text chunk is (default: 200)
- **Overlap**: Control overlap between chunks (default: 50)
- **Collection Name**: Route documents to different Redis collections

### 5. **Cleaner Code**
- Extracted chunking logic into a separate `ChunkDocument` method
- Async/await pattern for all operations
- Constants for default values

## API Usage

### Endpoint
```
POST /api/agent/seed-document
Content-Type: multipart/form-data
```

### Form Parameters
- **file** (required): Text file to upload
- **collectionName** (optional): Redis collection name (default: "nucleotidz")
- **chunkSize** (optional): Characters per chunk (default: 200)
- **overlap** (optional): Overlap between chunks (default: 50)

### Example cURL Request
```bash
curl -X POST http://localhost:5000/api/agent/seed-document \
  -F "file=@document.txt" \
  -F "collectionName=nucleotidz" \
  -F "chunkSize=200" \
  -F "overlap=50"
```

### Example with Postman
1. Set request type to `POST`
2. URL: `http://localhost:5000/api/agent/seed-document`
3. Go to "Body" tab ? Select "form-data"
4. Add form fields:
   - `file` (type: File) ? select your text file
   - `collectionName` (type: Text) ? "nucleotidz"
   - `chunkSize` (type: Text) ? "200"
   - `overlap` (type: Text) ? "50"

### Response

**Success (200 OK)**
```json
{
  "message": "Document 'document.txt' seeded successfully"
}
```

**Error - No File (400 Bad Request)**
```json
{
  "error": "File is required"
}
```

**Error - Invalid Input (400 Bad Request)**
```json
{
  "error": "Document content cannot be empty"
}
```

**Server Error (500 Internal Server Error)**
```json
{
  "error": "An error occurred while seeding the document",
  "details": "Error details here"
}
```

## Usage in Code

If you need to call `EmbedService` directly (for backend processes):

```csharp
public class YourService
{
    private readonly EmbedService embedService;

    public YourService(EmbedService embedService)
    {
        this.embedService = embedService;
    }

    public async Task ProcessDocument(string filePath)
    {
        string documentContent = File.ReadAllText(filePath);
        await embedService.SeedDataAsync(
            documentContent,
            Path.GetFileName(filePath),
            "collection-name",
            chunkSize: 200,
            overlap: 50
        );
    }
}
```

## Configuration

The default values can be customized per request:
- **Chunk Size**: How many characters per chunk (larger = fewer chunks, less granular)
- **Overlap**: How many characters to overlap between chunks (prevents losing context at boundaries)
- **Collection Name**: Organize documents into different Redis collections

## Benefits

? **File Upload**: Send documents as files via HTTP  
? **Server-Side Processing**: No need to read files on the client  
? **Dynamic**: Process multiple documents with different configurations  
? **Validated**: Input validation prevents runtime errors  
? **Testable**: Easy to unit test with dependency injection  
? **Flexible**: Customize chunk sizes and collections per document  
? **Error Handling**: Proper error responses from the API

## File Formats Supported

The endpoint accepts any text-based files:
- `.txt` - Plain text
- `.md` - Markdown
- `.csv` - Comma-separated values
- `.json` - JSON files (as text)
- `.xml` - XML files
- Or any other text-based format
