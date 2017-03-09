using OpenTK;
using OpenTK.Graphics.OpenGL;
using SolarSystem;
using OpenGL;
using System;
using System.IO;

namespace SolarSystem
{
    class Sun : Planet
    {
        public Sun(string dataBaseName, PlanetParameters param, Planet parent, VAO vao, Texture t)
            : base(dataBaseName, param, parent, vao, t)
        {
            this.DrawAxisTilt = false;
            this.DrawOrbit = false;
        }

        public override void Draw(Camera cam, ShaderProgram shader)
        {
            base.Draw(cam, shader);
        }
    }
}
