using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenGL;
using System;

namespace SolarSystem
{
    public partial class Planet : Model
    {
        #region Planet Properties
        //angle(radians) of axial tilt
        public float AxisTilt { get; protected set; }

        //distance from center to center of parent
        public double DistanceFromParent { get; protected set; }

        //size of the planet
        public double PlanetRadius { get; protected set; }

        //time it takes to complete a rotation
        public double RotationalPeriod { get; protected set; }

        //time it takes to complete an orbit
        public double OrbitalPeriod { get; protected set; }


        //-1 = clockwise
        public int OrbitalOrientation { get; protected set; }

        //current angle in the rotation
        public double RotationalPheta { get; protected set; }

        //current angle in the orbit
        public double OrbitalPheta { get; protected set; }

        #endregion

        //Graphical representation of the orbit
        public Orbit Orbit { get; protected set; }

        public bool DrawOrbit { get; set; }

        public bool DrawAxisTilt { get; set; }

        protected VAO axisLine;

        //parent wich it roatates around
        protected Planet parent;

        public Planet(string databaseName, PlanetParameters parameters, Planet parent, VAO vao, Texture t)
            : base(vao, t)
        {
            this.parent = parent;
            this.OrbitalOrientation = -1;
            this.DrawAxisTilt = true;
            this.DrawOrbit = true;

            this.AxisTilt = (float)MathHelper.DegreesToRadians(parameters.AxialTilt[databaseName]);
            this.PlanetRadius = parameters.PlanetRadius[databaseName];
            this.DistanceFromParent = parameters.DistanceFromSun[databaseName];

            this.RotationalPeriod = parameters.RotationPeriod[databaseName];
            this.OrbitalPeriod = parameters.OrbitalPeriod[databaseName];
            
            this.setScale((float)PlanetRadius);

            if (parent != null)
            {
                this.Orbit = new Orbit(this, parent.Translation, this.DistanceFromParent, 120);
                DrawOrbit = true;
            }

            Vector3[] axisVerts = { new Vector3(0, -2, 0), new Vector3( 0, 2, 0) };

            axisLine = new VAO(axisVerts);
        }

        public virtual void Draw(Camera cam, ShaderProgram shader, ShaderProgram lineShader)
        {
            shader.Bind();
            shader.SetUniform("lighting", true);

            base.Draw(cam, shader);

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

        public virtual void Update(double delta, double hoursPerSecond)
        {
            double deltaHours = hoursPerSecond * delta;
            double deltaDays = deltaHours / 24.0;

            if (OrbitalPeriod != 0)
                OrbitalPheta += deltaDays / OrbitalPeriod * Math.PI * 2;
            if (RotationalPeriod != 0)
                RotationalPheta += deltaHours / RotationalPeriod * Math.PI * 2;

            //limit angle to (0 - 360) 0 - 2PI
            if (OrbitalPheta > Math.PI * 2)
                OrbitalPheta -= (Math.PI * 2);
            if (RotationalPheta > Math.PI * 2)
                RotationalPheta -= (Math.PI * 2);

            ClearRoatation();
            RotateYBy((float)RotationalPheta);
            RotateXBy(AxisTilt);
          
            if (parent != null)
            {
                float x = (float)(DistanceFromParent * Math.Cos(OrbitalPheta) + parent.Translation.X);
                float y = 0;
                float z = (float)(DistanceFromParent * Math.Sin(OrbitalPheta) + parent.Translation.Z);
                this.SetTranslation(new Vector3(x, y, z));
            }

            if (parent is Earth)
                Orbit.updatePosition(parent.Translation);
        }
    }
}