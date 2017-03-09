using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace OpenGL
{
    public class SkyBox
    {
        private ShaderProgram Shader;
        public TextureCube Texture { get; private set; }

        private VAO Vao;

        public SkyBox(string directory, string ext = ".png")
        {
            Texture = new TextureCube(directory, ext);

            Vector3[] positions = new Vector3[]
            {
                new Vector3(1.0f, -1.0f, -1.0f),
                new Vector3(1.0f, -1.0f, 1.0f),
                new Vector3(-1.0f, -1.0f, 1.0f),
                new Vector3(-1.0f, -1.0f, -1.0f),
                new Vector3(1.0f, 1.0f, -1.0f),
                new Vector3(1.0f, 1.0f, 1.0f),
                new Vector3(-1.0f, 1.0f, 1.0f),
                new Vector3(-1.0f, 1.0f, -1.0f)
            };

            uint[] elements = new uint[]
            {
                0, 1, 2, 3,
                4, 7, 6, 5,
                0, 4, 5, 1,
                1, 5, 6, 2,
                2, 6, 7, 3,
                4, 0, 3, 7
            };

            Vao = new VAO(positions);
            Vao.addElementArray(elements);

            Shader = Shaders.SkyboxShader();
        }

        public void draw(Camera cam)
        {
            GL.DepthFunc(DepthFunction.Lequal);
            GL.CullFace(CullFaceMode.Front);

            GL.BindTexture(TextureTarget.TextureCubeMap, Texture.Id);

            Shader.Bind();
            Shader.SetUniform("proj", cam.Projection);
            Shader.SetUniform("view", cam.Transfrom);

            Vao.DrawElements(PrimitiveType.Quads);

            GL.DepthFunc(DepthFunction.Less);
            GL.CullFace(CullFaceMode.Back);
        }
    }
}
