using System.Collections.Generic;
//using BepuPhysics;
//using BepuPhysics.Collidables;
//using BepuUtilities.Memory;
using MinorEngine.engine.components;
using MinorEngine.engine.physics;
using MinorEngine.engine.rendering;
using OpenTK;

namespace MinorEngine.components
{
    public class MeshColliderComponent : AbstractComponent, IColliderComponent
    {
        //public BodyReference BodyReference { get; set; }
        private readonly ushort layer = ushort.MaxValue;
        private readonly ushort collidable = ushort.MaxValue;
        private readonly GameModel model;
        private readonly float mass;

        public MeshColliderComponent(float mass, ushort layer, ushort collidableLayers, GameModel mesh)
        {
            this.layer = layer;
            this.mass = mass;
            collidable = collidableLayers;
            model = mesh;
        }

        protected override void Awake()
        {
            base.Awake();

            Vector3 pos = Owner.GetLocalPosition();

            List<Vector3> vertexList = new List<Vector3>();
            for (int i = 0; i < model.Meshes.Count; i++) vertexList.AddRange(model.Meshes[i].ToSequentialVertexList());

            int len = vertexList.Count / 3;
            //BufferPool p = new BufferPool();
            //Buffer<Triangle> triBuf;
            //p.Take(len, out triBuf);

            //for (int vert = 0; vert < len; vert++)
            //    triBuf[vert] = new Triangle(
            //        new System.Numerics.Vector3(vertexList[vert].X, vertexList[vert].Y, vertexList[vert].Z),
            //        new System.Numerics.Vector3(vertexList[vert + 1].X, vertexList[vert + 1].Y, vertexList[vert + 1].Z),
            //        new System.Numerics.Vector3(vertexList[vert + 2].X, vertexList[vert + 2].Y,
            //            vertexList[vert + 2].Z));


            //Mesh m = new Mesh(triBuf, System.Numerics.Vector3.One, p);

            //BodyReference = Physics.AddMeshDynamic(mass, new System.Numerics.Vector3(pos.X, pos.Y, pos.Z), m);


            //ref Layer l = ref Physics.CollisionFilters.Allocate(BodyReference.Handle);
            //l.CollidableSubgroups = collidable;
            //l.SubgroupMembership = layer;
            //l.GroupId = 0;
        }

        protected override void Update(float deltaTime)
        {
            //Owner.SetLocalPosition(new Vector3(BodyReference.Pose.Position.X, BodyReference.Pose.Position.Y,
            //    BodyReference.Pose.Position.Z));
            //Owner.SetRotation(BodyReference.Pose.Orientation);
        }
    }
}