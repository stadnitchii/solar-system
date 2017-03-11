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

        void Draw(GameWindow gw, FrameEventArgs e);

        void Update(GameWindow gw, FrameEventArgs e);

        void MouseDown(MouseButtonEventArgs e);

        void MouseUp(MouseButtonEventArgs e);

        void MouseWheel(MouseWheelEventArgs e);

        void MouseMove(MouseMoveEventArgs e);

        void KeyDown(KeyboardKeyEventArgs e);

        void WindowResized(GameWindow gw);
    }
}
