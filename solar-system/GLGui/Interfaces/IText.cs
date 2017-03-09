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

using OpenGL;
using OpenTK;
using System.Drawing;

namespace GLGui.Interfaces
{
    /// <summary>
    /// Defines the iterface for controls that want to display text
    /// </summary>
    public interface IText
    {
        /// <summary>
        /// The text to be rendered
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// The font the text will be rendered with
        /// </summary>
        Font Font { get; set; }

        /// <summary>
        /// The color of the text
        /// </summary>
        Vector4 TextColor { get; set; }

        /// <summary>
        /// The background color of the text
        /// </summary>
        Vector4 TextBackgroundColor { get; set; }
        
        /// <summary>
        /// The text texture, this is what OpenGL will render to the control
        /// </summary>
        Texture TextImage { get; set; }

        /// <summary>
        /// The bounds of the text texture
        /// </summary>
        Rectangle TextBounds { get; }
    }
}
