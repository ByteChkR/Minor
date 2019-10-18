#include utils.cl
__kernel void denoise(__global uchar* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float exponent, float strength, int sampleRange, float thresh)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}


	int3 id3d = Get3DimensionalIndex(dimensions.x, dimensions.y, idx/4);
	float centerSample = image[idx]/maxValue;
	float total;
	float newCol;
	for(int x = -sampleRange; x <= sampleRange; x+=1)
	{
		for(int y = -sampleRange; y <= sampleRange; y+=1)
		{
			for(int z = -sampleRange; z <= sampleRange; z+=1)
			{
				int dx = clamp(id3d.x + x, 0, dimensions.x);
				int dy = clamp(id3d.y + y, 0, dimensions.y);
				int dz = clamp(id3d.z + z, 0, dimensions.z);
				int3 new3id = (int3)(dx, dy, dz);
				float sample = image[GetFlattenedIndex(new3id.x, new3id.y, new3id.z, dimensions.x, dimensions.y)] / maxValue;
				float dotres = dot((float3)(sample) - (float3)(centerSample), (float3)(thresh));
				float weight = 1.0f - ((dotres > 0) ? dotres : -dotres);
				newCol += sample * weight;
				total += weight;
			}
		}
	}

	image[idx] = newCol/total*maxValue;
	

}