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


### Login Manager
Login Manager is a clean separation of login/authentication logic from the central game servers.

- Impoved scalability: Concurrent login limit prevents the login server from being overwhelmed during spikes. The dedicated login server + cluster client allows independent scaling of auth traffic away from game logic.


### Sharded DatabaseNetworkManager
Added lanes, queueing, deferred/throttled saves, and a working in-memory cache. 

- Improved scalability: Vastly improved horizontal/concurrency scaling with sharded lanes + locks + ConcurrentDictionary support higher player counts and multi-threaded server ops without contention or overload. Limits (e.g., max saves/proceed) provide predictable load.


### Cell-Based Position Quantization
Cell-based position quantization dramatically improves network efficiency for entity movement.

- Improved scalability: Lower network traffic supports more concurrent players, higher update rates, and denser entity populations.
- LOD based compression: Close entities (the ones the player actually interacts with) keep high-precision modes, while distance entities (the majority in large MMO worlds) now send position data in as little as 4 bytes.

**World Size Assumptions:** The system uses a fixed square grid centered at the world origin. The maximum supported world size is determined by configurable CellSize. Positions outside the grid are clamped to edge cells.


### Jobs Movement Pipeline
All entity movement data processing converted from monothreaded per-entity updates to Unity Jobs + Burst parallel processing.

- Improved scalability: Combined with vector quantization and packed serialization, network payloads shrink dramatically, improving both server tick rate and bandwidth usage.
addons without touching core networking code.

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

## Yo! Where's the demo?

MmoKitCE includes BaseDemo, a minimal implementation that is intended to demonstrate scene setup. There is no content in the BaseDemo. For a more robust, developer-focused demo with content, check the Addon Manager for TinyEpicDemo.


## Updating MmoKitCE

To update CE, simply use a git pull.

```sh
$ cd <your project>/Assets/MmoKitCE
$ git pull
```

## Thanks

Huge thanks to Ittipon Teerapruettikulchai for open sourcing the original kit. Without his act of generosity, none of this would exist. Special thanks to the entire community of former customers and new developers who continue to keep this ecosystem alive.
