#include mod.cl #type0 #type1
__kernel void addval(__global #type0* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, #type0 value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	#type0 val = (image[idx]) + value;
	image[idx] = (#type0)(val / 2);
}

__kernel void addvalwrap(__global #type0* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, #type0 value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	#type0 val = image[idx] + value;
	image[idx] = Modulus(val, maxValue);
}

__kernel void addtexvalmask(__global #type0* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float mask, __global #type0* value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	#type0 val = image[idx] + (value[idx] * (#type0)(mask));
	image[idx] = (#type0)(val / 2);
}

__kernel void addtexvalmaskwrap(__global #type0* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float mask, __global #type0* value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	#type0 val = image[idx] + (value[idx] * (#type0)(mask));
	image[idx] = Modulus(val, maxValue);
}

__kernel void addtextexmask(__global #type0* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, __global #type0* mask, __global #type0* value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	#type0 weight = (#type0)(mask[idx] / (#type0)(maxValue));
	#type0 val = (#type0)((#type0)image[idx] + (value[idx] * weight));
	image[idx] = (#type0)(val / 2);
}

__kernel void addtextexmaskwrap(__global #type0* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, __global #type0* mask, __global #type0* value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	#type0 weight = (#type0)(mask[idx] / (#type0)maxValue);

	#type0 val = (#type0)((#type0)(image[idx]) + (value[idx] * weight));
	image[idx] = Modulus(val, maxValue);
}