# 🎮 The Marble Game

> **A Hybrid 3D/2D Physics Adventure built in Unity**
>
> *Featuring Custom Physics, HLSL Shaders from First Principles, and Predictive AI.*

<p align="center">
  <img src="media/Camera%20banking%20tilt.gif" width="100%" />
</p>

## 📖 Overview
This project is a technical showcase demonstrating advanced Unity mechanics across three distinct levels. It moves beyond standard assets by implementing core systems—such as camera tracking, procedural generation, and lighting shaders—entirely from scratch.

| Feature | Description |
| :--- | :--- |
| **Dual-Genre** | Seamlessly transitions between 3D exploration and 2D Side-scrolling. |
| **Custom Tech** | Shaders and Physics algorithms written from first principles. |
| **AI Systems** | Predictive targeting algorithms that calculate future intercept points. |

---

## 🕹️ Controls & Mechanics
* **Movement:** `W, A, S, D` (Relative to Camera)
* **Jump:** `Space` (ForceMode.Impulse)
* **Sprint:** `Shift` (Level 2 Only)
* **Pause:** `Esc` (TimeScale manipulation)

---

## 🔭 Level 1: 3D Physics & Dynamic Camera

<p align="center">
  <img src="media/Physics%20sim.gif" width="48%" />
  <img src="media/Basket%20drop.gif" width="48%" />
</p>

**Focus:** *Vector Mathematics & Player Control*

The camera system uses a custom split-axis script (`CamX.cs` and `CamY.cs`) to create a dynamic, "weighted" feel.

* **Physics Banking:** The camera tilts on the Z-axis based on the marble's lateral acceleration (dot product of acceleration and camera right vector).
* **Velocity Tracking:** `CamX` smooths position tracking while handling collision offsets.
* **Star System:** A weighted scoring system requiring both collectibles and speed (e.g., 3 Stars = All Coins + < 60s).

> **Code Highlight: Camera Banking**
> ```csharp
> // Calculates tilt based on lateral acceleration
> float lateralAccel = Vector3.Dot(flatAcceleration, right);
> velocityTiltZ = Mathf.Clamp(lateralAccel * tiltAngle, -maxTilt, maxTilt);
> ```

---

## 🏃 Level 2: 2D Procedural Generation

<p align="center">
  <img src="media/Camera%20banking%20tilt%202d%20level.gif" width="48%" />
  <img src="media/2d%20bumper%20interaction.gif" width="48%" />
</p>

**Focus:** *Algorithms & Custom Graphics Pipeline*

This level acts as an infinite 2D side-scroller.

### 🔄 The "Connector" Algorithm
The level generates infinitely using a segment-matching system (`GenerateNextPlatform.cs`).
1.  Platforms are named with connector logic (e.g., `A.B.Variant`).
2.  The system identifies the "End" connector of the current piece.
3.  It filters the library for pieces with a matching "Start" connector.
4.  **Memory Management:** Maintains a dynamic queue of 8 active platforms, destroying old geometry to optimize performance.

### 🎨 Custom Shaders (HLSL)
Three shaders were written entirely from first principles (no Shader Graph):
1.  **Sunset Skybox:** Procedural gradient generation with horizon blending.
2.  **Unlit Laser:** Texture coordinate manipulation (`uv.y += _Time.y`) for scrolling animation.
3.  **Toon Shader:** A two-pass rendering technique.
    * *Pass 1:* Vertex extrusion for black outlines.
    * *Pass 2:* Posterized lighting (stepped gradients) for the cel-shaded look.

---

## 👾 Level 3: The Predictive AI Boss

<p align="center">
  <img src="media/Level%203%20demo.gif" width="48%" />
  <img src="media/Predictive%20laser.gif" width="48%" />
</p>

**Focus:** *Artificial Intelligence & Vector Prediction*

The final level features a 1v1 battle against an AI marble using a **Negative Delta Prediction** algorithm.

### 🧠 How the AI Works
Instead of aiming at the player, the Boss calculates where the player *will be*.
1.  **Velocity Extraction:** Reads the player's `Rigidbody.linearVelocity`.
2.  **Prediction:** Projects the target position forward based on projectile speed.
3.  **Negative Delta:** Mirrors the predicted vector to "cut off" the player's escape route.
4.  **Detection:** Uses `Physics.OverlapCapsule` combined with Raycasts for precise, fair hit detection.

> **Code Highlight: Predictive Targeting**
> ```csharp
> // Calculate negative delta to mirror player trajectory
> Vector3 delta = rb.linearVelocity * predictionTime;
> Vector3 interceptPoint = playerPos - delta; 
> ```

---

## 🎨 Custom Assets: Blender Workflow

<p align="center">
  <img src="media/NPC%20rig%20made%20from%20first%20principles%20.png" width="48%" />
  <img src="media/Custom%20animation%20for%20ai%20made%20in%20blender%20dopsheet.png" width="48%" />
</p>

**Focus:** *3D Modeling, Rigging & Animation*

To ensure the project remained entirely original, no pre-made asset packs (like Mixamo) were used for the enemy characters. The AI entities were built from first principles in **Blender**.

* **Custom Rigging:** The enemy models were manually rigged with custom bone structures to support specific gameplay movements.
* **Dope Sheet Animation:** Attack cycles, idle states, and movement patterns were hand-keyed using Blender's Dope Sheet to synchronize perfectly with the Unity State Machine logic.

---

## 🛠️ Technical Implementation
* **Physics:** Uses `ForceMode.Acceleration` for movement and `ForceMode.Impulse` for jumps to simulate realistic mass (5kg marble).
* **Assets:** NPC Rig and animations created manually in **Blender**.
* **Architecture:** Abstracted "Shooting System" base class reused for both Level 1 Turrets and Level 3 Boss.

---

## 👥 Credits
* **Developer:** Kirone Gopaul, Job Ko, Keegan Naidoo, Matthew Mendes
* **Engine:** Unity 6
* **Language:** C# / HLSL
* **3D Tools:** Blender (Modeling/Rigging)

---
*Thank you for playing!*
