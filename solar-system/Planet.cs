using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenGL;
using SolarSystem;
using System;

namespace SolarSystem
{
    public partial class Planet : Model
    {
        #region Planet Properties
        //matrix representing the axis tilt
        public float AxisTilt { get; protected set; }

        //distance from center to center of parent
        public double ScenicDistance { get; protected set; }

        //distance from center to center of parent (realistic)
        public double RealisticDistance { get; protected set; }

        //angle of rotaton
        public double RotationAngle { get; protected set; }

        //self
        public double HoursPerRotation { get; protected set; }

        //angle of rotation from parent
        public double PeriodAngle { get; protected set; }

        //self
        public int RevolutionOrientation { get; protected set; }

        //self
        public double OrbitalPeriod { get; protected set; }

        //realistic size
        public double RealisticRadius { get; protected set; }

        //size for scenic view
        public double ScenicRadius { get; protected set; }
        #endregion

        //Graphical representation of the orbit
        public Orbit Orbit { get; protected set; }

        public bool DrawOrbit { get; set; }

        public bool DrawAxisTilt { get; set; }

        protected VAO axisLine;

        protected ShaderProgram lineShader;

        //parent wich it roatates around
        protected Planet parent;

        public Planet(string dataBaseName, PlanetParameters param, Planet parent, VAO vao, Texture t)
            : base(vao, t)
        {
            this.parent = parent;
            this.RevolutionOrientation = -1;
            this.DrawAxisTilt = true;
            this.DrawOrbit = false;

            this.ScenicDistance = param.DFSScenic[dataBaseName];
            this.HoursPerRotation = param.RotationPeriod[dataBaseName];
            this.AxisTilt =  (float)MathHelper.DegreesToRadians(param.AxialTilt[dataBaseName]);
            this.RealisticRadius = param.RadiusRealistic[dataBaseName];
            this.ScenicRadius = param.RadiusScenic[dataBaseName];
            this.setScale((float)ScenicRadius);
            this.OrbitalPeriod = param.OrbitalPeriod[dataBaseName];
            this.inclinationAngle = param.InclinationAngle[dataBaseName];
            this.longitudeAscendingNode = param.LongitudeAscendingNode[dataBaseName];
            this.longitudePerihelion = param.LongitudePerihelion[dataBaseName];
            this.eccentricity = param.Eccentricity[dataBaseName];
            this.longitudeAscendingNodeRadians = MathHelper.DegreesToRadians(longitudeAscendingNode);
            this.longitudePerihelionRadians = MathHelper.DegreesToRadians(longitudePerihelion);
            this.inclinationAngleRadians = MathHelper.DegreesToRadians(inclinationAngle);
            this.PeriodAngle = longitudePerihelionRadians;
            this.eccentricAnomaly = 0;

            if (parent != null)
            {
                this.Orbit = new Orbit(this, parent.Translation, this.ScenicDistance, 360);
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
          
            if (parent != null)
            {
                float x = (float)(ScenicDistance * Math.Cos(PeriodAngle) + parent.Translation.X);
                float y = 0;
                float z = (float)(ScenicDistance * Math.Sin(PeriodAngle) + parent.Translation.Z);
                this.SetTranslation(new Vector3(x, y, z));
            }

            if (parent is Earth)
                Orbit.updatePosition(parent.Translation);
        }

        public void setSize(double ratio)
        {
            //double range = ScenicRadius - RealisticRadius;
            //this.Scale = (float)(RealisticRadius + range * Math.Pow(ratio, 3)) * 10; // y = x^3
            //this.createTransform();
        }
    }
}