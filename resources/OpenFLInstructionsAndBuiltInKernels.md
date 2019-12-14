# OpenFL Instructions and Built-in Kernels
## FL Instructions
### Setting Active Buffers/Channels
#### `setactive <Buffer>`
Sets the specified `Buffer` as the currently active buffer.
#### `setactive <ChannelID> <ChannelID> ...`
Sets the specified `ChannelID` as active, while all unspecified channels will be set inactive.
#### `setactive <Buffer> <ChannelID> <ChannelID> ...`
Sets the specified `Buffer` as the currently active buffer and enables the specified channels.

### Getting Random Data
#### `rnd`
Writes Non Uniform Random Data to all active channels of the currently active buffer.
#### `rnd <Buffer>`
Writes Non Uniform Random Data to all active channels in the `Buffer`.
#### `urnd`
Writes Non Uniform Random Data to all active channels of the currently active buffer.
#### `urnd <Buffer>`
Writes Non Uniform Random Data to all active channels in the `Buffer`.

### Jumping to Code Blocks
#### `jmp <FunctionName>`
Starts executing code in the function with the specified `FunctionName`
After the function executed the execution will be continued from the line of the jump statement.

### Break Points
#### `brk`
Breaks the Execution and Returns a the name of the Currently Active Buffer in the Step Result of the interpreter.
#### `brk <Buffer>`
Breaks the Execution and Returns a the name of the `Buffer` in the Step Result of the interpreter.

## Built-in Kernels
### Add
#### `add <Buffer>`
Adds the Values of `Buffer` to the Currently Active Buffer.
#### `addv <Value>`
Adds a uniform `Value` to the Currently Active Buffer.
#### `addc <Buffer>`
Adds the Values of `Buffer` to the Currently Active Buffer, but clamps the output.
#### `addvc <Value>`
Adds a uniform `Value` to the Currently Active Buffer, but clamps the output.
___
### Adjust Level
#### `adjustlevel <MinVal> <MaxVal>`
Rescales the values to be 0 when `MinVal` and to be 1 when `MaxVal`.
#### `adjustlevelrescale <MinVal> <MaxVal>`
Rescales the values to be 0 when `MinVal` and to be 1 when `MaxVal`, clamping all other values to 0 or 1.
___
### Divide
#### `div <Buffer>`
Divides the Values of the Currently Active Buffer by the Values of `Buffer`.
#### `divv <Value>`
Divides the Values of the Currently Active Buffer by `Value`.
___
### Invert
#### `invert`
Inverts the Currently Active Buffer.
___
### Mix
#### `mixt <BufferImageData> <BufferWeightData>`
Lerps the Values of the currently active Buffer with the Values of `BufferImageData` by the Values of `BufferWeightData`.
#### `mixv <BufferImageData> <WeightValue>`
Lerps the Values of the currently active Buffer with the Values of `BufferImageData` by a static `Value`.
___
### Modulo
#### `mod <Buffer>`
Performs a Modulo operation on the currently active buffer and `Buffer`. 
#### `modv <Value>`
Performs a Modulo operation on the currently active buffer and a static `Value`. 
___
### Multiplication
#### `mul <Buffer>`
Multiplies all Values of the currently active buffer with the values of `Buffer`.
#### `mulv <Value>`
Multiplies all Values of the currently active buffer with a static `Value`.
___
### Perlin Noise
#### `perlin <Persistence> <Octaves>`
Creates Perlin noise with specified `Persistence` and `Octaves` in the Currently Active Buffer. Buffer needs to have random initialized values to get perlin noise.
___
### Setting Values
#### `set <Buffer>`
Copies the Values of `Buffer` into the currently active buffer.
#### `setv <Value>`
Copies a static `Value` in all values of the currently active buffer
___
### Shapes
#### `point <X> <Y> <Z> <Radius>`
Creates a Point at `X`, `Y`, `Z` and sets all the values in the `Radius` to their relative distance to the specified point.
#### `circle <X> <Y> <Z> <Radius>`
Creates a Point at `X`, `Y`, `Z` and sets all the values in the `Radius` to 1.
___
### Smooth Noise
#### `smooth <Octave>`
Creates Smooth noise with the specified `Octave` in the currently active buffer.
___
### Subtraction
#### `sub <Buffer>`
Subtracts all Values of the currently active buffer by the values of `Buffer`.
#### `subv <Value>`
Subtracts all Values of the currently active buffer by a static `Value`.