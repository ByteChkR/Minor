__kernel void inverse(__global #type0* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	#type0 val = (#type0)(maxValue) - image[idx];
	image[idx] = (#type0)val;
}


__kernel void set(__global #type0* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, __global #type0* other)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	image[idx] = other[idx];
}

__kernel void setvalf(__global #type0* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float other)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	image[idx] = (#type0)(other*maxValue);
}

__kernel void setvalb(__global #type0* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, #type0 other)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	image[idx] = other;
}