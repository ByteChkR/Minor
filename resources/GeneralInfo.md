# General Info
## Engine Settings
Engine Settings can be changed during startup of the engine.
Either by Loading the Engine Settings From File or Using/Editing the `EngineSettings.DefaultSettings`.
```csharp
EngineConfig.LoadConfig(
	"assets/configs/engine_settings.xml", //Load this File
	Assembly.GetAssembly(typeof(GameEngine)), //Get The Assembly Engine
	"Engine.Core"); //The Namespace where the settings will get applied to.
GameEngine engine = new GameEngine(EngineSettings.Settings); //Starting the Game Engine with the Modified Settings.
```
## Debug Settings
Debug Settings are Contained in the `EngineSettings` class. The Class contains LogStreamObjects that contain parameters for the underlying Debugging Framework to configure the Log Output
Debugging can be disabled entirely by setting `DebugSettings.Enabled` = false
The Stage Names are used as a Prefix for the different channels where logs can be sent to.
For more information take a look at [the ADL Repository](https://github.com/ByteChkR/ADL).

# Continue Reading
* [AI](AI.md)
* [Audio](Audio.md)
* [Creating A Scene](CreatingAScene.md)
* [Deploying](Deploying.md)
* [Getting Started](GettingStarted.md)
* [Into OpenCL](IntoOpenCL.md)
* [OpenFL](OpenFL.md)
* [OpenFL Advanced](OpenFL_Advanced.md)
* [OpenFL Instructions and Built-in Kernels](OpenFLInstructionsAndBuiltInKernels.md)
* [Physics](Physics.md)