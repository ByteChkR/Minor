#version 330 core
layout (location = 0) in vec2 aPos;
layout (location = 1) in vec2 aTexCoords;
uniform vec2	tiling;
uniform vec2	offset;


uniform mat4 transform;

out vec2 TexCoords;

void main()
{
	vec2 _uv = aTexCoords * tiling;
	TexCoords = _uv + offset;
    gl_Position = transform * vec4(aPos.x, aPos.y, 0.0, 1.0); 
}  

