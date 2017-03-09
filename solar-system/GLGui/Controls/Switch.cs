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
using OpenGL;
using OpenTK;
using System.Drawing;
using OpenTK.Input;

namespace GLGui.Controls
{
    class Switch : Control
    {
        public int SwitchWidth { get; set; }

        public Vector4 SwitchColor { get; set; }
        public Vector4 OnColor { get; set; }

        public bool On { get; set; }

        public event EventHandler OnToggle;

        public Switch()
        {
            BorderWidth = 1;
            Size = new Size(50, 25);
            BackgroundColor = new Vector4(96 / 255f, 96 / 255f, 96 / 255f, 1);
            BorderColor = new Vector4(.2f, .2f, .2f, 1);

            OnColor = new Vector4(0f, 118 / 255f, 215 / 255f, 1);
            SwitchColor = new Vector4(1, 1, 1, 1);
            SwitchWidth = GlobalBounds.Height;

            On = false;
        }

        private Rectangle getSwitchBounds()
        {
            if (On)
                return new Rectangle(GlobalBounds.X + GlobalBounds.Width - SwitchWidth, GlobalBounds.Y - 1, SwitchWidth, GlobalBounds.Height + 2);
            else
                return new Rectangle(GlobalBounds.X , GlobalBounds.Y - 1, SwitchWidth, GlobalBounds.Height + 2);
        }

        private Rectangle getOnBounds()
        {
                return new Rectangle(GlobalBounds.X, GlobalBounds.Y, GlobalBounds.Width - SwitchWidth + 1, GlobalBounds.Height);
        }

        protected override void OnMouseClick(MouseButtonEventArgs data)
        {
            On = !On;
            OnToggle?.Invoke(this, EventArgs.Empty);
        }

        public override void Draw(ShaderProgram shader, VAO vao, GameWindow gw)
        {
            //base.Draw(shader, vao, gw);

            shader.SetUniform("rect", GlobalBounds);
            shader.SetUniform("color", BackgroundColor);
            shader.SetUniform("borderWidth", BorderWidth);
            shader.SetUniform("borderColor", BorderColor);
            shader.SetUniform("useTexture", false);
            vao.DrawArrays(OpenTK.Graphics.OpenGL.PrimitiveType.TriangleStrip);

            shader.SetUniform("rect", getSwitchBounds());
            shader.SetUniform("color", SwitchColor);
            vao.DrawArrays(OpenTK.Graphics.OpenGL.PrimitiveType.TriangleStrip);

            if (On)
            {
                shader.SetUniform("rect", getOnBounds());
                shader.SetUniform("color", OnColor);
                vao.DrawArrays(OpenTK.Graphics.OpenGL.PrimitiveType.TriangleStrip);
            }
        }
    }
}
