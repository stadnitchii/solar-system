using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenGL;
using System;
using System.IO;

namespace SolarSystem
{
    class Earth : Planet
    {
        private Texture specularT;
        private Texture nightT;
        private Texture normalT;
        private Texture cloudsT;

        public ShaderProgram cloudShader;
        public bool drawClouds = false;

        public Earth(string dataBaseName, PlanetParameters param, Planet parent, VAO vbos, Texture diffuse, Texture specular, Texture night, Texture normal, Texture clouds)
            : base(dataBaseName, param, parent, vbos, diffuse)
        {
            this.specularT = specular;
            this.nightT = night;
            this.normalT = normal;
            this.cloudsT = clouds;

            //cloudShader = new ShaderProgram(
            //    @"E:\Visual Studio 2015\Projects\Solar_System\Solar_System\content\shaders\default.vert",
            //    @"E:\Visual Studio 2015\Projects\Solar_System\Solar_System\content\shaders\cloudShader.frag", 
            //    "cloudShader");
        }

        public override void Draw(Camera cam, ShaderProgram shader, ShaderProgram lineShader)
        {
            Texture?.Bind(0);
            specularT?.Bind(1);
            nightT?.Bind(2);
            shader.Bind();
            shader.SetUniform("model", Transform);
            Vao.DrawElements(PrimitiveType.Quads);

            //if (drawClouds)
            //{
            //    scaleBy(1.005f);
            //    cloudsT?.Bind();
            //    cloudShader.Bind();
            //    cloudShader.SetUniform("model", Transform);
            //    cloudShader.SetUniform("view", cam.Transfrom);
            //    cloudShader.SetUniform("proj", cam.Projection);
            //    cloudShader.SetUniform("lightPos", cam.LightPos);
            //    Vao.DrawElements(PrimitiveType.Quads);
            //    scaleBy(1 / 1.005f);
            //}

            //draw axis line
            if (DrawAxisTilt)
            {
                lineShader.Bind();
                lineShader.SetUniform("model", Transform);
                axisLine.DrawArrays(PrimitiveType.Lines);
            }

            //draw orbit
            if (Orbit != null && DrawOrbit)
                Orbit.Draw(lineShader);
        }

        public override void Update(double delta, double hoursPerSecond)
        {
            double deltaHours = hoursPerSecond * delta;
            double deltaDays = deltaHours / 24.0;

            if (PeriodAngle < Math.PI + longitudePerihelionRadians)
                this.eccentricAnomaly += deltaDays / OrbitalPeriod * Math.PI * 2;
            if (PeriodAngle > Math.PI + longitudePerihelionRadians)
                this.eccentricAnomaly -= deltaDays / OrbitalPeriod * Math.PI * 2;        

            if (OrbitalPeriod != 0)
                PeriodAngle += deltaDays / OrbitalPeriod * Math.PI * 2;
            if (HoursPerRotation != 0)
                RotationAngle += deltaHours / HoursPerRotation * Math.PI * 2;

            if (PeriodAngle > Math.PI * 2 + longitudePerihelionRadians)
                PeriodAngle -= (Math.PI * 2);
            if (RotationAngle > Math.PI * 2)
                RotationAngle -= (Math.PI * 2);

            ClearRoatation();
            RotateYBy((float)RotationAngle);
            RotateXBy(AxisTilt);

            //if (parent != null)
            //this.SetTranslation(new Vector3(Math.Cos(PeriodAngle) * ScenicDistance + parent.Translation.X, 0 + parent.Translation.Y, Math.Sin(PeriodAngle) * ScenicDistance + parent.Translation.Z));
            if (parent != null)
            {
                float x = (float)(ScenicDistance * Math.Cos(PeriodAngle) + parent.Translation.X);
                float y = 0;
                float z = (float)(ScenicDistance * Math.Sin(PeriodAngle) + parent.Translation.Z);
                this.SetTranslation(new Vector3(x, y, z));
                //this.setPosition(Math.Cos(PeriodAngle) * ScenicDistance + parent.Position.X, 0 + parent.Position.Y, Math.Sin(PeriodAngle) * ScenicDistance + parent.Position.Z);
            }
        }
    }
}
