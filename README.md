### Preparing MmoKitCE Repo

CE consists of upstreams which are setup as follows:

```sh
$ git remote add core https://github.com/denariigames/UnityMultiplayerARPG_Core.git
$ git subtree add --prefix=Core core upstream
```

### Preparing Upstream Repos

CE pulls from several repos as subtrees, forking from original UnityMultiplayerARPG repos. The main branch in each repo reflects what is in the source repo, while the upstream branch is a flattened version with no submodules and any CE modifications from the original repo.

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
