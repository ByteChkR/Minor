__kernel void inverse(__global uchar* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	int val = 255 - image[idx];
	image[idx] = val;
}


__kernel void set(__global uchar* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, __global uchar* other)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	image[idx] = other[idx];
}

__kernel void setvalf(__global uchar* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float other)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	image[idx] = (uchar)(other*255.0f);
}

__kernel void setvalb(__global uchar* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, uchar other)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	image[idx] = other;
}