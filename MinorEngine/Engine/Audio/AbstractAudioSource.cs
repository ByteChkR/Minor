using System;
using Engine.Core;
using Engine.DataTypes;
using OpenTK.Audio.OpenAL;

namespace Engine.Audio
{
    public class AbstractAudioSource : AbstractComponent
    {
        protected readonly int source;
        private AudioFile clip;


        public bool IsPlaying { get; private set; }
        public ALSourceState SourceState => AL.GetSourceState(source);


        #region Public Properties

        public float Pitch
        {
            get
            {
                AL.GetSource(source, ALSourcef.Pitch, out var pitch);
                return pitch;
            }
            set => AL.Source(source, ALSourcef.Pitch, value);
        }

        public float MaxDistance
        {
            get
            {
                AL.GetSource(source, ALSourcef.MaxDistance, out var maxDistance);
                return maxDistance;
            }
            set => AL.Source(source, ALSourcef.MaxDistance, value);
        }

        public float ReferenceDistance
        {
            get
            {
                AL.GetSource(source, ALSourcef.ReferenceDistance, out var referenceDistance);
                return referenceDistance;
            }
            set => AL.Source(source, ALSourcef.ReferenceDistance, value);
        }

        public float MaxGain
        {
            get
            {
                AL.GetSource(source, ALSourcef.MaxGain, out var referenceDistance);
                return referenceDistance;
            }
            set => AL.Source(source, ALSourcef.MaxGain, value);
        }

        public float MinGain
        {
            get
            {
                AL.GetSource(source, ALSourcef.MinGain, out var referenceDistance);
                return referenceDistance;
            }
            set => AL.Source(source, ALSourcef.MinGain, value);
        }

        public float Gain
        {
            get
            {
                AL.GetSource(source, ALSourcef.Gain, out var referenceDistance);
                return referenceDistance;
            }
            set => AL.Source(source, ALSourcef.Gain, value);
        }

        public float AirAbsorptionFactor
        {
            get
            {
                AL.GetSource(source, ALSourcef.EfxAirAbsorptionFactor, out var absorptionFactor);
                return absorptionFactor;
            }
            set => AL.Source(source, ALSourcef.EfxAirAbsorptionFactor, value);
        }


        public float RoomRollOffFactor
        {
            get
            {
                AL.GetSource(source, ALSourcef.EfxRoomRolloffFactor, out var rollOffFactor);
                return rollOffFactor;
            }
            set => AL.Source(source, ALSourcef.EfxRoomRolloffFactor, value);
        }

        public float RollOffFactor
        {
            get
            {
                AL.GetSource(source, ALSourcef.RolloffFactor, out var rollOffFactor);
                return rollOffFactor;
            }
            set => AL.Source(source, ALSourcef.RolloffFactor, value);
        }

        public float TrackPosition
        {
            get
            {
                AL.GetSource(source, ALGetSourcei.ByteOffset, out var current);
                var val = (float) current / clip.BufferSize;

                return Math.Clamp(val, 0f, 1f);
            }
            set
            {
                var val = Math.Clamp(value, 0f, 1f);
                AL.Source(source, ALSourcei.ByteOffset, (int) (clip.BufferSize * val));
            }
        }

        public bool Looping
        {
            get
            {
                AL.GetSource(source, ALSourceb.Looping, out var state);
                return state;
            }
            set => AL.Source(source, ALSourceb.Looping, value);
        }

        #endregion


        public AbstractAudioSource()
        {
            source = AL.GenSource();

            AL.Source(source, ALSourcef.ReferenceDistance, 0f);
            AL.Source(source, ALSourcef.MaxDistance, 50f);
            AL.Source(source, ALSourcef.Pitch, 1f);
            AL.Source(source, ALSourcef.Gain, 1f);
        }


        public void Play()
        {
            IsPlaying = true;
            AL.SourcePlay(source);
        }

        public void Pause()
        {
            IsPlaying = false;
            AL.SourcePause(source);
        }

        public void Stop()
        {
            IsPlaying = false;
            AL.SourceStop(source);
            TrackPosition = 0;
        }

        public void SetClip(AudioFile clip)
        {
            if (IsPlaying)
            {
                Stop();
            }

            this.clip = clip;


            AL.Source(source, ALSourcei.Buffer, clip.Buffer);
        }

        public AudioFile GetClip()
        {
            return clip;
        }
    }
}