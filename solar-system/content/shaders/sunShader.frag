#version 330

out vec4 outColor;
out vec4 bloomColor;

void main()
{
	outColor = vec4(1, .8f, .6f, 1);
	bloomColor = vec4(vec3(1, .8f, .6f) * 1, 1);
}