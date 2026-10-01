---
sessionId: session-260927-163350-pk5i
---

# Requirements

### Overview & Goals
The goal of this task is to enable the player to damage and destroy both asteroids and enemies using player weapons (the player laser). Resolving this ensures that player attacks reliably detect targets, apply damage, decrement target health, and trigger object destruction (`Die()` / `Explode()` / `QueueFree()`) without runtime exceptions.

### Scope
- **In Scope:**
  - Updating `scenes/EnemyScript.cs` to trigger `Die()` (`QueueFree()`) when `Health` reaches `<= 0` upon taking damage.
  - Refactoring `scenes/Laser.cs` (player laser) to properly detect collisions with `Asteroid` and `EnemyScript` / `EnemyBody` targets and dispatch damage directly to hit instances.
  - Cleaning up invalid node queries and unused dummy allocations in `scenes/Laser.cs`.
  - Verifying the build with `dotnet build`.
- **Out of Scope:**
  - Redesigning asteroid spawn distribution or enemy AI pathfinding algorithms.
  - Modifying HUD/UI health bars or audio effects.

### User Stories
- As a player, I want my laser attacks to deal damage to asteroids and enemy ships so that I can eliminate threats and clear space hazards.
- As a game developer, I want weapon collision detection and damage dispatch to be direct and type-safe, preventing null reference and node lookup exceptions.

### Functional Requirements
- When the player presses the shoot action (`ui_shoot`), `Laser.cs` must cast its raycast and render the beam.
- If the raycast collides with an `Asteroid`, it must call `TakeDamage(damage)` on that specific `Asteroid` instance, destroying it via `Explode()` when its health reaches `0` or below.
- If the raycast collides with an enemy (`EnemyScript` directly or child `EnemyBody`), it must call `TakeDamage(damage)` on that enemy instance.
- When an enemy ship's `Health` reaches `0` or below, `EnemyScript.Die()` must be triggered to remove the enemy node via `QueueFree()`.
- Invalid node lookups (`GetNode<EnemyScript>("Enemy")`) and redundant allocations (`new PlayerController()`) must be removed from `Laser.cs`.

### Non-Functional Requirements
- **Reliability:** Eliminate `NullReferenceException` and `NodeNotFoundException` during collision detection and damage processing.
- **Maintainability:** Ensure consistent method naming (`TakeDamage`) and robust node hierarchy traversal across Godot scenes.
- **Performance:** Avoid unnecessary allocations inside frame-by-frame physics processing.

# Technical Design

### Current Implementation
In the current codebase:
1. In `scenes/EnemyScript.cs`, the `takeDamage(int damage)` method decrements `Health`, but the `if (Health <= 0)` block is empty, meaning the enemy never dies when its health is depleted.
2. In `scenes/Laser.cs`, `_Ready()` attempts to run `enemyController = GetNode<EnemyScript>("Enemy");`, which fails because `Enemy` is not a child of `Laser`.
3. In `scenes/Laser.cs` `_PhysicsProcess()`, when `ui_shoot` is active and `laser.IsColliding() && laser.GetCollider() is Asteroid`, it calls `asteroid.TakeDamage(damage)` on an uninitialized private field `_asteroid` (`null`), throwing a `NullReferenceException`.
4. In `scenes/Laser.cs`, there is no check or damage call for enemy colliders.
5. In `scenes/Laser.cs`, lines 60–69 create a dummy `new PlayerController()` instance and query colliders every frame outside the shoot input check.

### Key Decisions
- **Direct Collider Casting & Hierarchy Resolution:** `Laser.cs` will retrieve `laser.GetCollider()`, check if it is an `Asteroid` or `EnemyScript` (or a child of `EnemyScript`, such as `EnemyBody`), and directly invoke `TakeDamage(damage)` on the detected entity.
- **Encapsulated Entity Lifecycles:**
  - `Asteroid.TakeDamage(int damage)` calls `Explode()` (`QueueFree()`) when `Health <= 0`.
  - `EnemyScript.TakeDamage(int damage)` will call `Die()` (`QueueFree()`) when `Health <= 0`.
- **Clean Physics Processing:** Remove dummy allocations and unassigned fields in `Laser.cs` to ensure clean, bug-free execution.

### Proposed Changes

#### `scenes/EnemyScript.cs`
- Standardize and implement `TakeDamage(int damage)`:
  ```csharp
  public void TakeDamage(int damage)
  {
      Health -= damage;
      if (Health <= 0)
      {
          Die();
      }
  }

  public void takeDamage(int damage) => TakeDamage(damage);
  ```

#### `scenes/Laser.cs`
- Remove unused fields and broken node retrievals (`enemyController = GetNode<EnemyScript>("Enemy");`, unassigned `asteroid` field).
- In `_PhysicsProcess(double delta)`, update laser firing logic:
  ```csharp
  if (Input.IsActionPressed("ui_shoot"))
  {
      isActive = true;
      line2d.Visible = true;
      UpdateBeamLength();
      line2d.SetPointPosition(1, new Vector2(beamLength, 0));

      if (laser.IsColliding())
      {
          var collider = laser.GetCollider();
          if (collider is Asteroid hitAsteroid)
          {
              hitAsteroid.TakeDamage(damage);
          }
          else if (collider is EnemyScript directEnemy)
          {
              directEnemy.TakeDamage(damage);
          }
          else if (collider is Node node && node.GetParent() is EnemyScript parentEnemy)
          {
              parentEnemy.TakeDamage(damage);
          }
      }
  }
  ```
- Remove lines 60–69 (`new PlayerController()`, redundant collider check).

### Data Models / Contracts
```csharp
// Asteroid damage interface
public void TakeDamage(int damage);
public void Explode();

// EnemyScript damage interface
public void TakeDamage(int damage);
public void Die();

// Laser weapon firing
public override void _PhysicsProcess(double delta);
```

### Components
- **`Laser` (`Node2D`)**: Attached to `Player`, controls beam visual and raycast hit detection against asteroids and enemies.
- **`Asteroid` (`RigidBody2D`)**: Space hazard entity that takes damage and explodes.
- **`EnemyScript` (`Node2D`) / `EnemyBody` (`CharacterBody2D`)**: Enemy entity that takes damage and dies when health depletes.

### File Structure
- `scenes/EnemyScript.cs`: Modified to handle damage and call `Die()` upon health depletion.
- `scenes/Laser.cs`: Modified to fix collision detection, direct damage dispatch, and remove broken node lookups.

### Risks & Mitigations
- **Enemy Collision Structure:** The enemy collision shape is on `EnemyBody` (`CharacterBody2D`), which is a child of the `Enemy` root node (`EnemyScript`). Checking both direct `EnemyScript` instances and parent `EnemyScript` nodes ensures hits are always handled regardless of scene structure.

# Testing

### Validation Approach
Verification will be performed through static code analysis, compiler checks with `dotnet build`, and collision/damage path tracing.

### Key Scenarios
1. **Player Shoots Asteroid:**
   - Player aims at an `Asteroid` and presses `ui_shoot`.
   - `Laser` raycast collides with `Asteroid`.
   - `hitAsteroid.TakeDamage(damage)` reduces asteroid health.
   - When health reaches `<= 0`, `Explode()` removes the asteroid via `QueueFree()`.
2. **Player Shoots Enemy:**
   - Player aims at an `Enemy` ship and presses `ui_shoot`.
   - `Laser` raycast collides with `EnemyBody` (or `Enemy`).
   - `EnemyScript.TakeDamage(damage)` reduces enemy health.
   - When health reaches `<= 0`, `Die()` removes the enemy via `QueueFree()`.
3. **Player Laser Misses:**
   - Player fires into empty space; beam extends to max range without null reference errors.

### Edge Cases
- Target is freed during active firing: laser safely handles null/freed colliders on subsequent frames without exceptions.
- Rapid firing against multiple targets: each frame's raycast applies damage directly to the current colliding instance.

# Delivery Steps

### ✓ Step 1: Implement EnemyScript damage handling and death lifecycle
`EnemyScript` accurately reduces health on taking damage and eliminates the enemy ship upon reaching zero health.

- Update `TakeDamage(int damage)` (and alias `takeDamage`) in `scenes/EnemyScript.cs` to subtract damage from `Health`.
- Call `Die()` inside `TakeDamage()` when `Health <= 0` so that `QueueFree()` is cleanly invoked.
- Ensure null and validity safety for any referenced player or laser instances.

### ✓ Step 2: Refactor player Laser collision detection and damage dispatch to Asteroids and Enemies
`scenes/Laser.cs` reliably detects raycast collisions with asteroids and enemies and applies damage directly without null reference exceptions.

- Remove broken child node lookups (`GetNode<EnemyScript>("Enemy")`) from `Laser._Ready()`.
- Refactor the raycast collision handling in `Laser._PhysicsProcess()` under `Input.IsActionPressed("ui_shoot")` to dynamically inspect `laser.GetCollider()`.
- Dispatch damage to `hitAsteroid.TakeDamage(damage)` if the collider is an `Asteroid`.
- Dispatch damage to `enemy.TakeDamage(damage)` if the collider is `EnemyScript` or a child node whose parent is `EnemyScript` (`EnemyBody`).
- Remove redundant dummy `PlayerController` instantiations and orphaned collider checks.

### ✓ Step 3: Validate combat interactions and build integrity
The entire combat loop allowing the player to target, damage, and eliminate enemies and asteroids compiles cleanly and operates without runtime errors.

- Run `dotnet build` to verify clean compilation with 0 errors and 0 warnings.
- Confirm type casting and safe damage delivery paths for both direct node colliders and nested parent bodies.