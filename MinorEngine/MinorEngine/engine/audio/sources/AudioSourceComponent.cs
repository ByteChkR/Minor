using Common;
using MinorEngine.engine.components;
using MinorEngine.engine.core;
using OpenTK;
using OpenTK.Audio.OpenAL;

namespace MinorEngine.engine.audio.sources
{
    public class AudioSourceComponent : AbstractAudioSource
    {
        public override void Update(float deltaTime)
        {
            Vector4 v = new Vector4(Owner.GetLocalPosition(), 1);
            v *= Owner.GetWorldTransform() * Owner.World.ViewMatrix;

            Vector3 v3 = new Vector3(v);

            AL.Source(source, ALSource3f.Position, ref v3);
            AL.Source(source, ALSourceb.SourceRelative, true);
            
        }
    }
}