using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenGL;

namespace SolarSystem
{
    class PlanetRing
    {
        private VAO vao;
        private Texture texture;
        private Planet parent;
        private Matrix4 transform;

        public PlanetRing(VAO vbos, Texture t, Planet p)
        {
            this.vao = vbos;
            this.texture = t;
            this.parent = p;
            this.transform = parent.Transform;
        }

        public void draw(Matrix4 cam, ShaderProgram shader)
        {      
            GL.BindTexture(TextureTarget.Texture2D, this.texture.Id);

            shader.SetUniform("model", parent.Transform);
            shader.SetUniform("lighting", false);


            vao.DrawArrays(PrimitiveType.Quads);
        }
    }
}
