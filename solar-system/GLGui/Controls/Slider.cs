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
using System.Drawing;
using OpenTK;
using OpenTK.Input;

namespace GLGui.Controls
{
    /// <summary>
    /// Slider control
    /// </summary>
    public class Slider : Control
    {
        public Vector4 SliderColor { get; set; }

        private Rectangle _barRect;
        private Rectangle _sliderRect;

        public float Value { get; set; }

        public event EventHandler ValueChanged;

        public Slider()
        {
            Location = new Point(0, 0);
            Size = new Size(75, 25);
            BackgroundColor = new Vector4(119/ 255f, 125/255f, 135/255f, 1f);
            SliderColor = new Vector4(0f, 120 / 255f, 215f / 255f, 1f);

            BorderWidth = 0;

            Value = 0;

            _sliderRect = getSliderRect();
            _barRect = getBarRect();
        }

        protected virtual Rectangle getSliderRect()
        {
           int a = (int)(GlobalBounds.Height / 10);
            float slidingSpace = _barRect.Width - _sliderRect.Width;
            return new Rectangle(GlobalBounds.X + +(int)(slidingSpace * Value), GlobalBounds.Y + a, 5, GlobalBounds.Height - (2 * a));
        }

        protected virtual Rectangle getBarRect()
        {
            int a = (int)(GlobalBounds.Height / 2.1);
            return new Rectangle(GlobalBounds.X, GlobalBounds.Y + a, GlobalBounds.Width, GlobalBounds.Height - (2 * a));
        }

        protected virtual void OnValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnLocationChanged()
        {
            base.OnLocationChanged();

            _barRect = getBarRect();
            _sliderRect = getSliderRect();
        }

        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();
            _sliderRect = getSliderRect();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs data)
        {
            var args = data as MouseWheelEventArgs;
            if (args.Delta > 0)
                Value += .1f;
            else
                Value -= .1f;

            if (Value > 1) Value = 1;
            if (Value < 0) Value = 0;

            float slidingSpace = _barRect.Width - _sliderRect.Width;
            _sliderRect.X = GlobalBounds.X + (int)(slidingSpace * Value);

            OnValueChanged();
        }

        protected override void OnMouseDown(MouseButtonEventArgs args)
        {
            float delta = args.X - GlobalBounds.X;

            float slidingSpace = _barRect.Width - _sliderRect.Width;
            Value = delta / slidingSpace;

            _sliderRect.X = GlobalBounds.X + (int)(slidingSpace * Value);

            OnValueChanged();
        }

        protected override void OnMouseMove(MouseMoveEventArgs data)
        {
            if(data.Mouse.LeftButton == ButtonState.Pressed)
            {
                float delta = data.X - GlobalBounds.X;

                float slidingSpace = _barRect.Width - _sliderRect.Width;
                Value = delta / slidingSpace;

                if (Value > 1) Value = 1;
                if (Value < 0) Value = 0;

                _sliderRect.X = GlobalBounds.X + (int)(slidingSpace * Value);

                OnValueChanged();
            }
        }

        public override void Draw(ShaderProgram shader, VAO vao, GameWindow gw)
        {
            //base.Draw(shader, vao, gw);

            shader.SetUniform("rect", _barRect);
            shader.SetUniform("borderWidth", 0);
            shader.SetUniform("useTexture", false);
            shader.SetUniform("color", BackgroundColor);
            vao.DrawArrays(OpenTK.Graphics.OpenGL.PrimitiveType.TriangleStrip);

            shader.SetUniform("color", SliderColor);
            shader.SetUniform("rect", _sliderRect);
            vao.DrawArrays(OpenTK.Graphics.OpenGL.PrimitiveType.TriangleStrip);
        }
    }
}
