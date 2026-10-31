# Unstable — Refactor Checklist

i'm going to implement crotching, pole climbing exactly at rain world, wall bouncing, water like rain world watcher, swim like rain world / gris

later i will add weapon, attacks, items, how to hold two items at once and store multiple items in inventory like animal well

## 1. Unified ID System

- [ ] Replace `CollectID` enum, `SessionID` SO, `EventID` enum with one `GameID : ScriptableObject`
- [ ] Update `WorldState`, `PlayerInventory`, `ItemPickup`, `SessionPickup`, `Enemy` to use `GameID`

## 2. Damageable Component

- [ ] Create `Damageable` with `health`, `OnHit` event, `OnDeath` event, `TakeDamage(int, Faction)`
- [ ] Add `Faction` enum (Player, Enemy, Neutral)
- [ ] Replace health logic in `Enemy`, `Breakable`, `PlayerHealth` with `Damageable`
- [ ] Update `Fireball` and `PlayerAttack` to call `damageable.TakeDamage()` instead of checking type

## 3. Input Abstraction

- [ ] Create `IMovementInput` interface (MoveDirection, JumpPressed, DashPressed, etc.)
- [ ] Create `PlayerInputProvider : MonoBehaviour, IMovementInput`
- [ ] Create `AIInputProvider : MonoBehaviour, IMovementInput`
- [ ] Refactor `PlayerWalk`, `PlayerJump`, `PlayerDash`, `PlayerClimb`, `PlayerSwim` to use `IMovementInput` instead of `InputManager` directly
- [ ] Rename Player movement scripts to generic names (`Moveable`, `Jumpable`, etc.)

## 4. State Machine

- [ ] Create `StateMachine` with states: Idle, Walk, Jump, Dash, Climb, Swim, Attack, Cast, Hurt, Dead
- [ ] Each state triggers events on Enter/Exit → `AnimationController` and `AudioController` listen
- [ ] Move input gating logic (e.g. can’t attack while climbing) into state transitions
- [ ] Remove direct `anim.SetTrigger()` calls from movement/action scripts

## 5. Event Decoupling

- [ ] `PlayerHealth.HandleDeath()` fires `OnDeath` event instead of directly disabling components
- [ ] `PlayerWalk`, `PlayerJump` subscribe to `OnDeath` and disable themselves
- [ ] Replace all `Update()` polls of `PlayerInventory.Instance` with `OnInventoryChanged` subscriptions
- [ ] Fix `HideInMM` — subscribe to `SceneManager.sceneLoaded` instead of checking every `Update()`

## 6. ScriptableObject Stats

- [ ] Create `PlayerStatsSO` with all player stats (damage, health, speeds, jump force, etc.)
- [ ] Create `EnemyStatsSO` per enemy type
- [ ] Move hardcoded values from `PlayerAttack`, `Fireball`, `Enemy` into their respective SOs

## 7. SceneLoader Cleanup

- [ ] Move `PlayerInventory.UpdateHealth(5)` and `UpdateFireballs(-10)` out of `SceneLoader`
- [ ] Add `PlayerInventory.ResetToDefault()` called from `WorldState.ResetSession()`

## 8. Boss Fix

- [ ] Remove manual `base.Update()` call in `Boss.Update()`
- [ ] Use `Enemy.OnDeath` event to trigger `DisplayFinalMessage()`

## 9. ItemPickup Fix

- [ ] Remove double `Destroy()` / double `MarkAsCollected()` call in `Execute()`

## 10. PlayerStates Protection

- [ ] Make state fields private with controlled setters or methods
- [ ] Prepare structure for State Machine migration

-----

**Priority order:** 1 → 2 → 3 → 5 → 6 → 7 → 8 → 9 → 4 → 10

State Machine (4) is last — everything else makes it easier to build.