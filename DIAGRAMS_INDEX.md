# Diagrams Index

Purpose: Central reference for all architecture & flow diagrams with guidance on narrative usage.

| File | Type | Narrative Usage | Key Audience |
|------|------|-----------------|--------------|
| `diagrams/context.md` | C4 Level 1 | Introduce external actors & platform boundary | Product / Architect |
| `diagrams/containers.md` | C4 Level 2 | Show deployment/runtime segmentation (frontend, API, workers) | Architect / Backend |
| `diagrams/components-backend.md` | Component | Explain service / module responsibilities | Backend / Security |
| `diagrams/components-frontend.md` | Component | Clarify feature domain boundaries & SSR role | Frontend / EM |
| `diagrams/sequence-automation.md` | Sequence | Walk through rule evaluation & execution path | Backend / Reviewer |
| `diagrams/sequence-ai-command.md` | Sequence | Demonstrate AI intent normalization & orchestration | AI Engineer / Architect |

## Narrative Tips
- Start with Context → Containers to orient scope before deep dives.
- Pair sequence diagrams with corresponding code snippets (automation loop, intent normalization) to bridge design → implementation.
- Use component diagrams to justify extensibility claims (plug-in triggers/actions/integrations).

## Maintenance Guidance
- Keep filenames stable to preserve README links.
- Update component diagrams when adding new cross-cutting services or feature domains.
- Version major structural shifts via commit messages + `CHANGELOG_SHOWCASE.md` entry.

---
(End of diagrams index)