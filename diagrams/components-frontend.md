# Frontend Components (Selected)

```mermaid
flowchart LR
    subgraph Core
        F1["App Shell<br/>Layout"]
        F2["Routing Config"]
        F3["Auth Guard"]
    end
    subgraph Features
        D1["Dashboard Feature"]
        A1["AI Chat Feature"]
        R1["Rules Feature"]
        C1["Cameras Feature"]
        I1["Integrations Feature"]
        FIB["Fibaro Feature"]
        DEV["Devices Feature"]
    end
    subgraph Shared
        SH1["UI Primitives"]
        SH2["HTTP Interceptors"]
        SH3["State Helpers<br/>Services"]
    end

    F1 --> F2 --> F3
    F3 --> D1
    F3 --> A1
    F3 --> R1
    F3 --> C1
    F3 --> I1
    F3 --> FIB
    F3 --> DEV
    D1 --> SH1
    A1 --> SH1
    R1 --> SH1
    C1 --> SH1
    I1 --> SH1
    FIB --> SH1
    DEV --> SH1
    SH2 --> D1
    SH2 --> A1
    SH2 --> R1
    SH2 --> C1
    SH2 --> I1
    SH2 --> FIB
    SH2 --> DEV
```

## Notes
- Standalone Angular components reduce module overhead.
- UI Primitives ensure consistent styling (cards, dialogs, form controls).
- Interceptors centralize base URL, auth token, and XSRF handling.
- Features lazy-loaded where appropriate for performance.

Next: see `sequence-automation.md` and `sequence-ai-command.md` for dynamic flows.
