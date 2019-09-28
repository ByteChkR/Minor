#include utils.cl #type0 #type1

uchar4 Checkerboard(int xIn, int yIn, float length)
{
	int xval = (int)fmod((float)(xIn / length), (float)2);
	int yval = (int)fmod((float)(yIn / length), (float)2);
	return xval == 0 ? (yval == 0 ? (uchar4)(255) : (uchar4)(0,0,0,255)) : (yval == 1 ? (uchar4)(255) : (uchar4)(0,0,0,255));
}