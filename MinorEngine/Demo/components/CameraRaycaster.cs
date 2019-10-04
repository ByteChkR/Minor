using System.Collections.Generic;
using MinorEngine.BEPUutilities;
using MinorEngine.components;
using MinorEngine.debug;
using MinorEngine.engine.components;
using MinorEngine.engine.core;
using MinorEngine.engine.physics;
using OpenTK.Input;
using Vector3 = OpenTK.Vector3;

namespace Demo.components
{
    public class CameraRaycaster:AbstractComponent
    {
        Layer cast;
        private GameObject sphereTargetMarker;
        public CameraRaycaster(GameObject targetmarker, Layer raycast)
        {
            cast = raycast;
            sphereTargetMarker = targetmarker;
        }

        protected override void Update(float deltaTime)
        {
            Ray r = ConstructRayFromMousePosition();
            bool ret = Physics.RayCastFirst(r, 1000, cast,
                out KeyValuePair<Collider, RayHit> arr);
            if (ret)
            {
                this.Log("Ray Dir: " + r.Direction, DebugChannel.Log);
                sphereTargetMarker.SetLocalPosition(arr.Value.Location);
            }
        }

        private Ray ConstructRayFromMousePosition()
        {
            Vector2 mpos = GameEngine.Instance.MousePosition;
            this.Log("Mouse Pos: "+mpos, DebugChannel.Log);
            Vector3 mousepos = GameEngine.Instance.convertScreenToWorldCoords((int)mpos.X, (int)mpos.Y);
            return new Ray(Owner.GetLocalPosition(), (mousepos - Owner.GetLocalPosition()).Normalized());
        }
    }
}