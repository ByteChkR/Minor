namespace Engine.Physics
{
    /// <summary>
    /// A Struct representing the constraints that a collider can have
    /// </summary>
    public struct ColliderConstraints
    {
        /// <summary>
        /// The constraints of linear velocity along the axes
        /// </summary>
        public FreezeConstraints PositionConstraints;
        /// <summary>
        /// The constraints of angular velocity along the axes
        /// </summary>
        public FreezeConstraints RotationConstraints;
    }
}