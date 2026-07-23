# Errors

## [ERR-20260723-001] git-diff-check-before-init

**Logged**: 2026-07-23T11:20:00+02:00
**Priority**: low
**Status**: resolved
**Area**: config

### Summary

The whitespace gate was called before the new project had a Git repository.

### Error

```text
warning: Not a git repository. Use --no-index to compare two paths outside a working tree
```

### Context

- Command: `git diff --check`
- The source, build, tests, and installed mod were unaffected.

### Suggested Fix

Initialize new project repositories before running Git-based quality gates.

### Metadata

- Reproducible: yes
- Related Files: none

### Resolution

- **Resolved**: 2026-07-23T11:20:00+02:00
- **Notes**: Initialized the repository on `main` and reran the gate.
