#!/usr/bin/env python3
import os, sys, argparse, re

# usings to skip
SKIP_USINGS = {
    "using System;", "using System.Collections.Generic;", "using System.Linq;",
    "using System.Text;", "using System.Threading.Tasks;", "using Microsoft.AspNetCore.Identity;",
    "using System.Xml.Linq;", "using System.ComponentModel;", "using System.Reflection;",
}

AUTO_EXCLUDE_FOLDERS = [
    "identity","auth","authentication","authorization","helpers","extensions",
    "middleware","middlewares","email","emails","notifications","seeding","seed",
    "configurations","filters","dto","viewmodels","views"
]

KEEP_KEYWORDS = ["entities","enums","services","repositories","controllers","context","data","interfaces"]

def is_relevant(path): return any(k in path.lower() for k in KEEP_KEYWORDS)
def auto_should_exclude(path): return any(ex in path.lower() for ex in AUTO_EXCLUDE_FOLDERS)

def clean_code(lines):
    code=[]
    in_block=False
    prev_blank=False
    for l in lines:
        l_strip = l.strip()
        if not l_strip: 
            if prev_blank: continue
            prev_blank=True
        else:
            prev_blank=False
        if l_strip.startswith("/*"): in_block=True
        if in_block:
            if l_strip.endswith("*/"): in_block=False
            continue
        if l_strip.startswith("//") or l_strip in SKIP_USINGS: continue
        code.append(l_strip)
    return code

def main():
    parser=argparse.ArgumentParser(description="Export compact .cs files")
    parser.add_argument("project_path")
    parser.add_argument("-o","--output",default="all_sources_compact.md")
    parser.add_argument("--exclude", nargs="*", default=["bin","obj",".git","migrations","wwwroot","logs"])
    args=parser.parse_args()

    project=os.path.abspath(args.project_path)
    out=os.path.abspath(args.output)
    if not os.path.isdir(project): print("Project path not found"); sys.exit(1)
    if os.path.exists(out): os.remove(out)

    entries=[]
    for root, dirs, files in os.walk(project):
        dirs[:] = [d for d in dirs if d.lower() not in [e.lower() for e in args.exclude]]
        for f in files:
            if not f.lower().endswith(".cs"): continue
            full=os.path.join(root,f)
            rel=os.path.relpath(full,project)
            if auto_should_exclude(rel) or not is_relevant(rel): continue
            entries.append((rel,full))
    entries.sort()

    with open(out,"w",encoding="utf-8") as wf:
        wf.write("## ⚠️ NOTE: Filtered & Compact Export\n---\n\n")
        for rel, full in entries:
            wf.write(f"# {os.path.basename(rel)}\n```cs\n")
            with open(full,"r",encoding="utf-8",errors="replace") as rf:
                lines = rf.readlines()
                code = clean_code(lines)
                wf.write("\n".join(code))
                wf.write("\n```\n\n")
    print("Compact export done ->", out)

if __name__=="__main__":
    main()
