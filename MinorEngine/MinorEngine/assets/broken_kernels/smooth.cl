#include source/smooth.cl

__kernel void smooth(__global #type0* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, int octaves)
{


	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	int samplePeriod = 1 << octaves;
	float sampleFrequency = 1.0f / samplePeriod;

	image[idx] = From#type1(GetSmoothNoise(image, idx, channel, dimensions.x, dimensions.y, dimensions.z, maxValue, samplePeriod, sampleFrequency)*maxValue);
}
