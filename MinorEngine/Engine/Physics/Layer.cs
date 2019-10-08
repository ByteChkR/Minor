﻿using System.Runtime.CompilerServices;

namespace Engine.Physics
{
    /// <summary>
    /// Bit masks which control whether different members of a group of objects can collide with each other.
    /// </summary>
    public struct Layer
    {
        /// <summary>
        /// A mask of 16 bits, each set bit representing a collision group that an object belongs to.
        /// </summary>
        public ushort SubgroupMembership { get; set; }

        /// <summary>
        /// A mask of 16 bits, each set bit representing a collision group that an object can interact with.
        /// </summary>
        public ushort CollidableSubgroups { get; set; }

        /// <summary>
        /// Id of the owner of the object. Objects belonging to different groups always collide.
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// Initializes a collision filter that collides with everything in the group.
        /// </summary>
        /// <param name="groupId">Id of the group that this filter operates within.</param>
        public Layer(int groupId)
        {
            GroupId = groupId;
            SubgroupMembership = ushort.MaxValue;
            CollidableSubgroups = ushort.MaxValue;
        }

        /// <summary>
        /// Initializes a collision filter that belongs to one specific subgroup and can collide with any other subgroup.
        /// </summary>
        /// <param name="groupId">Id of the group that this filter operates within.</param>
        /// <param name="subgroupId">Id of the subgroup to put this collidable into.</param>
        public Layer(int groupId, int subgroupId)
        {
            GroupId = groupId;
            SubgroupMembership = (ushort) (1 << subgroupId);
            CollidableSubgroups = ushort.MaxValue;
        }

        /// <summary>
        /// Disables a collision between this filter and the specified subgroup.
        /// </summary>
        /// <param name="subgroupId">Subgroup id to disable collision with.</param>
        public void DisableCollision(int subgroupId)
        {
            CollidableSubgroups ^= (ushort) (1 << subgroupId);
        }

        /// <summary>
        /// Modifies the interactable subgroups such that filterB does not interact with the subgroups defined by filter a and vice versa.
        /// </summary>
        /// <param name="a">Filter from which to remove collisions with filter b's subgroups.</param>
        /// <param name="b">Filter from which to remove collisions with filter a's subgroups.</param>
        public static void DisableCollision(ref Layer filterA, ref Layer filterB)
        {
            filterA.CollidableSubgroups &= (ushort) ~filterB.SubgroupMembership;
            filterB.CollidableSubgroups &= (ushort) ~filterA.SubgroupMembership;
        }

        /// <summary>
        /// Checks if the filters can collide by checking if b's membership can be collided by a's collidable groups.
        /// </summary>
        /// <param name="a">First filter to test.</param>
        /// <param name="b">Second filter to test.</param>
        /// <returns>True if the filters can collide, false otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AllowCollision(Layer a, Layer b)
        {
            return a.GroupId != b.GroupId || (a.CollidableSubgroups & b.SubgroupMembership) > 0;
        }
    }
}