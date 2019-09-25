#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D sourceTexture;


void main()
{
    vec3 col = texture(sourceTexture, TexCoords).rgb;
    FragColor = vec4(col, 1.0);
} 

