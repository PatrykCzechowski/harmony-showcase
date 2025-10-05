# Context Diagram (C4 Level 1)

High-level external view of Harmony Platform.

```mermaid
flowchart LR
    User([End User / Resident]) --> UI[Web App (Angular SSR + PWA)]
    Admin([Admin / Power User]) --> UI
    UI --> API[Harmony API (.NET 8)]
    API --> Auth[(Identity & Auth)]
    API --> Automation[Automation Engine]
    API --> AI[AI Layer / Embeddings]
    API --> Streaming[Camera Streaming FFmpeg Wrapper]
    API --> Integrations[Integrations Layer]
    Integrations --> Fibaro[(Fibaro System)]
    Integrations --> HA[(Home Assistant)]
    Integrations --> Tuya[(Tuya Cloud)]
    Streaming --> Cameras[(IP Cameras RTSP)]
    Automation --> DB[(Relational DB)]
    AI --> DB
    API --> DB
```

## Notes
- Users interact through a responsive Angular application (SSR for fast first paint).
- API centralizes device abstraction, rule processing, AI orchestration, and streaming control.
- External ecosystems remain authoritative for their own device state; Harmony orchestrates and augments.

## Next
See `containers.md` for internal container breakdown.
