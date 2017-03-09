using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem
{
    interface Scene
    {

        void Draw(GameWindow gw);

        void Update(GameWindow gw, double delta);

        void MouseDown(int x, int y);

        void MouseUp();

        void MouseWheel(int delta);

        void MouseMove(int x, int y);

        void KeyDown(KeyboardKeyEventArgs e);

        void WindowResized(int width, int height);

        void setFocus(int index);

        void setHoursPerSecond(double value);
    }
}
