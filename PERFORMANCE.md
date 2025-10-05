# Performance Targets & Benchmarks (Showcase)

Empirical private measurements summarized (no proprietary logs included). Targets guide future optimization and communicate intent.

## Targets
| Domain | Metric | Target | Rationale |
|--------|--------|--------|-----------|
| Automation Loop | Start-of-minute alignment drift | < 150 ms | Ensures consistent rule timing |
| Automation Loop | Iteration duration (avg) | < 40 ms | Leaves headroom for spikes |
| AI Intent | Intent normalization latency (P95) | < 1200 ms | Conversational responsiveness |
| Streaming | First frame availability | < 3 s | User perception of “instant” stream |
| Streaming | Segment duration | 2 s | Latency vs buffering balance |
| AuthZ | Middleware evaluation | < 2 ms | Negligible overhead per request |

## Observed (Illustrative)
| Domain | Metric | Observed Range | Note |
|--------|--------|----------------|------|
| Automation Loop | Drift | 40–90 ms | Stable under moderate load |
| AI Intent | P95 latency | 900–1100 ms | Model + retrieval combined |
| Streaming | First frame | 1.6–2.4 s | Network + playlist readiness |
| AuthZ | Evaluation | ~0.7 ms | Cached policy table |

## Measurement Approach
- Automation: Stopwatch around scheduler iteration (private code instrumentation).
- AI: Timestamp from prompt submission → structured intent object + provenance ready.
- Streaming: HLS playlist creation time until first segment served.
- AuthZ: Middleware stopwatch (excluding downstream controller).

## Optimization Levers
| Domain | Lever | Impact |
|--------|-------|--------|
| Automation | Parallel trigger evaluation | Lower iteration time (caution: ordering) |
| AI | Embedding cache reuse | Lower retrieval latency |
| Streaming | Pre-warm FFmpeg process | Faster first frame |
| AuthZ | Precompiled resource/action map | Constant-time lookup |

## Future Experiments
- Adaptive segment sizing under fluctuating network.
- Intent latency budget split (retrieval vs model vs post-processing) instrumentation.
- Rule batching heuristics for heavy schedules.

---
(End Performance Targets)