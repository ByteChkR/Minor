#include utils.cl

uchar4 Checkerboard(int xIn, int yIn, float length)
{
	int xval = (int)fmod((float)(xIn / length), (float)2);
	int yval = (int)fmod((float)(yIn / length), (float)2);
	return xval == 0 ? (yval == 0 ? (uchar4)(255) : (uchar4)(0,0,0,255)) : (yval == 1 ? (uchar4)(255) : (uchar4)(0,0,0,255));
}



__kernel void overlay(__global uchar* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, __global uchar* overlay, float weightOverlay)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	image[idx]=Mix(image[idx], overlay[idx], weightOverlay);
}


__kernel void checkerboard(__global uchar4* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float length)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	int2 idx2d = Get2DIndex(idx, dimensions.x);
	image[idx]= Checkerboard(idx2d.x, idx2d.y, length);
}