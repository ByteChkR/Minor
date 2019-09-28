__kernel void mulval(__global #type0* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}
	float imgVal = (float)(image[idx]/maxValue);
	float otherVal = (float)(value);
	float val = (float)(imgVal * otherVal * maxValue);
	image[idx] = val;
}
__kernel void multexvalmask(__global #type0* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float mask, __global #type0* value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}	
	float weight = (float)(mask);
	float imgVal = (float)(image[idx] / maxValue);
	float otherVal = (float)(value[idx] / maxValue);
	float val = (float)(imgVal * (otherVal * weight) * maxValue);
	image[idx] = (#type0)val;
}

__kernel void multextexmask(__global #type0* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, __global #type0* mask, __global #type0* value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}
	float weight = (float)(mask[idx] / maxValue);
	float imgVal = (float)(image[idx] / maxValue);
	float otherVal = (float)(value[idx] / maxValue);
	float val = (float)(imgVal * (otherVal * weight) * maxValue);
	image[idx] = (#type0)val;
}
