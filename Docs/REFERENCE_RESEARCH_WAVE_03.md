# VANTA — Reference Research Wave 03

This research wave is system-level only. It does not import protected game content.

## Systemic open-world AI

Public GDC material on Assassin's Creed Unity describes systemic crowd events that create spontaneous gameplay opportunities, while material on Watch Dogs: Legion describes a procedural character/schedule backbone that supports persistent social relationships and world simulation. Ubisoft's work on action planning in Assassin's Creed Odyssey also describes moving toward GOAP-style planning for more dynamic NPC behavior.

### VANTA application
- Keep world events independent from individual missions.
- Treat NPC schedules as data that can later feed relationships, locations and event triggers.
- Add planning only where deterministic state machines become insufficient; keep hard gameplay outcomes bounded and testable.

## Dynamic events and streaming

GDC material on dynamic events and streaming open worlds emphasizes the engineering cost of maintaining navigation and world state while the environment changes.

### VANTA application
- Keep event definitions deterministic.
- Separate world-state rules from presentation.
- Defer district streaming/pooling until Unity runtime profiling is available.

## Navigation

Unity's 2022.3 documentation lists AI Navigation 1.1.4 as a released package for Unity 2022.3. The existing generator therefore keeps NavMeshSurface generation in the Unity acceptance phase rather than pretending source-only generation proves runtime navigation.

## AI-assisted production

Current 2026 game-development coverage shows AI tools are increasingly used for code assistance, asset prototyping, NPC dialogue and testing, but also highlights the need for human verification and the difference between development-time assistance and player-facing runtime generation.

### VANTA application
- AI is a development aid, not an authority over game state.
- Generated assets/dialogue must remain subject to originality, licensing and human review.
- Any future runtime NPC AI must have deterministic state contracts, bounded failure behavior and an offline-safe fallback.
