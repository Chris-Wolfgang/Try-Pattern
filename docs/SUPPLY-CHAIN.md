# Supply-chain integrity

`Wolfgang.TryPattern` ships four independent supply-chain signals with
every release. A consumer can verify all four to prove that the
package they downloaded from NuGet.org was built from this repo at the
tagged commit, with no injection at any link.

## What's shipped

Every GitHub Release attaches:

| Artifact                          | Purpose                                                                                          |
| --------------------------------- | ------------------------------------------------------------------------------------------------ |
| `Wolfgang.TryPattern.<v>.nupkg`   | The main package.                                                                                |
| `Wolfgang.TryPattern.<v>.snupkg`  | Symbol package with SourceLink-embedded PDBs.                                                    |
| `Wolfgang.TryPattern.bom.json`    | CycloneDX SBOM listing every transitive dependency.                                              |
| `reproducible-build-manifest.json`| SHA-256 of each `.nupkg` / `.snupkg` / `lib/<tfm>/*.dll` + SDK / commit / timestamp. See #193.   |
| Sigstore build-provenance         | SLSA level 3 attestation via `actions/attest-build-provenance`. See `gh attestation`.            |

## What we do NOT ship (yet)

- **Author code-signing** via a certificate (NuGet trusted signers).
  Requires paid procurement + KeyVault setup + user account action; deferred
  pending an explicit decision. Sigstore provenance below covers the "built
  by this repo" chain-of-custody; a code-signing cert would additionally
  cover "signed by named author".

## Verification procedure

### 1. Verify the SBOM

```bash
gh release download <tag> -R Chris-Wolfgang/Try-Pattern \
  -p 'Wolfgang.TryPattern.bom.json'
# CycloneDX viewer (any CycloneDX-compatible tool) or plain jq:
jq '.components[] | {name: .name, version: .version, license: (.licenses[0].expression // .licenses[0].license.id)}' \
  Wolfgang.TryPattern.bom.json
```

Compare the listed transitive dependencies against your own security
requirements. The `license-audit.yaml` workflow enforces the licence
allowlist inside our CI (see [`docs/../.github/license/`](../.github/license)),
but you can re-run the check locally on your own copy.

### 2. Verify the reproducible-build manifest

Rebuild locally and compare hashes — see
[`REPRODUCIBLE-BUILD.md`](REPRODUCIBLE-BUILD.md) for the full step-by-step.

### 3. Verify the SLSA build-provenance attestation

`actions/attest-build-provenance` produces a Sigstore-signed attestation
cryptographically linking the `.nupkg` to this repo + commit + workflow
run. Verify with the GitHub CLI:

```bash
gh release download <tag> -R Chris-Wolfgang/Try-Pattern \
  -p 'Wolfgang.TryPattern.*.nupkg'
gh attestation verify Wolfgang.TryPattern.<version>.nupkg \
  --repo Chris-Wolfgang/Try-Pattern
```

Successful output includes the workflow name (`Release on Published
Release`), the commit SHA the release was cut from, and the OIDC
identity of the workflow that produced the attestation. Any mismatch
(different repo, tampered file, forged attestation) fails verification.

### 4. Verify SourceLink resolves for every embedded document

For F11 step-into to work in an IDE, the SourceLink URLs baked into
the `.pdb` files (inside the `.snupkg`) must resolve to actual source
content. Our `sourcelink-verify.yaml` workflow verifies this on every
build; a consumer can re-verify the shipped `.snupkg` at their end
using [`sourcelink test`](https://github.com/ctaggart/SourceLink).

## Reporting a discrepancy

Open a `security` issue if any of the four verification steps fails
for a shipped release. Include the tag, the output of the failed
`gh attestation verify` / SBOM diff / hash mismatch, and your local
`gh --version` / `dotnet --info`.

See [SECURITY.md](../SECURITY.md) for the "Release path & compromise
scope" appendix — that section is the on-call reference for the
release identity being compromised.
