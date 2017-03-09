#version 330

layout(location = 0) in vec3 vertIn;
layout(location = 1) in vec2 uvIn;
layout(location = 2) in vec3 normalIn;

uniform mat4 proj;
uniform mat4 view;
uniform mat4 model;

void main()
{
	gl_Position = proj * view * model * vec4(vertIn, 1);
	//vert = vec3(model * vec4(vertIn , 1));
	//uv = uvIn;
	//normal = vec3(model * vec4(normalIn, 0));
}