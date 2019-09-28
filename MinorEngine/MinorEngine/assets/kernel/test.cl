#include utils.cl #type0 #type1

uchar4 Checkerboard(int xIn, int yIn, float length)
{
	int xval = (int)fmod((float)(xIn / length), (float)2);
	int yval = (int)fmod((float)(yIn / length), (float)2);
	return xval == 0 ? (yval == 0 ? (uchar4)(255) : (uchar4)(0,0,0,255)) : (yval == 1 ? (uchar4)(255) : (uchar4)(0,0,0,255));
}



__kernel void overlay(__global #type0* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, __global #type0* overlay, float weightOverlay)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	image[idx]=From#type1(Lerpf(To#type1(image[idx]), To#type1(overlay[idx]), (#type1)(weightOverlay)));
}