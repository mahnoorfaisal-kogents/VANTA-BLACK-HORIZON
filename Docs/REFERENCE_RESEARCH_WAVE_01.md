# VANTA: BLACK HORIZON — Reference Research Wave 01

Date: 2026-09-24

This document records a systems-level study of official publisher/developer material. It is a design reference, not a request to copy protected characters, maps, dialogue, missions, logos, music, art, or other assets. VANTA uses original names, fiction, layouts, missions, art direction and implementation.

## Reference systems and what VANTA takes from the category

| Reference | Observed published system | VANTA adaptation |
|---|---|---|
| Grand Theft Auto V | Large urban/open-world structure, vehicles, missions, police/crime loop | District-based original city, vehicle sandbox, wanted system, mission graph |
| Red Dead Redemption 2 | Persistent world activities, systemic NPC/world interactions, travel/exploration | Persistent world-state layer, dynamic events, discovery and NPC schedules |
| Cyberpunk 2077 | District identity, vehicle variety, traffic, dynamic time-of-day behavior, quests | District metadata, traffic director, vehicle classes, time-aware world simulation |
| Watch Dogs 2 | Connected-device interaction and multi-neighborhood open world | Original network/utility interaction layer planned for infrastructure, drones and security systems |
| Far Cry 6 | Territory/camps, secret bases, customizable vehicles, weapon customization, loadouts | Safehouses, faction territory, garage customization, loadouts and weapon attachments |
| Assassin's Creed Shadows | Observation, information-driven exploration, safehouses, scouting, stealth via light/shadows, parkour | Discovery system, safehouses, information/scouting, shadow-aware stealth, traversal upgrades |
| Call of Duty: Black Ops 6 | High-intensity campaign spaces, heists/spy activity, Omnimovement | Mission set-piece framework, infiltration/heist missions, expanded traversal/movement |
| Need for Speed Unbound | Heat, police pressure, races, risk/reward cash, car customization, evolving activities | Pursuit escalation, race/event director, risk/reward economy, garage customization |

## Important constraints

1. Use official sources for factual reference whenever possible.
2. Reference gameplay systems, not protected content.
3. Never import another game's map, characters, dialogue, music, branding, or proprietary assets into VANTA.
4. Every feature is accepted only after source implementation, integration and runtime verification.
5. Research is continuous; this is Wave 01, not a claim that every game or every page on the internet has been exhausted.

## Official sources reviewed in this wave

- Rockstar Games — Grand Theft Auto V: https://www.rockstargames.com/gta-v
- Rockstar Games — Red Dead Redemption 2: https://www.rockstargames.com/reddeadredemption2
- Cyberpunk 2077 official universe: https://www.cyberpunk.net/
- Ubisoft — Watch Dogs 2: https://www.ubisoft.com/en-us/game/watch-dogs/watch-dogs-2
- Ubisoft — Far Cry 6: https://www.ubisoft.com/en-us/game/far-cry/far-cry-6
- Ubisoft — Assassin's Creed Shadows: https://www.ubisoft.com/en-us/game/assassins-creed/shadows
- Call of Duty — Black Ops 6: https://www.callofduty.com/blackops6
- EA — Need for Speed Unbound: https://www.ea.com/games/need-for-speed/need-for-speed-unbound

## Current VANTA gap map

### Already present
- Health/armor/death
- Third-person movement/camera/aim
- Five weapon categories and hitscan combat
- Enemy/civilian AI foundations
- Wanted level and police chase foundation
- Stealth/parkour foundations
- District definitions
- Dynamic world-event director
- Economy/inventory/factions
- Mission runtime states
- Save/load foundation
- Vehicle movement/interaction foundation

### Next implementation wave
- Real editor-generated playable district and menu
- Weapon asset catalog and environment materials
- Discovery/map knowledge and safehouse services
- Police pursuit states: dispatch, intercept, search, cooldown
- Traffic lanes and civilian vehicle population
- Mission objective graph/checkpoints
- Faction territory control and world-state effects
- Lighting/weather presentation
- Performance systems: pooling, LOD, culling and profiling
- Original content pack: districts, vehicles, weapons, missions, NPC archetypes and world events

## Design target

VANTA should become a systems-driven original open-world action sandbox where exploration, crime/heat, stealth, combat, driving, factions, missions and world events affect one another. “More advanced” means deeper interaction and systemic consistency, not copying or merely increasing the number of assets.
