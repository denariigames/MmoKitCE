# MmoKitCE: A Unity MMO framework designed to scale, stay stable, and remain secure

![image](Resources/MmoKitCE.png)

**MmoKitCE** is an _opinonated_ community edition distribution of [MMORPG Kit](https://github.com/suriyun-mmorpg/UnityMultiplayerARPG_Core). After the original asset was removed from the Unity Asset Store, Ittipon Teerapruettikulchai ([insthync](https://github.com/insthync)) open sourced his work. MmoKitCE exists to preserve, improve, and evolve this foundation, and will continue to pull improvements and fixes from his core repos into this distribution where it makes sense.

### The Three S's Guiding Principle

Every change, fix, or removal in MmoKitCE is evaluated against these core goals:

- **Scalability**: Can the system handle hundreds or thousands of concurrent players?
- **Stability**: Does it reduce bugs, crashes, edge cases, and unexpected behavior?
- **Security**: Does it harden the codebase against exploits, cheating, and data leaks?

**No other feature requests or enhancements** are considered unless they demonstrably advance one or more of these three goals. In fact, non-essential or problematic features may be **removed** or **moved to addons** if doing so improves any of the three S's.

## What's New in CE

### Addon Manager
Addon Manager is an in-editor interface that allows the community and team to modularize functionality.

- Former "core" features that were too niche, experimental, or optional can be extracted into addons.
- Addons are discovered, installed, and updated directly inside Unity, similar to a private Unity Package Manager.
- This keeps the **core distribution lean**, focused, and easier to maintain long-term.
- The Addon Manager pulls from a central, curated manifest (similar in spirit to how many modern Unity ecosystems work).


### Cell-Based Position Quantization
Cell-based position quantization dramatically improves network efficiency for entity movement.

- Significant bandwidth reduction: Position updates shrink from 12 bytes (full Vector3) to 7 bytes (1 byte cell ID + 6 bytes quantized local offset).
- Improved scalability: Lower network traffic supports more concurrent players, higher update rates, and denser entity populations.
- Strong foundation for spatial partitioning: Integer cell IDs enable fast, efficient grid-based systems such as Area of Interest (AOI) management, neighbor culling, and future sharding/zoning.
- Better determinism: Reduces floating-point drift over long distances and play sessions.

**World Size Assumptions:** The system uses a fixed square grid centered at the world origin. The maximum supported world size is determined by configurable CellSize. Positions outside the grid are clamped to edge cells.


## Quick Start / Installation Wizard

1. **Install dependencies**

<img width="631" height="263" alt="install-package" src="https://github.com/user-attachments/assets/6e63c1d8-7f65-4b10-9bcc-8bca07cbfe5e" />

Open Window → **Package Manager** and click **Add package from git URL**
```
https://github.com/denariigames/MmoKitCE_Installer.git
```

2. **Apply recommended project settings**

<img width="609" height="512" alt="setup-wizard" src="https://github.com/user-attachments/assets/cab53039-f83f-4034-82d3-d3a101b6afb2" />

A setup wizard will appear after the package is installed. If the Wizard does not appear or is inadventently closed, you can reopen at Window → MMORPG KIT → MMOKitCE → **Show Setup Wizard**

Click **Import Settings** to install base project settings. The following settings will be overwritten by this process:

 - ProjectSettings/DynamicsManager.asset
 - ProjectSettings/InputManager.asset
 - ProjectSettings/Physics2DManager.asset
 - ProjectSettings/ProjectSettings.asset
 - ProjectSettings/QualitySettings.asset
 - ProjectSettings/TagManager.asset
 - ProjectSettings/TimeManager.asset

The original Kit had additional settings which were not included in CE: AudioManager, EditorBuildSettings, EditorSettings, GraphicSettings, ShaderGraphSettings, UnityConnectSettings, VersionControlSettings

3. **Clone git repo**

The preferred method of installing CE is with a git subrepo within your project Assets directory. 

```sh
$ cd <your project>/Assets
$ git clone https://github.com/denariigames/MmoKitCE.git
```

After installation, browse available addons via the Addon Manager window (Window → MMORPG Kit → MmoKitCE → **Addon Manager**). Have fun building!

## Updating MmoKitCE

To update CE, simply use a git pull.

```sh
$ cd <your project>/Assets/MmoKitCE
$ git pull
```

## Thanks

Huge thanks to Ittipon Teerapruettikulchai for open sourcing the original kit. Without his act of generosity, none of this would exist. Special thanks to the entire community of former customers and new developers who continue to keep this ecosystem alive.
