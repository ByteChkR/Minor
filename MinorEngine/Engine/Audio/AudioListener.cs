using Engine.Core;
using OpenTK;
using OpenTK.Audio.OpenAL;

namespace Engine.Audio
{
    public class AudioListener : AbstractComponent
    {
        protected override void Update(float deltaTime)
        {
            var v = new Vector4(Owner.GetLocalPosition(), 1);
            v *= Owner.GetWorldTransform() * Owner.Scene.Camera.ViewMatrix;

            var v3 = new Vector3(v);

            AL.Listener(ALListener3f.Position, ref v3);
        }
    }
}