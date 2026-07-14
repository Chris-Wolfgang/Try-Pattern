# Disaster Recovery Procedure

Documented procedures for responding to a compromise of the NuGet or
GitHub account that publishes `Wolfgang.TryPattern`. Time-critical
actions are easy to fumble under stress — this file exists so you don't
have to invent the recovery steps at 2 AM after a breach.

> **Canonical source**: this document is fleet-wide and lives in
> `repo-template`. The Try-Pattern copy is a synced snapshot. Fixes
> that apply to every Wolfgang.* repo should go to `repo-template`
> first and fan out; Try-Pattern-specific carve-outs (if any) go in a
> `## Try-Pattern-specific` section at the end of THIS file, never
> edited-only-here.

## Contents

1. [Account ownership](#account-ownership)
2. [Credential locations](#credential-locations)
3. [If NUGET_API_KEY is compromised](#if-nuget_api_key-is-compromised)
4. [If a malicious package version is published](#if-a-malicious-package-version-is-published)
5. [If the GitHub account is compromised](#if-the-github-account-is-compromised)
6. [Consumer communication template](#consumer-communication-template)
7. [Post-incident checklist](#post-incident-checklist)
8. [Quarterly review](#quarterly-review)

## Account ownership

| System | Owner | Backup contact |
|---|---|---|
| **nuget.org account** publishing `Wolfgang.*` packages | Chris Wolfgang (personal account) | *(none — single-owner)* |
| **github.com/Chris-Wolfgang** organization | Chris Wolfgang | *(none — personal account)* |
| **Chris Wolfgang email** used for both accounts | Chris Wolfgang | *(none)* |

**Single-owner exposure is a known risk.** In a real incident where
the owner is unreachable, there is no side channel to revoke a
compromised key or unlist a package. The mitigation is
[NuGet trusted publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing)
(GitHub OIDC), which removes the standing `NUGET_API_KEY` secret from
GitHub entirely — see the "Future work" section at the bottom.

## Credential locations

**NUGET_API_KEY** (only NuGet credential currently in use):

- **Storage**: GitHub Actions secret in each publishing repo
  (`Chris-Wolfgang/Try-Pattern` → *Settings → Secrets and variables →
  Actions → Repository secrets → NUGET_API_KEY*).
- **Not stored anywhere else**: not in a password manager, not in
  local machine keychains, not in any documentation. The key never
  leaves nuget.org → GitHub Actions.
- **Rotation cadence**: yearly by convention. Set on the nuget.org
  side to expire after 12 months; a fresh key must be minted and
  pasted into every publishing repo's GitHub Secrets before the old
  one expires.
- **Scope on the nuget.org side**: "Push new packages and package
  versions" only. NEVER "Full access" (which would allow unlisting
  and delete). Unlist actions must go through a signed-in nuget.org
  browser session, not via the API key.

**GitHub Personal Access Tokens**:

- Not used for release publishing (workflows use the built-in
  `GITHUB_TOKEN` for repo operations and `NUGET_API_KEY` for the
  external push).
- If PATs are in use for local `gh` CLI work, they are unrelated to
  release publishing and don't need urgent revocation on a NuGet
  compromise.

**SSH keys**:

- Personal SSH keys on the owner's development machine push commits
  to `Chris-Wolfgang/*` repos. A compromised SSH key permits
  arbitrary pushes but cannot publish to NuGet directly — the NuGet
  push only happens via GitHub Actions using the repo secret.

## If NUGET_API_KEY is compromised

**Do these steps IN ORDER — the revoke must happen before the
package publish loop can be exploited further.**

1. **REVOKE the key immediately.** Sign in to https://www.nuget.org
   → *Account → API Keys* → find the key → *Delete*. This
   invalidates it globally within minutes.
2. **Mint a replacement key** on the same page. Scope: "Push new
   packages and package versions". Expiration: 365 days. Copy the
   token (it is shown only once).
3. **Rotate the GitHub Actions secret** in every publishing repo:
   - `Chris-Wolfgang/Try-Pattern` → *Settings → Secrets and variables
     → Actions → NUGET_API_KEY* → *Update*.
   - Repeat for every other Wolfgang.* library repo that publishes to
     NuGet. See
     `Wolfgang-fleet-repos.md` in `repo-template` for the current
     list (or `gh repo list Chris-Wolfgang --no-archived --limit 100
     --json name`).
4. **Verify** by re-running the most recent release workflow (or a
   dry-run publish) — the workflow's "Validate NUGET_API_KEY is
   configured" step should still pass.
5. **Audit recent published versions.** Go to
   https://www.nuget.org/packages/Wolfgang.TryPattern and check that
   every listed version corresponds to a real `v*` tag on
   `Chris-Wolfgang/Try-Pattern`. Repeat for every published package.
   Any version that was pushed via the compromised key but does NOT
   have a matching git tag is malicious.

## If a malicious package version is published

Compromised keys can push new versions. Deleted versions cannot be
recovered by name (the version number is burned forever), so the
correct action is **unlist**, not delete.

1. **UNLIST the malicious version first** to stop new installs. Sign
   in to nuget.org → the package page → *Manage Package* → find the
   version → *Unlist*. Unlisted versions do not appear in search or
   default installs but existing lock files still resolve to them.
2. **Publish an advisory** via
   [GitHub Security Advisories](https://github.com/Chris-Wolfgang/Try-Pattern/security/advisories)
   → *New draft security advisory*. Fill in:
   - Affected version(s) — the malicious version and any version
     range that transitively depends on it.
   - Description — plain English of what the malicious version does
     if executed.
   - Severity — pick honestly; a package that exfiltrates env vars
     is High, one that appends to log files is Low.
   - CVE — request one via the advisory UI. GitHub coordinates with
     Mitre.
3. **Contact NuGet support** at
   [support@nuget.org](mailto:support@nuget.org) to request a
   permanent takedown of the malicious version if unlisting is not
   enough (e.g. exploited in the wild by lock-file version pin). The
   NuGet team can hard-block a specific version from being resolved
   by clients that still list it in their `packages.lock.json`.
4. **Publish a clean superseding version** with the malicious code
   removed. Bump the PATCH number past the malicious one so
   consumers on `>=`- or floating-version references pick up the
   fix automatically. Reference the security advisory in the release
   notes.
5. **Post the consumer communication template** (below) via the
   channels listed in it.

## If the GitHub account is compromised

A compromised GitHub account can push malicious code to `main`,
trigger the release workflow, and thus publish a malicious NuGet
package via the legitimate `NUGET_API_KEY`. The NuGet-side steps
above still apply; you also need to secure the GitHub side.

1. **Reset the account password** and **rotate all authentication
   factors**: revoke and re-enroll every 2FA method
   (https://github.com/settings/security).
2. **Revoke all Personal Access Tokens** at
   https://github.com/settings/tokens. Re-issue only the ones you
   actually need, each with the narrowest scopes.
3. **Revoke all SSH keys** at https://github.com/settings/keys.
   Re-add the current machine's key only.
4. **Revoke all OAuth apps** at
   https://github.com/settings/applications. Re-authorize
   individually as needed.
5. **Review the security audit log** at
   https://github.com/settings/security-log for unfamiliar activity
   in the days before you noticed the compromise. Look for
   `oauth_authorization.create`, `public_key.create`,
   `personal_access_token.create`, `repo.change_merge_setting`,
   `protected_branch.destroy`, and any push events from unknown
   sources.
6. **Force-push-recover** any repository whose `main` has been
   corrupted by the compromise. Use the last-known-good commit from
   local machine or a fresh clone made before the compromise
   timestamp. Notify consumers if you have to force-push a public
   branch (see communication template).
7. **Notify GitHub Support** at
   https://support.github.com/contact — they can help audit the
   scope of the compromise, restore branches, and coordinate with
   nuget.org if the malicious NuGet publish went through the same
   incident.

## Consumer communication template

Paste, fill in the bracketed sections, and post via **every channel**
consumers use to reach the project (GitHub Security Advisories tab,
the affected repo's README banner, X/Mastodon if applicable).

```
Security notice — Wolfgang.TryPattern [affected version(s)]

On [DATE], version [X.Y.Z] of Wolfgang.TryPattern was published from
a compromised account. That version has been unlisted from nuget.org
and a security advisory is published at [ADVISORY URL].

Impact: [one paragraph — what the malicious code does if run].

If you have installed [X.Y.Z]:
1. Uninstall / roll back to [LAST GOOD VERSION].
2. Rotate any secrets that may have been exposed to the compromised
   binary [ONLY IF the malicious code exfiltrated data — otherwise
   remove this line].
3. Purge the malicious .nupkg from any local package caches
   (e.g. ~/.nuget/packages/wolfgang.trypattern/[X.Y.Z]).

A clean superseding version [X.Y.Z+1] has been published with the
malicious code removed. Consumers on floating or >= version
references will pick it up on next restore.

We are sorry for the disruption. Post-mortem to follow at
[POST-MORTEM URL, if applicable].
```

## Post-incident checklist

After the immediate response is complete, close the loop:

- [ ] All rotated credentials verified in a green release run.
- [ ] Malicious version unlisted; superseding version published.
- [ ] Security advisory in `Accepted` state with a CVE.
- [ ] Consumer notification posted in every channel.
- [ ] GitHub security audit log reviewed end-to-end for the incident
      window.
- [ ] Post-mortem drafted (even if internal-only) — what allowed the
      compromise, what worked in the response, what to improve.
- [ ] This document updated with any procedure changes discovered
      during the response, then fanned out to the rest of the fleet
      via `repo-template`.

## Quarterly review

Every **March / June / September / December** (aligned with a
convenient calendar quarter), spend ~15 minutes on this checklist:

- [ ] Are the URLs in this document still correct?
      (`support@nuget.org`, security-audit-log path, advisories URL,
      etc.)
- [ ] Is the NuGet API key on schedule to expire in > 3 months? If
      not, rotate proactively per the "If NUGET_API_KEY is
      compromised" flow — rotation for calendar reasons is the same
      procedure as rotation for compromise.
- [ ] Is anyone new on the notification list (support contacts, CVE
      reviewers)? Update.
- [ ] Any fleet-wide changes to the credential locations or account
      ownership? Sync via `repo-template`.
- [ ] Any incidents in the past quarter with lessons learned? Fold
      into this doc.

## Future work — NuGet trusted publishing (OIDC)

The current NuGet auth model (`NUGET_API_KEY` GitHub secret) has a
standing credential that survives across releases. If GitHub is
compromised, the key can be used to publish immediately.

[NuGet trusted publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing)
uses GitHub Actions OIDC to mint a short-lived token PER RELEASE from
nuget.org, so there is no standing secret to compromise. The
`ETL-SqlBulkCopy` repo has piloted this successfully (v0.1.0 shipped
via OIDC without a `NUGET_API_KEY` secret). Once the pattern is
proven across a few more repos, this document will be updated to
reflect the trusted-publishing flow — most of the "If NUGET_API_KEY
is compromised" section becomes obsolete under that model, since
there is no key to rotate.
