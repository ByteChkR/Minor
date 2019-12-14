# AI
The engine implements an A* Algorithm with AiNodes as Components
## Creating a Node / Grid
```csharp
	gameObject.AddComponent(new AiNode(true)); //Walkable = True
```
To create/remove connections with other nodes you can call Add/RemoveConnection(otherNode, reverse)
An example on how to create a node grid that connects every node to its neighbours, including diagonals

## Using the A* Algorithm
The Algorithm can be used like this:

```csharp
	List<AiNode> path = AStarResolver.FindPath(startNode, endNode, out bool foundPath);
```