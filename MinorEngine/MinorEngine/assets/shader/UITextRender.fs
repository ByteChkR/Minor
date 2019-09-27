#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D sourceTexture;
uniform vec3 textColor;


void main()
{
	vec4 sampled = vec4(texture(sourceTexture, TexCoords).rgb, 1.0f);
    FragColor = sampled * vec4(textColor, 1.0);
} 

