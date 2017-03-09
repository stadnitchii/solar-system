using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem
{
    partial class Planet
    {
        //inclination angle
        public double inclinationAngle { get; protected set; }

        //longitude of ascending node
        public double longitudeAscendingNode { get; protected set; }

        //longitude of perihelion
        public double longitudePerihelion { get; protected set; }

        //eccentricity
        public double eccentricity { get; protected set; }
        public double inclinationAngleRadians { get; protected set; }
        public double longitudeAscendingNodeRadians { get; protected set; }
        public double longitudePerihelionRadians { get; protected set; }
        public double eccentricAnomaly { get; protected set; }
    }
}
