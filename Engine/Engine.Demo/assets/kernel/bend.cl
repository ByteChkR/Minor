#include utils.cl
__kernel void bend(__global uchar* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, __global uchar* input, float offsetPow, float rads, float bendFactor)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}
	int3 i3d = Get3DimensionalIndex(dimensions.x, dimensions.y, idx/channelCount);

	float height = (float)dimensions.y - i3d.y;

	float offset = pow(height, offsetPow);
	offset*=(sin(rads)*bendFactor);
	int y =  (int)clamp((float)(i3d.y + offset), 0.0f, (float)dimensions.y);
	int newId = GetFlattenedIndex(i3d.x, y, i3d.z, dimensions.x, dimensions.y);
	image[idx] = input[newId];
}