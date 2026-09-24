# VANTA: BLACK HORIZON — Development Status

## Repository checkpoint
- Default branch: `main`
- Authoritative branch: `main`
- Unity: 2022.3.62f1
- All new implementation work in this phase is committed directly to `main`.
- Latest source checkpoint: deterministic AI orchestration, knowledge grounding, AI tick budgeting, world simulation pressure, population/streaming integration, gameplay-state hardening, combat/vehicle state integration, and mission persistence.

## Current phase — population-budget and district-streaming integration
- `PerformanceBudgetSystem` now exposes a reusable `PerformanceBudgetPolicy` and applies its configured `Application.targetFrameRate` target when enabled.
- `TrafficPopulationSystem` now consumes the central traffic budget before spawning.
- Added `CivilianPopulationSystem` with the same deterministic budget contract.
- `DistrictStreamingSystem` now supports configured district-to-scene mappings and additive `SceneManager.LoadSceneAsync` / unload lifecycle callbacks, while retaining deterministic state transitions for tests.
- Added a deterministic `AgentMemoryStore` with bounded memory, importance-based eviction, and keyword retrieval; this provides a game-owned grounding layer rather than a hard dependency on an external LLM service.
- Added `AgentDecisionSystem` utility scoring for threat, curiosity, social need, visibility, and target presence, with deterministic tie behavior across Idle/Patrol/Investigate/Flee/Pursue/Assist/Converse actions.
- Research-informed direction: Unity's current AI tooling emphasizes project-aware agents, MCP, AI Gateway, Generators, and local Sentis inference; NVIDIA ACE emphasizes small/on-device character intelligence, RAG and agent loops; Convai emphasizes knowledge banks, scene-aware actions, proactive agents, and multimodal/spatial interaction. VANTA adopts the useful architectural ideas without copying proprietary content or requiring those services at runtime. Unity AI, NVIDIA ACE, and Convai research patterns were used as architectural references only; VANTA does not depend on those services at runtime.
- Added regression coverage for shared population budgets, district lifecycle guards, and advanced AI behavior.
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
- Source edits: IMPLEMENTED directly on `main`.
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


## Latest source-completion wave
- Added deterministic AgentGoalSystem for goal arbitration.
- Added AITickBudget to cap per-frame AI work for constrained hardware.
- Added AgentKnowledgeBase for authored world-knowledge grounding.
- Added AgentActionExecutor as a safe action boundary.
- Added AgentOrchestrator implementing Perception/Context -> Decision -> Action -> Memory flow.
- Added VantaAgentBrain MonoBehaviour adapter with configurable AI tick rate.
- Added deterministic WorldSimulationDirector for systemic pressure/event-tier selection.
- Added VantaWindowsBuild editor build command for StandaloneWindows64.
- Added a manual GitHub Actions Windows build workflow using Unity 2022.3.62f1 and GameCI. The workflow requires repository secrets UNITY_EMAIL, UNITY_PASSWORD, and UNITY_SERIAL; it has not been executed from this environment.

## Final acceptance / build truth
- Source-level implementation wave: IMPLEMENTED on main.
- GitHub branch state: main is the authoritative branch; dev/core-gameplay-foundation still exists because the GitHub connector used here does not expose branch deletion.
- Unity Editor import/compile: UNVERIFIED in this environment.
- Unity EditMode tests: authored but UNVERIFIED; no Unity Editor/Test Runner is available here.
- Play Mode / generated vertical slice: UNVERIFIED.
- Windows .exe: NOT BUILT/NOT VERIFIED here. The repository now contains both a Unity Editor build command and a manual GitHub Actions workflow, but a Unity license/build runner was not available to execute them.
- Full authored content (final art, animation, audio, VFX, complete mission/campaign content): NOT COMPLETE.
- Performance profiling on target hardware: UNVERIFIED.

No runtime-complete or build-complete claim should be made until Unity actually imports the project, tests execute, the vertical slice runs, and the Windows build launches successfully.
