#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform float divWeight;
uniform sampler2D destinationTexture;
uniform sampler2D otherTexture;


void main()
{
    vec3 col = texture(otherTexture, TexCoords).rgb;
    col *= divWeight;
    FragColor = vec4(col + texture(destinationTexture, TexCoords).rgb, 1.0);
} 

