#include utils.cl

__kernel void mixt(__global uchar* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, __global uchar* overlay, __global uchar* weightOverlay)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	image[idx]=Lerp(image[idx], overlay[idx], weightOverlay[idx] / maxValue);
}

__kernel void mixv(__global uchar* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, __global uchar* overlay, float weightOverlay)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	image[idx]=Lerp(image[idx], overlay[idx], weightOverlay);
}