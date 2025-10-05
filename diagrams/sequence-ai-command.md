# Sequence – AI Command Orchestration

```mermaid
sequenceDiagram
    autonumber
    participant U as User
    participant UI as Web UI (Chat)
    participant AI as AI Service
    participant EM as Embedding Index
    participant INT as Intent Normalizer
    participant OR as Orchestrator
    participant DB as Persistence

    U->>UI: Natural language prompt
    UI->>AI: Submit prompt
    AI->>EM: Retrieve semantic context
    EM-->>AI: Relevant chunks
    AI->>INT: Derive structured intent
    INT-->>AI: Intent (type + params)
    alt Is actionable intent
        AI->>OR: Execute (device/scene/rule test)
        OR->>DB: Persist action / timeline event
        OR-->>AI: Execution result
    else Informational
        AI-->>AI: Generate answer only
    end
    AI-->>UI: Response + provenance (chunks, actions)
    UI-->>U: Display answer & optional actions
```

Highlights:
- Provenance (used chunks) surfaced to user for transparency.
- Intent layer isolates model phrasing changes from orchestration logic.
- Easy provider swap (model) since normalization boundary is stable.
