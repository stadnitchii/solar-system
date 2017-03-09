using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem
{
    public class PlanetParameters
    {
        public Dictionary<string, double> RadiusRealistic { get; private set; }
        public Dictionary<string, double> RadiusScenic { get; private set; }
        public Dictionary<string, double> DFSRealistic { get; private set; }
        public Dictionary<string, double> DFSScenic { get; private set; }
        public Dictionary<string, double> RotationPeriod { get; private set; }
        public Dictionary<string, double> OrbitalPeriod { get; private set; }
        public Dictionary<string, double> AxialTilt { get; private set; }
        public Dictionary<string, double> InclinationAngle { get; private set; }
        public Dictionary<string, double> LongitudeAscendingNode { get; private set; }
        public Dictionary<string, double> LongitudePerihelion { get; private set; }
        public Dictionary<string, double> Eccentricity { get; private set; }


        public PlanetParameters(Dictionary<string, double> rr, Dictionary<string, double> rs,
            Dictionary<string, double> dfsr, Dictionary<string, double> dfss, Dictionary<string, double> rp,
            Dictionary<string, double> op, Dictionary<string, double> at, Dictionary<string, double> ia, Dictionary<string, double> lan, Dictionary<string, double> lp, Dictionary<string, double> ec)
        {
            this.RadiusRealistic = rr;
            this.RadiusScenic = rs;
            this.DFSRealistic = dfsr;
            this.DFSScenic = dfss;
            this.RotationPeriod = rp;
            this.OrbitalPeriod = op;
            this.AxialTilt = at;
            this.InclinationAngle = ia;
            this.LongitudeAscendingNode = lan;
            this.LongitudePerihelion = lp;
            this.Eccentricity = ec;
        }

        public static PlanetParameters readFromFile(string path)
        {
            Dictionary<string, double> rr = new Dictionary<string, double>();
            Dictionary<string, double> rs = new Dictionary<string, double>();
            Dictionary<string, double> dfsr = new Dictionary<string, double>();
            Dictionary<string, double> dfss = new Dictionary<string, double>();
            Dictionary<string, double> rp = new Dictionary<string, double>();
            Dictionary<string, double> op = new Dictionary<string, double>();
            Dictionary<string, double> at = new Dictionary<string, double>();
            Dictionary<string, double> ia = new Dictionary<string, double>();
            Dictionary<string, double> lan = new Dictionary<string, double>();
            Dictionary<string, double> lp = new Dictionary<string, double>();
            Dictionary<string, double> ec = new Dictionary<string, double>();

            List<Dictionary<string, double>> list = new List<Dictionary<string, double>>();
            list.Add(rr);
            list.Add(rs);
            list.Add(dfsr);
            list.Add(dfss);
            list.Add(rp);
            list.Add(op);
            list.Add(at);
            list.Add(ia);
            list.Add(lan);
            list.Add(lp);
            list.Add(ec);

            string line = "";
            int count = 0;
            StreamReader reader = new StreamReader(path);

            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                string[] info = line.Split(new string[] { " ", "  ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                addInfo(info, list[count]);
                count++;
            }

            return new PlanetParameters(rr, rs, dfsr, dfss, rp, op, at, ia, lan, lp, ec);
        }

        private static void addInfo(string[] info, Dictionary<string, double> cd)
        {
            double[] nums = new double[info.Length];
            int count = 0;
            foreach (double num in nums)
            {
                nums[count] = double.Parse(info[count]);
                count++;
            }

            cd.Add("sun", nums[0]);
            cd.Add("mercury", nums[1]);
            cd.Add("venus", nums[2]);
            cd.Add("earth", nums[3]);
            cd.Add("moon", nums[4]);
            cd.Add("mars", nums[5]);
            cd.Add("jupiter", nums[6]);
            cd.Add("saturn", nums[7]);
            cd.Add("uranus", nums[8]);
            cd.Add("neptune", nums[9]);
            cd.Add("pluto", nums[10]);

        }
    }
}
