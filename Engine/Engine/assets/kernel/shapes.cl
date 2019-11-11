#include utils.cl

__kernel void point(__global uchar* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float positionX, float positionY, float positionZ, float radius)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	int3 id3d = Get3DimensionalIndex(dimensions.x, dimensions.y, idx/channelCount);
	float rad = radius * dimensions.x;
	float dist = distance((float3)(positionX*dimensions.x,positionY*dimensions.y,positionZ*dimensions.z), (float3)(id3d.x, id3d.y, id3d.z));
	float addition = (1-dist/rad) * maxValue;
	image[idx] += clamp(addition, (float)0, maxValue);
}

__kernel void circle(__global uchar* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float positionX, float positionY, float positionZ, float radius)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}
	int3 id3d = Get3DimensionalIndex(dimensions.x, dimensions.y, idx/channelCount);
	float rad = radius * dimensions.x;
	float dist = distance((float3)(positionX*dimensions.x,positionY*dimensions.y,positionZ*dimensions.z), (float3)(id3d.x, id3d.y, id3d.z));
	if(dist <= rad)
	{
		image[idx] = 255;
	}
}