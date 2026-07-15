# Architecture Decision Records

This folder captures non-obvious design decisions made in
`Wolfgang.TryPattern` — the *why* behind choices whose motivation is not
self-evident from the code six months later.

## When to write an ADR

Add an ADR when a design decision:

- Trades off two or more reasonable alternatives (there IS another way
  that a reader might reach for first).
- Is load-bearing for future maintenance (breaking it silently breaks
  consumers, testing coverage, or ship-ability).
- Constrains future work (bans an API, pins a version, requires a
  specific shape).

Skip ADRs for choices where the code is its own documentation
(e.g. "use `List<T>` because we need Add"). ADRs are for the surprising
choices only.

## When to write it

**Alongside the PR that introduces the decision** — the ADR is part of
the review, not a post-hoc rationalisation. Retroactive ADRs are fine
for decisions already in place, but tag them clearly as such.

## Format

We use a compact [MADR](https://adr.github.io/madr/)-style four-section
template. See [`TEMPLATE.md`](TEMPLATE.md).

- **Number**: monotonically increasing, zero-padded to four digits
  (`0001`, `0002`, …). Never reused — even if an ADR is superseded, its
  number stays.
- **Filename**: `NNNN-short-kebab-slug.md`.
- **Status**: one of `Accepted` / `Superseded by ADR-NNNN` /
  `Deprecated`. Never `Draft` — if a decision is not accepted, the PR
  is not ready to merge.

## Index

The full list of ADRs, most recent first, lives in
[`index.md`](index.md). Keep it updated in the same PR that adds or
supersedes an ADR.

## Fleet convention

Some ADRs in this folder document Wolfgang.* fleet-wide decisions that
originated in `repo-template` (e.g. AssemblyVersion pinning for binding
stability). Those ADRs cross-reference the canonical repo-template
version. Others (e.g. `Result.Success` singleton caching) are genuinely
Try-Pattern-specific.
