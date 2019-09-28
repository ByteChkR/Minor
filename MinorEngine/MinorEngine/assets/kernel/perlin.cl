#include smooth.cl #type0
#include utils.cl #type0

float GetPerlinNoise(__global #type0* image, int idx, int channel, float maxValue, int width, int height, int depth, float persistence, int octaves)
{

	float amplitude = 1;
	float totalAmplitude = 0;
	float result = image[idx]/maxValue;

	for(int i = octaves-1; i >= 0; i--)
	{
		int samplePeriod = 1 << (i + 1);
		float sampleFrequency = 1.0f / samplePeriod;
		result += GetSmoothNoise(image, idx, channel, width, height, depth, samplePeriod, sampleFrequency) * amplitude;
		totalAmplitude += amplitude;
		amplitude *= persistence;
	}

	result /= totalAmplitude;
	return (#type0)clamp(result*maxValue,0.0f, maxValue);
}
__kernel void perlin(__global #type0* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float persistence, int octaves)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}
	image[idx] = GetPerlinNoise(image, idx, channel, maxValue, dimensions.x, dimensions.y, dimensions.z, persistence, octaves)*maxValue;
	
}