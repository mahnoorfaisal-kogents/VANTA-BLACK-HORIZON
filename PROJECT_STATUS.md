# VANTA: BLACK HORIZON — Development Status

## Repository checkpoint
- Default branch: main
- Active implementation branch: dev/core-gameplay-foundation
- Unity: 2022.3.62f1
- main remains untouched by implementation work.
- Source-level implementation has expanded substantially; Unity Editor/Play Mode/build execution is still required for runtime verification.

## Research and architecture
Gameplay-system comparison is documented in `Docs/GAMEPLAY_REFERENCE_COMPARISON.md`. Reference categories were taken from representative published open-world/action games; VANTA content remains original.

## Implemented foundations

### Playable vertical-slice generation
- Editor command `VANTA/Build Playable Vertical Slice`
- Generates original `MainMenu` and `PlayableDistrict` scenes under `Assets/Generated`
- Generates environment materials and five weapon ScriptableObject assets
- Generates player, camera, enemy, civilians, vehicles, HUD and world-system wiring
- Configures Unity Build Settings with both generated scenes

### Core
- Health/armor/death contract
- Gameplay session states
- Movement math tests
- Camera-relative movement
- Sprint/stamina
- Crouch
- Jump/gravity
- Third-person camera follow/look
- Aim/FOV
- Camera collision
- Camera binding

### Combat/AI
- Data-driven weapon definitions
- Pistol/SMG/Assault Rifle/Shotgun/Precision Rifle taxonomy
- Hitscan damage
- Spread/pellets
- Magazine/reload
- Enemy detection/chase/combat/death
- Civilian wander/flee/recover foundation
- Wanted/heat 0–5 foundation
- Police chase foundation
- Stealth visibility foundation
- Parkour/vault foundation

### World/systems
- World clock + night detection
- Weather state foundation
- District definition data
- Dynamic world-event director
- World marker taxonomy
- Inventory
- Economy/cash
- Faction reputation/hostility
- Mission definitions + runtime mission states
- JSON save/load foundation

### Vehicles
- Rigidbody vehicle movement foundation
- Vehicle health
- Enter/exit interaction foundation

### Tooling
Unity 2022.3-compatible packages currently include Input System 1.6.1, Cinemachine 2.9.7, AI Navigation 1.1.4 and Animation Rigging 1.2.1.

## Systemic gameplay integration wave
- Mission consequence model: cash, XP, faction reputation, territory influence, wanted heat and intel reveals.
- World event lifecycle: Scheduled -> Active -> Resolved/Failed.
- Faction territory effects: danger multiplier, civilian threat and mission access state.
- Traffic population capacity/reuse model plus route registry.
- NPC schedule resolution including overnight schedules.
- Safehouse/garage service registry.
- Intel map reveal registry.
- Progression/XP level system.
- Wanted-level police escalation and pursuit coordinator.
- Persistent world snapshot foundation for time, weather, discoveries, missions and territory influence.
- Vertical-slice generator now wires systemic services and bakes an AI Navigation NavMesh surface during editor generation.
- Added deterministic integration tests for systemic state transitions.

## Verification truth
- Source implementation: completed for the current source-level scope.
- Deterministic tests: authored, but this environment does not provide a Unity Editor/Test Runner, so test execution is NOT claimed.
- Unity Play Mode: not executed in this environment.
- Windows .exe build: not executed in this environment.
- Runtime-generated scenes: generator updated, but generated scenes/NavMesh still require a real Unity Editor run for final acceptance.
- main branch: untouched; implementation remains on dev/core-gameplay-foundation.

## Remaining acceptance work
- Unity Editor import/compile verification
- Play Mode verification of generated scenes
- Windows x64 build and launch verification
- Runtime tuning for NavMesh agents, traffic, NPC schedules, police, vehicles and combat
- Full content pass: authored districts, missions, dialogue, audio, VFX, animation and art
- Performance pass: LOD, pooling, culling, profiling and memory budgets
- UI/HUD/settings polish and accessibility pass
- End-to-end save/load verification with persistent world state

## Quality gate
The next acceptance gate is a real playable district:
Main Menu -> New Game -> Player -> Camera -> Move -> Aim -> Fire -> Enemy Damage -> Enemy Death -> Enemy Attack -> Player Damage -> Player Death -> Restart.

A system is not labeled COMPLETE until source, scene/prefab integration and runtime verification are demonstrated.
