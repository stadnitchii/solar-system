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

using System;
using OpenTK.Graphics.OpenGL;

namespace OpenGL
{
    class GaussianBlur : IDisposable
    {
        Framebuffer horizontal;
        Framebuffer vertical;

        ShaderProgram shader;

        FrameRenderer fr;

        public GaussianBlur(int width, int height)
        {
            horizontal = new Framebuffer(width, height);
            horizontal.AttachColorBuffer(type : PixelType.Float, internalFormat : PixelInternalFormat.Rgba16f);

            vertical = new Framebuffer(width, height);
            vertical.AttachColorBuffer(type: PixelType.Float, internalFormat: PixelInternalFormat.Rgba16f);

            shader = Shaders.BlurShader();
            fr = FrameRenderer.Instance;
        }

        public Texture Blur(Texture source, int times)
        {
            shader.Bind();
            Texture txt = source;

            for (int i = 0; i < times; i++)
            {
                shader.SetUniform("horizontal", true);
                horizontal.Bind();
                fr.Draw(Area.Full, txt, shader);
                txt = horizontal.Textures[0];

                shader.SetUniform("horizontal", false);
                vertical.Bind();
                fr.Draw(Area.Full, txt, shader);
                txt = vertical.Textures[0];
            }

            return vertical.Textures[0];
        }

        public void Dispose()
        {
            
        }
    }
}
