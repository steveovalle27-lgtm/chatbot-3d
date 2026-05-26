# Graph Report - .  (2026-05-24)

## Corpus Check
- Corpus is ~1,768 words - fits in a single context window. You may not need a graph.

## Summary
- 63 nodes · 55 edges · 10 communities
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- [[_COMMUNITY_Node Package Config|Node Package Config]]
- [[_COMMUNITY_Server Logic Core|Server Logic Core]]
- [[_COMMUNITY_External Dependencies|External Dependencies]]
- [[_COMMUNITY_API Lipsync Data 0|API Lipsync Data 0]]
- [[_COMMUNITY_API Lipsync Data 1|API Lipsync Data 1]]
- [[_COMMUNITY_Intro Lipsync Data 0|Intro Lipsync Data 0]]
- [[_COMMUNITY_Intro Lipsync Data 1|Intro Lipsync Data 1]]
- [[_COMMUNITY_VS Layout Backup|VS Layout Backup]]
- [[_COMMUNITY_VS Layout State|VS Layout State]]
- [[_COMMUNITY_VS Workspace State|VS Workspace State]]

## God Nodes (most connected - your core abstractions)
1. `scripts` - 3 edges
2. `metadata` - 3 edges
3. `metadata` - 3 edges
4. `metadata` - 3 edges
5. `metadata` - 3 edges
6. `execCommand()` - 2 edges
7. `lipSyncMessage()` - 2 edges
8. `openai` - 1 edges
9. `supabase` - 1 edges
10. `app` - 1 edges

## Surprising Connections (you probably didn't know these)
- None detected - all connections are within the same source files.

## Communities (10 total, 0 thin omitted)

### Community 0 - "Node Package Config"
Cohesion: 0.17
Nodes (11): author, description, devDependencies, nodemon, main, name, scripts, dev (+3 more)

### Community 1 - "Server Logic Core"
Cohesion: 0.22
Nodes (7): app, execCommand(), history, lipSyncMessage(), messages, openai, supabase

### Community 2 - "External Dependencies"
Cohesion: 0.29
Nodes (7): dependencies, cors, dotenv, elevenlabs-node, express, openai, @supabase/supabase-js

### Community 3 - "API Lipsync Data 0"
Cohesion: 0.40
Nodes (4): metadata, duration, soundFile, mouthCues

### Community 4 - "API Lipsync Data 1"
Cohesion: 0.40
Nodes (4): metadata, duration, soundFile, mouthCues

### Community 5 - "Intro Lipsync Data 0"
Cohesion: 0.40
Nodes (4): metadata, duration, soundFile, mouthCues

### Community 6 - "Intro Lipsync Data 1"
Cohesion: 0.40
Nodes (4): metadata, duration, soundFile, mouthCues

### Community 7 - "VS Layout Backup"
Cohesion: 0.40
Nodes (4): DocumentGroupContainers, Documents, Version, WorkspaceRootPath

### Community 8 - "VS Layout State"
Cohesion: 0.40
Nodes (4): DocumentGroupContainers, Documents, Version, WorkspaceRootPath

### Community 9 - "VS Workspace State"
Cohesion: 0.50
Nodes (3): ExpandedNodes, PreviewInSolutionExplorer, SelectedNode

## Knowledge Gaps
- **43 isolated node(s):** `openai`, `supabase`, `app`, `history`, `messages` (+38 more)
  These have ≤1 connection - possible missing edges or undocumented components.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `dependencies` connect `External Dependencies` to `Node Package Config`?**
  _High betweenness centrality (0.046) - this node is a cross-community bridge._
- **What connects `openai`, `supabase`, `app` to the rest of the system?**
  _43 weakly-connected nodes found - possible documentation gaps or missing edges._