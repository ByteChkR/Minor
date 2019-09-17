#include smooth.cl
#include utils.cl

uchar GetPerlinNoise(__global uchar* image, int idx, int channel, int width, int height, int depth, float persistence, int octaves)
{

	float amplitude = 1;
	float totalAmplitude = 0;
	float result = image[idx];

	for(int i = octaves-1; i >= 0; i--)
	{
		int samplePeriod = 1 << (i + 1);
		float sampleFrequency = 1.0f / samplePeriod;
		result += GetSmoothNoise(image, idx, channel, width, height, depth, samplePeriod, sampleFrequency) * amplitude;
		totalAmplitude += amplitude;
		amplitude *= persistence;
	}

	result /= totalAmplitude;

	return (uchar)clamp(result,0.0f, 255.0f);
}
__kernel void perlin(__global uchar* image, int3 dimensions, int channelCount, __global uchar* channelEnableState, float persistence, int octaves)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}
	image[idx] = GetPerlinNoise(image, idx, channel, dimensions.x, dimensions.y, dimensions.z, persistence, octaves);
	
}