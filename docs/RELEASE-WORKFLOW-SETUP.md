# Release Workflow Setup Guide

This guide explains how to configure a repository to use the standard `release.yaml` workflow. The same checklist applies whether you are bootstrapping a new repo from `repo-template` or auditing an existing one.

## Overview

The release workflow triggers when you **publish a GitHub Release** and implements a comprehensive validation and automatic deployment process that:
- ✅ Tests all target frameworks per test project on Windows
- ✅ Enforces the two-tier coverage gate: 90% for production assemblies (`CODECOV_MINIMUM`) and 100% for test assemblies (`CODECOV_TEST_MINIMUM`)
- ✅ Validates NuGet package integrity with smoke tests
- ✅ Automatically publishes to NuGet.org after validation passes
- ✅ Eliminates duplicate build work for faster releases

## Required Configuration

Complete the following one-time setup so that the workflow can publish releases:

### Configure NuGet.org Trusted Publishing (no API key secret)

The workflow does **not** use a long-lived `NUGET_API_KEY` secret. The `publish-nuget` job requests a GitHub OIDC token (`permissions: id-token: write`) and `NuGet/login` exchanges it for a short-lived nuget.org publish token. Do not create a `NUGET_API_KEY` repository secret; nothing reads it.

**Location:** nuget.org → your account → **Trusted Publishing**

1. Add a trusted publishing policy for this repository:
   - **Owner:** `Chris-Wolfgang`
   - **Repository:** `Try-Pattern`
   - **Workflow file:** `release.yaml`
2. The package owner on nuget.org must be the account named in the workflow's `NuGet/login` step (`user: Chris-Wolfgang`).

**What this does:** Lets only `release.yaml` in this repository publish, with a token that expires shortly after the run. If the policy is missing or does not match, `NuGet/login` fails before any push is attempted. See the [NuGet Trusted Publishing docs](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing).

### Verify Branch Protection Rules

**Location:** Settings → Branches → main (or Settings → Rules → Rulesets)

> **Note:** This repo ships `scripts/Fix-BranchRuleset.ps1`, which inspects the existing branch rulesets, shows a plan, and (after confirmation) disables them and recreates the standard configuration. Review the plan before confirming. Otherwise configure the equivalent settings manually using the checklist below.

Ensure the following settings are enabled:

- ✅ **Require a pull request before merging**
  - **Single developer repos:** 0 approvals (default)
  - **Multi-developer repos:** 1+ approvals (recommended)
- ✅ **Require status checks to pass before merging**
  - Required checks should include the following status check contexts:
    - "Stage 1: Linux Tests (.NET 5.0-10.0) + Coverage Gate"
    - "Stage 2: Windows Tests (.NET 5.0-10.0, Framework 4.6.2-4.8.1)"
    - "Stage 3: macOS Tests (.NET 6.0-10.0)"
    - "Security Scan (DevSkim)"
    - "Security Scan (CodeQL) (csharp)"
    - "Secrets Scan (gitleaks)"
    - "Detect .NET Projects"
    - "ReSharper InspectCode"
  - `pr.yaml` also runs "Changelog Fragment Check"; it is not a required check on the `main` ruleset today.
- ✅ **Require branches to be up to date before merging**
- ✅ **Require conversation resolution before merging**
- ✅ **Do not allow bypassing the above settings** (recommended, even for admins)
- ✅ **Restrict deletions**
- ✅ **Require linear history** (optional but recommended)

**What this does:** Ensures all code merged to `main` has passed comprehensive validation, preventing broken releases.

## Testing the Release Workflow

After completing the setup, test the workflow by creating a GitHub Release:

1. Go to your repository's **Releases** page
2. Click **"Draft a new release"**
3. Choose or create a tag (e.g., `v0.0.1-test`)
4. Add a title and description (optional for a test)
5. Check **"Set as a pre-release"** for test releases
6. Click **"Publish release"**

The workflow triggers automatically when the release is published.

### Expected Workflow Behavior

1. **Job 1: validate-release** (3-10 minutes)
   - Runs all framework tests with coverage
   - Enforces the coverage gate (90% production, 100% test assemblies)
   - Uploads coverage report
   - ✅ Auto-passes if tests succeed

2. **Job 2: pack-and-validate** (2-5 minutes)
   - Packs NuGet packages
   - Performs smoke test installation
   - Uploads packages as artifacts
   - ✅ Auto-passes if packages are valid

3. **Job 3: verify-docs-build** (2-5 minutes)
   - Runs DocFX `metadata` + `build` to confirm the docs site builds
     cleanly against the release commit
   - Blocks the NuGet push if docs would fail to deploy
   - ✅ Auto-passes if DocFX completes without errors

4. **Job 4: publish-nuget** (1-2 minutes)
   - Exchanges the run's OIDC token for a short-lived nuget.org token (`NuGet/login`)
   - Publishes packages to NuGet.org automatically
   - ✅ Auto-completes if the Trusted Publishing policy matches

5. **Job 5: trigger-docs** (workflow_call)
   - Invokes `docfx.yaml` as a reusable workflow to deploy the just-
     released docs to `gh-pages` under `versions/<tag>/` and (when
     `deploy_as_latest` is true) `versions/latest/` + the site-root
     redirect

6. **Job 6: update-release-artifacts** (1-2 minutes)
   - Attaches the validated `.nupkg` + `.snupkg` artifacts to the
     GitHub Release object

### Monitoring the Workflow

- **Actions Tab:** Shows workflow progress in real-time
- **Artifacts:** Each job uploads artifacts (coverage reports, packages)
- **Releases:** Check the Releases page after successful completion

## Troubleshooting

### `NuGet/login` Fails in `publish-nuget`

**Problem:** The "Log in to NuGet.org via trusted publishing (OIDC)" step fails.

**Solution:**
1. On nuget.org, check the Trusted Publishing policy names owner `Chris-Wolfgang`, repository `Try-Pattern` and workflow `release.yaml` exactly
2. Check the job still has `permissions: id-token: write`
3. Re-run the workflow from the Actions tab (do not re-publish the release)

### Tests Fail on Specific Framework

**Problem:** Tests pass on some frameworks but fail on others (e.g., net462).

**Solution:**
1. Check the test logs for framework-specific issues
2. Fix compatibility issues in your code
3. Test locally: `dotnet test --framework net462`
4. Push fix, then re-publish the release (or re-run the workflow from the Actions tab)

### Coverage Below the Gate

**Problem:** Workflow fails at coverage validation step.

**Solution:**
1. Review `CoverageReport/Summary.txt` artifact
2. Add tests for uncovered code paths
3. Ensure tests run on all frameworks
4. Push fix, then re-publish the release (or re-run the workflow from the Actions tab)

### Smoke Test Fails to Install Package

**Problem:** Package packs successfully but fails smoke test installation.

**Solution:**
1. Check package dependencies in `.csproj`
2. Verify framework compatibility in `<TargetFrameworks>`
3. Test locally: `dotnet pack` then try installing in a test project
4. Fix packaging issues and re-publish the release (or re-run the workflow from the Actions tab)

## Production Release Checklist

Before creating a production GitHub Release (e.g., `v1.0.0`):

- [ ] All tests pass on all platforms (pr.yaml workflow)
- [ ] Code coverage meets the gate (90% production, 100% test assemblies)
- [ ] Security scan shows no critical issues
- [ ] Version numbers updated in `.csproj` files
- [ ] `CHANGELOG.md` updated with release notes (if applicable)
- [ ] All PRs merged to `main` branch
- [ ] Local build succeeds: `dotnet build --configuration Release`
- [ ] Local tests pass: `dotnet test --configuration Release`

**Create a production release:**
1. Go to your repository's **Releases** page
2. Click **"Draft a new release"**
3. Choose or create the version tag (e.g., `v1.0.0`) targeting `main`
4. Add a title and release notes
5. Click **"Publish release"**

**After workflow completes:**
- [ ] Verify packages appear on NuGet.org
- [ ] Test installing package from NuGet.org in a clean project
- [ ] Announce release (if applicable)

## Workflow Architecture

```
┌─────────────────────────────────────────────────────────────┐
│  Trigger: Published GitHub Release                          │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│  Job 1: validate-release (Windows)                          │
│  • Restore & Build                                          │
│  • Test all frameworks (net5.0-10.0, net462-481)           │
│  • Collect coverage                                         │
│  • Enforce coverage gate (90% src / 100% tests)             │
│  • Upload coverage artifacts                                │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼ (only if tests pass)
┌─────────────────────────────────────────────────────────────┐
│  Job 2: pack-and-validate (Windows)                         │
│  • Restore & Build (fresh)                                  │
│  • Pack NuGet packages                                      │
│  • Smoke test installation                                  │
│  • Upload package artifacts                                 │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼ (only if packing succeeds)
┌─────────────────────────────────────────────────────────────┐
│  Job 3: verify-docs-build (Ubuntu)                          │
│  • docfx metadata + docfx build                             │
│  • Blocks downstream jobs if docs would fail to deploy      │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼ (only if docs build succeeds)
┌─────────────────────────────────────────────────────────────┐
│  Job 4: publish-nuget (Windows)                             │
│  • Download packages                                        │
│  • NuGet/login: OIDC -> short-lived nuget.org token         │
│  • Publish to NuGet.org automatically                       │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼ (only if NuGet publish succeeds)
┌─────────────────────────────────────────────────────────────┐
│  Job 5: trigger-docs (workflow_call → docfx.yaml)           │
│  • Deploy _site/ to gh-pages /versions/<tag>/               │
│  • Deploy _site/ to gh-pages /versions/latest/              │
│  • Refresh root index.html (meta-refresh redirect)          │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│  Job 6: update-release-artifacts                            │
│  • Attach .nupkg + .snupkg to the GitHub Release            │
└─────────────────────────────────────────────────────────────┘
```

## Key Improvements Over Previous Workflow

| Issue | Before | After |
|-------|--------|-------|
| **Framework Coverage** | Default framework only | All frameworks (net5.0-10.0, net462-481) |
| **Code Coverage** | Not enforced | 90% src / 100% test assemblies enforced |
| **Package Validation** | None | Smoke test installation |
| **Deployment** | Incomplete publish script | Automatic publishing after validation |
| **Publish Credential** | Long-lived API key | OIDC Trusted Publishing, no standing secret |
| **GitHub Releases** | Not used as trigger | Workflow triggered by published release |
| **Build Efficiency** | Duplicate builds in each job | Build once per job with dependencies |
| **Test Logging** | No logger parameter | Console logging with verbosity |
| **Permissions** | Read-only | Write access for releases |

## Support

If you encounter issues not covered in this guide:

1. Check the Actions tab of this repository on GitHub for detailed logs
2. Review artifacts uploaded by failed jobs
3. Consult the [GitHub Actions documentation](https://docs.github.com/en/actions)
4. Open an issue in this repository with:
   - Workflow run URL
   - Error message and logs
   - Steps to reproduce
