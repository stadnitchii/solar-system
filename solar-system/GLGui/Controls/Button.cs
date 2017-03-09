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

using GLGui.Interfaces;
using OpenTK;
using System.Drawing;
using OpenGL;
using System;
using OpenTK.Input;

namespace GLGui.Controls
{
    /// <summary>
    /// Generic button
    /// </summary>
    public class Button : Label
    {
        /// <summary>
        /// Constructs the button and sets default properties such as location and size
        /// </summary>
        public Button()
        {
            Size = new Size(75, 25);
            BackgroundColor = new Vector4(.88f, .88f, .88f, 1f);

            BorderWidth = 1;
            BorderColor = new Vector4(.68f, .68f, .68f, 1f);

            Text = "button";
        }

        /// <summary>
        /// Changes the borderColor when mouse enters
        /// </summary>
        /// <param name="data"></param>
        protected override void OnMouseEnter(MouseMoveEventArgs data)
        {
            base.OnMouseEnter(data);

            BorderColor = new Vector4(24 / 255f, 131 / 255f, 215 / 255f, 1f);
            BackgroundColor = new Vector4(228 / 255f, 241 / 255f, 251 / 255f, 1f);
        }

        /// <summary>
        /// Changes the borderColor when mouse enters
        /// </summary>
        /// <param name="data"></param>
        protected override void OnMouseLeave(MouseMoveEventArgs data)
        {
            base.OnMouseLeave(data);

            BorderColor = new Vector4(.68f, .68f, .68f, 1f);
            BackgroundColor = new Vector4(.88f, .88f, .88f, 1f);
        }
    }
}
