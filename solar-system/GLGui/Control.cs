using GLGui.Interfaces;
using OpenGL;
using OpenTK;
using OpenTK.Input;
using System;
using System.Drawing;


namespace GLGui
{
    public abstract class Control : IDrawable
    {
        #region Properties
        public Control Parent { get; set; }

        public ControlCollection Controls { get; private set; }
        #endregion

        #region Interface Implementations
        public Vector4 PickBufferColor { get; set; }

        public Vector4 BackgroundColor { get; set; }

        public Vector4 BorderColor { get; set; }
        public int BorderWidth { get; set; }

        public Texture BackgroundImage { get; set; }

        public Rectangle _globalBounds;
        public Rectangle GlobalBounds
        {
            get { return _globalBounds; }
        }

        private Point relativeLocation;

        public Point Location
        {
            get { return new Point(relativeLocation.X, relativeLocation.Y); }
            set
            {
                int x = value.X;
                int y = value.Y;

                relativeLocation.X = x;
                relativeLocation.Y = y;

                if (Parent != null)
                {
                    x += Parent.GlobalBounds.X + Parent.BorderWidth;
                    y += Parent.GlobalBounds.Y + Parent.BorderWidth;
                }

                _globalBounds.X = x;
                _globalBounds.Y = y;

                OnLocationChanged();
            }
        }

        public Size Size
        {
            get { return new Size(_globalBounds.Width, _globalBounds.Height); }
            set
            {
                _globalBounds.Width = value.Width;
                _globalBounds.Height = value.Height;
                OnSizeChanged();
            }
        }
        #endregion

        #region Event Variables
        private bool _mouseDown;
        #endregion

        #region Events
        public delegate void MouseEventHandler(object sender, MouseEventArgs e);

        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event MouseEventHandler MouseClick;
        public event MouseEventHandler MouseWheel;

        public event MouseEventHandler MouseLeave;
        public event MouseEventHandler MouseEnter;
        public event MouseEventHandler MouseMove;

        public event EventHandler LocationChanged;
        public event EventHandler SizeChanged;
        #endregion

        public Control()
        {
            Controls = new ControlCollection();
            Controls.OnControlAdded += Controls_OnControlAdded;
        }

        public virtual void DrawToPickBuffer(ShaderProgram shader, VAO vao, GameWindow gw)
        {
            shader.SetUniform("color", new Vector4(PickBufferColor.X / 255, PickBufferColor.Y / 255, PickBufferColor.Z / 255, PickBufferColor.W / 255));
            shader.SetUniform("rect", GlobalBounds);
            shader.SetUniform("borderWidth", 0);
            shader.SetUniform("useTexture", false);
            vao.DrawArrays(OpenTK.Graphics.OpenGL.PrimitiveType.TriangleStrip);
        }

        public virtual void DrawChildrenToPickBuffer(ShaderProgram shader, VAO vao, GameWindow gw)
        {
            foreach (var c in Controls)
            {
                GuiManager.setScisor(GlobalBounds, BorderWidth, gw);
                c.DrawToPickBuffer(shader, vao, gw);
            }

            foreach (var c in Controls)
                c.DrawChildrenToPickBuffer(shader, vao, gw);
        }

        public virtual void Draw(ShaderProgram shader, VAO vao, GameWindow gw)
        {
            shader.SetUniform("rect", GlobalBounds);
            shader.SetUniform("color", BackgroundColor);
            shader.SetUniform("borderWidth", BorderWidth);
            shader.SetUniform("borderColor", BorderColor);

            if (BackgroundImage != null)
            {
                BackgroundImage.Bind();
                shader.SetUniform("useTexture", true);
            }
            else
            {
                shader.SetUniform("useTexture", false);
            }

            vao.DrawArrays(OpenTK.Graphics.OpenGL.PrimitiveType.TriangleStrip);

            var temp = this as IText;
            if (temp != null && !string.IsNullOrWhiteSpace(temp.Text))
            {
                GuiManager.setScisor(GlobalBounds, BorderWidth, gw, Parent);
                temp.TextImage.Bind();
                shader.SetUniform("useTexture", true);
                shader.SetUniform("rect", temp.TextBounds);
                shader.SetUniform("borderWidth", 0);

                vao.DrawArrays(OpenTK.Graphics.OpenGL.PrimitiveType.TriangleStrip);
            }
        }

        public virtual void DrawChildren(ShaderProgram shader, VAO vao, GameWindow gw)
        {
            foreach (var c in Controls)
            {
                GuiManager.setScisor(GlobalBounds, BorderWidth, gw);
                c.Draw(shader, vao, gw);
            }

            foreach (var c in Controls)
                c.DrawChildren(shader, vao, gw);
        }

        public void SetAplha(float alpha)
        {
            BackgroundColor = new Vector4(BackgroundColor.X, BackgroundColor.X, BackgroundColor.X, alpha);
            var bordered = this as IDrawable;
            if (bordered != null)
                bordered.BorderColor = new Vector4(bordered.BorderColor.X, bordered.BorderColor.X, bordered.BorderColor.X, alpha);
        }

        #region Events
        public void RecieveMessage(Message msg)
        {
            switch (msg.id)
            {
                case MessageId.MouseDown:
                    OnMouseDown((MouseButtonEventArgs)msg.data);
                    break;

                case MessageId.MouseUp:
                    OnMouseUp((MouseButtonEventArgs)msg.data);
                    break;

                case MessageId.MouseWheel:
                    OnMouseWheel((MouseWheelEventArgs)msg.data);
                    break;

                case MessageId.MouseMove:
                    OnMouseMove((MouseMoveEventArgs)msg.data);
                    break;

                case MessageId.MouseEnter:
                    OnMouseEnter((MouseMoveEventArgs)msg.data);
                    break;

                case MessageId.MouseLeave:
                    OnMouseLeave((MouseMoveEventArgs)msg.data);
                    break;
            }
        }

        protected virtual void OnMouseDown(MouseButtonEventArgs args)
        {
            _mouseDown = true;
            MouseDown?.Invoke(this, args);
        }

        protected virtual void OnMouseUp(MouseButtonEventArgs args)
        {
            MouseUp?.Invoke(this, args);
            if (_mouseDown)
            {
                OnMouseClick(args);
                _mouseDown = false;
            }
        }

        protected virtual void OnMouseClick(MouseButtonEventArgs data)
        {
            MouseClick?.Invoke(this, data);
        }

        protected virtual void OnMouseWheel(MouseWheelEventArgs data)
        {
            MouseWheel?.Invoke(this, data);
        }

        protected virtual void OnMouseMove(MouseMoveEventArgs data)
        {
            MouseMove?.Invoke(this, data);
        }

        protected virtual void OnMouseEnter(MouseMoveEventArgs data)
        {
            MouseEnter?.Invoke(this, (MouseEventArgs)data);
            _mouseDown = false;
        }

        protected virtual void OnMouseLeave(MouseMoveEventArgs data)
        {
            MouseLeave?.Invoke(this, (MouseEventArgs)data);
            _mouseDown = false;
        }

        protected virtual void OnSizeChanged()
        {
            SizeChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnLocationChanged()
        {
            LocationChanged?.Invoke(this, EventArgs.Empty);
            foreach (var c in Controls)
                c.Location = c.Location;
        }

        protected virtual void Controls_OnControlAdded(Control control)
        {
            control.Parent = this;
            control.Location = control.Location;
            GuiManager.setControlPickColor(control);
        }
        #endregion
    }
}