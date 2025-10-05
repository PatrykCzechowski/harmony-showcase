# Threat Model (Light STRIDE Overview)

Scope: Public API surface (Auth, Automation, AI, Streaming), internal automation worker, and FFmpeg wrapper process.

## Components
- API Gateway / ASP.NET Core layer
- Authorization Middleware (Resource/Action)
- Automation Scheduler Worker
- AI Intent Normalization Layer
- Streaming Manager (FFmpeg process supervisor)
- Data Stores (Rules metadata, execution history, user accounts, encrypted TOTP secrets)

## STRIDE Summary
| STRIDE | Risk Example | Mitigation | Residual Risk |
|--------|--------------|-----------|---------------|
| Spoofing | Token theft → unauthorized rule edit | Short-lived JWT + refresh rotation + deny-by-default | Refresh compromise window |
| Tampering | Modify automation history entries | Server-side integrity, no client-side mutation endpoints | Insider DB access |
| Repudiation | User denies performing an action | Execution history & timeline with timestamps + user context | Clock skew / log tampering (future signing) |
| Information Disclosure | Exposure of device command endpoints | Missing metadata blocked (403) | Misclassified resource naming |
| Denial of Service | Flood AI endpoint with prompts | Rate limiter + Retry-After | Distributed attack volume |
| Elevation of Privilege | Bypass authZ via missing attribute | Deny-by-default prevents execution | Attribute misconfiguration |

## Data Classification
| Data | Classification | Controls |
|------|---------------|----------|
| User Credentials (hash) | Sensitive | Secure hashing (private repo implementation) |
| TOTP Secret (encrypted) | Sensitive | AES-GCM + versioned prefix | 
| Device State Metadata | Internal | AuthZ gated endpoints |
| Execution History | Internal (user-owned) | Least privilege read per user |
| AI Prompt + Provenance | Internal | Redaction of secrets in prompt assembly |

## Attack Surface Notes
- Streaming endpoints isolated; no arbitrary shell since FFmpeg args validated (private repo detail).
- AI layer isolated from direct device invocation (intent boundary required).
- No unauthenticated endpoints modify state.

## Future Hardening Opportunities
| Area | Enhancement | Value |
|------|------------|-------|
| Logging Integrity | Signed / hashed log records | Non-repudiation | 
| Permissions Visualization | Graph export of resource/action graph | Easier audits |
| Secret Management | External secrets manager (e.g. Vault) | Central rotation | 
| Anomaly Detection | Rate + pattern analysis on automations | Early intrusion signals |

---
(End Threat Model)