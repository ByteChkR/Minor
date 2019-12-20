#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D sourceTexture;
uniform vec3 textColor;


void main()
{
	//vec2(TexCoords.x, 1-TexCoords.y) < When changing how font loading works (flipping y)

#if WIN

	float x = TexCoords.x;
	float y = 1-TexCoords.y;

#elseif OSX || LINUX

	float x = TexCoords.x;
	float y = TexCoords.y;

#endif

	vec2 tcoords = vec2(x,y);
	float f = texture(sourceTexture, tcoords).r;
	vec4 sampled = vec4(f,f,f,f);
    FragColor = sampled * vec4(textColor, 1.0f);
} 

