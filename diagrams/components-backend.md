# Backend Components (Selected)

```mermaid
flowchart LR
    subgraph API Layer
        C1[Controllers]
        C2[AuthZ Middleware]
    end
    subgraph Core Services
        S1[Device Orchestrator]
        S2[Automation Scheduler]
        S3[Rule Evaluators]
        S4[Action Executors]
        S5[AI Chat Service]
        S6[Streaming Manager]
    end
    subgraph Integrations
        I1[Fibaro Adapter]
        I2[Home Assistant Adapter]
        I3[Tuya Adapter]
    end
    subgraph Persistence
        P1[Repositories]
        P2[UnitOfWork]
    end

    C1 --> C2
    C1 --> S1
    C1 --> S5
    C1 --> S6
    S2 --> S3 --> S4 --> S1
    S1 --> I1
    S1 --> I2
    S1 --> I3
    S2 --> P1
    S4 --> P1
    S5 --> P1
    S6 --> P1
    P1 --> P2
```

## Descriptions
- Device Orchestrator: Unifies heterogeneous device/scene commands.
- Automation Scheduler: Deterministic loop aligning to minute boundaries.
- Rule Evaluators: Pluggable trigger evaluation (time, device, future event types).
- Action Executors: Strategy objects executing domain actions (scene trigger, device command).
- Streaming Manager: Supervises FFmpeg processes and HLS artifact lifecycle.
- AI Chat Service: Embedding retrieval + intent normalization + orchestration.

## Extensibility Points
- Add a new trigger → implement evaluator and register DI.
- Add a new action → implement executor; no core changes.
- Add a new integration → adapter + registration + mapping layer.

See `components-frontend.md` for the frontend composition.
