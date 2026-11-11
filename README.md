# Graph Editor

A desktop application for creating, editing, and analyzing graphs.

## Technology

- C# on .NET 8
- Windows Presentation Foundation (WPF) for the user interface, with XAML
- XML serialization for saving and loading graph documents

## Features

- Create and edit graphs on a canvas using nodes and edges.
- Move nodes and edge bend points to arrange the graph visually.
- Select multiple vertices and edge bend points with a drag-selection rectangle, then move the selection together.
- Zoom the canvas around the cursor with `Ctrl` + mouse wheel.
- Create directed and undirected edges, including weighted edges.
- Name nodes and inspect graph statistics such as vertex count, edge count, and a selected node's degree.
- Open a summary of every vertex and its degree with the `J` key.
- Find all simple paths between selected vertices and browse the results.
- Find a minimum-cost path using edge weights and display its total cost.
- Check whether the graph is connected and automatically add edges to connect disconnected components.
- Check Eulerian conditions and display Eulerian cycles when they exist.
- Save and load graph layouts as XML files.
- Work with multiple graph tabs.

## Getting Started

In the root directory:
```
dotnet run --project .\GraphEditor\
```

Create a tab with `[` and use the editor.

## Keyboard Shortcuts

Keyboard shortcuts are handled by the active graph window, canvas, or selected graph element. Some commands depend on which element has focus.

| Key | Action |
| --- | --- |
| `[` | Open a new graph tab |
| `]` | Close the active graph tab |
| `A` | Add a vertex at the cursor position |
| `R` | Toggle edge creation mode |
| `K` | Toggle shortest-path selection mode |
| `Q` | Toggle all-paths selection mode |
| `N` | Show the next found path |
| `L` | Save the current graph as XML |
| `M` | Load a graph from XML |
| `C` | Check graph connectivity |
| `F` | Connect disconnected graph components |
| `E` | Find and show Eulerian cycles |
| `J` | Show the degree of each vertex |
| `Ctrl` + mouse wheel | Zoom the canvas |
| `Delete` (on a vertex) | Delete the selected vertex and its incident edges |
| `Delete` (on an edge) | Delete the selected edge |
| `I` (on a vertex) | Set the vertex name |
| `I` (on an edge) | Set the edge weight |
| `O` (on an edge) | Toggle edge orientation |
| `P` (on an edge) | Reverse the edge direction |

## Notes

Shortest-path costs are calculated from edge weights. XML files preserve graph data and layout, allowing a saved graph to be reopened in the editor.
