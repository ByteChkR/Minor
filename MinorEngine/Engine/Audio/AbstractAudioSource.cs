using System;
using Engine.Core;
using Engine.DataTypes;
using OpenTK.Audio.OpenAL;

namespace Engine.Audio
{
    /// <summary>
    /// Abstract Component that implements the AL Bindings to play sounds
    /// The Component is available as BackgroundMusicSource and as AudioSourceComponent
    /// </summary>
    public abstract class AbstractAudioSource : AbstractComponent
    {
        /// <summary>
        /// The OpenAL Handle to the Source in 3D Space
        /// </summary>
        protected readonly int source;


        /// <summary>
        /// Backing field for the Public Property Clip
        /// </summary>
        private AudioFile _clip;

        /// <summary>
        /// The Clip that is loaded by OpenAL
        /// </summary>
        public AudioFile Clip
        {
            get => _clip;
            set
            {
                bool wasPlaying = IsPlaying;
                if (IsPlaying)
                {
                    Stop();
                }

                _clip = value;


                AL.Source(source, ALSourcei.Buffer, _clip.Buffer);
                if (wasPlaying)
                {
                    Play();
                }
            }
        }


        /// <summary>
        /// A Flag to get the Playing state of the AudioSource
        /// </summary>
        public bool IsPlaying { get; private set; }

        /// <summary>
        /// A Wrapper to return the Current State of the OpenCL Audio Source bound to this object
        /// </summary>
        public ALSourceState SourceState => AL.GetSourceState(source);


        #region Public Properties

        /// <summary>
        /// A Property to get or set the pitch of the AudioSource
        /// </summary>
        public float Pitch
        {
            get
            {
                AL.GetSource(source, ALSourcef.Pitch, out var pitch);
                return pitch;
            }
            set => AL.Source(source, ALSourcef.Pitch, value);
        }

        /// <summary>
        /// A Property to get or set the MaxDistance of the AudioSource
        /// </summary>
        public float MaxDistance
        {
            get
            {
                AL.GetSource(source, ALSourcef.MaxDistance, out var maxDistance);
                return maxDistance;
            }
            set => AL.Source(source, ALSourcef.MaxDistance, value);
        }

        /// <summary>
        /// A Property to get or set the ReferenceDistance of the AudioSource
        /// </summary>
        public float ReferenceDistance
        {
            get
            {
                AL.GetSource(source, ALSourcef.ReferenceDistance, out var referenceDistance);
                return referenceDistance;
            }
            set => AL.Source(source, ALSourcef.ReferenceDistance, value);
        }


        /// <summary>
        /// A Property to get or set the MaxGain of the AudioSource
        /// </summary>
        public float MaxGain
        {
            get
            {
                AL.GetSource(source, ALSourcef.MaxGain, out var referenceDistance);
                return referenceDistance;
            }
            set => AL.Source(source, ALSourcef.MaxGain, value);
        }


        /// <summary>
        /// A Property to get or set the MinGain of the AudioSource
        /// </summary>
        public float MinGain
        {
            get
            {
                AL.GetSource(source, ALSourcef.MinGain, out var referenceDistance);
                return referenceDistance;
            }
            set => AL.Source(source, ALSourcef.MinGain, value);
        }


        /// <summary>
        /// A Property to get or set the Gain of the AudioSource
        /// </summary>
        public float Gain
        {
            get
            {
                AL.GetSource(source, ALSourcef.Gain, out var referenceDistance);
                return referenceDistance;
            }
            set => AL.Source(source, ALSourcef.Gain, value);
        }

        /// <summary>
        /// A Property to get or set the AirAbsorptionFactor of the AudioSource
        /// </summary>
        public float AirAbsorptionFactor
        {
            get
            {
                AL.GetSource(source, ALSourcef.EfxAirAbsorptionFactor, out var absorptionFactor);
                return absorptionFactor;
            }
            set => AL.Source(source, ALSourcef.EfxAirAbsorptionFactor, value);
        }

        /// <summary>
        /// A Property to get or set the RoomRollOffFactor of the AudioSource
        /// </summary>
        public float RoomRollOffFactor
        {
            get
            {
                AL.GetSource(source, ALSourcef.EfxRoomRolloffFactor, out var rollOffFactor);
                return rollOffFactor;
            }
            set => AL.Source(source, ALSourcef.EfxRoomRolloffFactor, value);
        }

        /// <summary>
        /// A Property to get or set the RollOffFactor of the AudioSource
        /// </summary>
        public float RollOffFactor
        {
            get
            {
                AL.GetSource(source, ALSourcef.RolloffFactor, out var rollOffFactor);
                return rollOffFactor;
            }
            set => AL.Source(source, ALSourcef.RolloffFactor, value);
        }


        /// <summary>
        /// A Property to get or set the Track Position of the AudioSource/Clip
        /// </summary>
        public float TrackPosition
        {
            get
            {
                AL.GetSource(source, ALGetSourcei.ByteOffset, out var current);
                var val = (float)current / Clip.BufferSize;

                return Math.Clamp(val, 0f, 1f);
            }
            set
            {
                var val = Math.Clamp(value, 0f, 1f);
                AL.Source(source, ALSourcei.ByteOffset, (int)(Clip.BufferSize * val));
            }
        }

        /// <summary>
        /// A Flag to get or set if the Clip should be looped after it ended
        /// </summary>
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


        /// <summary>
        /// Protected Constructor that will be called from inheriting classes to set up a audio source
        /// </summary>
        protected AbstractAudioSource()
        {
            source = AL.GenSource(); //Generating Audio Source

            //Setting Default Values
            AL.Source(source, ALSourcef.ReferenceDistance, 0f);
            AL.Source(source, ALSourcef.MaxDistance, 50f);
            AL.Source(source, ALSourcef.Pitch, 1f);
            AL.Source(source, ALSourcef.Gain, 1f);
        }

        /// <summary>
        /// Instructs the audio source to play the clip that has been assigned to it.
        /// </summary>
        public void Play()
        {
            IsPlaying = true;
            AL.SourcePlay(source);
        }

        /// <summary>
        /// Instructs the audio source to pause the clip that has been assigned to it.
        /// </summary>
        public void Pause()
        {
            IsPlaying = false;
            AL.SourcePause(source);
        }

        /// <summary>
        /// Instructs the audio source to stop the clip that has been assigned to it.
        /// </summary>
        public void Stop()
        {
            IsPlaying = false;
            AL.SourceStop(source);
            TrackPosition = 0;
        }
    }
}