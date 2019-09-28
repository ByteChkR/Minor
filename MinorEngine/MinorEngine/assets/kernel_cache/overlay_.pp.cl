float Lerp(float a, float b, float weightB)
{
    return (float)((float)a * (1 - weightB) + (float)b * weightB);
}
float Lerpf(float a, float b, float weightB)
{
    return a * (1 - weightB) + b * weightB;
}

int GetFlattenedIndex(int x, int y, int z, int width, int height)
{
    return (z * width * height) + (y * width) + x;
}

int3 Get3DimensionalIndex(int width, int height, int index)
{
    int d1, d2, d3;
    d3 = index / (width * height);
    int i = index - (d3 * width * height);
    d2 = i / width;
    d1 = (int)fmod((float)i, (float)width);
    return (int3)( d1, d2, d3 );
}

int2 Get2DIndex(int index, int width)
{
	int x = (int)fmod((float)index,(float)width);
	int y = index / width;
	int2 ret = (int2)(x, y);
	return ret;
}

uchar4 Checkerboard(int xIn, int yIn, float length)
{
	int xval = (int)fmod((float)(xIn / length), (float)2);
	int yval = (int)fmod((float)(yIn / length), (float)2);
	return xval == 0 ? (yval == 0 ? (uchar4)(255) : (uchar4)(0,0,0,255)) : (yval == 1 ? (uchar4)(255) : (uchar4)(0,0,0,255));
}



__kernel void overlay(__global float* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, __global float* overlay, float weightOverlay)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	image[idx]=Lerp(image[idx], overlay[idx], weightOverlay);
}
