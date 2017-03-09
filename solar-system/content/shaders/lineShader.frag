#version 330

out vec4 outColor;
out vec4 outColor2;

void main()
{
	vec3 color = vec3(1,1,1) * .5;

	outColor = vec4(color,1);

	outColor2 = vec4(1,1,1,1);
}