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

namespace GLGui.Controls
{
    /// <summary>
    /// A generic label
    /// </summary>
    public class Label : Control, IText
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                if (!string.IsNullOrWhiteSpace(value))
                    TextImage = GuiManager.createTextureFromText(_text, Font, TextImage, TextColor, TextBackgroundColor, out _textBounds);
            }
        }

        public Font Font { get; set; }

        public Vector4 TextBackgroundColor { get; set; }

        public Vector4 TextColor { get; set; }

        public Texture TextImage { get; set; }

        private Rectangle _textBounds;
        public Rectangle TextBounds
        {
            get
            {
                return GuiManager.centerTextBounds(GlobalBounds, _textBounds);
            }
        }

        public Label()
        {
            Font = new Font("Arial", 14f, FontStyle.Regular, GraphicsUnit.Pixel);
            TextBackgroundColor = new Vector4(0, 0, 0, 0);
            TextColor = new Vector4(0, 0, 0, 1);

            Location = new Point(0, 0);
            Size = new Size(75, 25);
            BackgroundColor = new Vector4(0, 0, 0, 0);

            BorderWidth = 0;

            Text = "label";
        }
    }
}
