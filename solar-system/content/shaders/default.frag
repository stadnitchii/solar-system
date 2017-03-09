#version 330

in vec3 vert;
in vec2 uv;
in vec3 normal;

uniform sampler2D texture;

uniform vec3 lightPos;
uniform vec3 lightIntensities;
uniform float ambientCoefficient;
uniform bool lighting;
uniform bool bloom;

layout(location = 0) out vec4 outColor;
layout(location = 1) out vec4 outColor2;

vec3 gamma(vec3 color)
{
	return vec3(pow(color, vec3(1 / 2.2)));
}

void main()
{
	vec3 normall = normalize(normal);
	vec4 texColor = texture2D(texture, uv);
	vec3 diffuseColor = texColor.xyz;
    
    vec3 ambient = ambientCoefficient * diffuseColor * lightIntensities;

    vec3 lightVector = normalize(lightPos - vert);
    float diffuseC = clamp(dot(lightVector, normall), 0, 1);

    vec3 diffuse = diffuseC * diffuseColor * lightIntensities;

	vec3 finalColor = gamma(ambient + diffuse);

	if(lighting)
		outColor = vec4(finalColor, texColor.w);
	else
		outColor = texColor;

	outColor2 = vec4(0,0,0,1);
}