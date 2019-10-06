using System;
using MinorEngine.BEPUphysics.BroadPhaseEntries;
using MinorEngine.BEPUphysics.BroadPhaseEntries.MobileCollidables;
using MinorEngine.BEPUphysics.CollisionShapes.ConvexShapes;
using MinorEngine.BEPUphysics.CollisionTests.CollisionAlgorithms;
using MinorEngine.BEPUutilities;
using MinorEngine.BEPUutilities.DataStructures;

namespace MinorEngine.BEPUphysics.NarrowPhaseSystems.Pairs
{
    /// <summary>
    /// Handles the tests between a DetectorVolume and a convex collidable.
    /// </summary>
    public class DetectorVolumeConvexPairHandler : DetectorVolumePairHandler
    {
        private ConvexCollidable convex;

        /// <summary>
        /// Gets or sets whether or not to check the convex object for total containment within the detector volume.
        /// </summary>
        public bool CheckContainment { get; set; } = true;

        public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
        {
            base.Initialize(entryA, entryB);
            convex = entryA as ConvexCollidable;
            if (convex == null)
            {
                convex = entryB as ConvexCollidable;
                if (convex == null)
                {
                    throw new ArgumentException("Incorrect types passed to pair handler.");
                }
            }
        }

        public override void CleanUp()
        {
            base.CleanUp();
            convex = null;
            CheckContainment = true;
        }

        public override EntityCollidable Collidable => convex;

        private RawList<int> overlaps = new RawList<int>(8);
        private TriangleShape triangle = new TriangleShape {collisionMargin = 0};

        public override void UpdateCollision(float dt)
        {
            WasContaining = Containing;
            WasTouching = Touching;


            var transform = new RigidTransform {Orientation = Quaternion.Identity};
            DetectorVolume.TriangleMesh.Tree.GetOverlaps(convex.boundingBox, overlaps);
            for (var i = 0; i < overlaps.Count; i++)
            {
                DetectorVolume.TriangleMesh.Data.GetTriangle(overlaps.Elements[i], out triangle.vA, out triangle.vB,
                    out triangle.vC);
                Vector3.Add(ref triangle.vA, ref triangle.vB, out transform.Position);
                Vector3.Add(ref triangle.vC, ref transform.Position, out transform.Position);
                Vector3.Multiply(ref transform.Position, 1 / 3f, out transform.Position);
                Vector3.Subtract(ref triangle.vA, ref transform.Position, out triangle.vA);
                Vector3.Subtract(ref triangle.vB, ref transform.Position, out triangle.vB);
                Vector3.Subtract(ref triangle.vC, ref transform.Position, out triangle.vC);

                //If this triangle collides with the convex, we can stop immediately since we know we're touching and not containing.)))
                //[MPR is used here in lieu of GJK because the MPR implementation tends to finish quicker when objects are overlapping than GJK.  The GJK implementation does better on separated objects.]
                if (MPRToolbox.AreShapesOverlapping(convex.Shape, triangle, ref convex.worldTransform, ref transform))
                {
                    Touching = true;
                    //The convex can't be fully contained if it's still touching the surface.
                    Containing = false;

                    overlaps.Clear();
                    goto events;
                }
            }

            overlaps.Clear();
            //If we get here, then there was no shell intersection.
            //If the convex's center point is contained by the mesh, then the convex is fully contained.
            //If this is a child pair, the CheckContainment flag may be set to false.  This is because the parent has
            //already determined that it is not contained (another child performed the check and found that it was not contained)
            //and that it is already touching somehow (either by intersection or by containment).
            //so further containment tests are unnecessary.
            if (CheckContainment && DetectorVolume.IsPointContained(ref convex.worldTransform.Position, overlaps))
            {
                Touching = true;
                Containing = true;
                goto events;
            }

            //If we get here, then there was no surface intersection and the convex's center is not contained- the volume and convex are separate!
            Touching = false;
            Containing = false;

            events:
            NotifyDetectorVolumeOfChanges();
        }
    }
}