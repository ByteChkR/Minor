#include utils.cl


__kernel void adjustlevelrescale(__global uchar* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float minVal, float maxVal)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}
	uchar minValB = maxValue*minVal;
	uchar maxValB = maxValue*maxVal;
	uchar dist = maxValB-minValB;
	


	if(minValB > image[idx])
	{
		image[idx] = 0;
	}
	else if(maxValB < image[idx])
	{
		image[idx] = (uchar)maxValue;
	}
	else
	{
		image[idx] = image[idx] - minValB / dist;
	}

}

__kernel void adjustlevel(__global uchar* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float minVal, float maxVal)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}
	
	image[idx] = (uchar)clamp((float)image[idx], minVal*maxValue, maxVal*maxValue);

}
