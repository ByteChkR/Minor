using System;
using System.Collections.Generic;
using Engine.Core;
using Engine.Debug;
using Engine.Exceptions;

namespace Engine.AI
{
    public class AINode : AbstractComponent, IComparable<AINode>
    {
        private List<AINode> Connections;


        //Search Specific
        public AINode ParentNode;
        public bool Walkable;
        public float WalkCostMultiplier = 1;
        public AINodeState NodeState;


        public float CurrentCost;
        public float EstimatedCost;

        public float TotalCost => CurrentCost + EstimatedCost;
        public int ConnectionCount => Connections.Count;

        public AINode(bool walkable)
        {
            Connections = new List<AINode>();
            Walkable = walkable;
        }

        public AINode GetConnection(int index)
        {
            if (index < 0 || index >= Connections.Count)
            {
                Logger.Crash(new ItemNotFoundExeption("AI Node", "Could not find the AI Node at index: " + index), true);
                return null;
            }

            return Connections[index];
        }

        public void AddConnection(AINode other, bool addReverse = true)
        {

            if (Connections.Contains(other))
            {
                Logger.Log("Connection already established in AI node.", DebugChannel.Warning, 10);
                return;
            }

            Connections.Add(other);
            if (addReverse) other.AddConnection(this, false); //Add the other way around

        }


        public void RemoveConnection(AINode other, bool removeReverse = true)
        {
            if (!Connections.Contains(other))
            {
                Logger.Log("Connection not found in AI node.", DebugChannel.Warning, 10);
                return;
            }

            Connections.Remove(other);
            if (removeReverse)
                other.RemoveConnection(this, false);
        }

        public int CompareTo(AINode other)
        {
            return TotalCost.CompareTo(other.TotalCost);
        }
    }
}