#version 330

in vec3 vert;
in vec2 uv;
in vec3 normal;

uniform sampler2D texture;
uniform sampler2D specularTexture;
uniform sampler2D nightTexture;
uniform sampler2D normalTexture;
uniform sampler2D cloudsTexture;

uniform vec3 lightPos;
uniform vec3 lightIntensities;
uniform vec3 eyePos;

layout (location = 0) out vec4 outColor;
layout (location = 1) out vec4 outColor2;

vec3 normalN;
vec3 lightToVert;
vec3 eyeToVert;

float specularIntensity;

float diffuse()
{
	return clamp(dot(lightToVert, normalN), 0, 1);
}

vec3 gamma(vec3 color)
{
	return vec3(pow(color, vec3(1 / 2.2)));
}

float gamma(float color)
{
	return pow(color, 1 / 2.2);
}

float specular()
{
	//vec3 halfNormal = normalize(eyeVector + lightVector);
	vec3 reflectedLight = reflect(-lightToVert, normalN);
	return pow(max(dot(reflectedLight, eyeToVert), 0), 20) * specularIntensity;
}

float rimLight()
{
	return pow(1 - dot(eyeToVert, normalN), 2) * .5;
}

void main()
{
	vec3 diffuseColor = vec3(texture2D(texture, uv));
	vec3 specularColor = vec3(texture2D(specularTexture, uv));
	vec3 nightColor = vec3(texture2D(nightTexture, uv));
	vec3 normalColor = vec3(texture2D(normalTexture, uv));
	vec3 cloudsColor = vec3(texture2D(cloudsTexture, uv));
	specularIntensity = specularColor.r;

	nightColor *= vec3(.2,.5,1);

	normalN = normalize(normal);	
	lightToVert = normalize(lightPos - vert);
	eyeToVert = normalize(eyePos - vert);

	float diffuseC = diffuse();
	nightColor = nightColor * pow(1 - diffuseC, 20);
	if(nightColor.r  > .15) nightColor += vec3(1);
	vec3 finalColor = diffuseColor * diffuseC + nightColor; // diffuse
	finalColor += vec3(1,.9,.8) * specular(); //specular
	//finalColor += nightColor * pow(1 - diffuseC, 10);

	finalColor = gamma(finalColor);

	outColor = vec4(finalColor, 1);
	outColor2 = vec4(vec3( .5, .5, 1) * rimLight() * (diffuseC + .5) ,1);
	if(nightColor.r > .7) outColor2 += vec4(1);
}