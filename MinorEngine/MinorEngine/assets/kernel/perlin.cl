#include smooth.cl #type0 #type1
#include utils.cl #type0 #type1
#include f_convert.cl #type0 #type1

#type0 GetPerlinNoise(__global #type0* image, int idx, int channel, float maxValue, int width, int height, int depth, float persistence, int octaves)
{

	#type1 amplitude = (#type1)(1);
	#type1 totalAmplitude = (#type1)(0);
	#type1 result = To#type1(image[idx])/(#type1)(maxValue);

	for(int i = octaves-1; i >= 0; i--)
	{
		int samplePeriod = 1 << (i + 1);
		float sampleFrequency = 1.0f / samplePeriod;
		result += GetSmoothNoise(image, idx, channel, width, height, depth, samplePeriod, sampleFrequency) * amplitude;
		totalAmplitude += amplitude;
		amplitude *= persistence;
	}

	result /= totalAmplitude;
	return From#type1(clamp(result*(#type1)maxValue, (#type1)0.0f, (#type1)maxValue));
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