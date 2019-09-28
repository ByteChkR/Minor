__kernel void inverse(__global float* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	float val = maxValue - image[idx];
	image[idx] = (float)val;
}


__kernel void set(__global float* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, __global float* other)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	image[idx] = other[idx];
}

__kernel void setvalf(__global float* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float other)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	image[idx] = (float)(other*maxValue);
}

__kernel void setvalb(__global float* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float other)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	image[idx] = other;
}
