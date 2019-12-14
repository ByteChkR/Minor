# Into OpenCL
Thanks to the rather big wrapper around the OpenCL Implementation, running an OpenCL Kernel can be done in less than 10 lines.
In the Example we will create a Texture that is dark red.
First we specify the image size:
```csharp
	int imageSize = 512 * 512 * 4; //Width * Height * ChannelCount
```
Now we need to Create an OpenCL Kernel Database that will do some behind the scenes work and will provide us with a nicer interface.
We Specify a folder where all the OpenCL Kernels are.
We Specify a the Uchar1 data type which is equivalent for byte in C#.
```csharp
	KernelDatabase db = new KernelDatabase(Clapi.MainThread, "assets/test_kernel/", DataTypes.Uchar1);
```
We now receive the kernel from the database by specifying the kernel name
```csharp
	bool foundKernel = db.TryGetClKernel("kernel_red", out CLKernel redKernel);
```
For an image we need some space in OpenCL Memory to do our computations on.
The Clapi class has a lot of wrappers to create MemoryBuffers.
Here we are using Clapi.CreateEmpty<T>()
```csharp
	MemoryBuffer imageBuffer = Clapi.CreateEmpty<byte>(
		Clapi.MainThread, //Instance of the Main Thread
		imageSize, //The Image Size in bytes
		MemoryFlag.ReadWrite); //We want to read AND write to the memory buffer
```
We can now start to set the arguments to the kernel
```csharp
	//With plain OpenCL you would need to Set the Arguments/Buffers by their argument index/types/size/yada yads,
	//thanks to the CL abstraction for the engine, we can just specify the argument name how we do in OpenGL Shaders(But Faster).
    redKernel.SetBuffer("imageData", imageBuffer);

    //Set Arg has the capabilities to automatically cast the value that is passed to the right type,
    //however this is not really fast and can be avoided by specifying the correct type directly.
    redKernel.SetArg("strength", 0.5f); //We directly pass a float, no casting required

    //When We pass something as byte(uchar in cl), we need to cast it.
    //If we dont the Engine OpenCL Wrapper will automatically convert the integer into a byte, but it will apply rescaling
    //  This takes over automatic type conversion from float(opengl) to byte(System.Bitmap/opencl)
    //  Calculation when not passed as byte: (4 / Int32.MaxSize) * byte.MaxValue.
    redKernel.SetArg("channelCount", (byte)4); 
```
Running the Kernel is also quite straight forward:
```csharp
	Clapi.Run(Clapi.MainThread, redKernel, imageSize);
```
The Only thing we now need to do now is to create a Texture from the memory buffer we just ran our kernel on.
This can be done by using the TextureLoader class.
```csharp
	Texture tex = TextureLoader.BytesToTexture(
		Clapi.ReadBuffer<byte>(Clapi.MainThread, imageBuffer, imageSize), //We Read the Buffer from the memory object, which returns byte[]
		512, //We pass the width
		512); //And height.
	Note: Channel count can be ommitted, since the engine is only working with 4 channel textures
```

# Continue Reading
* [AI](AI.md)
* [Audio](Audio.md)
* [Creating A Scene](CreatingAScene.md)
* [Deploying](Deploying.md)
* [General Info](GeneralInfo.md)
* [Getting Started](GettingStarted.md)
* [OpenFL](OpenFL.md)
* [OpenFL Advanced](OpenFL_Advanced.md)
* [OpenFL Instructions and Built-in Kernels](OpenFLInstructionsAndBuiltInKernels.md)
* [Physics](Physics.md)