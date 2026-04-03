# Contributing to MmoKitCE

The master branch will always reflect the latest release, while development for the next release is queued in the develop branch. **You should create your own feature branch and pull request into develop.** All changes to develop and master (on release) branches requires a pull request.

### CE Structure
CE pulls from many source repos. Each of these forked repos has an upstream branch where modifications from the source repo can occur. The directory structure is flattened for use with subtree, instead of submodules, and placed into a sensible structure:

- previously nested MMO_Database, MMO_Server, and SharedData are now at the root
- preiouvsly nested repos under Core are moved to ThirdParty
- Tools include MmoKitCE specific tooling

Setting up the CE project looks like this:

```sh
$ git remote add core https://github.com/denariigames/UnityMultiplayerARPG_Core.git
$ git remote add mmo https://github.com/denariigames/UnityMultiplayerARPG_MMO.git
$ git remote add mmosrv https://github.com/denariigames/UnityMultiplayerARPG_MMOSource.git
$ git remote add mmodb https://github.com/denariigames/UnityMultiplayerARPG_DatabaseManagerSource.git
$ git remote add shared https://github.com/denariigames/UnityMultiplayerARPG_SharedData.git
$ git remote add aat https://github.com/denariigames/unity-addressable-asset-tools.git
$ git remote add audm https://github.com/denariigames/unity-audio-manager.git
$ git remote add cam https://github.com/denariigames/unity-camera-and-input.git
$ git remote add devex https://github.com/denariigames/unity-dev-extension.git
$ git remote add litenet https://github.com/denariigames/LiteNetLibManager.git
$ git remote add rest https://github.com/denariigames/unity-rest-client.git
$ git remote add scb https://github.com/denariigames/SerializableCallback.git
$ git remote add sps https://github.com/denariigames/unity-spatial-partitioning-systems.git
$ git remote add ueu https://github.com/denariigames/unity-editor-utils.git
$ git remote add ugs https://github.com/denariigames/unity-graphic-settings.git
$ git remote add uss https://github.com/denariigames/unity-serialization-surrogates.git
$ git remote add uum https://github.com/denariigames/unity-update-manager.git
$ git remote add xnode https://github.com/denariigames/xNode.git

$ git subtree add --prefix=Core core upstream
$ git subtree add --prefix=MMO mmo upstream
$ git subtree add --prefix=Server mmosrv upstream
$ git subtree add --prefix=Database mmodb upstream
$ git subtree add --prefix=SharedData shared upstream
$ git subtree add --prefix=ThirdParty/AudioManager audm upstream
...etc
```

### Updating CE from Source Repos
To pull the latest changes from upstream repos,

```sh
$ git subtree pull --prefix=Core core upstream
...etc
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

