uchar Lerp(uchar a, uchar b, float weightB)
{
    return (uchar)((float)a * (1 - weightB) + (float)b * weightB);
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

uchar Mix(uchar a, uchar b, float weightB)
{
	return a * (1 - weightB) + b * weightB;
}

int2 Get2DIndex(int index, int width)
{
	int x = (int)fmod((float)index,(float)width);
	int y = index / width;
	int2 ret = (int2)(x, y);
	return ret;
}