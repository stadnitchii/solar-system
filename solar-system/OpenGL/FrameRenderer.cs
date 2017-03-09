using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace OpenGL
{
    public struct Area
    {
        public static Area Full = new Area(0, 0, 1, 1);
        public static Area TopLeft = new Area(0, 0, .5f, .5f);
        public static Area TopRight = new Area(.5f, 0, .5f, .5f);
        public static Area BottomLeft = new Area(0, .5f, .5f, .5f);
        public static Area BottomRight = new Area(.5f, .5f, .5f, .5f);

        public VAO Vao { get; private set; }

        public Area(float x, float y, float width, float height)
        {
            //conver 0 - 1 to (-1) - 1
            x = 2 * x - 1;
            y = -2 * y + 1;
            width *= 2f;
            height *= 2f;

            Vector2[] verts = new Vector2[]
            {
                    new Vector2(x, y),
                    new Vector2(x, y - height),
                    new Vector2(x + width, y),
                    new Vector2(x + width, y - height),
            };

            Vector2[] uvs = new Vector2[]
            {
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(1, 1),
                    new Vector2(1, 0)
            };

            Vao = new VAO(verts);
            Vao.addAttributeArray(uvs, 2, 1);
        }
    }

    public class FrameRenderer
    {
        private ShaderProgram shader;

        private static FrameRenderer instance;
        public static FrameRenderer Instance
        {
            get
            {
                if (instance == null)
                    instance = new FrameRenderer();
                return instance;
            }
        }

        private FrameRenderer()
        {
            shader = Shaders.FrameShader();
        }

        public void Draw(Area a, Texture texture)
        {
            texture.Bind();
            shader.Bind();
            a.Vao.DrawArrays(PrimitiveType.TriangleStrip);
        }

        public void Draw(Area area, Texture texture, ShaderProgram shader)
        {
            texture.Bind();
            shader.Bind();
            area.Vao.DrawArrays(PrimitiveType.TriangleStrip);
        }

        public void Draw(Area area, Texture texture1, Texture texture2, ShaderProgram shader)
        {
            texture1.Bind(0);
            texture2.Bind(1);
            shader.Bind();
            area.Vao.DrawArrays(PrimitiveType.TriangleStrip);
        }
    }
}
