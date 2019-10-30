#version 330 core
in vec3 aPos;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;


void main()
{
    gl_Position = modelMatrix * viewMatrix * projectionMatrix * vec4(aPos, 1.0);
} 