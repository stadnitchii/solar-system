#version 330

in vec3 uv;

out vec4 outColor;

uniform samplerCube skybox;

void main()
{
	outColor = texture(skybox, uv);
}