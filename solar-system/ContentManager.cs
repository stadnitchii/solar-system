using OpenGL;
using System;
using System.Collections.Generic;

namespace SolarSystem
{
    class ContentManager
    {
        public string ContentDirectory { get; set; }

        public string ShaderFolder { get; set; }
        private Dictionary<string, ShaderProgram> _shaders;

        public string MeshFolder { get; set; }
        private Dictionary<string, VAO> _meshes;

        public string TextureFolder { get; set; }
        private Dictionary<string, Texture> _textures;

        public ContentManager(string contentDirectory)
        {
            _shaders = new Dictionary<string, ShaderProgram>();
            _meshes = new Dictionary<string, VAO>();
            _textures = new Dictionary<string, Texture>();
            ContentDirectory = contentDirectory;
        }

        public ShaderProgram LoadShader(string name, Action init = null)
        {
            if (_shaders.ContainsKey(name))
                return _shaders[name];
            else
            {
                var shader = new ShaderProgram(ContentDirectory + ShaderFolder + name + ".vert", ContentDirectory + ShaderFolder + name + ".frag", name);
                _shaders.Add(name, shader);
                return shader;
            }
        }

        public String ReloadShader(string name)
        {
            try
            {
                return _shaders[name].Reload();
            }
            catch { }
            return null;
        }

        public ShaderProgram GetShader(string name)
        {
            return _shaders[name];
        }

        public VAO LoadVao(string name)
        {
            if (_shaders.ContainsKey(name))
                return _meshes[name];
            else
            {
                var mesh = ObjLoader.LoadAsVAO(ContentDirectory + MeshFolder + name + ".obj");
                _meshes.Add(name, mesh);
                return mesh;
            }
        }

        public VAO GetVao(string name)
        {
            return _meshes[name];
        }

        public Texture LoadTexture(string name)
        {
            if (_textures.ContainsKey(name))
                return _textures[name];
            else
            {
                var texture = new Texture2d(ContentDirectory + TextureFolder + name);
                _textures.Add(name, texture);
                return texture;
            }
        }

        public Texture LoadTexture(string name, string keyName)
        {
            var texture = new Texture2d(ContentDirectory + TextureFolder + name);
            _textures.Add(keyName, texture);
            return texture;
        }

        public Texture getTexture(string keyName)
        {
            return _textures[keyName];
        }
    }
}
