# Harmony – Feature Matrix

Curated catalogue of implemented (or designed) functional and technical capabilities. Use this file in portfolio submissions to give reviewers a scannable map of scope and depth.

Legend: ✅ implemented • 🔄 in progress / designed • 🧭 planned / roadmap

## 1. Core Functional Domains
| Category | Feature | Status | Notes / Depth |
|----------|---------|--------|---------------|
| Devices | Unified multi‑vendor device list | ✅ | Adapter abstraction normalizing heterogeneous models |
| Devices | Scene invocation (Fibaro) | ✅ | Orchestrator dispatch with granular error mapping |
| Automation | IF/THEN rule editor | ✅ | Trigger + actions + enable/disable flags |
| Automation | Execution history log | ✅ | Timestamp, outcome, message, correlation to timeline |
| Automation | Deterministic scheduler (minute alignment) | ✅ | Debounce prevents duplicate firings in same slot |
| AI | Conversational device control | ✅ | Intent normalization → orchestration layer |
| AI | Knowledge embeddings (docs) | ✅ | Vector context build per query (usedChunks provenance) |
| Cameras | RTSP → low‑latency HLS streaming | ✅ | FFmpeg managed process, 2s segments, cleanup strategy |
| Cameras | Multi camera overview | ✅ | Grid / placeholder states |
| Auth | Email/password auth | ✅ | JWT issuance, refresh ready |
| Auth | Password reset flow | ✅ | Token validation + staged UX |
| Security | Resource/Action authorization (deny‑by‑default) | ✅ | Missing metadata ⇒ 403 enforcement |
| Security | TOTP secret encryption | ✅ | AES‑GCM versioned format + idempotent guard |
| Observability | System timeline | ✅ | Aggregated events: rules, devices, user actions |
| Observability | Structured automation logs | ✅ | Decision context + action outcomes |
| Extensibility | Plug‑in action executors | ✅ | DI multi-registration model |
| Extensibility | Add new integration provider | ✅ | Service adapter + registration + UI panel |
| UX | Lazy loaded feature domains | ✅ | Reduced initial bundle size |
| UX | Dynamic device control panels | ✅ | Type‑based component mapping |
| Performance | Short‑segment HLS (2s) | ✅ | Latency vs bandwidth balance |
| Reliability | Process cleanup before new stream | ✅ | Avoids stale segments / ghost playback |
| Roadmap | MFA (WebAuthn + TOTP UI) | 🧭 | Backend hooks prepared |
| Roadmap | AI rule suggestions | 🧭 | Prompt templates planned |
| Roadmap | Energy analytics | 🧭 | Time‑series ingestion design |
| Roadmap | Push notifications (PWA) | 🧭 | Service worker extension |
| Roadmap | Integration marketplace | 🧭 | Contract boundaries defined |

## 2. Automation Engine Detail
- Trigger Evaluators: time-based (daily). (Planned: sensor, sunrise/sunset)
- Action Executors: Fibaro device/scene actions (extensible interface).
- Debounce Strategy: per rule minute-slot stamp preventing double execution.
- Persistence: execution log with outcome + message fields for diagnostics.

## 3. Security & Hardening Highlights
| Aspect | Mechanism | Value |
|--------|-----------|-------|
| Authorization | Deny-by-default middleware | Minimizes accidental exposure |
| Permissions | Resource/Action attributes | Generates clear permission matrix |
| Secrets | AES-GCM encryption for TOTP | Integrity + confidentiality |
| Rate Limiting | Retry-After header for AI | Client aware backoff guidance |
| Logging Hygiene | Credential masking | Reduces leak risk in diagnostics |

## 4. AI Orchestration
Flow: Prompt → Embed context → Intent resolution → (Optional) Action execution → Answer + provenance.
Provides transparent usedChunks listing to explain why an answer was formed.

## 5. Extensibility Surfaces
| Surface | Add Steps | Effort |
|---------|-----------|--------|
| New Action Type | Implement IAutomationActionExecutor + DI register | Low |
| New Trigger Type | Implement IAutomationTriggerEvaluator + register | Low |
| New Integration | Adapter service + models + UI panel | Medium |
| New AI Provider | Swap model/env vars in HarmonyAI.SDK config | Low |
| New Streaming Param | Adjust FFmpeg args constants | Low |

## 6. Portfolio Talking Points
- Explicit extensibility seams reduce future feature cost.
- Deterministic scheduling improves trust and debuggability.
- Security posture intentionally strict (opt-in endpoints only).
- AI integrated as orchestrator, not generic chat widget.
- Observability first: timeline + execution context logging.

## 7. Suggested Live Demo Script
1. Show dashboard + unified devices.
2. Open AI chat: issue natural command triggering a scene.
3. Create a rule; wait for scheduled execution; inspect history.
4. Start a camera stream; refresh to show low-latency segments.
5. Highlight denial behavior by hitting an endpoint w/out metadata (403 conceptually).

---
Need a condensed one-pager? Extract sections 1 + 6.
