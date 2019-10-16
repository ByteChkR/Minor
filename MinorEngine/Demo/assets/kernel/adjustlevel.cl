#include utils.cl

__kernel void adjustlevel(__global uchar* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float minVal, float maxVal)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}
	uchar minValB = maxValue*minVal;
	uchar maxValB = maxValue*maxVal;
	
	if(minValB > image[idx])
	{
		image[idx] = minValB;
	}
	else if(maxValB < image[idx])
	{
		image[idx] = maxValB;
	}

}
