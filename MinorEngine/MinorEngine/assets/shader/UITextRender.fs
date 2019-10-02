#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D sourceTexture;
uniform vec3 textColor;


void main()
{
	//vec2(TexCoords.x, 1-TexCoords.y) < When changing how font loading works (flipping y)
	float f = texture(sourceTexture, TexCoords).r;
	vec4 sampled = vec4(f,f,f,f);
    FragColor = sampled * vec4(textColor, 1.0f);
} 

