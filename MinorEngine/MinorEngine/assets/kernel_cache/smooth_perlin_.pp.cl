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

float GetSmoothNoise(__global float* image, int idx, int channel, int width, int height, int depth, int samplePeriod, float sampleFrequency)
{
	int index = idx / 4;
	int3 index3D = Get3DimensionalIndex(width, height, index);

	int sample_w0 = (index3D.x / samplePeriod) * samplePeriod;
	int sample_w1 = (int)fmod((float)(sample_w0 + samplePeriod), (float)width);
	float horizontalBlend = (index3D.x - sample_w0) * sampleFrequency;

	int sample_h0 = (index3D.y / samplePeriod) * samplePeriod;
	int sample_h1 = (int)fmod((float)(sample_h0 + samplePeriod), (float)height);
	float verticalBlend = (index3D.y - sample_h0) * sampleFrequency;

	int sample_d0 = (index3D.z / samplePeriod) * samplePeriod;
	int sample_d1 = (int)fmod((float)(sample_d1 + samplePeriod), (float)depth);
	float depthBlend = (index3D.z - sample_d0) * sampleFrequency;

	int w0h0d0 = GetFlattenedIndex(sample_w0, sample_h0, sample_d0, width, height) * 4 + channel;
	int w1h0d0 = GetFlattenedIndex(sample_w1, sample_h0, sample_d0, width, height) * 4 + channel;
	int w0h1d0 = GetFlattenedIndex(sample_w0, sample_h1, sample_d0, width, height) * 4 + channel;
	int w1h1d0 = GetFlattenedIndex(sample_w1, sample_h1, sample_d0, width, height) * 4 + channel;
	int w0h0d1 = GetFlattenedIndex(sample_w0, sample_h0, sample_d1, width, height) * 4 + channel;
	int w1h0d1 = GetFlattenedIndex(sample_w1, sample_h0, sample_d1, width, height) * 4 + channel;
	int w0h1d1 = GetFlattenedIndex(sample_w0, sample_h1, sample_d1, width, height) * 4 + channel;
	int w1h1d1 = GetFlattenedIndex(sample_w1, sample_h1, sample_d1, width, height) * 4 + channel;

	float top0 = Lerpf(image[w0h0d0], image[w1h0d0], horizontalBlend);
	float top1 = Lerpf(image[w0h0d1], image[w1h0d1], horizontalBlend);
	float bottom0 = Lerpf(image[w0h1d0], image[w1h1d0], horizontalBlend);
	float bottom1 = Lerpf(image[w0h1d1], image[w1h1d1], horizontalBlend);
	float top = Lerpf(top0, top1, depthBlend);
	float bottom = Lerpf(bottom0, bottom1, depthBlend);

	return Lerpf(top, bottom, verticalBlend);


}


__kernel void smooth(__global float* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, int octaves)
{


	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	int samplePeriod = 1 << octaves;
	float sampleFrequency = 1.0f / samplePeriod;

	image[idx] = (float)(GetSmoothNoise(image, idx, channel, dimensions.x, dimensions.y, dimensions.z, samplePeriod, sampleFrequency)*maxValue);
}

float GetPerlinNoise(__global float* image, int idx, int channel, float maxValue, int width, int height, int depth, float persistence, int octaves)
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
	return (float)clamp(result*maxValue,0.0f, maxValue);
}
__kernel void perlin(__global float* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, float persistence, int octaves)
{
	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, (float)channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}
	image[idx] = GetPerlinNoise(image, idx, channel, maxValue, dimensions.x, dimensions.y, dimensions.z, persistence, octaves)*maxValue;
	
}
