# Contributing to MmoKitCE

The master branch will always reflect the latest release, while development for the next release is queued in the develop branch. **You should create your own feature branch and pull request into develop.** All changes to develop and master (on release) branches requires a pull request.

1. **Start with an Issue**. Give a sensible name so that when a feature branch is created, you can tell what the branch is about.

2. Tag the issue with a label like scability, security or stability.
<img width="400" alt="image" src="https://github.com/user-attachments/assets/6ff91962-ff9f-415d-bd36-c2bdf6008834" />

3. Select MmoKitCE under Projects. This will place the issue on the [Kanban board](https://github.com/orgs/denariigames/projects/2) and allow fellow developers know what you are currently working on and whether it is ready for review.
<img width="372" height="229" alt="image" src="https://github.com/user-attachments/assets/323a1154-443f-43ce-a50b-a68bca7ff1b2" />
 
4. Under Development, click **Create a branch** and base it off of **develop**.
<img width="400" height="194" alt="image" src="https://github.com/user-attachments/assets/16bf6ede-48be-4ee5-91a5-5adf439f24d0" />

5. When you are done working on the issue, open a Pull Request on your branch.

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
