#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform float divWeight;
uniform sampler2D destinationTexture;
uniform sampler2D otherTexture;


void main()
{
    vec4 col = texture(otherTexture, TexCoords);
    //col *= divWeight;
    if(col == vec4(0,0,0,1))
    {
    	FragColor = texture(destinationTexture, TexCoords);
    }
    else
    {
    	FragColor = col;
    }
} 

