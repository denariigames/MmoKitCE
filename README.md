![image](MmoKitCE.png)

**MmoKitCE** is an _opinonated_ community edition distribution of [MMORPG Kit](https://github.com/suriyun-mmorpg/UnityMultiplayerARPG_Core). Kit developer Ittipon Teerapruettikulchai ([insthync](https://github.com/insthync)) open sourced his work after removing the asset from the Unity store, and we former customers are eternally grateful for this generosity. CE will continue to pull improvements and fixes from his core repos into this distribution.

The guiding principle behind CE are the three "S"es, and any changes from the core repos will be towards these goals:

- Scalability
- Stability
- Security

No other feature requests or enhancements will be considered unless it moves CE closer toward these goals. In fact, core functionality may be _removed_ if it helps improve in these regards.

## Installing MmoKitCE

The preferred method of installing CE is with a git subrepo within your project Assets directory.

```sh
$ cd <your project>/Assets
$ git clone https://github.com/denariigames/MmoKitCE.git
```

CE includes a MmoKitCE_Settings.unitypackage which will install base project settings. After your project is up and running, you can safely delete this file. The following settings will be overwritten by this process:

- ProjectSettings/DynamicsManager.asset
- ProjectSettings/InputManager.asset
- ProjectSettings/Physics2DManager.asset
- ProjectSettings/ProjectSettings.asset
- ProjectSettings/QualitySettings.asset
- ProjectSettings/TagManager.asset
- ProjectSettings/TimeManager.asset

The original Kit had additional settings which were not included in CE: AudioManager, EditorBuildSettings, EditorSettings, GraphicSettings, ShaderGraphSettings, UnityConnectSettings, VersionControlSettings

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
$ git subtree add --prefix=Core core upstream
$ git subtree add --prefix=MMO mmo upstream
```

To pull the latest changes from upstream repos,

```sh
$ git subtree pull --prefix=Core core upstream
$ git subtree pull --prefix=MMO mmo upstream
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
