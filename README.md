# Thinklib

Library of mechanics and editor utilities for Unity games (Platformer, Topdown, and Point & Click).

## Requirements

* Unity **2022.3+**
* Dependencies resolved automatically via UPM:

  * TextMesh Pro
  * UGUI

## Installation (Git URL)

In Unity: **Window → Package Manager → + → Add package from Git URL…**

Use:

```txt
https://github.com/LibraryThinklib/thinklib-core.git#v0.1.2
```

## How to use (Editor menus)

When you import the package, you’ll see **four menus** at the top of Unity:

### Platformer

Tools for platform games.

* **Create Player/Enemy Animator Controller** – generates controllers with parameters, states, and transitions.
* **Saves to:** `Assets/Thinklib/Platformer/...`.

### Topdown

Tools for top-down (8-direction) games.

* **Create Player/Enemy Animator Controller** – controllers with preconfigured **2D Blend Trees (Horizontal/Vertical)**.
* **Saves to:** `Assets/Thinklib/Topdown/...`.

### Point Click (Point & Click)

Quick data creators for adventure/inventory games.

* **Create Item** – creates an **Item (ScriptableObject)**.
* **Create Combination Recipe** – creates a **CombinationRecipe (ScriptableObject)**.
* **Saves to:** `Assets/Thinklib/PointAndClick/...`.

### Thinklib

General utilities for the library.

* **Project/setup helpers** and **common tools**.

💡 **Tip:** All creators **automatically** create the folder chain under `Assets/Thinklib/...`.
⚠️ **Note:** If a menu **doesn’t show up**, check the **Console** — any compile error hides editor menus.
