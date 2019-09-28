#include ..\utils.cl #type0 #type1

#type1 GetSmoothNoise(__global #type0* image, int idx, int channel, int width, int height, int depth, float maxValue, int samplePeriod, float sampleFrequency)
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

	#type1 _w0h0d0 = ToFloatN(image[w0h0d0], maxValue);
	#type1 _w1h0d0 = ToFloatN(image[w1h0d0], maxValue);
	#type1 _w0h0d1 = ToFloatN(image[w0h0d1], maxValue);
	#type1 _w1h0d1 = ToFloatN(image[w1h0d1], maxValue);

	#type1 _w0h1d0 = ToFloatN(image[w0h1d0], maxValue);
	#type1 _w1h1d0 = ToFloatN(image[w1h1d0], maxValue);
	#type1 _w0h1d1 = ToFloatN(image[w0h1d1], maxValue);
	#type1 _w1h1d1 = ToFloatN(image[w1h1d1], maxValue);

	#type1 top0 = Lerpf(_w0h0d0, _w1h0d0 , horizontalBlend);
	#type1 top1 = Lerpf(_w0h0d1, _w1h0d1, horizontalBlend);

	#type1 bottom0 = Lerpf(_w0h1d0, _w1h1d0, horizontalBlend);
	#type1 bottom1 = Lerpf(_w0h1d1, _w1h1d1, horizontalBlend);

	#type1 top = Lerpf(top0, top1, depthBlend);
	#type1 bottom = Lerpf(bottom0, bottom1, depthBlend);

	return Lerpf(top, bottom, verticalBlend);


}
