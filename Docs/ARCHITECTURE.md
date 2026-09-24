# Architecture

The game is intentionally modular and data-driven. Runtime code is grouped by responsibility rather than by scene.

- Core: contracts and global gameplay state
- Player: locomotion and player health
- Camera: third-person camera/binding
- Combat: weapons and damage flow
- AI: enemy state and behavior
- World: scene/world bootstrap
- Tests: fast EditMode behavior tests

## Design rules

1. Systems communicate through small contracts/events where practical.
2. Weapon configuration belongs in WeaponData assets, not hard-coded per weapon.
3. Runtime claims require Play Mode/build evidence before being marked verified.
4. Original assets, characters, maps, missions, dialogue and branding only.
5. Do not turn placeholders into fake completed features.
