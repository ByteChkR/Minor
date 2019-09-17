__kernel void mulval(__global uchar* image, int3 dimensions, int channelCount, __global uchar* channelEnableState, float value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}
	float imgVal = (float)(image[idx]/255.0f);
	float otherVal = (float)(value);
	int val = (int)(imgVal * otherVal * 255.0f);
	image[idx] = val;
}
__kernel void multexvalmask(__global uchar* image, int3 dimensions, int channelCount, __global uchar* channelEnableState, float mask, __global uchar* value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}	
	float weight = (float)(mask);
	float imgVal = (float)(image[idx] / 255.0f);
	float otherVal = (float)(value[idx] / 255.0f);
	int val = (int)(imgVal * (otherVal * weight) * 255.0f);
	image[idx] = val;
}

__kernel void multextexmask(__global uchar* image, int3 dimensions, int channelCount, __global uchar* channelEnableState, __global uchar* mask, __global uchar* value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}
	float weight = (float)(mask[idx] / 255.0f);
	float imgVal = (float)(image[idx] / 255.0f);
	float otherVal = (float)(value[idx] / 255.0f);
	int val = (int)(imgVal * (otherVal * weight) * 255.0f);
	image[idx] = val;
}
