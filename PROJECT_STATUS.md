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
- Added deterministic `AgentMemoryStore` with bounded memory, importance-based eviction, and keyword retrieval; this provides a game-owned grounding layer rather than a hard dependency on an external LLM service.
- Added `AgentGoalSystem`, `AgentActionExecutor`, `AgentKnowledgeBase`, `AgentOrchestrator`, `VantaAgentBrain`, `AITickBudget`, `AIFrameBudgetSystem`, `AgentSquadSystem`, `AgentTelemetry`, and `WorldSimulationDirector`.
- Added `AgentDecisionSystem` utility scoring for threat, curiosity, social need, visibility, and target presence, with deterministic tie behavior across Idle/Patrol/Investigate/Flee/Pursue/Assist/Converse actions.
- Research-informed direction: Unity's current AI tooling emphasizes project-aware agents, MCP, AI Gateway, Generators, and local Sentis inference; NVIDIA ACE emphasizes small/on-device character intelligence, RAG and agent loops; Convai emphasizes knowledge banks, scene-aware actions, proactive agents, and multimodal/spatial interaction. VANTA adopts the useful architectural ideas without copying proprietary content or requiring those services at runtime. Unity AI, NVIDIA ACE, and Convai research patterns were used as architectural references only; VANTA does not depend on those services at runtime.
- Added regression coverage for shared population budgets, district lifecycle guards, and advanced AI behavior.
- Unity runtime execution remains explicitly unverified in this environment; source-level fixes were made after inspection, but they cannot substitute for Unity's compiler/Test Runner.

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

## Latest AI integration wave
- Added AgentTacticalSystem to convert perception/context into deterministic tactical goals and actions.
- Integrated tactical arbitration into EnemyAI, including flee/chase/combat selection.
- Integrated tactical arbitration into CivilianAI, including flee/investigate/wander/recover reactions.
- Integrated tactical arbitration into PoliceAI pursuit speed/behavior selection.
- Added AIDirector + AIDirectorModel for systemic pressure evaluation and contextual world-event requests.
- Added runtime AgentSquadCoordinator and AgentSquadCoordinatorComponent; live child VantaAgentBrain instances are assigned deterministic Leader/Assault/Support/Scout roles.
- Added regression coverage for tactical arbitration, AI Director pressure tiers and squad assignments.
- Source-level AI integration: IMPLEMENTED on main.
- Unity compile, EditMode execution, Play Mode, NavMesh, and Windows build remain UNVERIFIED.

## Latest consolidated AI phase — squad execution, knowledge propagation, contextual event lifecycle
- Added deterministic `AgentSquadCommandSystem` with role-aware `Advance`, `Flank`, `Suppress`, `Search`, `Retreat`, and `Regroup` commands.
- Extended `AgentSquadCoordinator` with command assignments and fixed its missing `Clear()` implementation; the coordinator now respects its configured maximum size.
- Added `AgentKnowledgePropagationSystem` and `AgentSquadKnowledgeCoordinator` so selected high-value contextual facts can be shared between cooperating `VantaAgentBrain` instances without an external AI service.
- `VantaAgentBrain` now grounds `last_known_player` / `player_lost` knowledge facts from its own observations.
- `AIDirector` now retains the active contextual event, emits event lifecycle notifications, and resolves the event after its configured active duration before starting its cooldown.
- Added EditMode regression coverage for squad command arbitration, leader-missing regroup behavior, knowledge propagation, and coordinator capacity/clear behavior.
- Source-level implementation for this consolidated AI phase: IMPLEMENTED on `main`.
- Unity Editor compile, EditMode execution, Play Mode, NavMesh, build, and target-hardware profiling remain UNVERIFIED because no Unity runtime/Editor execution environment was available here.

## Consolidated playable-slice completion pass — source implementation
- Generated vertical-slice pipeline now creates `FirstContact` mission content with three sequential objectives and systemic consequences.
- Added `VerticalSliceMissionBootstrap` and `MissionObjectiveTrigger` so the generated district has an actual objective progression path rather than only mission framework APIs.
- Generated district now wires `AIDirector`, squad coordination, squad knowledge sharing, mission bootstrap, and mission save definitions.
- Fixed an editor setup compile issue where `CreateCamera` expected a `GameObject` but received `Transform`.
- Existing main menu, HUD, pause/settings, save/load, combat, movement, vehicles, world simulation, population, faction, wanted, AI and streaming foundations remain wired through the generated scene.
- Source implementation pass: IMPLEMENTED.
- GitHub Actions currently has zero workflow runs for this repository; therefore no CI/Unity execution evidence exists.
- Unity Editor import/compile, Test Runner execution, Play Mode, NavMesh runtime, mission runtime, vehicle runtime, performance profiling, and Windows `.exe` build remain UNVERIFIED. These require an actual Unity Editor/CI execution environment and cannot truthfully be marked complete from source inspection alone.

## Final verification / automated Windows build checkpoint — 2026-09-24
- Fixed generated vertical-slice AIDirector initialization ordering so WantedSystem exists before AIDirector.Awake() and its serialized references are assigned.
- Added automatic Windows build trigger on pushes to main.
- GitHub Actions Project Integrity workflow: PASS on the pre-build checkpoint.
- Automated Windows build was actually triggered for commit fb0de6a1852b83f3de224006d8f098bba848e92c and reached the Unity builder.
- Build result: BLOCKED before Unity project compilation because the repository has no configured Unity license secret/serial. The workflow log reports: Missing Unity License File and no Serial was found.
- Therefore no .exe artifact was produced by this run. Unity compile, EditMode, Play Mode, NavMesh runtime, and Windows executable remain UNVERIFIED until a licensed Unity activation is supplied to GitHub Actions or the project is opened/built in a local Unity Editor.