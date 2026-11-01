# Echo Maze: Build Guide

This guide explains how every part of the project is put together in the Unity Editor, using
the same steps as the course lectures. You can use it to:

- explain to your instructor how the level was built,
- rebuild any part of a level by hand, or
- change the maze (move, stretch or delete a wall, add spikes, and so on).

**Nothing in this project builds the level with code.** Every wall, spike, spinner, pad and light
is its own object in the scene's Hierarchy, with its own Transform values you can see and edit.
Scripts handle gameplay only (movement, Echoes, lives, win and lose).

---

## 1. What each lecture contributed

| Lecture (Moodle) | Technique | Where it is used in Echo Maze |
|---|---|---|
| Week 2: Get Started with the Unity Editor | Hierarchy, Inspector, Scene view, Transform tools, Play mode | Building and testing every scene |
| Week 4: Roll a Ball 1 | `PlayerController` with the **Player Input** component, `OnMove()`, `Rigidbody.AddForce()` in `FixedUpdate()` | Player ball movement (`Scripts/PlayerController.cs`) |
| Week 4: Roll a Ball 1 | `CameraController`: store an offset in `Start()`, follow in `LateUpdate()` | Camera that follows the ball (`Scripts/CameraController.cs`) |
| Week 4: Roll a Ball 1 | `Rotator`: `transform.Rotate(... * Time.deltaTime)` | Spinner arms and the spinning goal (`Scripts/Rotator.cs`) |
| Week 5: Roll a Ball 2 | Empty parent object (**Walls**), Cube walls set by Position and Scale, **Duplicate** (Ctrl/Cmd+D), wall material | All maze walls (`Maze > OuterWalls / InnerWalls`), `Materials/Walls.mat` |
| Week 5: Roll a Ball 2 | Collectible made from a Cube, tilted 45/45/45, its own material, `Rotator` | The green **Goal** cube |
| Week 5: Roll a Ball 2 | **Prefabs** in a `Prefabs` folder, placed many times under a parent (**PickUp Parent**), Top view and Global handles | `Prefabs/Spikes`, `Prefabs/Spinner`, placed under **Hazards** |
| Week 5: Roll a Ball 2 | Box Collider with **Is Trigger** off for walls (solid) | Every wall |
| Roll a Ball (collecting pickups, as in Assignment 1) | `OnTriggerEnter` + `CompareTag`, trigger colliders, UI text for score and win message | Hazards, goal, lives and message text |
| Week 1 & Week 3: Intro, MDA | Game design principles, Mechanics → Dynamics → Aesthetics | `Docs/GameDesignDocument.md` |

Two small things go a little past the lecture slides. Both are explained in the scripts' comments:
- `List<Vector3>` stores the player's recorded run, so an Echo can replay it.
- `SceneManager.LoadScene` moves to the next level, and `Keyboard.current` reads SPACE and R.

---

## 2. Scene layout (same in every level)

```
Level1 (scene)
├── Main Camera          CameraController (Player dragged into "player")
├── Directional Light
├── Ground               Plane, material Ground
├── Maze                 empty, Transform reset to (0,0,0)
│   ├── OuterWalls       Wall_North, Wall_South, Wall_West, Wall_East
│   └── InnerWalls       Wall_01, Wall_02, ...  (one Cube per straight wall)
├── StartPad             flat blue Cube (no collider, just a marker)
├── Goal                 green Cube, Box Collider (Is Trigger), tag "Goal", Rotator
├── Hazards              empty parent
│   ├── Spikes_01 ...    Spikes prefab
│   └── Spinner_01 ...   Spinner prefab
├── Player               Sphere + Rigidbody + Player Input + PlayerController, tag "Player"
├── Canvas               LivesText, EchoesText, MessageText (UI > Legacy > Text)
└── GameManager          empty object with the GameManager script
```

## 3. How to build a level by hand (step by step)

These are the same steps as Lecture 5 ("Setting up the Play Area"), repeated for a maze.

**Ground**
1. Hierarchy → right-click → **3D Object → Plane**. Rename it `Ground`. Reset its Transform.
2. A Plane is 10 × 10 units, so set Scale X = maze width ÷ 10 and Z = maze depth ÷ 10
   (Level 1 is 16 m × 12 m → Scale 1.6, 1, 1.2).
3. Drag `Materials/Ground` onto it.

**Outer walls** (exactly as in Lecture 5)
1. Right-click → **Create Empty** → rename `Maze` → Reset Transform. Inside it create empties `OuterWalls` and `InnerWalls`.
2. Right-click `OuterWalls` → **3D Object → Cube**, rename `Wall_West`.
   Set Position (-8, 0.6, 0) and Scale (0.3, 1.2, 12.3) for Level 1.
3. Drag `Materials/Walls` onto it.
4. **Duplicate** (Ctrl/Cmd + D) → rename `Wall_East` → Position X = 8.
5. Duplicate again → `Wall_North` → Position (0, 0.6, 6), Scale (16.3, 1.2, 0.3). Duplicate → `Wall_South` → Z = -6.

**Inner walls**
1. Switch the Scene view to **Top view** (click the Y arrow on the Scene Gizmo) and set the handles to **Global**.
2. Select any wall, **Duplicate**, drag it into `InnerWalls`, and rename it (`Wall_01`, `Wall_02` ...).
3. Type the Position and Scale from the tables below. A wall that runs left–right has a long **X** scale.
   A wall that runs up–down has a long **Z** scale. Thickness is always 0.3 and height is always 1.2.
4. Every wall length is (number of 2 m squares × 2) + 0.3. The extra 0.3 closes the corners.

**Start pad and goal**
- `StartPad`: Cube, Scale (1.6, 0.05, 1.6), material StartPad. Remove its Box Collider so the ball rolls over it smoothly.
- `Goal`: Cube, Rotation (45, 45, 45), Scale (0.8, 0.8, 0.8), material Goal. Tick **Is Trigger** on its Box Collider.
  Set Tag = `Goal`. Add the `Rotator` script, like the PickUp in Lecture 5.

**Hazards (prefabs, like the PickUp prefab)**
- Create an empty `Hazards`. Drag `Prefabs/Spikes` and `Prefabs/Spinner` from the Project window into it.
  Place each one using the tables. Duplicate with Ctrl/Cmd + D to add more.
- Spikes that lie along a left–right corridor are turned **Rotation Y = 90**.
- Change a spinner's speed on its **Rotator** component (Rotation Speed Y). A negative value spins the other way.

**Player**
1. **3D Object → Sphere**, rename `Player`, Tag = `Player`, Scale (0.7, 0.7, 0.7). Put it on the start pad at Y = 0.35.
2. Add **Rigidbody** (Linear Damping 2, Collision Detection Continuous).
3. Add **Player Input**. Set Actions = `InputSystem_Actions`, Default Map = `Player`, Behavior = **Send Messages**.
4. Add `PlayerController`, set Speed = 10, and drag the `GameManager` object into its *Game Manager* field.

**Camera**
- Main Camera Position = player position + (0, 12, -6), Rotation X = 63.4. Add `CameraController` and drag `Player` into it.

**UI** (Hierarchy → **UI → Legacy → Text**, which also creates the Canvas)
- `LivesText`: anchored top-left. `EchoesText`: below it. `MessageText`: middle of the screen, size 1500 × 500, centred.
- Canvas Scaler: **Scale With Screen Size**, 1920 × 1080.

**GameManager**
- Create Empty → `GameManager` → add the `GameManager` script. Drag in: Player, `Prefabs/Echo`, and the three Text objects.
- Type the Level Title, Level Tip and Next Scene Name (`Level2`, `Level3`, or leave it empty on the last level).

**Tags and build settings**
- Tags `Hazard` and `Goal` are added in *Edit → Project Settings → Tags and Layers*.
- *File → Build Profiles → Scene List* contains Level1, Level2, Level3, in that order.

## 4. Prefabs

| Prefab | Made of | Collider | Tag | Script |
|---|---|---|---|---|
| `Spikes` | Cube, Scale (0.6, 0.15, 1.4), red | Box, **Is Trigger** | Hazard | none |
| `Spinner` | Empty parent (Rotator, speed Y 60) → child `Pivot` (Cylinder 0.5/0.6/0.5, solid) + child `Arm` (Cube 4.4 × 0.35 × 0.3, orange) | Arm: Box, **Is Trigger** | Arm: Hazard | Rotator on the parent |
| `Echo` | Sphere, Scale 0.7, violet | Sphere, **Is Trigger** | Hazard | EchoGhost |

The Echo prefab is never placed in a scene. The GameManager creates one with `Instantiate` each time the player dies.

## 5. How to change the maze

- **Move a wall:** select it in `InnerWalls` and drag it with the Move tool (hold Ctrl/Cmd to snap), or type a new Position.
- **Open a gap:** shorten the wall's Scale, or delete the wall and add a shorter one.
- **Add a wall:** duplicate an existing wall and change its Position and Scale.
- **Make it harder:** add another `Spikes` prefab, or raise a spinner's Rotation Speed Y.
- After any change, press **Play** and roll through the new layout to check that you can still reach the goal.

The top-down maps in `Docs/LevelMaps/` show every level from above. One square is 2 m.

---

## 6. Every object in every level

Positions are in metres, rotations in degrees. Walls are Cubes with the `Walls` material and a solid Box Collider.

### Level1

| Object | Position (X, Y, Z) | Rotation | Scale (X, Y, Z) |
|---|---|---|---|
| Main Camera | (-7, 12, -1) | (63.4, 0, 0) | (1, 1, 1) |
| Ground | (0, 0, 0) | (0, 0, 0) | (1.6, 1, 1.2) |
| Wall_North | (0, 0.6, 6) | (0, 0, 0) | (16.3, 1.2, 0.3) |
| Wall_01 | (-4, 0.6, 4) | (0, 0, 0) | (8.3, 1.2, 0.3) |
| Wall_02 | (5, 0.6, 4) | (0, 0, 0) | (2.3, 1.2, 0.3) |
| Wall_03 | (-2, 0.6, 2) | (0, 0, 0) | (8.3, 1.2, 0.3) |
| Wall_04 | (7, 0.6, 2) | (0, 0, 0) | (2.3, 1.2, 0.3) |
| Wall_05 | (4, 0.6, 0) | (0, 0, 0) | (4.3, 1.2, 0.3) |
| Wall_06 | (-3, 0.6, -4) | (0, 0, 0) | (10.3, 1.2, 0.3) |
| Wall_South | (0, 0.6, -6) | (0, 0, 0) | (16.3, 1.2, 0.3) |
| Wall_West | (-8, 0.6, 0) | (0, 0, 0) | (0.3, 1.2, 12.3) |
| Wall_07 | (-6, 0.6, 0) | (0, 0, 0) | (0.3, 1.2, 4.3) |
| Wall_08 | (-4, 0.6, -3) | (0, 0, 0) | (0.3, 1.2, 2.3) |
| Wall_09 | (2, 0.6, 4) | (0, 0, 0) | (0.3, 1.2, 4.3) |
| Wall_10 | (4, 0.6, 0) | (0, 0, 0) | (0.3, 1.2, 8.3) |
| Wall_11 | (6, 0.6, -3) | (0, 0, 0) | (0.3, 1.2, 2.3) |
| Wall_East | (8, 0.6, 0) | (0, 0, 0) | (0.3, 1.2, 12.3) |
| StartPad | (-7, 0.03, 5) | (0, 0, 0) | (1.6, 0.05, 1.6) |
| Goal | (7, 0.6, 5) | (45, 45, 45) | (0.8, 0.8, 0.8) |
| Spikes_01 (prefab: Spikes) | (-1, 0.07, 4.45) | (0, 90, 0) | (1, 1, 1) |
| Spikes_02 (prefab: Spikes) | (-1, 0.07, -4.45) | (0, 90, 0) | (1, 1, 1) |
| Spikes_03 (prefab: Spikes) | (5, 0.07, -5.55) | (0, 90, 0) | (1, 1, 1) |
| Spinner_01 (prefab: Spinner) — Rotator speed Y = 50 | (-1, 0, -1) | (0, 0, 0) | (1, 1, 1) |
| Player | (-7, 0.35, 5) | (0, 0, 0) | (0.7, 0.7, 0.7) |

### Level2

| Object | Position (X, Y, Z) | Rotation | Scale (X, Y, Z) |
|---|---|---|---|
| Main Camera | (-9, 12, 1) | (63.4, 0, 0) | (1, 1, 1) |
| Ground | (0, 0, 0) | (0, 0, 0) | (2, 1, 1.6) |
| Wall_North | (0, 0.6, 8) | (0, 0, 0) | (20.3, 1.2, 0.3) |
| Wall_01 | (-7, 0.6, 6) | (0, 0, 0) | (6.3, 1.2, 0.3) |
| Wall_02 | (4, 0.6, 6) | (0, 0, 0) | (8.3, 1.2, 0.3) |
| Wall_03 | (0, 0.6, 4) | (0, 0, 0) | (8.3, 1.2, 0.3) |
| Wall_04 | (9, 0.6, 4) | (0, 0, 0) | (2.3, 1.2, 0.3) |
| Wall_05 | (-7, 0.6, 2) | (0, 0, 0) | (6.3, 1.2, 0.3) |
| Wall_06 | (7, 0.6, 2) | (0, 0, 0) | (2.3, 1.2, 0.3) |
| Wall_07 | (6, 0.6, 0) | (0, 0, 0) | (4.3, 1.2, 0.3) |
| Wall_08 | (-7, 0.6, -2) | (0, 0, 0) | (2.3, 1.2, 0.3) |
| Wall_09 | (-3, 0.6, -2) | (0, 0, 0) | (2.3, 1.2, 0.3) |
| Wall_10 | (2, 0.6, -2) | (0, 0, 0) | (4.3, 1.2, 0.3) |
| Wall_11 | (-9, 0.6, -4) | (0, 0, 0) | (2.3, 1.2, 0.3) |
| Wall_12 | (-1, 0.6, -4) | (0, 0, 0) | (6.3, 1.2, 0.3) |
| Wall_13 | (8, 0.6, -4) | (0, 0, 0) | (4.3, 1.2, 0.3) |
| Wall_14 | (-7, 0.6, -6) | (0, 0, 0) | (2.3, 1.2, 0.3) |
| Wall_15 | (-1, 0.6, -6) | (0, 0, 0) | (2.3, 1.2, 0.3) |
| Wall_16 | (4, 0.6, -6) | (0, 0, 0) | (4.3, 1.2, 0.3) |
| Wall_South | (0, 0.6, -8) | (0, 0, 0) | (20.3, 1.2, 0.3) |
| Wall_West | (-10, 0.6, 0) | (0, 0, 0) | (0.3, 1.2, 16.3) |
| Wall_17 | (-8, 0.6, 3) | (0, 0, 0) | (0.3, 1.2, 2.3) |
| Wall_18 | (-8, 0.6, -1) | (0, 0, 0) | (0.3, 1.2, 2.3) |
| Wall_19 | (-8, 0.6, -5) | (0, 0, 0) | (0.3, 1.2, 2.3) |
| Wall_20 | (-6, 0.6, 5) | (0, 0, 0) | (0.3, 1.2, 2.3) |
| Wall_21 | (-6, 0.6, -3) | (0, 0, 0) | (0.3, 1.2, 2.3) |
| Wall_22 | (-4, 0.6, 3) | (0, 0, 0) | (0.3, 1.2, 2.3) |
| Wall_23 | (-4, 0.6, -4) | (0, 0, 0) | (0.3, 1.2, 4.3) |
| Wall_24 | (-2, 0.6, 7) | (0, 0, 0) | (0.3, 1.2, 2.3) |
| Wall_25 | (2, 0.6, -5) | (0, 0, 0) | (0.3, 1.2, 2.3) |
| Wall_26 | (4, 0.6, 0) | (0, 0, 0) | (0.3, 1.2, 8.3) |
| Wall_27 | (6, 0.6, 4) | (0, 0, 0) | (0.3, 1.2, 4.3) |
| Wall_28 | (6, 0.6, -4) | (0, 0, 0) | (0.3, 1.2, 4.3) |
| Wall_29 | (8, 0.6, -1) | (0, 0, 0) | (0.3, 1.2, 2.3) |
| Wall_East | (10, 0.6, 0) | (0, 0, 0) | (0.3, 1.2, 16.3) |
| StartPad | (-9, 0.03, 7) | (0, 0, 0) | (1.6, 0.05, 1.6) |
| Goal | (-9, 0.6, -7) | (45, 45, 45) | (0.8, 0.8, 0.8) |
| Spikes_01 (prefab: Spikes) | (-1, 0.07, 7.55) | (0, 90, 0) | (1, 1, 1) |
| Spikes_02 (prefab: Spikes) | (-9.55, 0.07, -3) | (0, 0, 0) | (1, 1, 1) |
| Spikes_03 (prefab: Spikes) | (1, 0.07, -7.55) | (0, 90, 0) | (1, 1, 1) |
| Spikes_04 (prefab: Spikes) | (7.55, 0.07, 5) | (0, 0, 0) | (1, 1, 1) |
| Spinner_01 (prefab: Spinner) — Rotator speed Y = 60 | (-1, 0, 1) | (0, 0, 0) | (1, 1, 1) |
| Player | (-9, 0.35, 7) | (0, 0, 0) | (0.7, 0.7, 0.7) |

### Level3

| Object | Position (X, Y, Z) | Rotation | Scale (X, Y, Z) |
|---|---|---|---|
| Main Camera | (-11, 12, 2) | (63.4, 0, 0) | (1, 1, 1) |
| Ground | (0, 0, 0) | (0, 0, 0) | (2.4, 1, 1.8) |
| Wall_North | (0, 0.6, 9) | (0, 0, 0) | (24.3, 1.2, 0.3) |
| Wall_01 | (-8, 0.6, 7) | (0, 0, 0) | (8.3, 1.2, 0.3) |
| Wall_02 | (-1, 0.6, 7) | (0, 0, 0) | (2.3, 1.2, 0.3) |
| Wall_03 | (5, 0.6, 7) | (0, 0, 0) | (2.3, 1.2, 0.3) |
| Wall_04 | (-6, 0.6, 5) | (0, 0, 0) | (8.3, 1.2, 0.3) |
| Wall_05 | (2, 0.6, 5) | (0, 0, 0) | (4.3, 1.2, 0.3) |
| Wall_06 | (7, 0.6, 5) | (0, 0, 0) | (2.3, 1.2, 0.3) |
| Wall_07 | (4, 0.6, 3) | (0, 0, 0) | (4.3, 1.2, 0.3) |
| Wall_08 | (-9, 0.6, -1) | (0, 0, 0) | (2.3, 1.2, 0.3) |
| Wall_09 | (-2, 0.6, -1) | (0, 0, 0) | (4.3, 1.2, 0.3) |
| Wall_10 | (-11, 0.6, -3) | (0, 0, 0) | (2.3, 1.2, 0.3) |
| Wall_11 | (-5, 0.6, -3) | (0, 0, 0) | (6.3, 1.2, 0.3) |
| Wall_12 | (-6, 0.6, -5) | (0, 0, 0) | (8.3, 1.2, 0.3) |
| Wall_13 | (6, 0.6, -5) | (0, 0, 0) | (8.3, 1.2, 0.3) |
| Wall_14 | (-5, 0.6, -7) | (0, 0, 0) | (10.3, 1.2, 0.3) |
| Wall_15 | (6, 0.6, -7) | (0, 0, 0) | (8.3, 1.2, 0.3) |
| Wall_South | (0, 0.6, -9) | (0, 0, 0) | (24.3, 1.2, 0.3) |
| Wall_West | (-12, 0.6, 0) | (0, 0, 0) | (0.3, 1.2, 18.3) |
| Wall_16 | (-10, 0.6, 2) | (0, 0, 0) | (0.3, 1.2, 6.3) |
| Wall_17 | (-10, 0.6, -4) | (0, 0, 0) | (0.3, 1.2, 2.3) |
| Wall_18 | (-8, 0.6, -2) | (0, 0, 0) | (0.3, 1.2, 2.3) |
| Wall_19 | (-2, 0.6, 6) | (0, 0, 0) | (0.3, 1.2, 2.3) |
| Wall_20 | (0, 0.6, 0) | (0, 0, 0) | (0.3, 1.2, 14.3) |
| Wall_21 | (2, 0.6, 8) | (0, 0, 0) | (0.3, 1.2, 2.3) |
| Wall_22 | (2, 0.6, -1) | (0, 0, 0) | (0.3, 1.2, 8.3) |
| Wall_23 | (4, 0.6, 6) | (0, 0, 0) | (0.3, 1.2, 2.3) |
| Wall_24 | (8, 0.6, 5) | (0, 0, 0) | (0.3, 1.2, 8.3) |
| Wall_25 | (10, 0.6, 1) | (0, 0, 0) | (0.3, 1.2, 12.3) |
| Wall_East | (12, 0.6, 0) | (0, 0, 0) | (0.3, 1.2, 18.3) |
| StartPad | (-11, 0.03, 8) | (0, 0, 0) | (1.6, 0.05, 1.6) |
| Goal | (11, 0.6, 8) | (45, 45, 45) | (0.8, 0.8, 0.8) |
| Spikes_01 (prefab: Spikes) | (-3, 0.07, 8.55) | (0, 90, 0) | (1, 1, 1) |
| Spikes_02 (prefab: Spikes) | (5, 0.07, 7.45) | (0, 90, 0) | (1, 1, 1) |
| Spikes_03 (prefab: Spikes) | (-5, 0.07, -7.45) | (0, 90, 0) | (1, 1, 1) |
| Spikes_04 (prefab: Spikes) | (5, 0.07, -8.55) | (0, 90, 0) | (1, 1, 1) |
| Spikes_05 (prefab: Spikes) | (11.55, 0.07, 0) | (0, 0, 0) | (1, 1, 1) |
| Spinner_01 (prefab: Spinner) — Rotator speed Y = 75 | (-5, 0, 2) | (0, 0, 0) | (1, 1, 1) |
| Spinner_02 (prefab: Spinner) — Rotator speed Y = -75 | (5, 0, -2) | (0, 0, 0) | (1, 1, 1) |
| Player | (-11, 0.35, 8) | (0, 0, 0) | (0.7, 0.7, 0.7) |