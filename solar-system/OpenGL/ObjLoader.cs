using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace OpenGL
{
    struct VertexData
    {
        public Vector3 position { get; set; }
        public Vector2 uv { get; set; }
        public Vector3 normal { get; set; }

        public VertexData(Vector3 position, Vector2 uv, Vector3 normal)
        {
            this.position = position;
            this.uv = uv;
            this.normal = normal;
        }
    }

    class ObjLoader
    {
        private ObjLoader() { }

        private static bool hasUvs { get; set; }
        private static bool hasNormals { get; set; }

        public static VAO LoadAsVAO(string path)
        {
            List<Vector3> positions = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Vector3> normals = new List<Vector3>();
            List<VertexData> verts = new List<VertexData>();

            hasUvs = false;
            hasNormals = false;

            if (!File.Exists(path))
                throw new FileNotFoundException();

            string[] lines = File.ReadAllLines(path);

            foreach (string str in lines)
            {
                if (!string.IsNullOrWhiteSpace(str) && str.Length > 2)
                {
                    switch (str.Substring(0, 2))
                    {
                        case "v ":
                            parseVert(str, positions);
                            break;

                        case "vt":
                            parseTexture(str, uvs);
                            break;

                        case "vn":
                            parseNormal(str, normals);
                            break;

                        case "f ":
                            parseFace(str, positions, uvs, normals, verts);
                            break;
                    }
                }
            }

            Console.WriteLine("Loaded file {0}, with {1} with vertices", path, verts.Count);

            List<Vector3> p = new List<Vector3>();
            List<Vector2> u = new List<Vector2>();
            List<Vector3> n = new List<Vector3>();

            uint[] e = new uint[verts.Count];

            Dictionary<VertexData, int> vertices = new Dictionary<VertexData, int>();

            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < verts.Count; i++)
            {
                if (!vertices.ContainsKey(verts[i]))
                {
                    p.Add(verts[i].position);
                    u.Add(verts[i].uv);
                    n.Add(verts[i].normal);

                    e[i] = (uint)p.Count - 1;
                    vertices.Add(verts[i], vertices.Count);
                }
                else
                {
                    e[i] = (uint)vertices[verts[i]];
                }
            }
            watch.Stop();

            Console.WriteLine(watch.ElapsedMilliseconds);

            VAO vao = new VAO(p.ToArray());

            if (hasUvs) vao.addAttributeArray(u.ToArray(), 2, 1);
            if (hasNormals) vao.addAttributeArray(n.ToArray(), 3, 2);
            vao.addElementArray(e);

            return vao;
        }

        #region Parsing

        static private void parseVert(string str, List<Vector3> positions)
        {
            string[] parts = str.Split(new string[] { " ", "  ", "   " }, StringSplitOptions.RemoveEmptyEntries);
            float x = float.Parse(parts[1]);
            float y = float.Parse(parts[2]);
            float z = float.Parse(parts[3]);

            positions.Add(new Vector3(x, y, z));
        }

        static private void parseTexture(string str, List<Vector2> uvs)
        {
            string[] parts = str.Split(new string[] { " ", "  ", "   " }, StringSplitOptions.RemoveEmptyEntries);
            float x = float.Parse(parts[1]);
            float y = float.Parse(parts[2]);

            uvs.Add(new Vector2(x, y));
        }

        static private void parseNormal(string str, List<Vector3> normals)
        {
            string[] parts = str.Split(new string[] { " ", "  ", "   " }, StringSplitOptions.RemoveEmptyEntries);
            float x = float.Parse(parts[1]);
            float y = float.Parse(parts[2]);
            float z = float.Parse(parts[3]);

            normals.Add(new Vector3(x, y, z));
        }

        static private void parseFace(string str, List<Vector3> positions, List<Vector2> uvs, List<Vector3> normals, List<VertexData> verts)
        {
            List<string> verticies = new List<string>(str.Split(new string[] { " ", "  ", "   " }, StringSplitOptions.RemoveEmptyEntries));
            verticies.RemoveAt(0);

            foreach (string s in verticies)
            {
                // 1/2/3 = vertex 1 / textureCoord 2 / normal 3
                string[] parts = s.Split(new string[] { "/" }, StringSplitOptions.None);

                VertexData vertex = new VertexData();

                int pi; //positionIndex
                if (int.TryParse(parts[0], out pi))
                    if (pi > 0)
                        vertex.position = positions[pi - 1]; //obj format is starts at 1
                    else
                        vertex.position = positions[positions.Count + pi];

                int uvi; //uvIndex
                if (int.TryParse(parts[1], out uvi))
                {
                    if (uvi > 0)
                        vertex.uv = uvs[uvi - 1]; //obj format is starts at 1
                    else
                        vertex.uv = uvs[uvs.Count + uvi];
                    hasUvs = true;
                }

                int ni; //normalIndex
                if (int.TryParse(parts[2], out ni))
                {
                    if (ni > 0)
                        vertex.normal = normals[ni - 1]; //obj format is starts at 1
                    else
                        vertex.normal = normals[normals.Count + ni];
                    hasNormals = true;
                }

                verts.Add(vertex);
            }
        }
        #endregion
    }
}
