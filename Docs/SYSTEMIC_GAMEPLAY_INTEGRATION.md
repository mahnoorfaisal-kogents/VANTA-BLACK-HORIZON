# VANTA Systemic Gameplay Integration

This wave connects the game's systemic layers without copying protected content from reference games.

## Mission consequences
Mission outcomes can carry cash, XP, faction reputation, territory influence, wanted heat and intel reveals. The deterministic consequence model is testable independently of Unity scene objects.

## Dynamic world
World events now have an explicit lifecycle: Scheduled, Active, Resolved or Failed. Traffic has route registration plus a bounded population model. NPC schedules resolve activities across normal and overnight time windows.

## Factions and territory
Territory control feeds a deterministic world-effect model that can alter danger, civilian threat and mission access. This keeps consequences data-driven instead of hard-coding individual missions.

## Police
Wanted level maps to Patrol, Response, Tactical and Major escalation. The pursuit coordinator transitions between Dormant and Intercepting/other pursuit states while remaining deterministic for tests.

## Intel and progression
Scouting/discovery can feed the IntelMapSystem. Progression tracks XP and level thresholds. SafehouseGarageSystem exposes explicit service capabilities.

## Persistence
PersistentWorldState captures time, weather, discoveries, completed missions and territory influence. Full runtime serialization and restoration remain part of the Unity acceptance phase.

## Navigation
The editor vertical-slice generator now adds Unity AI Navigation's NavMeshSurface and invokes BuildNavMesh during scene generation. Unity's 2022.3 documentation lists AI Navigation 1.1.4 as a compatible release.

## Verification boundary
Source and deterministic test code are implemented. Unity Editor/Test Runner and Windows build execution are not available in the current execution environment, so runtime pass/build claims are intentionally withheld.
