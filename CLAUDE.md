# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.
## Project Overview

This project exposes a REST API as the primary entry point. The API connects to a Shipment Agent that handles user queries using a combination of tools and Retrieval-Augmented Generation (RAG).

The agent can answer questions such as:
- "What is the origin port of the shipment?"
- "What is the claims policy?"

---

## Key Capabilities

### 1. Intelligent Query Handling
- Uses tools + RAG search to retrieve accurate and contextual information
- Dynamically decides whether to perform a RAG search based on user intent

### 2. Intent Classification
- Includes an intent classifier to detect the type of user query
- Determines whether external knowledge retrieval (RAG) is required

### 3. Conversation Memory
- Stores each conversation in Redis
- Uses a custom chat history provider
- Tracks conversations using:
  - User ID
  - Thread ID

### 4. Safety & Guardrails
The system implements multiple guardrails to ensure safe and reliable responses:

- Jailbreak detection → Prevents prompt injection attacks
- PII detection → Protects sensitive user data
- Groundedness detection → Ensures responses are based on factual/contextual data

---
1. User sends request → REST API  
2. API forwards request → Shipment Agent  
3. Intent classifier evaluates query through a middleware 
4. Agent decides:
   - Use tools  
   - Perform RAG search  
   - Or both  
5. Response generated and returned  
6. Conversation stored in Redis

## Tech Stack

### Language & Platform
- **C#**
- **.NET (ASP.NET Core Web API)**

### AI & Agent Framework
- **Microsoft Agent Framework**
  - Used to build the Shipment Agent
  - Handles orchestration, tool usage, and reasoning
  - Supports agentic workflows with modular capabilities

### AI & LLM Integration
- **Azure OpenAI**
  - Powers natural language understanding and response generation
  - Enables Retrieval-Augmented Generation (RAG)

### Memory & State Management
- **Redis**
  - Stores conversation history
  - Custom chat history provider based on:
    - User ID
    - Thread ID

### Retrieval (RAG)
- Vector-based or indexed knowledge retrieval
- Used for answering domain-specific queries (e.g., shipment details, policies)

### Safety & Guardrails
- **Azure Content Safety / Guardrails Layer**
  - Jailbreak detection
  - PII (Personally Identifiable Information) detection
  - Groundedness validation

### API Layer
- **REST API (ASP.NET Core)**
  - Entry point for all client requests
  - Routes requests to the Shipment Agent

### Additional Components
- **Intent Classifier**
  - Determines user intent
  - Decides whether to trigger RAG or direct tool execution
