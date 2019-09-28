#include f_convert.cl #type0 #type1


#type1 Lerpf(#type1 a, #type1 b, #type1 weightB)
{
    return a * ((#type1)1 - weightB) + b * weightB;
}

#type1 ToFloatN(#type0 value, float maxValue)
{
    #type1 maxV = To#type1((#type0)maxValue);
    #type1 val = To#type1(val);
    return val / maxV;
}

#type0 FromFloatN(#type1 value, float maxValue)
{
    #type0 val = From#type1(val * (#type1)maxValue);
    return val;
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