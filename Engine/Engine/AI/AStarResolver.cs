using System.Collections.Generic;

namespace Engine.AI
{
    public class AStarResolver
    {
        public static List<AiNode> FindPath(AiNode startPoint, AiNode endPoint,
            out bool foundPath)
        {
            if (startPoint == endPoint)
            {
                foundPath = true;
                return new List<AiNode>();
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
                    List<AiNode> ret = GeneratePath(endPoint);
                    ResetNodes(todo, doneList);

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
            }


            ResetNodes(todo, doneList);
            foundPath = false;
            return new List<AiNode>();
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