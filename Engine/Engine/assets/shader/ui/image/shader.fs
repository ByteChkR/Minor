#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D uiTexture;
uniform float alpha;


void main()
{
	vec4 ret = texture(uiTexture, TexCoords);
	ret.a *= alpha;
    FragColor = ret;
} 

