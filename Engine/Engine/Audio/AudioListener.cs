using Engine.Core;
using OpenTK;
using OpenTK.Audio.OpenAL;

namespace Engine.Audio
{
    /// <summary>
    /// Simple component that updates the Audio Listener position and sends it to OpenAL for 3D Sound
    /// </summary>
    public class AudioListener : AbstractComponent
    {
        /// <summary>
        /// Update Function
        /// </summary>
        /// <param name="deltaTime">Delta Time in Seconds</param>
        protected override void Update(float deltaTime)
        {
            Vector3 v3 = Owner.GetWorldPosition();

            AL.Listener(ALListener3f.Position, ref v3);
        }
    }
}