#version 330

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
uniform vec2	tiling;
uniform vec2	offset;

layout (location = 0)in vec3 vertex;
layout (location = 1)in vec3 normal;
layout (location = 2)in vec2 uv;

//out float diffuseIntensity;
out vec3 eye;
out vec3 worldNormal;
out vec2 texCoords;
out vec3 fragPos;

void main (void) {
    gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(vertex,1);
    fragPos = vertex;
    //make sure normal is in same space as light direction. You should be able to explain the 0.
    vec2 _uv = uv * tiling;
	texCoords = _uv + offset;
    worldNormal = vec3 (modelMatrix * vec4 (normal, 0));
	eye = vec3((modelMatrix) * vec4(vertex,1));

    //take the dot of the direction from surface to light with the world surface normal

    //diffuseIntensity = max (0, dot(-normalize(directionalLightVector), normalize (worldNormal)));
}