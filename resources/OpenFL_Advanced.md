# OpenFL Advanced
## FLRunner
FLRunner classes can be used to simplify and abstract the usage of the Interpreter by introducing processing queues.
A FLRunner Gets created by supplying it with a CLApi instance
```csharp
	FlRunner flRunner = new FlRunner(Clapi.MainThread);
```
You Can enqueue FlExecutionContexts by calling `flRunner.Enqueue(executionContext);` and process all enqueued elements by calling `flRunner.Process()`.

### FlExecutionContexts
The FLRunner Class works with FLExecutionContexts, which can be created with its constructor.
```csharp
	FlExecutionContext fle = new FlExecutionContext(
		path, //Path to the FL Script File
		_tex, //The Input Texture
		texMap, //The Texture Map
		null); //Action that gets called when the processing of this element finished.
```
Texture Maps are Dictionaries that map the Interpreters Output after execution to Textures.
```csharp
Dictionary<string, Texture> texMap2 = new Dictionary<string, Texture>
                {
                	//"result" is always the Output of the interpreter.GetResult<T> Call
                    {"result", _tex2 } 
                };
```
This enables applying buffers to more than one output texture providing you know the name of the buffer in the FLScript file.
For example you could define a texture in the FL Script that is called "specular_out" and while generating the main texture you can generate the specular texture as well. In C# you could then just specify the specular texture with `{"specular_out", specularTexture}` in the Texture Map.

## Multi Threaded OpenFL
For Multithreaded OpenFL Script Processing you can use the `FlMultiThreadRunner` class that inherits from the FLRunner class and has the same behaviour. When using the MultiThreaded Version of the FLRunner, it is advisable to define the onFinishCallback parameter when creating `FlExecutionContexts`

# Continue Reading
* [AI](AI.md)
* [Audio](Audio.md)
* [Creating A Scene](CreatingAScene.md)
* [Deploying](Deploying.md)
* [General Info](GeneralInfo.md)
* [Getting Started](GettingStarted.md)
* [Into OpenCL](IntoOpenCL.md)
* [OpenFL](OpenFL.md)
* [OpenFL Instructions and Built-in Kernels](OpenFLInstructionsAndBuiltInKernels.md)
* [Physics](Physics.md)