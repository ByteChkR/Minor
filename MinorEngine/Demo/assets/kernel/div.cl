#include utils.cl

__kernel void div(__global uchar* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, __global uchar* overlay)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	image[idx] /= overlay[idx] / maxValue;
}

__kernel void divv(__global uchar* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	image[idx] /= value;
}