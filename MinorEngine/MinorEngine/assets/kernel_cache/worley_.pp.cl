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

float GetWorleyDistance(float3 point, float3 worleypoint, float pmax)
{
	float value=pmax;

	for(float z = -1.0; z < 1.1; z+=1.0)
	{
		for(float y = -1.0; y < 1.1; y+=1.0)
		{
			for(float x = -1.0; x < 1.1; x+=1.0)
			{
				float3 delta = worleypoint-point+(float3)(x, y, z);
				float dist = length(delta);
				if(dist < value){
					value=dist;
				}
			}
		}
	}
	return value;
}

float GetWorleyValue(float3 pos, float3 worleypoint, float max_distance)
{
	float value = max_distance;

	value = GetWorleyDistance(pos, worleypoint, value);

	return clamp((float)(value/max_distance), 0.0f, 1.0f);
}


__kernel void worley(__global uchar *image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, __global float* positions, int poscount,  float max_distance)
{
	/* code */
	int idx = get_global_id(0);
	int pixelIndex = idx / 4;
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}


	int3 idx3d = Get3DimensionalIndex(dimensions.x, dimensions.y, pixelIndex);
	float3 idx3dNorm = (float3)(idx3d.x/(float)dimensions.y, idx3d.y/(float)dimensions.x, idx3d.z/(float)dimensions.z);
	float3 fpos = fmod(idx3dNorm, 1.0f);


	float value = max_distance;



	for(int i = 0; i < poscount; i++)
	{

		float3 position = (float3)(positions[i], positions[i+1], positions[i+2]);
		value = GetWorleyDistance(fpos, position, value);
	
	}

	float result = clamp(value / max_distance, 0.0f, 1.0f);
	

	image[idx] = (uchar)(clamp(result * maxValue, 0.0f, maxValue));

}
