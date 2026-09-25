# VANTA: BLACK HORIZON — Gameplay Reference Comparison

This document compares VANTA against representative published open-world/action games using their publicly documented features. It is a design reference, not a copy target. All VANTA characters, maps, missions, dialogue, assets, factions, names, and story content remain original.

## Reference games and observed systems

### Grand Theft Auto V
Rockstar describes Los Santos as a criminal open-world setting with heists, multiple protagonists, vehicles, activities, social spaces and a broad progression ecosystem. GTA Online also demonstrates player-created/community missions and a large activity/update structure.

**Useful VANTA lessons**
- Layer main missions with repeatable activities.
- Treat vehicles, properties, economy and world activities as interconnected systems.
- Build a world where travel itself can generate encounters.
- Keep all VANTA narrative/content original.

### Red Dead Redemption 2
Rockstar's documented Red Dead Online updates show free-roam mission givers, mission variations, dynamic world events, contests and location-based activities.

**Useful VANTA lessons**
- Add a world-event director rather than only fixed mission markers.
- Allow event variants and reusable encounter templates.
- Make NPC/world activity exist independently of the main quest.

### Cyberpunk 2077 / Phantom Liberty
CD PROJEKT RED describes Phantom Liberty as adding a new district, skill tree, gigs, side quests, new equipment and repeatable open-world activities.

**Useful VANTA lessons**
- Structure the world as districts.
- Separate main missions, side missions, gigs and repeatable activities.
- Use district progression and unlocks rather than one giant quest list.

### Watch Dogs 2
Ubisoft describes a large dynamic Bay Area with San Francisco, Marin, Oakland and Silicon Valley, plus hacking of people, vehicles, infrastructure, drones and other connected devices.

**Useful VANTA lessons**
- World objects can become gameplay tools.
- Build systemic interactions between NPCs, vehicles, security and infrastructure.
- Allow multiple approaches to objectives.

### Far Cry 6
Ubisoft documents a large contrasted world, guerrilla camps, weapons, customizable vehicles, horses/tanks/DIY vehicles, resources, missions and faction conflict.

**Useful VANTA lessons**
- Safehouses/camps should be functional hubs.
- Factions should affect encounters and world control.
- Vehicles and resources should connect to progression.

### Assassin's Creed Shadows
Ubisoft documents cities, ports, shrines, rural areas, changing weather/seasons, reactive environments, stealth using noise/light/shadows, parkour, scouting, information gathering, allies and an objective-board exploration loop.

**Useful VANTA lessons**
- Build stealth perception around visibility, sound and investigation.
- Make information itself a progression/reward layer.
- Give exploration a structured objective board/map relationship.
- Add parkour routes and traversal opportunities to the environment.

### Call of Duty: Black Ops 6
Activision documents campaign set pieces, heists, spy activity, inventory/equipment, health/armor, safehouse upgrades and an evidence board used for mission planning.

**Useful VANTA lessons**
- Missions need more than a destination: use objectives, evidence/intel, equipment and set pieces.
- Safehouses can be gameplay systems, not decorative menus.
- Mission planning can expose optional objectives and rewards.

### Need for Speed Heat / Unbound
EA documents open-world racing, day/night risk loops, Bank/REP progression, heat, police pursuit, activities, garages, dealerships, map/minimap and vehicle customization.

**Useful VANTA lessons**
- Separate normal-world and high-risk driving states.
- Make wanted/heat escalation affect police composition.
- Give garages real progression value.
- Integrate races and vehicle activities into the open world.

## VANTA feature-gap matrix

| System | VANTA current branch | Target |
|---|---|---|
| Player locomotion | Partial | Camera-relative movement, sprint, stamina, crouch, jump |
| Third-person camera | Partial | Follow, look, aim, collision, shoulder modes |
| Combat | Partial | Five weapon classes, ammo, reload, spread, damage feedback |
| Enemy AI | Partial | Full perception + navigation + combat/search/flee loop |
| Civilian AI | Missing | Wander, react, flee, recover, schedules |
| Stealth | Missing | Visibility, sound, suspicion, takedown, alert propagation |
| Parkour | Missing | Vault, climb, ledge, mantle, drop, traversal routes |
| Vehicles | Missing | Driving, damage, enter/exit, camera, traffic |
| Police | Missing | Wanted 1–5, patrol, pursuit, search, escalation |
| Missions | Missing | Main/side/gig/event objectives, checkpoints, rewards/failure |
| Factions | Missing | Relationship matrix, reputation, territory and encounters |
| World events | Missing | Event director + templates + cooldowns |
| Districts | Missing | District metadata, ownership, activities, unlocks |
| Map | Missing | World map, minimap, markers, routing, filters |
| Safehouse | Missing | Save, gear, mission planning, upgrades |
| Economy | Missing | Money, vendors, garage, rewards, costs |
| Inventory | Missing | Weapons, ammo, equipment, consumables |
| Day/night | Missing | Time progression and gameplay hooks |
| Weather | Missing | Weather states + visibility/gameplay hooks |
| Streaming | Missing | Additive district/chunk streaming |
| Save/load | Missing | Persistent player/world/mission state |
| Cinematics | Partial/placeholder | Timeline-driven original sequences |
| Performance | Partial | Profiling, pooling, LOD, culling, streaming |

## Architecture derived from the comparison

VANTA should be built as interacting layers:

1. **Core simulation** — time, world state, save/load, events.
2. **Player** — locomotion, combat, stealth, traversal, inventory.
3. **AI** — perception, state machines, navigation, combat, schedules.
4. **World** — districts, interiors, traffic, civilians, activities.
5. **Systems** — factions, wanted, economy, missions, progression.
6. **Presentation** — camera, UI, audio, animation, cinematics.
7. **Persistence** — save slots and world-state serialization.
8. **Performance** — streaming, pooling, LOD, culling and profiling.

## Development priority

Do not implement all systems simultaneously.

### Gate A — Core combat sandbox
Player + camera + movement + health + weapon + enemy + restart.

### Gate B — AI sandbox
Navigation + perception + investigation + combat + search + civilian reaction.

### Gate C — Vehicle/police sandbox
Driving + traffic + wanted + police pursuit.

### Gate D — Mission/faction sandbox
Mission graph + objectives + rewards + factions + reputation.

### Gate E — District sandbox
Streaming + map + activities + safehouse + economy + world events.

### Gate F — Full open-world production
Expand districts, interiors, content, cinematics and optimization.

## IP boundary

These reference games are used only for publicly documented gameplay-system research. VANTA must not reproduce their protected characters, dialogue, missions, maps, logos, music, artwork, models, or storylines.


## Reference Research Wave 03 — requested game families

### Grand Theft Auto
GTA V's documented design centers on a large connected city/countryside/coast setting, mission-based progression, vehicles, activities and multiple approaches to open-world play. VANTA should borrow the systemic principle — travel, crime, economy, missions and encounters interacting — while keeping its city, characters and story original.

### Call of Duty
Modern COD material demonstrates tightly authored campaign spaces, multiple mission types, tactical movement, weapon/loadout progression, squad play and replayable combat systems. VANTA should use these lessons for mission set pieces, evidence/intel planning, loadout preparation and tactical squad behavior rather than copying campaign scenes or characters.

### Assassin's Creed
Ubisoft describes the franchise as open-world action-adventure with stealth and RPG elements, while Shadows adds strong exploration, parkour, light/noise/shadow stealth and environmental discovery. VANTA should expand traversal routes, information gathering, stealth perception and vertical districts.

### IGI
The name "IGI 3" is not a reliable current official title reference. For research, VANTA should treat the Project I.G.I. lineage and the officially announced *IGI Origins* as the relevant stealth/tactical reference. The useful design pattern is mission reconnaissance, controlled weapon use, infiltration, alarms and extraction — not reproducing a protected level or story.

### Need for Speed
EA documents deep visual/performance vehicle customization, tuning, risk/reward driving and police pursuit loops. VANTA should connect garage progression, vehicle handling, illegal races, heat and pursuit consequences.

### Asphalt
Asphalt 9 is explicitly arcade racing with accessible controls, large car rosters, progression, multiplayer and time-limited activities. VANTA can use an arcade event layer for optional street challenges while keeping its main vehicle physics coherent with the open-world simulation.

### GRID Autosport
GRID Autosport combines believable handling with multiple disciplines, career progression, vehicle tuning/upgrades and damage that can affect handling. VANTA should add distinct race archetypes, tuning trade-offs and mechanical damage consequences rather than one generic race activity.

### Max Payne
Rockstar documents precision shooting, Bullet Time, Shootdodge, individually modeled bullets and reactive injury presentation. VANTA now has a deterministic Bullet Time foundation that can later be connected to camera, input and combat presentation without copying Max Payne's story or presentation.

### Far Cry
Ubisoft describes open-world FPS play with multiple approaches such as stealth, close combat and sniping, plus diverse environments and systemic encounters. VANTA should use this as a reference for approach freedom, outposts/events, environmental variety and tactical combat.

### Tom Clancy
Splinter Cell is an explicit stealth-shooter reference, while the broader Tom Clancy family supplies tactical squad, reconnaissance and stealth patterns. VANTA should continue improving perception, squad roles, search behavior, cover and mission planning.

### Mafia
Mafia: Definitive Edition is a story-led crime saga set in a recreated 1930s city. Its useful lesson is authored cinematic mission pacing, period-specific world identity and character-driven progression. VANTA should use original factions, characters and districts.

### Sleeping Dogs
Sleeping Dogs combines an undercover-cop story with martial arts, gunfights, takedowns, driving, boats, illegal races and a dense Hong Kong setting. VANTA should expand close-quarters combat, vehicle activities and faction-cover consequences as original systems.

### Watch Dogs
Ubisoft describes a living city where connected infrastructure, vehicles and public systems can become gameplay tools. VANTA's new WorldInteractionModel is the first foundation for safe, game-owned infrastructure interactions; it deliberately does not execute arbitrary operating-system commands.

### Just Cause
Just Cause 3 documents vertical freedom through grappling, parachute and wingsuit traversal, vehicle variety and large-scale destructible chaos. VANTA now has a deterministic GrappleTraversalModel foundation; future integration should focus on traversal and controlled world-object interactions with performance-safe constraints.

## Graphics, physics and map lessons

Across these references, three patterns are particularly useful for VANTA:

1. **Layered maps:** a district should combine roads, alleys, interiors, rooftops, landmarks, traversal routes and mission/activity spaces rather than only a flat terrain grid.
2. **Physics with gameplay consequences:** vehicle damage should affect handling; traversal distance should be constrained; world interactions should change simulation state; combat should produce readable reactions.
3. **Presentation follows systems:** weather/time/lighting should affect visibility; police/world events should alter ambient activity; race and mission states should drive HUD/camera/audio feedback.

These are design references only. VANTA must use original characters, story, dialogue, maps, missions, logos, music, models, textures and other assets.

## Wave 03 implementation

The first implementation slice derived from this research adds deterministic foundations for:
- connected-world infrastructure interaction;
- Bullet Time as a reusable combat presentation state;
- bounded grapple traversal;
- ordered multi-lap racing events;
- Windows installer packaging through Inno Setup after a verified Unity Windows build.

Runtime wiring remains subject to Unity compile/Test Runner/Play Mode verification.
