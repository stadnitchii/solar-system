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
        public Dictionary<string, double> PlanetRadius { get; private set; }
        public Dictionary<string, double> DistanceFromSun { get; private set; }
        public Dictionary<string, double> RotationPeriod { get; private set; }
        public Dictionary<string, double> OrbitalPeriod { get; private set; }
        public Dictionary<string, double> AxialTilt { get; private set; }

        public PlanetParameters(
            Dictionary<string, double> pr, Dictionary<string, double> dfs,
            Dictionary<string, double> rp, Dictionary<string, double> op,
            Dictionary<string, double> at

            )
        {
            this.PlanetRadius = pr;
            this.DistanceFromSun = dfs;
            this.RotationPeriod = rp;
            this.OrbitalPeriod = op;
            this.AxialTilt = at;
        }

        public static PlanetParameters readFromFile(string path)
        {
            Dictionary<string, double> pr = new Dictionary<string, double>();
            Dictionary<string, double> dfs = new Dictionary<string, double>();
            Dictionary<string, double> rp = new Dictionary<string, double>();
            Dictionary<string, double> op = new Dictionary<string, double>();
            Dictionary<string, double> at = new Dictionary<string, double>();

            List<Dictionary<string, double>> list = new List<Dictionary<string, double>>();
            list.Add(pr);
            list.Add(dfs);
            list.Add(rp);
            list.Add(op);
            list.Add(at);

            string[] lines = File.ReadAllLines(path);

            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) throw new IOException($"Could not read {path}, incorrect format.");
                string[] info = lines[i].Split(new string[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                addInfo(info, list[i]);
            }

            return new PlanetParameters(pr, dfs, rp, op, at);
        }

        private static void addInfo(string[] info, Dictionary<string, double> cd)
        {
            double[] nums = new double[info.Length];
            for (int i = 0; i < nums.Length; i++)
            {
                nums[i] = double.Parse(info[i]);
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
