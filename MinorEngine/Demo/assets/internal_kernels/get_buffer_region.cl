
int3 Get3DimensionalIndex(int width, int height, int index)
{
    int d1, d2, d3;
    d3 = index / (width * height);
    int i = index - (d3 * width * height);
    d2 = i / width;
    d1 = (int)fmod((float)i, (float)width);
    return (int3)( d1, d2, d3 );
}

int GetFlattenedIndex(int x, int y, int z, int width, int height)
{
    return (z * width * height) + (y * width) + x;
}


__kernel void getregion(__global uchar* image, int3 dimensions, int channelCount, __global uchar* destination, int3 start, int3 bounds)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	int index = idx/channelCount;

	int3 index3d = Get3DimensionalIndex(dimensions.x, dimensions.y, index);

	int3 endPos = start + bounds;

	if(index3d.x >= start.x && index3d.x < endPos.x &&
		index3d.y >= start.y && index3d.y < endPos.y &&
		index3d.z >= start.z && index3d.z < endPos.z)
	{
		int3 idx3dDst = idx-start;
		int idxDst = GetFlattenedIndex(idx3dDst.x, idx3dDst.y, idx3dDst.z, bounds.x, bounds.y);
		destination[idxDst] = image[idx];
	}

}