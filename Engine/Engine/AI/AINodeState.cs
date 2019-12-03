namespace Engine.AI
{
    /// <summary>
    /// The possible states of an AiNode
    /// </summary>
    public enum AiNodeState
    {
        /// <summary>
        /// Algorithm has not encountered this node in this run before
        /// </summary>
        Unconsidered,
        /// <summary>
        /// The Algorithm has seen this node but it has not been considered yet
        /// </summary>
        Open,
        /// <summary>
        /// The node was checked
        /// </summary>
        Closed
    }
}