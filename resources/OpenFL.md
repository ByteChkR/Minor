# OpenFL
OpenFL is an Interpreter Language that comes with the engine, it is aimed at reducing the effort required to generate textures and to create sequences of CL Kernel executions.

## Setting up the Interpreter in C#
The interpreter operates on a script and an input buffer.
The code from the setup is taken from the Example Repository.
First we need to create an input buffer almost exactly like in the IntoOpenCL Example.
```csharp
	int texWidth = 128; //Width of the input texture
    int texHeight = 128; //Height of the input texture
    MemoryBuffer buffer = //Creating the Input Buffer
        Clapi.CreateEmpty<byte>(
            Clapi.MainThread, //We use the Main thread instance
            texWidth * texHeight * 4, //The image size in bytes
            MemoryFlag.ReadWrite); //We want to read and write to the texture
```
Then we can already start to initialize the Interpreter
Note: The Constructor is parsing the Script and is Processing the Define Statements, so this can take some time depending on the script
```csharp
	Interpreter i = new Interpreter( //Creating an interpreter instance
        Clapi.MainThread, //We use the main thread
        "assets/filter/red.fl", //The file to execute
        DataTypes.Uchar1, //The Data type of our input buffer
        buffer, //The buffer
        texWidth, //Width
        texHeight, //Height
        1, //Depth, for images always 1
        4, //Channel count(BGRA)
        "assets/kernel/", //Directory for all kernels
        true); //the "brk" statement is ignored.
```
So now we can start processing the script we supplied.
The Script:
```yaml
Main: #Every FL script that will be executed needs a Main function that is serving as entry point.
    setactive 2 #Activate channel 2 (BG[R]A)
    setv 1 #Set the channel value to 1
```
We do this by making a do while loop like so.
```csharp
	do
	{
    	i.Step(); //Step through the Instructions one by one until the script terminated.
    } while (!i.Terminated);

```
After the Script terminated, the Result buffer(The buffer that is left as active by the script) can be returned by calling interpreter.GetResult<T>().
Alternatively any buffer can be returned by calling interpreter.GetBuffer(string) providing the name as an argument.
Doing this will result in the buffer not beeing disposed when the interpreter is disposing.
```csharp
	byte[] buf = i.GetResult<byte>(); //Returning the Result buffer as Bytes
	Texture tex = TextureLoader.BytesToTexture(buf, texWidth, texHeight); //Making a texture.
```

# Continue Reading
* [AI](AI.md)
* [Audio](Audio.md)
* [Creating A Scene](CreatingAScene.md)
* [Deploying](Deploying.md)
* [General Info](GeneralInfo.md)
* [Getting Started](GettingStarted.md)
* [Into OpenCL](IntoOpenCL.md)
* [OpenFL Advanced](OpenFL_Advanced.md)
* [OpenFL Instructions and Built-in Kernels](OpenFLInstructionsAndBuiltInKernels.md)
* [Physics](Physics.md)