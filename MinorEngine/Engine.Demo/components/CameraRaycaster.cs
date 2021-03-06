﻿using System.Collections.Generic;
using Engine.Core;
using Engine.Physics;
using Engine.Physics.BEPUutilities;
using OpenTK.Input;
using Vector3 = OpenTK.Vector3;

namespace Engine.Demo.components
{
    public class CameraRaycaster : AbstractComponent
    {
        private int cast;
        private GameObject sphereTargetMarker;
        private GameObject looker;
        private float yoff;


        public CameraRaycaster(GameObject targetmarker, float yOffset, GameObject looker)
        {
            cast = LayerManager.NameToLayer("raycast");
            sphereTargetMarker = targetmarker;
            this.looker = looker;
            yoff = yOffset;
        }

        protected override void Update(float deltaTime)
        {
            Ray r = ConstructRayFromMousePosition();
            bool ret = PhysicsEngine.RayCastFirst(r, 1000, cast,
                out KeyValuePair<Collider, RayHit> arr);
            if (ret)
            {
                Vector3 pos = arr.Value.Location;
                pos.Y = looker.LocalPosition.Y;
                sphereTargetMarker.SetLocalPosition(pos);
                looker.LookAt(sphereTargetMarker);
            }
        }

        protected override void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.B)
            {
                Ray r = ConstructRayFromMousePosition();
                bool ret = PhysicsEngine.RayCastFirst(r, 1000, cast,
                    out KeyValuePair<Collider, RayHit> arr);
                if (ret)
                {
                    Vector3 pos = arr.Value.Location;
                    pos.Y += looker.LocalPosition.Y;
                    sphereTargetMarker.SetLocalPosition(pos);
                    looker.LookAt(sphereTargetMarker);
                }
            }
        }

        private Ray ConstructRayFromMousePosition()
        {
            Vector2 mpos = GameEngine.Instance.MousePosition;
            Vector3 mousepos = GameEngine.Instance.ConvertScreenToWorldCoords((int) mpos.X, (int) mpos.Y);
            return new Ray(Owner.GetLocalPosition(), (mousepos - Owner.GetLocalPosition()).Normalized());
        }
    }
}