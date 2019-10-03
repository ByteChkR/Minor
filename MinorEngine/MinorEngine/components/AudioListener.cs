using GameEngine.engine.components;
using OpenTK;
using OpenTK.Audio.OpenAL;

namespace GameEngine.engine.audio
{
    public class AudioListener : AbstractComponent
    {
        protected override void Update(float deltaTime)
        {
            Vector4 v = new Vector4(Owner.GetLocalPosition(), 1);
            v *= Owner.GetWorldTransform() * Owner.World.Camera.ViewMatrix;

            Vector3 v3 = new Vector3(v);

            AL.Listener(ALListener3f.Position, ref v3);
        }
    }
}