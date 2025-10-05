# Sequence – Rule Automation Execution

```mermaid
sequenceDiagram
    autonumber
    participant S as Scheduler
    participant RE as Rule Evaluator(s)
    participant AE as Action Executor(s)
    participant OR as Device Orchestrator
    participant DB as Persistence

    S->>DB: Fetch enabled rules (batch)
    loop For each rule
        S->>RE: Evaluate trigger
        RE-->>S: Match? (bool)
        alt Match == true
            S->>AE: For each active action
            AE->>OR: Execute action command
            OR->>DB: Persist side effects / logs
            AE->>DB: Record execution result
        else Not matched
            S-->>S: Skip
        end
    end
```

Notes:
- Deterministic: loop aligns with minute boundary (reduces drift).
- Debounce: prevents duplicate execution within same minute slot.
- Extensibility: new trigger/action requires only new evaluator/executor registered in DI.
