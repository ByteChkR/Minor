# Audio
The Audio System of the Engine is using OpenAL as backend.
At the point of writing the only file that is supported is the WAV file format.
Note: There is no classical "Volume" Property. Instead it is called Gain and can be used to archieve the same effect.
## Creating an Audio Listener
```csharp
	bc.AddComponent(new AudioListener()); //Only one listener is allowed in a scene.
```

## Creating an Audio Source
```csharp
	AudioSourceComponent asc = new AudioSourceComponent(); //Creating the Audio Source
	if (!AudioLoader.TryLoad("assets/sound.wav", out AudioFile file))
	{
		throw new ApplicationException("Could not load audio file: assets/sound.wav"); //No Audio File Found or not recognized as WAV/RIFF file
	}
	asc.Clip = file; //The Clip we loaded
	asc.Looping = true; //Loop it for maximum earbleed
    asc.UpdatePosition = true; //Enable 3D Tracking the Gameobjects movements and apply it to the audio source(Only works with mono audio)
	asc.Play(); //Playing it.
	box.AddComponent(asc);
```

# Continue Reading
* [AI](AI.md)
* [Creating A Scene](CreatingAScene.md)
* [Deploying](Deploying.md)
* [General Info](GeneralInfo.md)
* [Getting Started](GettingStarted.md)
* [Into OpenCL](IntoOpenCL.md)
* [OpenFL](OpenFL.md)
* [OpenFL Advanced](OpenFL_Advanced.md)
* [OpenFL Instructions and Built-in Kernels](OpenFLInstructionsAndBuiltInKernels.md)
* [Physics](Physics.md)