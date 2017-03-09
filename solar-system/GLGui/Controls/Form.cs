using OpenGL;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL.Extentions;
using GLGui.Interfaces;
using OpenTK.Input;

namespace GLGui.Controls
{
    public class Form : Control, IUpdate, IDrawable
    {
        private bool dragging;

        private Point lastPos;       

        public Rectangle textBounds
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Font Font
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Texture textImage
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string Text
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        private class CloseButton : Button
        {
            public CloseButton()
            {
                Size = new Size(30, 30);
                Location = new Point(this.Location.X + this.Size.Width - 30, this.Location.Y);
                BackgroundColor = new Vector4(1, 1, 1, 1);
                BorderWidth = 2;
                BorderColor = new Vector4(51 / 255f, 51 / 255f, 51 / 255f, 1);
                Text = "X";
            }

            protected override void OnMouseEnter(MouseMoveEventArgs data)
            {
                BackgroundColor = new Vector4(1, .3f, .3f,1);
            }

            protected override void OnMouseLeave(MouseMoveEventArgs data)
            {
                BackgroundColor = new Vector4(1, 1, 1, 1);
            }
        }

        public Form()
        {
            Location = new Point(0, 0);
            Size = new Size(400, 400);
            BackgroundColor = new Vector4(1f, 1f, 1f, 1f);

            BorderWidth = 2;
            BorderColor = new Vector4(51 / 255f, 51 / 255f, 51 / 255f, 1);

            var p = new Panel()
            {
                //BorderColor = new Vector4(51 / 255f, 51 / 255f, 51 / 255f, 1),
                //Border = true,
                //BorderWidth = 2,
                //Size = new Size(this.Size.Width, 30),
            };
            p.MouseDown += (o, e) => { dragging = true; lastPos = e.Position; };
            p.MouseUp += (o, e) => { dragging = false; };

            var b = new CloseButton();
            b.Location = new Point(Location.X + Size.Width - b.Size.Width, Location.Y);

            p.Controls.Add(b);

            Controls.Add(p);
        }

        public void Update(GameWindow gw)
        {
            var e = gw.Mouse;

            if (e.X < 0 || e.X > gw.Width || e.Y < 0 || e.Y > gw.Height)
            {
                dragging = false;
                return;
            }

            Point delta = new Point(e.X - lastPos.X, e.Y - lastPos.Y);
            if (dragging)
            {
                //Location = new Point(
                //    (Location.X + delta.X).Clamp(0, gw.Width - Size.Width),
                //    (Location.Y + delta.Y).Clamp(0, gw.Height - Size.Height));

                Location = new Point(
                    (Location.X + delta.X),
                    (Location.Y + delta.Y));
            }

            lastPos = new Point(e.X, e.Y);


            for (int i = 0; i < Controls.Count; i++)
            {
                var c = Controls[i] as IUpdate;
                c?.Update(gw);
            }
        }
    }
}
