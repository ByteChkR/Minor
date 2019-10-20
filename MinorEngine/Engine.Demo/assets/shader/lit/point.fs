#version 330

struct LightSource
{
	int type;
	vec3 position;
	vec3 attenuation;
	vec3 color;
	float ambientContribution;
	float intensity;
};

uniform sampler2D texture_diffuse1;
uniform sampler2D texture_specular1;
uniform int lightCount;
uniform LightSource lights[8];
uniform float shininess;

in vec3 worldNormal;
in vec3 eye;
in vec2 texCoords;
in vec3 fragPos;
out vec4 sColor;
vec3 GetLightDirection(LightSource source, vec3 vdir){
	if(source.type == 0)
		return normalize(source.position); //Directional Light(i just use the gameobject position. deal with it.)
	else
		return normalize(source.position-vdir); //The light direction if its a point light
}



vec3 CalculateLight(LightSource source, vec3 normal, vec3 lightdir)
{
	//If the light direction and the normal are "in the same dir"
	//light dir is from eyeposition to light source.
	float diffuseIntensity = max(dot(normalize(normal), lightdir),0.0);


	//Falloff
	float distance = length(source.position);
	float falloff = source.attenuation.x+
					(distance*source.attenuation.y)+
					(distance*distance*source.attenuation.z);

	diffuseIntensity /= falloff;




	float spec = 0.0;
	if(true)
	{
		vec3 viewdir = normalize(eye - fragPos);
		//Specular(Blinn)
		vec3 halfDir = normalize(lightdir + viewdir);
		//the dotproduct of the reflected light dir and the light dir.
		//if the angle is big, there is little to no shininess.
		spec = pow(max(dot(normal, halfDir), 0.0), shininess*2);
	}
	else
	{
		vec3 reflectDirection = reflect(-lightdir, normal);
		//Phong
		spec = pow(max(dot(lightdir, reflectDirection), 0.0), shininess);
	}


	
	

	//Putting it together
	//Ambient  = ambient color * how much light * how much contribution(not sure)
	vec3 ambient = texture(texture_diffuse1, texCoords).rgb * source.ambientContribution;
	// Diffuse = (object color + light color * light intensity) * how much light
	vec3 diffuse = texture(texture_diffuse1, texCoords).rgb * diffuseIntensity;
	// specular = specular color * specularintensity * how much light
	vec3 specular = texture(texture_specular1, texCoords).rgb * spec * diffuseIntensity;

	diffuse *= (source.color * source.intensity / falloff);

	return ambient + diffuse + specular;
}

void main (void) {

	vec3 result = vec3(0);
	vec3 wn = normalize(worldNormal); //Normalizing the worldnormal

	for(int i = 0; i < lightCount; i++) //Go through each light
	{
		vec3 vdir = GetLightDirection(lights[i], eye); //Get the light direction depending on the type
		result += CalculateLight(lights[i], wn, vdir); //calculate the lightning
	}
	//sColor = vec4(1,0,0,1);
	sColor = vec4(result,1); //All the light sources together will be the final color.
}