#include f_convert.cl #type0 #type1
__kernel void mulval(__global #type0* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float value)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}
	#type0 imgVal = (#type0)(image[idx]);
	float otherVal = value;
	#type0 val = (#type0)(imgVal * (#type0)(1-otherVal) + (#type0)(otherVal) * (#type0)maxValue);
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
	#type0 imgVal = image[idx];
	#type0 otherVal = value[idx];
	#type0 val = (#type0)(imgVal * (#type0)(1 - weight) + (otherVal * (#type0)(weight)));
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
	#type1 weight = To#type1(mask[idx]) / (#type1)(maxValue);
	#type1 imgVal = To#type1(image[idx]);
	#type1 otherVal = To#type1(value[idx]);
	#type0 val = From#type1(imgVal * ((#type1)(maxValue) - weight)) + From#type1(otherVal * weight);
	image[idx] = (#type0)val;
}
