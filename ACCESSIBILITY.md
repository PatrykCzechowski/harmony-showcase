# Accessibility & Alt-Text Strategy

Goal: Ensure showcase materials communicate effectively to reviewers using assistive tech and uphold inclusive presentation practices.

## Alt-Text Principles
| Principle | Application |
|-----------|------------|
| Describe Purpose | Focus on what the screen conveys (e.g. "Automation history list with statuses") rather than raw UI chrome |
| Be Concise | Keep under ~120 characters unless critical context needed |
| Avoid Redundancy | Skip words like "image of"; already implied |
| Highlight Data | Mention dynamic values/state (e.g. success/failure badge) when meaningful |

## Current State
- All major screenshots include descriptive `alt` attributes in README tables.
- Planned GIFs will receive captions + alt-text summarizing the flow outcome.

## Color & Contrast
| Aspect | Approach |
|--------|---------|
| Badge Colors | Use distinct hues with adequate contrast (shields.io defaults mostly AA). |
| Diagrams | Prefer text labels (not color-only) for semantics. |
| Tables | Rely on textual symbols (✅/🔜) plus status wording for colorblind friendliness. |

## Planned Improvements
| Item | Action |
|------|--------|
| GIF Accessibility | Provide text description underneath animations |
| Keyboard Focus (future live demo) | Ensure tab order logical (private repo implementation) |
| High Contrast Mode | Document theme variable strategy (pending) |

## Testing Suggestions
- Use a screen reader (NVDA / VoiceOver) to navigate README headings & tables.
- Simulate color blindness filters to verify icon + text redundancy.

---
(End Accessibility)