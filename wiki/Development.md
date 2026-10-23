# Development

How AFMA's applications are stored, built and released.

| Page | Status |
|---|---|
| [Repositories and Branches](/Development/Repositories-and-Branches) | **VERIFIED** 2026-09-14 |
| [Build and Environment](/Development/Build-and-Environment) | **VERIFIED** — and the verdict is that neither application builds |
| [Deployment Process](/Development/Deployment-Process) | **GAP** — the actual process is undocumented |

## The short version

- Two repositories, `AFMA-STAT` and `AFMA-MMS`, plus an empty `AFMA-WEB`.
- `AFMA-MMS` is a fork of `AFMA-STAT` with no shared history. They have diverged.
- **Neither repository can be built from a clean clone.** Both fail dependency restore on private
  packages we do not hold.
- There is no CI, no CD, no test suite and no branch policy.
- Deployment is a manual Visual Studio folder publish from a developer's machine, by a route nobody
  has written down.

Everything above is verified, not inferred. See [Build and Environment](/Development/Build-and-Environment)
for the commands and their output.
