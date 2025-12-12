#!/usr/bin/env python3
import os, sys, argparse, re, json

ENTITY_KEYWORDS = ["entities", "models"]
CONTEXT_KEYWORDS = ["context", "dbcontext"]
RELATION_PATTERNS = [
    r'public\s+ICollection<(\w+)>',
    r'public\s+List<(\w+)>',
    r'public\s+(\w+)\s+(\w+Id)'
]

def find_entities(project_path):
    entities = {}
    for root, _, files in os.walk(project_path):
        if not any(k in root.lower() for k in ENTITY_KEYWORDS):
            continue
        for f in files:
            if not f.lower().endswith(".cs"): 
                continue
            path = os.path.join(root, f)
            with open(path, "r", encoding="utf-8", errors="replace") as rf:
                text = rf.read()

            match = re.search(r'class\s+(\w+)', text)
            if match:
                cls = match.group(1)
                entities[cls] = {"relations": []}

                for m in re.findall(r'public\s+(\w+)\s+(\w+Id)', text):
                    entities[cls]["relations"].append({"type": "FK", "target": m[0]})

                for m in re.findall(r'public\s+ICollection<(\w+)>', text):
                    entities[cls]["relations"].append({"type": "OneToMany", "target": m})

                for m in re.findall(r'public\s+List<(\w+)>', text):
                    entities[cls]["relations"].append({"type": "OneToMany", "target": m})
    return entities


def find_dbcontext(project_path):
    for root, _, files in os.walk(project_path):
        if not any(k in root.lower() for k in CONTEXT_KEYWORDS):
            continue
        for f in files:
            if "context" not in f.lower():
                continue
            path = os.path.join(root, f)
            with open(path, "r", encoding="utf-8", errors="replace") as rf:
                return rf.read()
    return None


def generate_architecture(entities, dbcontext_text):
    arch = []

    arch.append("## 📌 Database Architecture Description\n")
    arch.append("The project uses a relational database (likely SQL Server). Data is structured in normalized tables representing core domain entities.\n")

    arch.append("\n## 📌 Key Entities & Relationships\n")
    for name, info in entities.items():
        arch.append(f"### 🔸 {name}")
        if not info["relations"]:
            arch.append("No explicit relationships found.\n")
            continue

        for r in info["relations"]:
            if r["type"] == "FK":
                arch.append(f"- Has a foreign key reference to **{r['target']}**.")
            elif r["type"] == "OneToMany":
                arch.append(f"- One-to-Many relationship with **{r['target']}**.")

        arch.append("")

    arch.append("\n## 📌 Data Flow Description\n")
    arch.append("""
1. **Data Collection**  
   Data is received from user requests through Controllers, validated, and passed to Services/Repositories.

2. **Data Storage**  
   EF Core maps entities to database tables using the DbContext via DbSet<> collections.

3. **Data Access**  
   CRUD operations are executed through EF Core.  
   Navigation properties allow fetching related data automatically via lazy or eager loading.
""")

    return "\n".join(arch)


def main():
    parser = argparse.ArgumentParser(description="Analyze DB architecture")
    parser.add_argument("project_path")
    parser.add_argument("-o", "--output", default="db_architecture.md")
    args = parser.parse_args()

    project = os.path.abspath(args.project_path)

    entities = find_entities(project)
    dbcontext = find_dbcontext(project)

    architecture_doc = generate_architecture(entities, dbcontext)

    with open(args.output, "w", encoding="utf-8") as wf:
        wf.write(architecture_doc)

    print("Architecture analysis generated ->", args.output)


if __name__ == "__main__":
    main()
