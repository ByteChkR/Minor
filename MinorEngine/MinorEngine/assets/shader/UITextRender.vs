#version 330 core
layout (location = 0) in vec4 vertexinfo;


uniform mat4 transform;

out vec2 TexCoords;

void main()
{
    TexCoords = vertexinfo.zw;
    gl_Position = transform * vec4(vertexinfo.x, vertexinfo.y, 0.0, 1.0); 
}  

