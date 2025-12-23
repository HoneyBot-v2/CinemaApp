# Git Commit & Branch Guide (Personal Projects - LLM Reference)

This guide defines commit message standards and branch structure for personal .NET projects. Optimized for LLM reference when generating commits.

## Commit Message Standards

### Format

**Basic:**
```
type(scope): summary
```

**Breaking Changes:**
```
type(scope)!: summary
```

**With Body:**
```
type(scope): summary

Why we made this change.
What changed from the previous behavior.

Refs: #123
```

### Rules

- **type** is required
- **scope** is recommended (optional)
- **summary** must be short and specific (50-72 characters)
- Use body when "why" isn't obvious
- No period at end of summary

### Commit Types

| Type | When to Use | Example |
|------|-------------|---------|
| `feat` | New user-visible feature | `feat(api): add GET /orders endpoint` |
| `fix` | Bug fix or incorrect behavior | `fix(maui): prevent crash on startup` |
| `refactor` | Code restructure, no behavior change | `refactor(api): extract OrderService` |
| `docs` | Documentation only | `docs(readme): add setup instructions` |
| `test` | Tests only | `test(api): add orders endpoint tests` |
| `perf` | Performance improvement | `perf(db): optimize reservation query` |
| `ci` | CI/CD pipeline changes | `ci: add unit test job to pipeline` |
| `build` | Build tooling, dependencies | `build(deps): upgrade EntityFrameworkCore` |
| `chore` | Maintenance, cleanup | `chore: remove unused code` |

### Scopes

| Scope | When to Use |
|-------|-------------|
| `api` | API project changes |
| `maui` | MAUI/client application changes |
| `console` | Console application changes |
| `nuget` | NuGet package project changes |
| `db` | Entity Framework, migrations, database schema |
| `sp` | Stored procedure changes |
| `auth` | Authentication/authorization |
| `docker` | Dockerfile, compose.yaml |
| `ci` | CI/CD pipeline |
| `deps` | NuGet package updates |
| `solution` | Solution file changes |

**Scope Rules:**
- Keep short and stable
- Use lowercase
- One scope only
- Omit if it doesn't add clarity

### Examples

**Good:**
```
feat(api): add GET /orders endpoint
fix(maui): prevent crash when window handle is null
refactor(api): extract OrderService from controller
docs(readme): add local development setup
build(deps): upgrade EntityFrameworkCore to 10.0.1
```

**Bad:**
```
add orders endpoint
fix bug
updates
wip
change MoviesController
```

### Breaking Changes

Mark with `!` after type/scope:
```
feat(api)!: rename /health to /healthz
```

Optionally include BREAKING CHANGE footer:
```
feat(api)!: change auth token format

BREAKING CHANGE: Tokens issued before this release are no longer valid.
```

### When to Include Body

Add body when:
- The "why" isn't obvious from code changes
- Change has risks or tradeoffs
- Behavior changed in non-trivial way
- Known limitation or follow-up needed

**Example:**
```
fix(db): reduce sqlite lock errors on startup

SQLite was occasionally returning "database is locked" during warm startup.
We now set busy_timeout and retry the initial migration once.

Refs: #123
```

### Common Mistakes

1. **Vague messages** - Use specific behavior descriptions
2. **Wrong type** - If users notice it → `feat`/`fix`, otherwise → `refactor`/`test`/`ci`/`build`/`chore`
3. **Missing scope** - Include scope when it helps identify affected project
4. **Mixing unrelated changes** - One logical change per commit
5. **File-based summaries** - Describe behavior, not file edits

---

## Branch Structure

### Trunk-Based Development

**Long-Lived Branch:**
- `main` - The trunk (only long-lived branch, production-ready code)

**Short-Lived Branches:**
- `feature/*` - New work from `main`, merged back to `main`
- All branches are short-lived and merge directly to `main`
- No `develop` branch
- No `release` branches (tags created directly on `main`)
- Hotfixes are just `feature/*` branches

### Branch Naming

**Format:** `feature/<ticket>-<component>-<description>`

**Rules:**
- Lowercase only
- Hyphens only (no spaces, underscores)
- Include ticket/work item when available
- Include component name (`api`, `maui`, `console`, `nuget`, `db`) when applicable

**Examples:**
- `feature/ABC-123-api-orders-endpoint`
- `feature/ABC-124-maui-login-screen`
- `feature/ABC-125-console-data-export`
- `feature/ABC-126-nuget-shared-utilities`
- `feature/ABC-127-db-movie-schema`
- `feature/ABC-128-hotfix-auth-nullref` (hotfixes are feature branches)

### Branch Lifecycle

**Feature Branch:**
Create from `main` → Develop → PR to `main` → Merge → Delete

**Releases:**
Tag directly on `main` after merging feature branches. No release branches needed.

---

## Changelog Generation with git-cliff

Personal projects use **git-cliff** to automatically generate changelogs from commit history. git-cliff reads Conventional Commits and creates formatted changelogs.

### How git-cliff Works

- Reads commit messages following Conventional Commits format
- Groups commits by type (`feat`, `fix`, `refactor`, etc.)
- Generates changelog entries between tags
- Supports semantic versioning tags (`v1.4.2`, etc.)

### Why Conventional Commits Matter

Using the commit message standards in this guide ensures git-cliff can:
- Categorize changes correctly (Features, Bug Fixes, etc.)
- Generate clean, readable changelogs
- Support automated release notes

### Version Tags

- Create tags on `main` branch: `v1.4.2`
- git-cliff generates changelog between tags
- Example: `git-cliff --tag v1.4.2` generates changelog from previous tag to v1.4.2

### Example git-cliff Output

```
## [1.4.2] - 2024-01-15

### Features
- feat(api): add GET /orders endpoint
- feat(maui): add login screen

### Bug Fixes
- fix(api): prevent null reference in authentication
- fix(db): reduce sqlite lock errors on startup

### Refactoring
- refactor(api): extract OrderService from controller
```

---

## Prompt Examples for LLMs

### Generating Commit Messages

**Basic Prompt:**
```
Generate a commit message following Conventional Commits format for these changes:
[describe changes]

Reference: docs/git-commit-branch-trunk.md
```

**With Context:**
```
Based on the following code changes, generate a commit message using Conventional Commits format.
The changes affect the API project and add a new orders endpoint.

Changes:
[code diff or description]

Reference: docs/git-commit-branch-trunk.md
```

**Multiple Commits:**
```
Analyze these changes and suggest appropriate commit messages. Split into logical commits if needed.
Each commit should follow Conventional Commits format.

Changes:
[code diff or description]

Reference: docs/git-commit-branch-trunk.md
```

**With Type Selection:**
```
Is this change a 'feat', 'fix', 'refactor', or other type? Generate the appropriate commit message.

Change: [description]

Reference: docs/git-commit-branch-trunk.md
```

### Suggesting Branch Names

**Prompt:**
```
Suggest a branch name for this work following the format: feature/<ticket>-<component>-<description>

Work: Adding orders endpoint to API project
Ticket: ABC-123

Reference: docs/git-commit-branch-trunk.md
```

**Response:** `feature/ABC-123-api-orders-endpoint`

### Changelog Generation

**Prompt:**
```
Generate a changelog using git-cliff for changes between tag v1.4.1 and v1.4.2.

Reference: docs/git-commit-branch-trunk.md
```

**Response:** git-cliff will generate a formatted changelog grouping commits by type (feat, fix, refactor, etc.)

### Commit Type Decision

**Prompt:**
```
Help me choose the correct commit type for this change:
[describe change]

Options: feat, fix, refactor, docs, test, perf, ci, build, chore

Reference: docs/git-commit-branch-trunk.md
```

### Scope Selection

**Prompt:**
```
What scope should I use for changes to:
- API project
- MAUI application
- Console application
- NuGet package project
- Database migrations
- Solution file

Reference: docs/git-commit-branch-trunk.md
```

### Complete Workflow

**Prompt:**
```
I'm working on adding a new login screen to the MAUI application.
1. Suggest a branch name
2. Generate commit messages for the work (split into logical commits)
3. How will these commits appear in the git-cliff changelog?

Reference: docs/git-commit-branch-trunk.md
```

---

## Quick Reference

### Commit Message Template

```
type(scope): summary

Why:
- (context / problem)

What changed:
- (behavior change)

Refs: #123
```

### Commit Type Decision Tree

```
Is it a new feature users will notice?
├─ Yes → feat
└─ No → Is it fixing incorrect behavior?
    ├─ Yes → fix
    └─ No → Is it only tests?
        ├─ Yes → test
        └─ No → Is it only documentation?
            ├─ Yes → docs
            └─ No → Is it code restructuring with no behavior change?
                ├─ Yes → refactor
                └─ No → Is it performance improvement?
                    ├─ Yes → perf
                    └─ No → Is it CI/CD pipeline?
                        ├─ Yes → ci
                        └─ No → Is it build/dependencies?
                            ├─ Yes → build
                            └─ No → chore
```

### Branch Naming Patterns

| Branch Type | Pattern | Example |
|-------------|---------|---------|
| Feature | `feature/<ticket>-<component>-<description>` | `feature/ABC-123-api-orders-endpoint` |
| Hotfix (as feature) | `feature/<ticket>-hotfix-<description>` | `feature/ABC-128-hotfix-auth-nullref` |

### Scope Quick Reference

| Scope | Use For |
|-------|---------|
| `api` | API project changes |
| `maui` | MAUI/client application changes |
| `console` | Console application changes |
| `nuget` | NuGet package project changes |
| `db` | Entity Framework, migrations, database schema |
| `sp` | Stored procedure changes |
| `auth` | Authentication/authorization |
| `docker` | Dockerfile, compose.yaml |
| `ci` | CI/CD pipeline |
| `deps` | NuGet package updates |
| `solution` | Solution file changes |

---

## Summary

**Commit Messages:**
- Use Conventional Commits: `type(scope): summary`
- Choose appropriate type (`feat`, `fix`, `refactor`, etc.)
- Include scope when it helps identify affected project
- Add body when "why" isn't obvious

**Branches:**
- Follow trunk-based development (`main` as trunk, short-lived `feature/*` branches)
- Use format: `feature/<ticket>-<component>-<description>`
- Lowercase, hyphens only
- All branches merge directly to `main`

**Changelog Generation:**
- Use git-cliff to generate changelogs from Conventional Commits
- Create version tags on `main` branch (e.g., `v1.4.2`)
- git-cliff automatically categorizes commits by type for clean changelogs

