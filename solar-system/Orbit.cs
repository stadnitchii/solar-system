using OpenTK;
using OpenTK.Graphics.OpenGL;
using SolarSystem;
using OpenGL;
using System;
using System.Collections.Generic;

namespace SolarSystem
{
    public class Orbit
    {
        private VAO vao;
        private Planet parent;

        public Vector3 Positon { get; private set; }
        public float Radius { get; private set; }
        public int Sides { get; private set; }
        public Matrix4 Transform { get; private set; }

        public Orbit(Planet parent, Vector3 center, double radius, int sides)
        {
            this.parent = parent;
            this.Positon = center;
            this.Radius = (float)radius;
            this.Sides = sides;
            this.Transform = Matrix4.CreateTranslation(center);

            List<Vector3> verts = new List<Vector3>();

            float step = (float)(2 * Math.PI / sides);
            for (float alpha = 0; alpha < 2 * Math.PI; alpha += step)
            {
                float x = (float)(radius * Math.Cos(alpha));
                float y = 0;
                float z = (float)(radius * Math.Sin(alpha));
                verts.Add(new Vector3(x, y, z));

                alpha += step;
            }

            vao = new VAO(verts.ToArray());
        }

        public void Draw(ShaderProgram lineshader)
        {
            lineshader.Bind();
            lineshader.SetUniform("model", Transform);
           
            vao.DrawArrays(PrimitiveType.LineLoop);
        }

        public void updatePosition(Vector3 pos)
        {
            this.Transform = Matrix4.CreateTranslation(pos);
        }
    }
}
