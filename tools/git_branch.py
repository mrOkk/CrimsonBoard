#!/usr/bin/env python3
"""Git branch helper.

Subcommands:
    current                   Print current branch name.
    clean                     Exit 0 if clean tree, else exit 1 with GIT_BRANCH FAIL: dirty_tree.
    commit-push --add <path> [--add <path>...] --message <text>
                              Stage paths, commit (skip if nothing staged), push.
    compute-slug <text>       Normalize text to branch-safe kebab slug (max 40 chars).

Exit codes: 0 success, 1 failure.
Stderr on failure: GIT_BRANCH FAIL: <code>\n<message>
"""
import argparse, re, subprocess, sys

def fail(code, msg):
    print(f"GIT_BRANCH FAIL: {code}", file=sys.stderr)
    print(msg, file=sys.stderr)
    sys.exit(1)

def run(cmd, **kw):
    return subprocess.run(cmd, capture_output=True, text=True, encoding="utf-8", errors="replace", **kw)

def main():
    p = argparse.ArgumentParser()
    sub = p.add_subparsers(dest="cmd")
    sub.add_parser("current")
    sub.add_parser("clean")
    cp = sub.add_parser("commit-push")
    cp.add_argument("--add", dest="paths", action="append", required=True)
    cp.add_argument("--message", required=True)
    cs = sub.add_parser("compute-slug")
    cs.add_argument("text")

    args = p.parse_args()

    if args.cmd == "current":
        r = run(["git", "rev-parse", "--abbrev-ref", "HEAD"])
        if r.returncode != 0:
            fail("not_a_repo", r.stderr.strip())
        b = r.stdout.strip()
        if b == "HEAD":
            fail("detached_head", "HEAD is detached.")
        print(b)

    elif args.cmd == "clean":
        r = run(["git", "status", "--short"])
        if r.returncode != 0:
            fail("not_a_repo", r.stderr.strip())
        if r.stdout.strip():
            fail("dirty_tree", r.stdout.strip())

    elif args.cmd == "commit-push":
        r = run(["git", "add"] + args.paths)
        if r.returncode != 0:
            fail("git_add_failed", r.stderr.strip())
        r = run(["git", "diff", "--cached", "--quiet"])
        if r.returncode != 0:
            r = run(["git", "commit", "-m", args.message])
            if r.returncode != 0:
                if "hook" in r.stderr.lower():
                    fail("hook_failure", r.stderr.strip())
                fail("commit_failed", r.stderr.strip())
        r = run(["git", "push"])
        if r.returncode != 0:
            branch_r = run(["git", "rev-parse", "--abbrev-ref", "HEAD"])
            branch = branch_r.stdout.strip()
            r2 = run(["git", "push", "--set-upstream", "origin", branch])
            if r2.returncode != 0:
                fail("push_failed", r2.stderr.strip())

    elif args.cmd == "compute-slug":
        slug = re.sub(r"[^a-z0-9]+", "-", args.text.lower()).strip("-")[:40]
        if not slug:
            fail("slug_empty", "Input has no usable ASCII alphanumeric content.")
        print(slug)

    else:
        p.print_help(); sys.exit(1)

if __name__ == "__main__":
    main()
