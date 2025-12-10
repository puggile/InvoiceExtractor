# 🧾 Intelligent Invoice Extractor (IDP)

A modern Intelligent Document Processing (IDP) solution powered by .NET 9 and Multimodal AI (OllamaQwen-VL).
This project demonstrates how to move beyond legacy OCR templates by leveraging Vision Language Models (VLMs) to semantically understand and extract data from unstructured financial documents.

## 🏗 Architecture & Stack

Designed with Vertical Slice Architecture principles and Cloud-Native readiness.

 Core .NET 9, Minimal APIs.
 AI Engine Local inference using Ollama (Tested with Qwen3-VL).
 Database SQL Server 2025 (running in Docker).
 ORM Entity Framework Core 9 (Code-First).
 Resilience Implemented using Polly (Retry policies with exponential backoff).
 Containerization Docker & Docker Compose for orchestration.
 
## ✨ Flow

flowchart LR
    User[User / Client] -->|Uploads Invoice Image| API[.NET 9 API]
    API -->|Convert to Base64| Ollama[Ollama Local AI]
    Ollama -->|Returns Structured JSON| API
    API -->|Persists Data| DB[(SQL Server 2025)]
    DB -->|Returns Result| User

## 🚀 Quick Start

### Prerequisites
 [Docker Desktop](httpswww.docker.com)
 [Ollama](httpsollama.com) installed locally.

### 1. Pull & Run the AI Model
Open a terminal and pull the recommended vision model (optimized for local use):

```bash
ollama pull qwen3-vl:8b
```

Verify it’s running:

```bash
ollama run qwen3-vl:8b
```

### 2. Clone and Launch the Project
```bash
git clone https://github.com/puggile/InvoiceExtractor/
cd intelligent-invoice-extractor
```

Start the application with Docker Compose:

```bash
docker-compose up --build
```
The API will be available at:

```
http://localhost:5000/scalar/v1
```