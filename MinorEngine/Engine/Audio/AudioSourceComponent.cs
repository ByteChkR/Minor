﻿using OpenTK;
using OpenTK.Audio.OpenAL;

namespace Engine.Audio
{
    /// <summary>
    /// Audio Source Component that implements the Functionality to Play 3D Sounds
    /// </summary>
    public class AudioSourceComponent : AbstractAudioSource
    {
        /// <summary>
        /// OnDestroy Function. Gets called when the Component or the GameObject got removed from the game
        /// This function is called AFTER the engines update function. So it can happen that before the object is destroyed it can still collide and do other things until its removed at the end of the frame.
        /// </summary>
        protected override void OnDestroy()
        {
            if (IsPlaying)
            {
                Stop();
            }
        }

        /// <summary>
        /// Update Function
        /// </summary>
        /// <param name="deltaTime">Delta Time in Seconds</param>
        protected override void Update(float deltaTime)
        {
            var v = new Vector4(Owner.GetLocalPosition(), 1);
            v *= Owner.GetWorldTransform() * Owner.Scene.Camera.ViewMatrix;

            var v3 = new Vector3(v);

            AL.Source(source, ALSource3f.Position, ref v3);
        }
    }
}