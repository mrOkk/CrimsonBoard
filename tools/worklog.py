#!/usr/bin/env python3
"""Worklog metadata helper.

Subcommands:
    read <path> --field <name>        Print one metadata field value.
    advance <path> --from N --to M --note "<text>"
                                      Validate current_phase==N, set to M,
                                      refresh updated_at, append Phase History row.
    set-status <path> --status (active|done) --note "<text>"
                                      Set status field, append Phase History row.

Exit codes: 0 success, 1 failure.
Stderr on failure: WORKLOG FAIL: <code>\n<message>
"""
import argparse, re, sys
from datetime import datetime, timezone
from pathlib import Path

def now_iso():
    return datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")

def fail(code, msg):
    print(f"WORKLOG FAIL: {code}", file=sys.stderr)
    print(msg, file=sys.stderr)
    sys.exit(1)

def parse_metadata(text):
    """Return dict of metadata fields from the first Markdown table in text."""
    fields = {}
    for line in text.splitlines():
        m = re.match(r'\|\s*`([^`]+)`\s*\|\s*`?([^`|]+?)`?\s*\|', line)
        if m:
            fields[m.group(1).strip()] = m.group(2).strip()
    return fields

def set_field(text, field, value):
    """Replace a metadata field value in the table."""
    pattern = re.compile(
        r'(\|\s*`' + re.escape(field) + r'`\s*\|\s*)([^|\n]*)(\s*\|)',
        re.MULTILINE,
    )
    replacement = r'\g<1>`' + value + r'`\g<3>'
    new_text, count = pattern.subn(replacement, text)
    if count == 0:
        fail("unknown_field", f"Field '{field}' not found in metadata table.")
    return new_text

def append_history_row(text, when, direction, note):
    marker = "## Phase History"
    table_header = "| When | From → To | Note |"
    row = f"| {when} | {direction} | {note} |"
    if marker not in text:
        fail("metadata_malformed", "Phase History section not found.")
    idx = text.index(table_header)
    end = text.index("\n", idx)
    sep_end = text.index("\n", end + 1)
    return text[: sep_end + 1] + row + "\n" + text[sep_end + 1 :]

def main():
    p = argparse.ArgumentParser()
    sub = p.add_subparsers(dest="cmd")

    r = sub.add_parser("read")
    r.add_argument("path"); r.add_argument("--field", required=True)

    a = sub.add_parser("advance")
    a.add_argument("path"); a.add_argument("--from", dest="from_phase", type=int, required=True)
    a.add_argument("--to", dest="to_phase", type=int, required=True); a.add_argument("--note", required=True)

    s = sub.add_parser("set-status")
    s.add_argument("path"); s.add_argument("--status", required=True); s.add_argument("--note", required=True)

    args = p.parse_args()
    if not args.cmd:
        p.print_help(); sys.exit(1)

    path = Path(args.path)
    if not path.exists():
        fail("worklog_missing", f"File not found: {path}")
    text = path.read_text(encoding="utf-8")
    meta = parse_metadata(text)

    if args.cmd == "read":
        v = meta.get(args.field)
        if v is None:
            fail("unknown_field", f"Field '{args.field}' not found.")
        print(v)

    elif args.cmd == "advance":
        current = int(meta.get("current_phase", 0))
        if current != args.from_phase:
            fail("phase_mismatch", f"current_phase is {current}, expected {args.from_phase}.")
        ts = now_iso()
        text = set_field(text, "current_phase", str(args.to_phase))
        text = set_field(text, "updated_at", ts)
        text = append_history_row(text, ts, f"{args.from_phase} → {args.to_phase}", args.note)
        path.write_text(text, encoding="utf-8")

    elif args.cmd == "set-status":
        prev_status = meta.get("status", "active")
        ts = now_iso()
        text = set_field(text, "status", args.status)
        text = set_field(text, "updated_at", ts)
        text = append_history_row(text, ts, f"{prev_status} → {args.status}", args.note)
        path.write_text(text, encoding="utf-8")

if __name__ == "__main__":
    main()
