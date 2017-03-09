#version 330

in vec3 vert;
in vec2 uv;
in vec3 normal;

uniform vec3 lightPos;

uniform sampler2D texture;

out vec4 outColor;
out vec4 outColor2;

vec3 gamma(vec3 color)
{
	return vec3(pow(color, vec3(1 / 2.2)));
}


void main()
{
	vec3 normall = normalize(normal);
	vec3 texcolor = texture2D(texture, uv).xyz;
	
	float brightness =clamp(dot(normalize(lightPos - vert), normall), 0, 1);
	float alpha = pow(texcolor.x, 1/4f);
	//outColor = vec4(vec3(1,1,1) * brightness, 1);
	outColor = vec4(texcolor * brightness, alpha);
	outColor2 = vec4(0,0,0,0);
}