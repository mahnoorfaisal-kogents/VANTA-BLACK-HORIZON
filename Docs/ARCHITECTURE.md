# Architecture

VANTA uses modular, deterministic runtime systems so gameplay rules can be tested independently of Unity scenes.

- **Core**: session state and cross-system orchestration.
- **Player**: locomotion, stealth and health.
- **Camera**: third-person camera/binding.
- **Combat**: weapon data, firing and damage.
- **AI**: perception, state machines, police escalation and pursuit.
- **Missions**: definitions, objective graphs and consequence resolution.
- **Systems**: economy, inventory, factions, territory, progression, wanted, activities, interactions and safehouse services.
- **World**: time, weather, districts, events, traffic, schedules, discoveries, intel and dialogue.
- **Save**: slot IO plus game-state capture/restore orchestration.
- **Editor**: reproducible vertical-slice scene generation.
- **Tests**: deterministic EditMode regression coverage.

## Integration rules

1. Prefer data-driven definitions over hard-coded per-scene gameplay.
2. Keep deterministic state transitions in small testable classes.
3. Use events for cross-system notifications where practical.
4. Runtime claims require Unity execution evidence.
5. Generated scenes are integration fixtures, not substitutes for authored production content.
6. Public games and AI tools are references for systems/workflows only; VANTA content stays original.
7. Avoid introducing runtime generative AI into critical gameplay state until it has deterministic fallbacks, bounded latency/cost, and a testable contract.
