# VANTA: BLACK HORIZON — Development Status

## Repository checkpoint
- Default branch: `main`
- Active implementation branch: `dev/core-gameplay-foundation`
- Unity: 2022.3.62f1
- `main` remains untouched.
- Current source checkpoint includes the original systemic gameplay wave plus the next source-only integration wave.

## Source-only systems implemented
- System hardening regression coverage added for pursuit escalation, overnight schedules, traffic capacity and world-event lifecycle.
- Runtime systems were reviewed for deterministic transition guards and bounded population behavior.
- Source-side streaming/performance foundations are now represented without claiming actual profiling or scene streaming execution.

- Mission consequence data is attached to mission definitions and consumed on mission completion.
- `GameWorldCoordinator` connects mission rewards to economy, progression, faction reputation, territory influence, wanted heat and intel reveals.
- Wanted changes drive police escalation and pursuit state.
- Activity lifecycle: Locked -> Available -> Active -> Complete/Failed.
- Runtime district registry with unlock and active-district state.
- Interaction registry with deterministic interaction dispatch.
- Branching dialogue runtime with reputation-gated choices.
- End-to-end save orchestration for player transform, cash, wanted heat, XP, world time and active mission identity.
- Save-slot path sanitization and guarded file IO.
- Load hooks added to economy, progression and world time.
- Generated vertical slice now wires the coordinator, save coordinator and the new systemic services.
- Deterministic regression coverage added for the new activity/district/dialogue/interaction/save behavior.

## Existing systemic foundation retained
- Mission objective graph + runtime asset isolation
- Dynamic world events
- Faction territory + world effects
- Traffic routes/population
- NPC schedules
- Safehouse/garage services
- Intel map
- Progression/XP
- Police escalation + pursuit coordination
- Persistent world snapshot foundation
- NavMeshSurface wiring in the editor vertical-slice generator

## Research wave
The implementation direction continues to use public system-level references only. Current research emphasizes systemic crowd events, open-world AI scheduling/action planning, dynamic events, runtime navigation, and production AI tooling. VANTA characters, missions, dialogue, world, art, audio and other content remain original.

## Verification truth
- Source edits: IMPLEMENTED on the active development branch.
- Unity Editor import/compile: UNVERIFIED.
- Unity EditMode test execution: UNVERIFIED; Unity Editor/Test Runner is unavailable in this execution environment.
- Unity Play Mode: UNVERIFIED.
- Generated scene runtime behavior: UNVERIFIED.
- NavMesh bake/runtime agents: UNVERIFIED.
- Windows x64 .exe build and launch: UNVERIFIED.
- Performance profiling/LOD/pooling/streaming: UNVERIFIED.
- Full authored content pass: NOT COMPLETE.

## Next acceptance phases
1. Unity import/compile and execute all deterministic EditMode tests.
2. Generate the vertical slice in Unity and verify scene references, NavMesh and runtime object wiring.
3. Verify the combat/AI sandbox end-to-end.
4. Verify vehicle/traffic/police sandbox.
5. Verify mission/faction/activity/district/save flows.
6. Add authored districts, missions, dialogue, audio, VFX, animation and UI polish.
7. Add performance systems (pooling, LOD, culling, streaming) and profile on target hardware.
8. Build and launch Windows x64, then run final regression and acceptance.

No item above is labeled runtime-complete until execution evidence exists.
