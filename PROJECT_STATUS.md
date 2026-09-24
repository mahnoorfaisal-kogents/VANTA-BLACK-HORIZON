# VANTA: BLACK HORIZON — Development Status

## Repository checkpoint
- Default branch: `main`
- Active implementation branch: `dev/core-gameplay-foundation`
- Unity: 2022.3.62f1
- `main` remains untouched.
- Latest source checkpoint: gameplay-state, combat/vehicle state integration, mission persistence, and persistent-world serialization hardening.

## Current phase — performance/runtime-readiness hardening
- `PerformanceBudgetSystem` now applies its configured `Application.targetFrameRate` target when enabled.
- Added deterministic traffic/civilian spawn-budget queries (`CanSpawnTraffic`, `CanSpawnCivilian`) for population systems to consume without exceeding configured caps.
- Added regression coverage for population budget boundaries and invalid target-frame-rate clamping.
- Unity runtime execution remains explicitly unverified in this environment.

## Source implementation completed in this wave
- Unity-serializable persistent-world territory influence entries.
- Deterministic `GameplayStateService` with guarded lifecycle transitions and change events.
- `GameSession` routed through the guarded gameplay-state service.
- Enemy state machine transition guards; terminal Dead state cannot transition back into combat.
- Pursuit state machine transition guards.
- Mission objective graph exposes a required-objective completion gate.
- Mission completion now refuses to complete while required objectives remain incomplete.
- Deterministic vehicle damage state model integrated into `VehicleController`, with destruction event and repair support.
- Deterministic weapon magazine/ammo state model integrated into `WeaponController`.
- Mission runtime snapshots now capture objective progress and restore against known `MissionDefinition` assets.
- Save data now serializes mission runtime snapshots; malformed Windows path-separator handling in slot sanitization was corrected.
- Regression tests cover combat-state and mission-persistence behavior.

## Existing systemic foundation retained
- Mission objective graph + runtime asset isolation
- Mission consequences: economy, XP, faction reputation, territory influence, wanted heat and intel reveals
- Dynamic world events
- Faction territory + world effects
- Traffic routes/population
- NPC schedules
- Safehouse/garage services
- Intel map
- Progression/XP
- Police escalation + pursuit coordination
- Persistent world state
- NavMeshSurface wiring in the editor vertical-slice generator
- Activity lifecycle, district runtime, interaction registry and branching dialogue
- Save orchestration and guarded save-slot file IO
- Source-side streaming/performance foundations

## Verification truth
- Source edits: IMPLEMENTED on the active development branch.
- Regression tests authored: IMPLEMENTED.
- Unity Editor import/compile: UNVERIFIED.
- Unity EditMode test execution: UNVERIFIED; Unity Editor/Test Runner is unavailable in this execution environment.
- Unity Play Mode: UNVERIFIED.
- Generated scene runtime behavior: UNVERIFIED.
- NavMesh bake/runtime agents: UNVERIFIED.
- Vehicle source integration: IMPLEMENTED; runtime behavior UNVERIFIED.
- Weapon source integration: IMPLEMENTED; runtime behavior UNVERIFIED.
- Windows x64 .exe build and launch: UNVERIFIED.
- Performance profiling/LOD/culling/streaming: UNVERIFIED.
- Full authored content pass: NOT COMPLETE.

## Remaining acceptance phases
1. Unity import/compile and execute all deterministic EditMode tests.
2. Generate the vertical slice in Unity and verify scene references, NavMesh and runtime object wiring.
3. Verify combat/AI sandbox end-to-end.
4. Integrate and verify vehicle damage/repair and weapon ammo/reload with runtime presentation.
5. Verify vehicle/traffic/police sandbox.
6. Verify mission/faction/activity/district/save flows including restored objective progress and runtime world-state semantics.
7. Add authored districts, missions, dialogue, audio, VFX, animation and UI polish.
8. Profile pooling/LOD/culling/streaming and target-hardware performance.
9. Build and launch Windows x64, then run final regression and acceptance.

No item is labeled runtime-complete without execution evidence.
