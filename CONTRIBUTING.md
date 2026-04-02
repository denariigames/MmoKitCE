# Contributing to MmoKitCE

CE pulls from several repos using subtree (not submodule). **The master branch should always reflect the latest release, while development for the next release is queued in the develop branch.** You should create your own feature branch and pull request into develop.

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

