#include smooth.cl #type0 #type1
#include ../utils.cl #type0 #type1
#include ../f_convert.cl #type0 #type1
#include ../inverse.cl #type0 #type1

#type1 GetPerlinNoise(__global #type0* image, int idx, int channel, float maxValue, int width, int height, int depth, float persistence, int octaves)
{

	float amplitude = 1;
	float totalAmplitude = 0;
	float result = Tofloat(image[idx])/(float)(maxValue);

	for(int i = octaves-1; i >= 0; i--)
	{
		int samplePeriod = 1 << (i + 1);
		float sampleFrequency = 1.0f / samplePeriod;
		result += GetSmoothNoise(image, idx, channel, width, height, depth, maxValue, samplePeriod, sampleFrequency) * amplitude;
		totalAmplitude += amplitude;
		amplitude *= (float)persistence;
	}

	result /= totalAmplitude;
	return result;
}



__kernel void perlin(__global #type0* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float persistence, int octaves)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}
	float val = GetPerlinNoise(image, idx, channel, maxValue, dimensions.x, dimensions.y, dimensions.z, persistence, octaves);
	image[idx] = FromFloatN((#type1)val, maxValue);
	
}