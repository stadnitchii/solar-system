#version 330

in vec2 uv;

uniform sampler2D texture;

out vec4 finalColor;

void main()
{
	vec4 texColor = texture2D(texture, uv);
	finalColor = texColor;
}