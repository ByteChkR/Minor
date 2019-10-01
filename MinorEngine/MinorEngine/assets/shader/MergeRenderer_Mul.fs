#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform float divWeight;
uniform sampler2D destinationTexture;
uniform sampler2D otherTexture;


void main()
{
    vec4 dcol = texture(destinationTexture, TexCoords);
    vec4 ocol = texture(otherTexture, TexCoords);
    //col *= divWeight;

    vec4 col = vec4(dcol.rgb * ocol.rgb * vec3(ocol.a), 1);

    FragColor = col;
    
} 

