#version 330

in vec3 vertIn;
in vec2 uvIn;

uniform mat4 proj;
uniform mat4 model;
uniform mat4 view;

uniform float scale;
uniform vec3 center;
uniform vec3 up;

out vec2 uv;

void main()
{	
	mat4 modelview = view * model;

	vec4 c = modelview * vec4(center, 1.0);
    vec4 u = modelview * vec4(up, 0.0); // w=0 as this is a direction, not a position
    vec3 z = -c.xyz;
    vec3 x = normalize(cross(u.xyz, z));
    vec3 y = normalize(cross(z, x));
    mat2x3 m = mat2x3(x, y);
    vec4 p = c + vec4(m * (vertIn.xy) * scale, 0.0);
    gl_Position = proj * p;

	uv = uvIn;
}