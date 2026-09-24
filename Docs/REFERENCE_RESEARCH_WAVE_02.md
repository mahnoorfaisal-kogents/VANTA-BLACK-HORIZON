# VANTA: BLACK HORIZON — Reference Research Wave 02

Date: 2026-09-24

## New official-source findings

### Assassin's Creed Shadows
Official material describes unpredictable weather, changing seasons, reactive environments, stealth based on noise/light/shadows, scouting/Observe information, limited initial map information, scouts that reveal objectives, specialized allies, and expanded parkour including prone movement and physics-based grappling.

VANTA direction: weather/time/lighting affect visibility; Discovery/Observe reveals information; Scouts become an original intel resource; parkour expands into mantle/ledge/grapple traversal.

Sources:
- https://www.ubisoft.com/en-us/game/assassins-creed/shadows
- https://www.ubisoft.com/en-us/game/assassins-creed/news/4GKS1MvpHS3f7ebKWfxHmt
- https://www.ubisoft.com/en-us/game/assassins-creed/news/4TA6gKaTvtOC1mOjZIxCZd

### Watch Dogs 2
Official material emphasizes a dynamic open world and manipulation of connected infrastructure, vehicles, drones, cranes, security robots and other devices.

VANTA direction: original World Interaction/Network Device layer; non-lethal infrastructure interactions; devices affect traffic/security/mission state; no arbitrary shell/system execution.

Source:
- https://www.ubisoft.com/en-us/game/watch-dogs/watch-dogs-2

### Call of Duty: Black Ops 6
Official material highlights mission variety, different approaches to mission spaces, replayability, Omnimovement, slides/dives and improved loadout/weapon systems.

VANTA direction: mission objective graphs with approach tags; directional slide/dive/stance transitions; first-class loadout data; stealth and combat routes in mission spaces.

Sources:
- https://www.callofduty.com/en/store/games/blackops6
- https://www.callofduty.com/blog/2024/06/black-ops-6-worldwide-reveal-essential-intel-on-campaign-multiplayer-and-zombies
- https://www.callofduty.com/blog/2024/08/call-of-duty-next-black-ops-6-reveal-global-systems-key-innovations-announcement

### Need for Speed Unbound
Official material highlights heat, tactical police escapes, risk/reward racing, vehicle customization and Cops vs Racers with Pursuit Tech such as spike strips and EMPs.

VANTA direction: explicit pursuit state machine; unlockable pursuit-tech data; racing risk/reward economy; original vehicle archetypes and garage customization slots.

Sources:
- https://www.ea.com/en/games/need-for-speed/need-for-speed-unbound/features
- https://www.ea.com/games/need-for-speed/need-for-speed-unbound/news/vol8-cops-vs-racers-update

## Implemented from Wave 02
- DiscoverySystem and DiscoveryPoint
- PursuitStateMachine
- Police dispatch/intercept/search/cooldown behavior
- WeaponLoadout
- VehicleArchetype taxonomy
- PursuitTechSystem
- deterministic tests for new foundations
- procedural playable district generation
- five-weapon asset catalog

## Plugin/tool strategy
Unity-side implementation is designed to work with Unity MCP/Editor automation when the local Unity project is available. The documented installation command is:

unity-mcp-cli install-plugin <UnityProjectRoot>

Do not claim the MCP plugin is installed or connected until actual local output verifies it.

## Next research targets
- Rockstar open-world systemic NPC/vehicle/world design
- Ubisoft systemic AI, traversal and world activities
- EA/Need for Speed vehicle handling and pursuit design
- Unity official AI Navigation, Input System, Cinemachine and Addressables documentation
- original asset pipeline and low-cost asset workflows
- audio, VFX and animation systems suitable for modest target hardware
