#version 330

in vec3 vertIn;
in vec2 uvIn;

out vec2 uv;

void main()
{
    gl_Position = vec4(vertIn, 1);
    uv = uvIn;
}