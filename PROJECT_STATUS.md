# VANTA: BLACK HORIZON — Development Status

## Repository checkpoint
- Default branch: main
- Active implementation branch: dev/core-gameplay-foundation
- Latest implementation checkpoint is on the development branch.
- Unity version: 2022.3.62f1
- main has not been used for implementation changes.
- Runtime Play Mode/build verification is still pending.

## Research completed
A gameplay reference comparison was added at Docs/GAMEPLAY_REFERENCE_COMPARISON.md, covering representative systems from GTA V, Red Dead Redemption 2, Cyberpunk 2077/Phantom Liberty, Watch Dogs 2, Far Cry 6, Assassin's Creed Shadows, Call of Duty: Black Ops 6 and Need for Speed Heat/Unbound.

## Latest implementation
- Camera-relative movement utility + regression tests
- Player camera-relative movement and smooth rotation
- Crouch capsule transition
- Third-person aim/FOV and collision
- Player camera auto-binding helper
- Weapon type taxonomy
- Weapon spread and pellet support
- Enemy chase/combat/damage/death loop
- Unity 2022.3-compatible Input System, Cinemachine, AI Navigation and Animation Rigging packages

## Major-system status
| System | Status |
|---|---|
| Player locomotion | PARTIAL — source implemented; runtime pending |
| Camera | PARTIAL — source implemented; runtime pending |
| Combat | PARTIAL — source implemented; runtime pending |
| Enemy AI | PARTIAL — source implemented; navigation runtime pending |
| HUD | MISSING |
| Menus | MISSING |
| Civilian AI | MISSING |
| Stealth | MISSING |
| Parkour | MISSING |
| Vehicles/traffic | MISSING |
| Police/wanted | MISSING |
| Missions | MISSING |
| Factions | MISSING |
| Economy/inventory | MISSING |
| Map/minimap | MISSING |
| World events | MISSING |
| Day/night/weather | MISSING |
| World streaming | MISSING |
| Save/load | MISSING |
| Cinematics | PARTIAL — Timeline package exists |
| Performance | PARTIAL — profiling work pending |

## Next gate
Build and wire a minimal playable test district, then verify:

Main Menu -> New Game -> Player -> Camera -> Move -> Aim -> Fire -> Enemy Damage -> Enemy Death -> Enemy Attack -> Player Damage -> Player Death -> Restart

Only after this runtime gate is demonstrated should vehicle/police systems be expanded.
