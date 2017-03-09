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
    /// Defines the interface for a control that wants to be drawn
    /// </summary>
    public interface IDrawable
    {
        #region Colors
        /// <summary>
        /// The texture that will be drawn
        /// </summary>
        Vector4 BackgroundColor { get; set; }

        /// <summary>
        /// The color that will be drawn to the pick buffer, DONT MODIFY THIS
        /// </summary>
        Vector4 PickBufferColor { get; set; }

        /// <summary>
        /// The color of the border
        /// </summary>
        Vector4 BorderColor { get; set; }

        /// <summary>
        /// The image texture that will be drawn to the background, leave null if not desired
        /// </summary>
        Texture BackgroundImage { get; set; }
        #endregion

        #region Location/Size
        /// <summary>
        /// The bounds of the contorl in global coordinates
        /// </summary>
        Rectangle GlobalBounds { get; }

        /// <summary>
        /// The location of the control related to the parent
        /// </summary>
        Point Location { get; set; }

        /// <summary>
        /// The size of the control
        /// </summary>
        Size Size { get; set; }
        #endregion

        #region Other
        /// <summary>
        /// The width of the border in pixels
        /// </summary>
        int BorderWidth { get; set; }
        #endregion

        /// <summary>
        /// Draw to pick buffer, pick buffer will be used for mouse events
        /// </summary>
        /// <param name="shader"></param>
        /// <param name="vao"></param>
        /// <param name="gw"></param>
        void DrawToPickBuffer(ShaderProgram shader, VAO vao, GameWindow gw);

        /// <summary>
        /// Draw the cotrol's children to pick buffer, pick buffer will be used for mouse events
        /// </summary>
        /// <param name="shader"></param>
        /// <param name="vao"></param>
        /// <param name="gw"></param>
        void DrawChildrenToPickBuffer(ShaderProgram shader, VAO vao, GameWindow gw);

        /// <summary>
        /// Draw the control
        /// </summary>
        /// <param name="shader"></param>
        /// <param name="vao"></param>
        /// <param name="gw"></param>
        void Draw(ShaderProgram shader, VAO vao, GameWindow gw);

        /// <summary>
        /// Draw the control's children
        /// </summary>
        /// <param name="shader"></param>
        /// <param name="vao"></param>
        /// <param name="gw"></param>
        void DrawChildren(ShaderProgram shader, VAO vao, GameWindow gw);
    }
}