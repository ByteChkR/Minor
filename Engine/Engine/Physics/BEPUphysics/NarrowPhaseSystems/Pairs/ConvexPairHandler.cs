using Engine.Physics.BEPUphysics.BroadPhaseEntries;
using Engine.Physics.BEPUphysics.BroadPhaseEntries.MobileCollidables;
using Engine.Physics.BEPUphysics.BroadPhaseSystems;
using Engine.Physics.BEPUphysics.CollisionTests.CollisionAlgorithms.GJK;
using Engine.Physics.BEPUphysics.PositionUpdating;
using Engine.Physics.BEPUphysics.Settings;
using Engine.Physics.BEPUutilities;

namespace Engine.Physics.BEPUphysics.NarrowPhaseSystems.Pairs
{
    ///<summary>
    /// Pair handler that manages a pair of two convex shapes.
    ///</summary>
    public abstract class ConvexPairHandler : StandardPairHandler
    {
        public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
        {
            UpdateMaterialProperties();

            base.Initialize(entryA, entryB);
        }


        ///<summary>
        /// Updates the time of impact for the pair.
        ///</summary>
        ///<param name="requester">Collidable requesting the update.</param>
        ///<param name="dt">Timestep duration.</param>
        public override void UpdateTimeOfImpact(Collidable requester, float dt)
        {
            ConvexCollidable collidableA = CollidableA as ConvexCollidable;
            ConvexCollidable collidableB = CollidableB as ConvexCollidable;
            PositionUpdateMode modeA = collidableA.entity == null
                ? PositionUpdateMode.Discrete
                : collidableA.entity.PositionUpdateMode;
            PositionUpdateMode modeB = collidableB.entity == null
                ? PositionUpdateMode.Discrete
                : collidableB.entity.PositionUpdateMode;

            BroadPhaseOverlap overlap = BroadPhaseOverlap;
            if (
                (overlap.entryA.IsActive || overlap.entryB.IsActive) && //At least one has to be active.
                (
                    modeA == PositionUpdateMode.Continuous && //If both are continuous, only do the process for A.
                    modeB == PositionUpdateMode.Continuous &&
                    overlap.entryA == requester ||
                    (modeA == PositionUpdateMode.Continuous) ^ //If only one is continuous, then we must do it.
                    (modeB == PositionUpdateMode.Continuous)
                )
            )
            {
                //Only perform the test if the minimum radii are small enough relative to the size of the velocity.
                //Discrete objects have already had their linear motion integrated, so don't use their velocity.
                Vector3 velocity;
                if (modeA == PositionUpdateMode.Discrete)
                    //CollidableA is static for the purposes of this continuous test.
                {
                    velocity = collidableB.entity.linearVelocity;
                }
                else if (modeB == PositionUpdateMode.Discrete)
                    //CollidableB is static for the purposes of this continuous test.
                {
                    Vector3.Negate(ref collidableA.entity.linearVelocity, out velocity);
                }
                else
                    //Both objects are moving.
                {
                    Vector3.Subtract(ref collidableB.entity.linearVelocity, ref collidableA.entity.linearVelocity,
                        out velocity);
                }

                Vector3.Multiply(ref velocity, dt, out velocity);
                float velocitySquared = velocity.LengthSquared();

                float minimumRadiusA = collidableA.Shape.MinimumRadius * MotionSettings.CoreShapeScaling;
                timeOfImpact = 1;
                if (minimumRadiusA * minimumRadiusA < velocitySquared)
                {
                    //Spherecast A against B.
                    RayHit rayHit;
                    if (GJKToolbox.CCDSphereCast(new Ray(collidableA.worldTransform.Position, -velocity),
                        minimumRadiusA, collidableB.Shape, ref collidableB.worldTransform, timeOfImpact, out rayHit))
                    {
                        timeOfImpact = rayHit.T;
                    }
                }

                float minimumRadiusB = collidableB.Shape.MinimumRadius * MotionSettings.CoreShapeScaling;
                if (minimumRadiusB * minimumRadiusB < velocitySquared)
                {
                    //Spherecast B against A.
                    RayHit rayHit;
                    if (GJKToolbox.CCDSphereCast(new Ray(collidableB.worldTransform.Position, velocity), minimumRadiusB,
                        collidableA.Shape, ref collidableA.worldTransform, timeOfImpact, out rayHit))
                    {
                        timeOfImpact = rayHit.T;
                    }
                }

                //If it's intersecting, throw our hands into the air and give up.
                //This is generally a perfectly acceptable thing to do, since it's either sitting
                //inside another object (no ccd makes sense) or we're still in an intersecting case
                //from a previous frame where CCD took place and a contact should have been created
                //to deal with interpenetrating velocity.  Sometimes that contact isn't sufficient,
                //but it's good enough.
                if (timeOfImpact == 0)
                {
                    timeOfImpact = 1;
                }
            }
        }
    }
}