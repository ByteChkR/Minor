using System.Collections.Generic;
using OpenTK;

namespace Engine.AI
{
    public class AStarResolver
    {

        public static List<AINode> FindPath(AINode startPoint, AINode endPoint,
            out bool foundPath)
        {
            if (startPoint == endPoint)
            {
                foundPath = true;
                return new List<AINode>();
            }

            foundPath = false;

            PriorityQueue<AINode> todo = new PriorityQueue<AINode>();
            List<AINode> doneList = new List<AINode>();
            todo.Enqueue(startPoint);

            while (todo.Count != 0)
            {
                AINode current = todo.Dequeue();
                doneList.Add(current);
                current.NodeState = AINodeState.Closed;
                if (current == endPoint)
                {
                    foundPath = true;
                    List<AINode> ret = GeneratePath(endPoint);
                    ResetNodes(todo, doneList);
                    
                    return ret;
                }
                else
                {
                    for (int i = 0; i < current.ConnectionCount; i++)
                    {
                        AINode connection = current.GetConnection(i);

                        if (!connection.Walkable || connection.NodeState == AINodeState.Closed) continue;

                        float connD = (connection.Owner.GetWorldPosition() - current.Owner.GetWorldPosition()).Length;
                        if (connection.NodeState == AINodeState.Unconsidered)
                        {
                            connection.ParentNode = current;

                            connection.CurrentCost = current.CurrentCost + connD;

                            connection.EstimatedCost = (connection.Owner.GetWorldPosition() - endPoint.Owner.GetWorldPosition()).Length;
                            connection.NodeState = AINodeState.Open;
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

            }


            ResetNodes(todo, doneList);
            foundPath = false;
            return new List<AINode>();

        }

        private static List<AINode> GeneratePath(AINode endNode)
        {
            List<AINode> ret = new List<AINode>();
            AINode current = endNode;
            while (current != null)
            {
                ret.Add(current);
                current = current.ParentNode;
            }
            ret.Reverse();
            return ret;
        }


        private static void ResetNodes(PriorityQueue<AINode> todo, List<AINode> done)
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

        private static void ResetNode(AINode node)
        {
            node.ParentNode = null;
            node.CurrentCost = 0;
            node.EstimatedCost = 0;
            node.NodeState = AINodeState.Unconsidered;
        }


    }
}