using System;
using System.Collections.Generic;
using Engine.Core;
using Engine.Debug;
using Engine.Exceptions;

namespace Engine.AI
{
    public class AiNode : AbstractComponent, IComparable<AiNode>
    {
        private readonly List<AiNode> connections;


        public float CurrentCost;
        public float EstimatedCost;
        public AiNodeState NodeState;


        //Search Specific
        public AiNode ParentNode;
        public bool Walkable;
        public float WalkCostMultiplier = 1;

        public AiNode(bool walkable)
        {
            connections = new List<AiNode>();
            Walkable = walkable;
        }

        public float TotalCost => CurrentCost + EstimatedCost;
        public int ConnectionCount => connections.Count;

        public int CompareTo(AiNode other)
        {
            return TotalCost.CompareTo(other.TotalCost);
        }

        public AiNode GetConnection(int index)
        {
            if (index < 0 || index >= connections.Count)
            {
                Logger.Crash(new ItemNotFoundExeption("AI Node", "Could not find the AI Node at index: " + index),
                    true);
                return null;
            }

            return connections[index];
        }

        public void AddConnection(AiNode other, bool addReverse = true)
        {
            if (connections.Contains(other))
            {
                Logger.Log("Connection already established in AI node.", DebugChannel.Warning, 10);
                return;
            }

            connections.Add(other);
            if (addReverse)
            {
                other.AddConnection(this, false); //Add the other way around
            }
        }


        public void RemoveConnection(AiNode other, bool removeReverse = true)
        {
            if (!connections.Contains(other))
            {
                Logger.Log("Connection not found in AI node.", DebugChannel.Warning, 10);
                return;
            }

            connections.Remove(other);
            if (removeReverse)
            {
                other.RemoveConnection(this, false);
            }
        }
    }
}