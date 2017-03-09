#version 330

in vec3 vertIn;

out vec3 uv;

uniform mat4 view;
uniform mat4 proj;

void main()
{
	mat4 view2 = view;
	view2[3][0] = 0;
	view2[3][1] = 0;
	view2[3][2] = 0;
	vec4 pos = proj * view2 * vec4(vertIn, 1);
    gl_Position = pos.xyww;
	uv = vertIn;
}
