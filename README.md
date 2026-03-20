# MmoKitCE: A Unity MMO framework designed to scale, stay stable, and remain secure

![image](Resources/MmoKitCE.png)

**MmoKitCE** is an _opinonated_ community edition distribution of [MMORPG Kit](https://github.com/suriyun-mmorpg/UnityMultiplayerARPG_Core). After the original asset was removed from the Unity Asset Store, Ittipon Teerapruettikulchai ([insthync](https://github.com/insthync)) open sourced his work. MmoKitCE exists to preserve, improve, and evolve this foundation, and will continue to pull improvements and fixes from his core repos into this distribution where it makes sense.

### The Three S's Guiding Principle

Every change, fix, or removal in MmoKitCE is evaluated strictly against these core goals:

- **Scalability**: Can the system handle hundreds or thousands of concurrent players?
- **Stability**: Does it reduce bugs, crashes, edge cases, and unexpected behavior?
- **Security**: Does it harden the codebase against exploits, cheating, and data leaks?

**No other feature requests or enhancements** are considered unless they demonstrably advance one or more of these three goals. In fact, non-essential or problematic features may be **removed** or **moved to addons** if doing so improves any of the three S's.

## What's New in CE

### Addon Manager
One of the biggest changes in the Community Edition is the introduction of the **Addon Manager**, an in-editor interface that allows the community and team to modularize functionality.

- Former "core" features that were too niche, experimental, or optional can be extracted into addons.
- Addons are discovered, installed, updated, and managed directly inside Unity, similar to a private Unity Package Manager.
- This keeps the **core distribution lean**, focused, and easier to maintain long-term.
- The Addon Manager pulls from a central, curated manifest (similar in spirit to how many modern Unity ecosystems work).

## Quick Start / Installation Wizard

1. **Install dependencies**

Open Window → **Package Manager** and click **Add package from git URL**
```
https://github.com/denariigames/MmoKitCE_Installer?path=com.mmokitce.installer
```

2. **Apply recommended project settings**

A setup wizard will appear automatically after the package is installed. Click **Import Settings** to install base project settings. The following settings will be overwritten by this process:

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

## Contributing to MmoKitCE

CE pulls from several repos as subtrees, forking from original UnityMultiplayerARPG repos. 

```sh
$ git remote add core https://github.com/denariigames/UnityMultiplayerARPG_Core.git
$ git remote add mmo https://github.com/denariigames/UnityMultiplayerARPG_MMO.git
$ git remote add mmosrv https://github.com/denariigames/UnityMultiplayerARPG_MMOSource.git
$ git remote add mmodb https://github.com/denariigames/UnityMultiplayerARPG_DatabaseManagerSource.git
$ git subtree add --prefix=Core core upstream
$ git subtree add --prefix=MMO mmo upstream
$ git subtree add --prefix=MMO_SRV mmosrv upstream
$ git subtree add --prefix=MMO_DB mmodb upstream
```

To pull the latest changes from upstream repos,

```sh
$ git subtree pull --prefix=Core core upstream
$ git subtree pull --prefix=MMO mmo upstream
$ git subtree pull --prefix=MMO_SRV mmosrv upstream
$ git subtree pull --prefix=MMO_DB mmodb upstream
```

### Preparing Upstream Repos

The main branch in each repo forks from a source repo, while the upstream branch is a flattened version with no submodules and any CE modifications from the original repo.

To prepare the upstream branch,

1. Switch to upstream branch. **All work is done in upstream branch, main is only to pull from suriyun.**
```sh
$ git checkout upstream
```

2. Expand submodules.
```sh
$ git submodule update --init --recursive
```

3. Recursively delete .git and .gitignore from submodules. Note this will throw an error that the formerly removed .git is no longer present. Keep running until command returns empty (nothing more to process).
```sh
$ git submodule foreach --recursive 'rm -rf .git'
$ git submodule foreach --recursive 'rm -rf .gitignore'
```

4. Remove .gitmodules
```sh
$ rm -f .gitmodules
```

5. Clean gitlink entries and submodule markers
```sh
$ git rm --cached -r . 2>/dev/null || true 
```

6. Add flattened submodules and commit
```sh
$ git add .
$ git commit -m"core upstream"
```

## Thanks

Huge thanks to Ittipon Teerapruettikulchai for open sourcing the original kit. Without this act of generosity, none of this would exist. Special thanks to the entire community of former customers and new developers who continue to keep this ecosystem alive.
