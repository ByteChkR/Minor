#include source/perlin.cl


__kernel void perlin(__global #type0* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float persistence, int octaves)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}
	float val = GetPerlinNoise(image, idx, channel, maxValue, dimensions.x, dimensions.y, dimensions.z, persistence, octaves);
	image[idx] = FromFloatN((#type1)val);
	
}