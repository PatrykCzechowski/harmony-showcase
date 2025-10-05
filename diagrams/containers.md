# Container Diagram (C4 Level 2)

Logical containers composing the Harmony Platform.

```mermaid
flowchart TB
    subgraph Browser
        A1[Angular SPA + SSR bootstrap]
    end

    subgraph Backend[.NET 8 Harmony API]
        B1[API Gateway / Controllers]
        B2[Auth & Security Middleware]
        B3[Device Orchestrator]
        B4[Automation Engine (Hosted Service)]
        B5[AI Layer / Embedding & Chat]
        B6[Streaming Service (FFmpeg Manager)]
        B7[Integration Adapters]
        B8[Persistence (Repositories + UoW)]
    end

    subgraph External
        E1[(Fibaro)]
        E2[(Home Assistant)]
        E3[(Tuya)]
        E4[(IP Cameras RTSP)]
        E5[(Auth Store / DB)]
    end

    A1 --> B1
    B1 --> B2
    B1 --> B3
    B4 --> B3
    B1 --> B5
    B1 --> B6
    B3 --> B7
    B7 --> E1
    B7 --> E2
    B7 --> E3
    B6 --> E4
    B8 --> E5
    B4 --> B8
    B5 --> B8
    B3 --> B8
```

## Responsibilities
- B2 enforces deny-by-default authorization and rate limits.
- B4 executes deterministic minute-aligned rule cycles.
- B6 supervises temporary HLS segment generation, cleanup, and restart on failure.
- B5 provides intent normalization and semantic grounding.

## Cross-Cutting Concerns
- Structured logging flows through all containers.
- Feature flags / env gating for background workers and AI features.

## Next
See `components-backend.md` for finer-grained backend component decomposition.
