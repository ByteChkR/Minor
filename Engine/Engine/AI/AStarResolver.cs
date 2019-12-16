using System.Collections.Generic;
using System.Diagnostics;
using Engine.Debug;

namespace Engine.AI
{
    /// <summary>
    /// The Implementation of the A* Algorithm
    /// </summary>
    public class AStarResolver
    {
        /// <summary>
        /// Finds the path from the start node to the end node
        /// </summary>
        /// <param name="startPoint">The point to start the search from</param>
        /// <param name="endPoint">The target point to reach</param>
        /// <param name="foundPath">out parameter to indicate if there exists a valid path for the start/end configuration</param>
        /// <returns>Returns a List of Nodes containing the path from start to finish</returns>
        public static List<AiNode> FindPath(AiNode startPoint, AiNode endPoint,
            out bool foundPath)
        {
            int iterations = 0;
            int nodesConsidered = 2;
            Logger.Log($"A* Query Start:{startPoint} {endPoint}", DebugChannel.EngineAI | DebugChannel.Log, 10);
            Stopwatch sw = Stopwatch.StartNew();

            if (startPoint == endPoint)
            {
                sw.Stop();
                Logger.Log($"A* Iteration Found Path: ", DebugChannel.EngineAI | DebugChannel.Log, 9);

                WriteStatistics(nodesConsidered, iterations, sw.ElapsedMilliseconds, 0, 0);

                foundPath = true;
                return new List<AiNode>() { startPoint, endPoint };
            }

            foundPath = false;

            PriorityQueue<AiNode> todo = new PriorityQueue<AiNode>();
            List<AiNode> doneList = new List<AiNode>();
            todo.Enqueue(startPoint);

            while (todo.Count != 0)
            {
                AiNode current = todo.Dequeue();
                doneList.Add(current);

                current.NodeState = AiNodeState.Closed;
                if (current == endPoint)
                {
                    foundPath = true;
                    float debugCurrentCost = current.CurrentCost;
                    List<AiNode> ret = GeneratePath(endPoint);

                    ResetNodes(todo, doneList);


                    sw.Stop();
                    Logger.Log($"A* Iteration Found Path: ", DebugChannel.EngineAI | DebugChannel.Log, 9);

                    WriteStatistics(nodesConsidered, iterations, sw.ElapsedMilliseconds, debugCurrentCost, ret.Count);

                    return ret;
                }

                for (int i = 0; i < current.ConnectionCount; i++)
                {
                    AiNode connection = current.GetConnection(i);

                    if (!connection.Walkable || connection.NodeState == AiNodeState.Closed)
                    {
                        continue;
                    }

                    float connD = (connection.Owner.GetWorldPosition() - current.Owner.GetWorldPosition()).Length *
                                  connection.WalkCostMultiplier;
                    if (connection.NodeState == AiNodeState.Unconsidered)
                    {
                        connection.ParentNode = current;

                        connection.CurrentCost = current.CurrentCost + connD;

                        connection.EstimatedCost =
                            (connection.Owner.GetWorldPosition() - endPoint.Owner.GetWorldPosition()).Length;
                        connection.NodeState = AiNodeState.Open;
                        todo.Enqueue(connection);
                    }

                    if (current != connection) //Shouldnt be possible. but better check to avoid spinning forever
                    {
                        float newCost = current.CurrentCost + connD;
                        if (newCost < connection.CurrentCost)
                        {
                            connection.ParentNode = current;
                            connection.CurrentCost = newCost;
                        }
                    }
                }

                iterations++;
                nodesConsidered++;
            }

            ResetNodes(todo, doneList);
            foundPath = false;
            sw.Stop();
            Logger.Log($"A* Iteration did NOT find Path: ", DebugChannel.EngineAI | DebugChannel.Warning, 9);

            WriteStatistics(nodesConsidered, iterations, sw.ElapsedMilliseconds, 0, 0);
            return new List<AiNode>();
        }

        private static void WriteStatistics(int nodesConsidered, int iterations, long ellapsedMilliseconds, float pathLength, int pathNodeCount)
        {
            Logger.Log($"\t Statistics:", DebugChannel.EngineAI | DebugChannel.Log, 9);
            Logger.Log($"\t\tNodes Considered: {nodesConsidered}", DebugChannel.EngineAI | DebugChannel.Log, 9);
            Logger.Log($"\t\tIterations: {iterations}", DebugChannel.EngineAI | DebugChannel.Log, 9);
            Logger.Log($"\t\tTime: {ellapsedMilliseconds} ms", DebugChannel.EngineAI | DebugChannel.Log, 9);
            Logger.Log($"\t\tPath Length: {pathLength}", DebugChannel.EngineAI | DebugChannel.Log, 9);
            Logger.Log($"\t\tPath Length(Nodes): {pathNodeCount}", DebugChannel.EngineAI | DebugChannel.Log, 9);

        }
        private static List<AiNode> GeneratePath(AiNode endNode)
        {
            List<AiNode> ret = new List<AiNode>();
            AiNode current = endNode;
            while (current != null)
            {
                ret.Add(current);
                current = current.ParentNode;
            }
            ret.Reverse();
            return ret;
        }


        private static void ResetNodes(PriorityQueue<AiNode> todo, List<AiNode> done)
        {
            while (todo.Count != 0)
            {
                ResetNode(todo.Dequeue());
            }

            for (int i = 0; i < done.Count; i++)
            {
                ResetNode(done[i]);
            }
        }

        private static void ResetNode(AiNode node)
        {
            node.ParentNode = null;
            node.CurrentCost = 0;
            node.EstimatedCost = 0;
            node.NodeState = AiNodeState.Unconsidered;
        }
    }
}