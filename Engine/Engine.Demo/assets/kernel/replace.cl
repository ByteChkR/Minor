__kernel void replacebelow(__global uchar* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float thresh, float replace)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	if(image[idx] <= (uchar)(thresh*maxValue))
	{
		image[idx] = (uchar)(replace * maxValue);
	}
}

__kernel void replaceabove(__global uchar* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float thresh, float replace)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	if(image[idx] >= (uchar)(thresh*maxValue))
	{
		image[idx] = (uchar)(replace * maxValue);
	}
}