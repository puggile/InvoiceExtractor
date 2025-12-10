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

## 🚀 Quick Start

### Prerequisites
 [Docker Desktop](httpswww.docker.com)
 [Ollama](httpsollama.com) installed locally.

### 1. Setup AI Model
Pull the vision model (lighter and faster for local dev)
```bash
ollama run qwen3-vl:8b
```