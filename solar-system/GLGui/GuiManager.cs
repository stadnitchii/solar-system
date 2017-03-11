using GLGui.Interfaces;
using OpenGL;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
using System.Drawing;
using System;
using System.Collections;

namespace GLGui
{
    public class ControlCollection : IEnumerable<Control>
    {
        private List<Control> list;

        public delegate void ControlEvent(Control control);

        public event ControlEvent OnControlAdded;

        public int Count
        {
            get { return list.Count; }
        }

        public ControlCollection()
        {
            list = new List<Control>();
        }

        public void Add(Control control)
        {
            list.Add(control);

            OnControlAdded?.Invoke(control);
        }

        public void Add(params Control[] controls)
        {
            foreach (var c in controls)
            {
                list.Add(c);
                OnControlAdded?.Invoke(c);
            }
        }

        public void Remove(Control control)
        {
            list.Remove(control);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        public void moveToBack(Control c)
        {
            list.Remove(c);
            list.Add(c);
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(Control control)
        {
            return list.Contains(control);
        }

        public Control[] ToArray()
        {
            return list.ToArray();
        }

        public IEnumerator<Control> GetEnumerator()
        {
            foreach (var item in list)
                yield return item;

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public Control this[int i]
        {
            get { return list[i]; }
            set { list[i] = value; }
        }
    }

    public struct Message
    {
        public MessageId id;
        public object data;

        public Message(MessageId id, object data)
        {
            this.id = id;
            this.data = data;
        }
    }

    public enum MessageId
    {
        MouseDown,
        MouseUp,
        MouseClick,
        MouseWheel,
        MouseMove,
        MouseEnter,
        MouseLeave
    }

    public class GuiManager
    {
        FrameRenderer fr = FrameRenderer.Instance;

        private static Dictionary<Vector4, Control> controlColors;
        private static Vector4 currentControlPickColor;

        private Control selectedControl;

        private Framebuffer framebuffer;
        private VAO vao;
        private ShaderProgram shader;
        private Matrix4 projection;

        public ControlCollection Controls { get; private set; }

        private GameWindow gameWindow;

        public GuiManager(GameWindow gw)
        {
            gameWindow = gw;

            controlColors = new Dictionary<Vector4, Control>();

            currentControlPickColor = new Vector4(0, 0, 0, 255);

            Controls = new ControlCollection();
            Controls.OnControlAdded += (control) =>
            {
                setControlPickColor(control);
            };

            projection = Matrix4.CreateOrthographicOffCenter(0, gw.Width, gw.Height, 0, 0, 1);
            shader = new ShaderProgram(GuiManager.vertex, GuiManager.fragment, "guiShader", false);
            framebuffer = new Framebuffer(gw.Width, gw.Height);
            framebuffer.AttachColorBuffer(
                minFilter: OpenTK.Graphics.OpenGL.TextureMinFilter.Nearest,
                magFilter: OpenTK.Graphics.OpenGL.TextureMagFilter.Nearest
                );

            float[] verts = new float[]
            {
                1,2,3,4
            };

            Vector2[] uvs = new Vector2[]
            {
                new Vector2(0, 1),
                new Vector2(0, 0),
                new Vector2(1, 1),
                new Vector2(1, 0)
            };


            vao = new VAO(verts);
            vao.addAttributeArray(uvs, 2, 1);
        }

        public void Draw(GameWindow gw)
        {
            GL.Enable(EnableCap.ScissorTest);
            GL.Disable(EnableCap.DepthTest);

            shader.Bind();
            shader.SetUniform("proj", projection);

            //draw to color pick buffer
            framebuffer.Bind();
            GL.Viewport(0, 0, gw.Width, gw.Height);
            GL.Scissor(0, 0, gw.Width, gw.Height);
            GL.ClearColor(Color.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            foreach(var c in Controls)
            { 
                GL.Scissor(0, 0, gw.Width, gw.Height);
                c.DrawToPickBuffer(shader, vao, gw);
                c.DrawChildrenToPickBuffer(shader, vao, gw);
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            foreach (var c in Controls)
            {
                GL.Scissor(0, 0, gw.Width, gw.Height);
                c.Draw(shader, vao, gw);
                c.DrawChildren(shader, vao, gw);
            }

            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.ScissorTest);

        }

        public void Update(GameWindow gw)
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                var c = Controls[i] as IUpdate;
                c?.Update(gw);
            }

            int x = gw.Mouse.X;
            int y = gw.Mouse.Y;
        }

        #region Window Event Handlers
        public void WindowResized(GameWindow gw)
        {
            projection = Matrix4.CreateOrthographicOffCenter(0, gw.Width, gw.Height, 0, 0, 1);
        }

        public void MouseDown(OpenTK.Input.MouseButtonEventArgs e)
        {
            if (selectedControl != null)
            {
                selectedControl.RecieveMessage(new Message(MessageId.MouseDown, e));

                if (Controls.Contains(selectedControl))
                {
                    var temp = selectedControl;
                    while (temp.Parent != null)
                        temp = temp.Parent;

                    Controls.moveToBack(temp);
                }
            }
        }

        public void MouseUp(OpenTK.Input.MouseButtonEventArgs e)
        {
            selectedControl?.RecieveMessage(new Message(MessageId.MouseUp, e));
        }

        public void MouseWheel(OpenTK.Input.MouseWheelEventArgs e)
        {
            selectedControl?.RecieveMessage(new Message(MessageId.MouseWheel, e));
        }

        public void MouseMove(OpenTK.Input.MouseMoveEventArgs e)
        {
            //bind our color buffer and get pixel under mouse
            int height = gameWindow.Height;

            framebuffer.Bind();
            byte[] pixels = new byte[4];
            GL.ReadPixels(e.X, height- e.Y, 1, 1, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            //check of hashtabel to see if mouse is over a control
            Control c;
            controlColors.TryGetValue(new Vector4(pixels[0], pixels[1], pixels[2], pixels[3]), out c);

            if (c != null)
            {
                if (selectedControl == null)
                {
                    selectedControl = c;
                    selectedControl.RecieveMessage(new Message(MessageId.MouseEnter, e));
                }
                else if (selectedControl != c)
                {
                    selectedControl.RecieveMessage(new Message(MessageId.MouseLeave, e));
                    selectedControl = c;
                    c.RecieveMessage(new Message(MessageId.MouseEnter, e));
                }

                c.RecieveMessage(new Message(MessageId.MouseMove, e));
            }
            else if (selectedControl != null)
            {
                selectedControl.RecieveMessage(new Message(MessageId.MouseLeave, e));
            }

            selectedControl = c;
        }
        #endregion

        #region Static Helper Functions
        public static void setControlPickColor(Control control)
        {
            int inc = 100;
            var temp = new Vector4(currentControlPickColor);

            controlColors.Add(temp, control);
            control.PickBufferColor = temp;

            currentControlPickColor.X += inc;

            if (currentControlPickColor.X > 255)
            {
                currentControlPickColor.X -= 255;
                currentControlPickColor.Y += inc;

                if (currentControlPickColor.Y > 255)
                {
                    currentControlPickColor.Y -= 255;
                    currentControlPickColor.Z += inc;

                    if (currentControlPickColor.Z > 255)
                    {
                        throw new Exception("Ran out of colors");
                    }
                }
            }

        }

        public static void setScisor(Rectangle rect, int border, GameWindow gw, Control Parent = null)
        {
            if(Parent == null)
                GL.Scissor(rect.X + border, gw.Height - rect.Y - rect.Height + border, rect.Width - (2*border), rect.Height - (2*border));
            else
            {
                Rectangle rect2 = Parent.GlobalBounds;
                int x = rect.X > rect2.X ? rect.X : rect2.X;
                int y = rect.Y > rect2.Y ? rect.Y : rect2.Y;

                int w;
                int h;
                int diff = (rect.X + rect.Width) - (rect2.X + rect2.Width);
                if (diff < 0)
                    w = rect.Width;
                else
                    w = rect.Width - diff;

                diff = (rect.Y + rect.Height) - (rect2.Y + rect2.Height);
                if (diff < 0)
                    h = rect.Height;
                else
                    h = rect.Height - diff;

                GL.Scissor(x + border, gw.Height - y - h - border, w - (2 * border), h - (2 * border));
            }
        }

        public static Color toColor(Vector4 color)
        {
            return Color.FromArgb((int)color.W * 255, (int)color.X * 255,
                    (int)color.Y * 255, (int)color.Z * 255);
        }

        public static Texture createTextureFromText(string text, Font font,Texture txt, Vector4 textColor, Vector4 textBackGroundColor, out Rectangle textBounds)
        {
            Size size;
            using (Bitmap bmp = new Bitmap(1, 1))
            {
                var temp = Graphics.FromImage(bmp).MeasureString(text, font);
                size = new Size((int)temp.Width, (int)temp.Height);
            }

            using (Bitmap bmp = new Bitmap((int)size.Width, (int)size.Height))
            {
                Graphics g = Graphics.FromImage(bmp);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                g.Clear(Color.Transparent);

                g.DrawString(text, font, new SolidBrush(toColor(textColor)), 0, 0);
                txt?.Dispose();
                txt = new Texture2d(bmp);
            }

            textBounds = new Rectangle(0, 0, size.Width, size.Height);
            return txt;
        }

        public static Rectangle centerTextBounds(Rectangle bounds, Rectangle textBounds)
        {
            int x = bounds.X + bounds.Width / 2 - textBounds.Width / 2;
            int y = bounds.Y + bounds.Height / 2 - textBounds.Height / 2;
            return new Rectangle(x, y, textBounds.Width, textBounds.Height);
        }
        #endregion

        #region Shaders

        #region Vertex
        private const string vertex = @"
#version 330

layout(location=0) in float vertNum;
layout(location=1) in vec2 uvIn;

uniform mat4 proj;

uniform ivec4 rect;

out vec2 uv;
out vec4 position;

void main()
{
    float x = rect.x;
    float y = rect.y;
    float w = rect.z;
    float h = rect.w;
    
    if(vertNum == 1)
        gl_Position = proj * vec4(x, y , 0, 1);
    else if(vertNum == 2)
        gl_Position = proj * vec4(x, y + h, 0, 1);
    else if(vertNum == 3)
        gl_Position = proj * vec4(x + w, y, 0, 1);
    else
        gl_Position = proj * vec4(x + w, y + h, 0, 1);

    uv = uvIn;
}
";
        #endregion

        #region Fragment
        private const string fragment = @"
#version 330

layout(origin_upper_left, pixel_center_integer) in vec4 gl_FragCoord;
in vec2 uv;

uniform ivec4 rect;

uniform sampler2D txt;
uniform bool useTexture;

uniform vec4 color;

uniform vec4 borderColor;
uniform int borderWidth;

out vec4 colorOut;

void main()
{
    vec4 fillColor;

    if(useTexture)
        fillColor = texture(txt, uv);
    else
        fillColor = color;

    if(borderWidth > 0 && 
        (gl_FragCoord.x < rect.x + borderWidth ||
        gl_FragCoord.x > rect.x + rect.z - borderWidth - 1 ||
        gl_FragCoord.y < rect.y + borderWidth ||
        gl_FragCoord.y > rect.y + rect.w - borderWidth - 1) )
            fillColor = borderColor;

    colorOut = fillColor;
}
";
        #endregion

        #endregion
    }
}
