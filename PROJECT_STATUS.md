# VANTA: BLACK HORIZON — Development Status

## Repository checkpoint

- Default branch: main
- Active implementation branch: dev/core-gameplay-foundation
- Current work starts from an intentionally minimal repository.
- No claim is made that a Unity scene/build has been runtime-verified from GitHub alone.

## Implemented in this checkpoint

- Unity 2022.3 project/package metadata
- Damage contract (IDamageable)
- Health/armor/death events
- Player movement: walk, sprint, crouch, jump, gravity, grounded handling, stamina
- Third-person mouse camera follow/look
- Data-driven weapon definition
- Hitscan weapon firing, magazine, fire-rate gate, reload, damage application
- Enemy state model and baseline detection/chase/death transitions
- Game session state model
- Runtime/test assembly definitions
- EditMode tests for enemy state transitions

## Explicitly not yet verified

- Unity Editor import
- Scene wiring
- Prefab wiring
- Play Mode runtime
- Actual damage hit in a built scene
- Windows executable build
- Full AI combat/cover/pathfinding
- Vehicles, traffic, police/wanted, missions, factions, economy, save/load, world streaming, weather and advanced UI

## Next gate

Open the project in Unity, create/wire the first playable scene, run the EditMode tests, then verify the core loop:

Start -> Player moves -> Camera follows -> Aim -> Fire -> Enemy takes damage -> Enemy dies -> Player can be damaged -> Player death/restart

Only after that gate passes should vehicles/police/missions and open-world streaming be layered on.
