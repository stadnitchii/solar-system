#define LogWarnings

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace OpenGL
{
    public struct AttributeInfo
    {
        public string Name;
        public int Size;
        public int Address; 
        public ActiveAttribType Type;
    }

    public struct UniformInfo
    {
        public string Name;
        public int Size;
        public int Address;
        public ActiveUniformType Type;
    }

    class Shader
    {
        public int Pointer { get; private set; }

        public Shader(string source, OpenTK.Graphics.OpenGL.ShaderType type)
        {
            this.Pointer = GL.CreateShader(type);
            source += "\0";
            GL.ShaderSource(Pointer, source);
            GL.CompileShader(Pointer);
            int status = 0;
            GL.GetShader(Pointer, ShaderParameter.CompileStatus, out status);
            string infolog = GL.GetShaderInfoLog(Pointer);
            if(!string.IsNullOrWhiteSpace(infolog))
                Console.WriteLine(infolog);
        }
    }

    struct ShaderReloadInfo
    {
        public string VertexSource;
        public string FragmentSource;
        public string GeometrySource;
        public bool FromFile;
    }

    public class ShaderProgram
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        public IEnumerable<AttributeInfo> Attributes
        {
            get
            {
                int count = 0;
                GL.GetProgram(Id, GetProgramParameterName.ActiveAttributes, out count);

                for (int i = 0; i < count; i++)
                {
                    AttributeInfo info = new AttributeInfo();
                    info.Name = GL.GetActiveAttrib(Id, i, out info.Size, out info.Type);
                    info.Address = GL.GetAttribLocation(Id, info.Name);
                    yield return info;
                }
            }
        }
        public string AttributeInfo
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var attrib in Attributes)
                    sb.Append($"AttributeInfo [{attrib.Name}, {attrib.Type}[{attrib.Size}], {attrib.Address} ]\n");
                return sb.ToString();
            }
        }
        public IEnumerable<UniformInfo> Uniforms
        {
            get
            {
                int count = 0;
                GL.GetProgram(Id, GetProgramParameterName.ActiveUniforms, out count);

                for (int i = 0; i < count; i++)
                {
                    UniformInfo info = new UniformInfo();
                    info.Name = GL.GetActiveUniform(Id, i, out info.Size, out info.Type);
                    info.Address = GL.GetUniformLocation(Id, info.Name);
                    yield return info;
                }
            }
        }
        public string UnifromInfo
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var uniform in Uniforms)
                    sb.Append($"UniformInfo [{uniform.Name}, {uniform.Type}[{uniform.Size}], {uniform.Address} ]\n");
                return sb.ToString();
            }
        }

        public bool LogWarnings { get; set; }
        public List<string> WarningLogs { get; private set; }

        public Action Initialize { get; set; }
        private ShaderReloadInfo shaderReloadInfo;

        public ShaderProgram(string name, string path = "") :
            this(path + name + ".vert", path + name + ".frag", name)
        { }

        public ShaderProgram(string vertexShader, string fragmentShader, string name, bool fromFile = true, string geometryShader = null)
        {
            LogWarnings = true;
            Name = name;
            shaderReloadInfo.VertexSource = vertexShader;
            shaderReloadInfo.FragmentSource = fragmentShader;
            shaderReloadInfo.GeometrySource = geometryShader;
            shaderReloadInfo.FromFile = fromFile;

            string infolog = load(vertexShader, geometryShader, fragmentShader, fromFile);
            if (!string.IsNullOrEmpty(infolog))
            {
                Console.WriteLine("\n-----" + Name + "-----");
                Console.WriteLine(infolog);
            }
        }

        private string load(string vertexShader, string geometryShader, string fragmentShader, bool fromFile)
        {
            WarningLogs = new List<string>();

            if (fromFile)
            {
                vertexShader = File.ReadAllText(vertexShader);
                fragmentShader = File.ReadAllText(fragmentShader);
                if (geometryShader != null) geometryShader = File.ReadAllText(geometryShader);
            }

            Shader vertex = new Shader(vertexShader, ShaderType.VertexShader);
            Shader fragment = new Shader(fragmentShader, ShaderType.FragmentShader);
            Shader geometry = null;
            if (geometryShader != null)
                geometry = new Shader(geometryShader, ShaderType.GeometryShader);

            Id = GL.CreateProgram();
            GL.AttachShader(Id, vertex.Pointer);
            GL.AttachShader(Id, fragment.Pointer);
            if (geometry != null) GL.AttachShader(Id, geometry.Pointer);
            GL.LinkProgram(Id);
            string programInfoLog = GL.GetProgramInfoLog(Id);         

            GL.DetachShader(Id, vertex.Pointer);
            GL.DeleteShader(vertex.Pointer);
            GL.DetachShader(Id, fragment.Pointer);
            GL.DeleteShader(fragment.Pointer);
            if (geometry != null)
            {
                GL.DetachShader(Id, geometry.Pointer);
                GL.DeleteShader(geometry.Pointer);
            }

            return programInfoLog;
        }

        public string Reload()
        {
            Dispose();
            string msg = load(shaderReloadInfo.VertexSource, shaderReloadInfo.GeometrySource, shaderReloadInfo.FragmentSource, shaderReloadInfo.FromFile);
            if (string.IsNullOrWhiteSpace(msg))
                Initialize?.Invoke();
            return msg;
        }

        public void Bind()
        {
            GL.UseProgram(Id);
        }

        public void Init()
        {
            Bind();
            Initialize?.Invoke();
        }

        public void Dispose()
        {
            GL.UseProgram(0);
            GL.DeleteProgram(Id);
        }

        #region setUniform
        public void SetUniform(string name, bool value)
        {
            int location = GL.GetUniformLocation(Id, name);
            if (location != -1)
                GL.Uniform1(location, (value) ? 1 : 0);
            else if (LogWarnings)
                logWarning($"Warning: '{name}' not found in shader '{Name}'");
        }

        public void SetUniform(string name, int value)
        {
            int location = GL.GetUniformLocation(Id, name);
            if (location != -1)
                GL.Uniform1(location, value);
            else if (LogWarnings)
                logWarning($"Warning: '{name}' not found in shader '{Name}'");
        }

        public void SetUniform(string name, float value)
        {
            int location = GL.GetUniformLocation(Id, name);
            if (location != -1)
                GL.Uniform1(location, value);
            else if (LogWarnings)
                logWarning($"Warning: '{name}' not found in shader '{Name}'");
        }

        public void SetUniform(string name, float value, float value2)
        {
            int location = GL.GetUniformLocation(Id, name);
            if (location != -1)
                GL.Uniform2(location, value, value2);
            else if (LogWarnings)
                logWarning($"Warning: '{name}' not found in shader '{Name}'");
        }

        public void SetUniform(string name, float value, float value2, float value3)
        {
            int location = GL.GetUniformLocation(Id, name);
            if (location != -1)
                GL.Uniform3(location, value, value2, value3);
            else if (LogWarnings)
                logWarning($"Warning: '{name}' not found in shader '{Name}'");
        }

        public void SetUniform(string name, float value, float value2, float value3, float value4)
        {
            int location = GL.GetUniformLocation(Id, name);
            if (location != -1)
                GL.Uniform4(location, value, value2, value3, value4);
            else if (LogWarnings)
                logWarning($"Warning: '{name}' not found in shader '{Name}'");
        }

        public void SetUniform(string name, Matrix4 mat)
        {
            int location = GL.GetUniformLocation(Id, name);
            if (location != -1)
                GL.UniformMatrix4(location, false, ref mat);
            else if (LogWarnings)
                logWarning($"Warning: '{name}' not found in shader '{Name}'");
        }

        public void SetUniform(string name, Matrix4[] mat)
        {
            int location = GL.GetUniformLocation(Id, "{name}[0]");
            if (location != -1)
            {
                for (int i = 0; i < mat.Length; i++)
                {
                    int loc = GL.GetUniformLocation(Id, "{name}[{i}]");
                    GL.UniformMatrix4(loc, false, ref mat[i]);
                }
            }
            else if (LogWarnings)
                logWarning($"Warning: '{name}' not found in shader '{Name}'");
        }

        public void SetUniform(string name, Rectangle rect)
        {
            int location = GL.GetUniformLocation(Id, name);
            if (location != -1)
                GL.Uniform4(location, rect.X, rect.Y, rect.Width, rect.Height);
            else if (LogWarnings)
                logWarning($"Warning: '{name}' not found in shader '{Name}'");
        }

        public void SetUniform(string name, Vector2 vec)
        {
            SetUniform(name, vec.X, vec.Y);
        }

        public void SetUniform(string name, Vector3 vec)
        {
            SetUniform(name, vec.X, vec.Y, vec.Z);
        }

        public void SetUniform(string name, Vector4 vec)
        {
            SetUniform(name, vec.X, vec.Y, vec.Z, vec.W);
        }

        private void logWarning(string log)
        {
            if (!WarningLogs.Contains(log))
            {
                Console.WriteLine(log);
                WarningLogs.Add(log);
            }
        }
        #endregion
    }
}
