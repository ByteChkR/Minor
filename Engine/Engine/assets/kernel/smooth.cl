#include utils.cl

uchar GetSmoothNoise(__global uchar* image, int idx, int channel, int width, int height, int depth, int samplePeriod, float sampleFrequency)
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

	float top0 = Lerp(image[w0h0d0], image[w1h0d0], horizontalBlend);
	float top1 = Lerp(image[w0h0d1], image[w1h0d1], horizontalBlend);
	float bottom0 = Lerp(image[w0h1d0], image[w1h1d0], horizontalBlend);
	float bottom1 = Lerp(image[w0h1d1], image[w1h1d1], horizontalBlend);
	float top = Lerp(top0, top1, depthBlend);
	float bottom = Lerp(bottom0, bottom1, depthBlend);

	return (uchar)(Lerp(top, bottom, verticalBlend));


}


__kernel void smooth(__global uchar* image, int3 dimensions, int channelCount, float maxValue, __global uchar* channelEnableState, int octaves)
{


	int idx = get_global_id(0);
	int channel = (int)fmod((float)idx, channelCount);
	if(channelEnableState[channel]==0)
	{
		return;
	}

	int samplePeriod = 1 << octaves;
	float sampleFrequency = 1.0f / samplePeriod;

	image[idx] = GetSmoothNoise(image, idx, channel, dimensions.x, dimensions.y, dimensions.z, samplePeriod, sampleFrequency);
}