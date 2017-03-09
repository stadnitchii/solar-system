#version 330

uniform sampler2D frame;
uniform sampler2D bloom;
uniform bool useBloom;

in vec2 uv;

out vec4 outColor;

void main()
{
    vec3 frameColor = texture(frame, uv).rgb;
    vec3 bloomColor = texture(bloom, uv).rgb;
    vec3 finalColor = frameColor;
    if(useBloom) finalColor += bloomColor;
    outColor = vec4( finalColor, 1);
}