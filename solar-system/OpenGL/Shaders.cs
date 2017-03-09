namespace OpenGL
{
    public class Shaders
    {
        private Shaders() { }

        public static ShaderProgram SkyboxShader()
        {
            return new ShaderProgram(SkyboxShaderVertex, SkyboxShaderFragment, "SkyboxShader", false);
        }

        public static ShaderProgram FrameShader()
        {
            return new ShaderProgram(FrameShaderVertex, FrameShaderFragment, "FrameShader", false);
        }

        public static ShaderProgram BlurShader()
        {
            return new ShaderProgram(BlurShaderVertex, BlurShaderFragment, "BlurShader", false);
        }

        #region ShaderCode
        #region Skybox
        private const string SkyboxShaderVertex = @"
#version 330

layout (location = 0) in vec3 vertIn;

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
}";

        private const string SkyboxShaderFragment = @"
#version 330

in vec3 uv;

out vec4 outColor;
out vec4 outColor2;

uniform samplerCube skybox;

void main()
{
    outColor = texture(skybox, uv);
    outColor2 = vec4(0,0,0,1);
}";
        #endregion

        #region Frame
        private const string FrameShaderVertex = @"
#version 330

layout(location=0) in vec2 vertIn;
layout(location=1) in vec2 uvIn;

out vec2 uv;

void main()
{
    gl_Position = vec4(vertIn, 0, 1);
    uv = uvIn;
}
";

        private const string FrameShaderFragment = @"
#version 330

in vec2 uv;

uniform sampler2D text;

out vec4 outColor;

void main()
{
    outColor = texture2D(text, uv);
}
";
        #endregion

        #region Blur
        private const string BlurShaderVertex = @"
#version 330

in vec3 vertIn;
in vec2 uvIn;

out vec2 uv;

void main()
{
    gl_Position = vec4(vertIn,1);
    uv = uvIn;
}
            ";

        private const string BlurShaderFragment = @"
#version 330

uniform sampler2D txt;
uniform bool horizontal;

in vec2 uv;

out vec4 outColor;

//gausian kernel sigma = 2
const float[] w = float[] ( 0.198596, 0.175713, 0.121703, 0.065984, 0.028002, 0.0093 );

float weight(int index)
{
    return w[index] * 1.02f;
}

void main()
{
    vec2 offset = 1.0 / textureSize(txt, 0);
    vec3 result = texture(txt, uv).rgb * weight(0);
    if(horizontal)
    {
        for(int i = 1; i < 6; i++)
        {
            result += texture(txt, uv + vec2(offset.x * i, 0.0)).rgb * weight(i);
            result += texture(txt, uv - vec2(offset.x * i, 0.0)).rgb * weight(i);
        }
    }
    else
    {
        for(int i = 1; i < 6; i++)
        {
            result += texture(txt, uv + vec2(0.0, offset.y * i)).rgb * weight(i);
            result += texture(txt, uv - vec2(0.0, offset.y * i)).rgb * weight(i);
        }
    }

    outColor = vec4(result,1);
}";
        #endregion
        #endregion
    }
}
