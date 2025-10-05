# Harmony Platform - QuickStart Guide

## 1. Core Narrative (30s)
Smart‑home consolidation: one platform for devices, deterministic (predictable) automations, conversational AI, secure deny‑by‑default APIs, and low‑latency camera streaming. Transparency (provenance, execution history) and extensibility (triggers, actions, integrations) are first‑class.

## 2. Problem → Solution Snapshot
| Pain | Manifestation | Harmony Response |
|------|---------------|------------------|
| Fragmented control | Multiple vendor UIs & mental models | Unified device & scene abstraction |
| Opaque automations | Hard to know why a rule failed | Execution history + timeline + deterministic slots |
| Cognitive burden | Translating natural language to rule config | AI intent normalization boundary + provenance |
| Fragile streaming | DIY scripts, stale segments | Managed FFmpeg lifecycle (2s HLS) |
| Security drift | Accidental open endpoints | Resource/Action deny‑by‑default middleware |

## 3. Architecture Bird's Eye
- Frontend: Angular SSR + Tailwind feature domains (devices, rules, cameras, ai, auth).
- Backend: .NET 10 modular services, automation worker, AI orchestration layer, FFmpeg manager.
- Cross‑cutting: Structured logging, provenance, timeline, encrypted MFA secrets.

Diagrams:
- Context: `diagrams/context.md`
- Containers: `diagrams/containers.md`
- Automation Sequence: `diagrams/sequence-automation.md`
- AI Command Sequence: `diagrams/sequence-ai-command.md`

## 4. Key Differentiators (Delta Lens)
| Aspect | Typical OSS | Harmony | Value |
|--------|-------------|---------|-------|
| AuthZ | Role checks | Resource/Action deny-by-default | Attack surface clarity |
| Automation | Mixed cron/event timers | Minute-aligned predictable loop | Auditability |
| AI | Add-on chatbot | Intent boundary + provenance | Explainability |
| Streaming | External add-on | Managed lifecycle wrapper | Reliability + latency |
| Observability | Log scraping | User-facing timeline + history | Faster root cause |

See full detail: `CONTRAST.md`.

## 5. Code Highlights (Representative Snippets)
| Capability | File | Note |
|------------|------|------|
| Deny-by-default AuthZ | `code-snippets/security/deny-by-default-authorization.cs` | Central enforcement |
| Automation Loop | `code-snippets/automation/scheduler-loop.cs` | Slot alignment + debounce |
| Streaming Wrapper | `code-snippets/streaming/ffmpeg-start-wrapper.cs` | Playlist readiness wait |
| Intent Normalization | `code-snippets/ai/intent-normalization.cs` | Stable structured actions |
| MFA Secret Encryption | `code-snippets/security/totp-encryption-service.cs` | AES-GCM versioned format |
| Rate Limiter | `code-snippets/security/rate-limiter.cs` | Sliding window + lockout |

## 6. Evaluation Pointers
| Persona | Jump Point |
|---------|------------|
| Architect | Architecture Overview (README §3) + diagrams |
| Backend | Code Highlights + scheduler & auth snippets |
| Security | Contrast + Security & Privacy (README §7) |
| AI Engineer | AI & Automation Flow (README §6) + intent normalization |
| Product | Problem → Solution → Outcome + Screens (README §1 & §10) |

## 7. Demo CTA
For a live walkthrough of AI intent → execution + timeline correlation, see README section 16 (Request a Private Demo).

## 8. Polish Mirror
Pełne tłumaczenia sekcji (0–14 + rozszerzenia 15–16) znajdują się w README – przewiń do 🇵🇱 części.

---
<!-- End -->