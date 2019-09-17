__kernel void addval(__global uchar* image, int3 dimensions, int channelCount, __global uchar* channelEnableState, uchar value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	int val = image[idx] + value;
	image[idx] = val / 2;
}

__kernel void addvalwrap(__global uchar* image, int3 dimensions, int channelCount, __global uchar* channelEnableState, uchar value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	int val = image[idx] + value;
	image[idx] = (uchar)fmod((float)val, 255.0f);
}

__kernel void addtexvalmask(__global uchar* image, int3 dimensions, int channelCount, __global uchar* channelEnableState, float mask, __global uchar* value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	int val = (int)((float)image[idx] + (value[idx] * mask));
	image[idx] = val / 2;
}

__kernel void addtexvalmaskwrap(__global uchar* image, int3 dimensions, int channelCount, __global uchar* channelEnableState, float mask, __global uchar* value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	int val = (int)((float)image[idx] + (value[idx] * mask));
	image[idx] = (uchar)fmod((float)val, 255.0f);
}

__kernel void addtextexmask(__global uchar* image, int3 dimensions, int channelCount, __global uchar* channelEnableState, __global uchar* mask, __global uchar* value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	float weight = (float)(mask[idx] / 255.0f);
	int val = (int)((float)image[idx] + (value[idx] * weight));
	image[idx] = val / 2;
}

__kernel void addtextexmaskwrap(__global uchar* image, int3 dimensions, int channelCount, __global uchar* channelEnableState, __global uchar* mask, __global uchar* value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	float weight = (float)(mask[idx] / 255.0f);

	int val = (int)((float)(image[idx]) + (value[idx] * weight));
	image[idx] = (uchar)fmod((float)val, 255.0f);
}