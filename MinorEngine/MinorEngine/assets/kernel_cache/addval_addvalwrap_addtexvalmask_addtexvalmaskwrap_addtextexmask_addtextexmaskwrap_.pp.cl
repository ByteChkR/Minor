__kernel void addval(__global float4* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float4 value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	float val = (float)((float)(image[idx]) + value);
	image[idx] = (float4)(val / 2);
}

__kernel void addvalwrap(__global float4* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float4 value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	int val = image[idx] + value;
	image[idx] = (uchar)fmod((float)val, maxValue);
}

__kernel void addtexvalmask(__global float4* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float mask, __global float4* value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	float val = (float)((float)image[idx] + (value[idx] * mask));
	image[idx] = (float4)(val / 2);
}

__kernel void addtexvalmaskwrap(__global float4* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float mask, __global float4* value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	float val = (float)((float)image[idx] + (value[idx] * mask));
	image[idx] = (float4)fmod(val, maxValue);
}

__kernel void addtextexmask(__global float4* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, __global float4* mask, __global float4* value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	float weight = (float)(mask[idx] / maxValue);
	float val = (float)((float)image[idx] + (value[idx] * weight));
	image[idx] = (float4)(val / 2);
}

__kernel void addtextexmaskwrap(__global float4* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, __global float4* mask, __global float4* value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	float weight = (float)(mask[idx] / maxValue);

	float val = (float)((float)(image[idx]) + (value[idx] * weight));
	image[idx] = (float4)fmod((float)val, maxValue);
}
