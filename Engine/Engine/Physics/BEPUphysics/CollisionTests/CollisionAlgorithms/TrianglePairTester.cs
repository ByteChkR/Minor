using Engine.Physics.BEPUphysics.CollisionShapes.ConvexShapes;
using Engine.Physics.BEPUutilities.DataStructures;

namespace Engine.Physics.BEPUphysics.CollisionTests.CollisionAlgorithms
{
    ///<summary>
    /// Persistent tester that compares triangles against convex objects.
    ///</summary>
    public abstract class TrianglePairTester
    {
        ///<summary>
        /// Whether or not the pair tester was updated during the last attempt.
        ///</summary>
        public bool Updated;

        /// <summary>
        /// Whether or not the last found contact should have its normal corrected.
        /// </summary>
        public abstract bool ShouldCorrectContactNormal { get; }

        //Relies on the triangle being located in the local space of the convex object.  The convex transform is used to transform the
        //contact points back from the convex's local space into world space.
        ///<summary>
        /// Generates a contact between the triangle and convex.
        ///</summary>
        /// <param name="triangle">Triangle to test</param>
        ///<param name="contactList">Contact between the shapes, if any.</param>
        ///<returns>Whether or not the shapes are colliding.</returns>
        public abstract bool GenerateContactCandidates(TriangleShape triangle,
            out TinyStructList<ContactData> contactList);

        /// <summary>
        /// Gets the triangle region in which the contact resides.
        /// </summary>
        /// <param name="triangle">Triangle to compare the contact against.</param>
        /// <param name="contact">Contact to check.</param>
        /// <returns>Region in which the contact resides.</returns>
        public abstract VoronoiRegion GetRegion(TriangleShape triangle, ref ContactData contact);

        ///<summary>
        /// Initializes the pair tester.
        ///</summary>
        ///<param name="convex">Convex shape to use.</param>
        public abstract void Initialize(ConvexShape convex);

        /// <summary>
        /// Cleans up the pair tester.
        /// </summary>
        public abstract void CleanUp();
    }
}