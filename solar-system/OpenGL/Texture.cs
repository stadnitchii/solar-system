#region License
/*

MIT License

Copyright (c) 2016 Vasile Stadnitchii

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/
#endregion

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;


namespace OpenGL
{
    public abstract class Texture
    {
        public int Id { get; private set; }

        public Vector2 Size { get; private set; }

        public Texture()
        {
            Id = GL.GenTexture();
        }

        public virtual void Bind(int activeTexture = 0)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + activeTexture);
            GL.BindTexture(TextureTarget.Texture2D, Id);
        }

        public virtual void UnBind(int activeTexture = 0)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + activeTexture);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public virtual void Dispose()
        {
            GL.DeleteTexture(Id);
        }
    }
}
