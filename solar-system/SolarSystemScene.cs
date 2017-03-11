using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenGL;
using System.Collections.Generic;
using GLGui.Controls;
using OpenTK.Input;
using GLGui;
using System;

namespace SolarSystem
{
    class SolarSystemScene : Scene
    {
        ContentManager contentManager;
        Camera cam;
        List<Planet> planets;
        List<PlanetRing> rings;
        SkyBox skybox;
        double hoursPerSecond;

        ShaderProgram defaultShader;
        ShaderProgram sunShader;
        ShaderProgram lineShader;
        ShaderProgram earthShader;

        //these ones are special
        Sun sun;
        Earth earth;

        Framebuffer buffer;
        FrameRenderer frame;
        GaussianBlur blur;
        ShaderProgram finalShader;

        GuiManager guiManager;
        Panel panel;

        bool showBloomBuffer = false;
        bool bloom = true;

        public SolarSystemScene(GameWindow gw, PlanetParameters planetParams, ContentManager contentManager)
        {
            this.contentManager = contentManager;
            this.hoursPerSecond = 1;
            this.defaultShader = contentManager.GetShader("default");
            this.sunShader = contentManager.GetShader("sunShader");
            this.lineShader = contentManager.GetShader("lineShader");
            this.earthShader = contentManager.GetShader("earth");

            this.buffer = new Framebuffer(gw.Width, gw.Height);
            buffer.AttachColorBuffer(internalFormat: PixelInternalFormat.Rgba16f, type: PixelType.Float);
            buffer.AttachColorBuffer(internalFormat: PixelInternalFormat.Rgba16f, type: PixelType.Float);
            buffer.AttachDepthStencilBuffer();

            blur = new GaussianBlur((int)gw.Size.Width / 1, (int)gw.Size.Height / 1);

            frame = FrameRenderer.Instance;

            finalShader = contentManager.LoadShader("final");
            finalShader.Initialize = () =>
           {
               finalShader.Bind();
               finalShader.SetUniform("frame", 0);
               finalShader.SetUniform("bloom", 1);
           };
            finalShader.Init();

            cam = new Camera(60);
            cam.MaxDepth = 600;
            cam.LightPos = new Vector3(0, 0, 0);
            cam.Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70), gw.Width / (float)gw.Height, .1f, 100000f);

            this.loadContent(planetParams, contentManager);

            cam.SetFocus(earth);

            #region Gui
            guiManager = new GuiManager(gw);
            panel = new Panel(){
                Size = new System.Drawing.Size(200, gw.Height),
                BackgroundColor = new Vector4(36f / 255, 36f / 255, 36f / 255, 1),
                BorderColor = new Vector4(1, 1, 1, 1),
                Location = new System.Drawing.Point(-201, 0),
            };

            var titleLabel = new Label(){          
                Size = new System.Drawing.Size(200, 50),
                Location = new System.Drawing.Point(0, 10),
                Font = new System.Drawing.Font("Arial", 20, System.Drawing.FontStyle.Underline),
                TextColor = new Vector4(1, 1, 1, 1),
                Text = "Options"
            };

            int y = 70;
            var label = new Label() {
                Size = new System.Drawing.Size(120, 25),
                Location = new System.Drawing.Point(10, y),
                TextColor = new Vector4(1, 1, 1, 1),
                Text = "Axial Tilt"
            };
            var @switch = new Switch() { Location = new System.Drawing.Point(130, y), On = true };
            @switch.OnToggle += (o, e) =>
            {
                this.setAxialTiltDraw(((Switch)o).On);
            };
            panel.Controls.Add(titleLabel, label, @switch);

            y += 30;
            label = new Label() {
                Size = new System.Drawing.Size(120, 25),
                Location = new System.Drawing.Point(10, y),
                TextColor = new Vector4(1, 1, 1, 1),
                Text = "Orbits"
            };
            @switch = new Switch() { Location = new System.Drawing.Point(130, y), On = true };
            @switch.OnToggle += (o, e) =>
            {
                this.setOrbitDraw(((Switch)o).On);
            };
            panel.Controls.Add(label, @switch);

            y += 30;
            label = new Label() {
                Size = new System.Drawing.Size(120, 25),
                Location = new System.Drawing.Point(10, y),
                TextColor = new Vector4(1, 1, 1, 1),
                Text = "Bloom Buffer"
            };
            @switch = new Switch() { Location = new System.Drawing.Point(130, y) };
            @switch.OnToggle += (o, e) =>
            {
                this.showBloomBuffer = ((Switch)o).On;
            };
            panel.Controls.Add(label, @switch);

            y += 30;
            label = new Label() {
                Size = new System.Drawing.Size(120, 25),
                Location = new System.Drawing.Point(10, y),
                TextColor = new Vector4(1, 1, 1, 1),
                Text = "Bloom"
            };
            @switch = new Switch() { Location = new System.Drawing.Point(130, y), On = true };
            @switch.OnToggle += (o, e) =>
            {
                this.bloom = ((Switch)o).On;
            };
            panel.Controls.Add(label, @switch);

            guiManager.Controls.Add(panel);
            #endregion
        }

        private void loadContent(PlanetParameters planetParams, ContentManager content)
        {
            this.planets = new List<Planet>();
            this.rings = new List<PlanetRing>();

            VAO planetVao = content.GetVao("sphere");

            skybox = new SkyBox("content/textures/skybox");

            sun = new Sun("sun", planetParams, null, planetVao, content.getTexture("sun"));
            Planet mercury = new Planet("mercury", planetParams, sun, planetVao, content.getTexture("mercury"));
            Planet venus = new Planet("venus", planetParams, sun, planetVao, content.getTexture("venus"));
            earth = new Earth("earth", planetParams, sun, planetVao, content.getTexture("earth"), content.getTexture("earth_spec"),
                content.getTexture("earth_night"), content.getTexture("earth_normal"), content.getTexture("earth_clouds"));
            Planet moon = new Planet("moon", planetParams, earth, planetVao, content.getTexture("moon"));
            Planet mars = new Planet("mars", planetParams, sun, planetVao, content.getTexture("mars"));
            Planet jupiter = new Planet("jupiter", planetParams, sun, planetVao, content.getTexture("jupiter"));
            Planet saturn = new Planet("saturn", planetParams, sun, planetVao, content.getTexture("saturn"));
            Planet uranus = new Planet("uranus", planetParams, sun, planetVao, content.getTexture("uranus"));
            Planet neptune = new Planet("neptune", planetParams, sun, planetVao, content.getTexture("neptune"));
            Planet pluto = new Planet("pluto", planetParams, sun, planetVao, content.getTexture("pluto"));

            //PlanetRing saturnRings = new PlanetRing(content.GetVao("saturnRings"), content.getTexture("saturnRings.png"), saturn);
            //PlanetRing uranusRings = new PlanetRing(content.GetVao("uranusRings"), content.getTexture("uranusRings.png"), uranus);

            planets.Add(mercury);
            planets.Add(venus);
            planets.Add(moon);
            planets.Add(mars);
            planets.Add(jupiter);
            planets.Add(saturn);
            planets.Add(uranus);
            planets.Add(neptune);
            planets.Add(pluto);

            //rings.Add(saturnRings);
            //rings.Add(uranusRings);
        }

        public void Draw(GameWindow gw, FrameEventArgs e)
        {
            //----------------------------setting up shaders-------------
            #region ShaderUniforms
            defaultShader.Bind();
            //defaultShader.SetUniform("bloom", false);
            defaultShader.SetUniform("proj", cam.Projection);
            defaultShader.SetUniform("view", cam.Transfrom);
            defaultShader.SetUniform("lightPos", cam.LightPos);

            earthShader.Bind();
            earthShader.SetUniform("view", cam.Transfrom);
            earthShader.SetUniform("proj", cam.Projection);
            earthShader.SetUniform("eyePos", cam.Position);
            earthShader.SetUniform("lightPos", cam.LightPos);

            sunShader.Bind();
            sunShader.SetUniform("view", cam.Transfrom);
            sunShader.SetUniform("proj", cam.Projection);

            lineShader.Bind();
            lineShader.SetUniform("view", cam.Transfrom);
            lineShader.SetUniform("proj", cam.Projection);
            #endregion

            //textured items
            buffer.Bind();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            skybox.draw(cam);

            foreach (Planet p in planets)
                p.Draw(cam, defaultShader, lineShader);

            sun.Draw(cam, sunShader);
            earth.Draw(cam, earthShader, lineShader);

            //transparent items
            //foreach (PlanetRing r in rings)
            //    r.draw(cam.Transfrom, defaultShader);

            int blurrPasses = bloom ? 5 : 0;
            var blurredTexture = blur.Blur(buffer.Textures[1], blurrPasses);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, gw.Width, gw.Height);

            //frame.Draw(Area.TopLeft, buffer.Textures[0]);
            //frame.Draw(Area.TopRight, buffer.Textures[1]);

            if (showBloomBuffer)
                frame.Draw(Area.BottomRight, blurredTexture);

            finalShader.Bind();
            finalShader.SetUniform("useBloom", bloom);
            frame.Draw(Area.Full, buffer.Textures[0], blurredTexture, finalShader);

            guiManager.Draw(gw);
        }

        public void Update(GameWindow gw, FrameEventArgs e)
        {
            if (gw.Mouse.X < 200 && panel.Location.X < 0)
                panel.Location = new System.Drawing.Point(0, 0);
            else if (gw.Mouse.X > 200 && panel.Location.X == 0)
                panel.Location = new System.Drawing.Point(-201, 0);

            foreach (Planet p in planets)
                p.Update(e.Time, hoursPerSecond);
            earth.Update(e.Time, hoursPerSecond);

            cam.update();
            guiManager.Update(gw);
        }

        public void MouseDown(MouseButtonEventArgs e)
        {
            cam.mouseDown(e.X, e.Y);
            guiManager.MouseDown(e);
        }

        public void MouseUp(MouseButtonEventArgs e)
        {
            cam.mouseUp();
            guiManager.MouseUp(e);
        }

        public void MouseWheel(MouseWheelEventArgs e)
        {
            cam.mouseWheel(e.Delta);
            guiManager.MouseWheel(e);
        }

        public void MouseMove(MouseMoveEventArgs e)
        {
            cam.mouseMove(e.X, e.Y);
            guiManager.MouseMove(e);
        }

        public void WindowResized(GameWindow gw)
        {
            cam.Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70), gw.Width / (float)gw.Height, .1f, 100000f);
            GL.Viewport(0, 0, gw.Width, gw.Height);

            guiManager.WindowResized(gw);
        }

        public void KeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            //cam.keyDown(e);
            int index = -1;

            if (e.Key == OpenTK.Input.Key.Number1)
                index = 0;
            if (e.Key == OpenTK.Input.Key.Number2)
                index = 1;
            if (e.Key == OpenTK.Input.Key.Number3)
                index = 2;
            if (e.Key == OpenTK.Input.Key.Number4)
                index = 3;
            if (e.Key == OpenTK.Input.Key.Number5)
                index = 4;
            if (e.Key == OpenTK.Input.Key.Number6)
                index = 5;
            if (e.Key == OpenTK.Input.Key.Number7)
                index = 6;
            if (e.Key == OpenTK.Input.Key.Number8)
                index = 7;
            if (e.Key == OpenTK.Input.Key.Number9)
                index = 8;
            if (e.Key == OpenTK.Input.Key.Number0)
                index = 9;

            if (index != -1)
            {
                if (index == 9)
                    cam.SetFocus(sun);
                else
                    cam.SetFocus(planets[index]);
            }

            if (e.Key == Key.R)
            {
                var result = contentManager.ReloadShader("earth");
                System.Console.WriteLine(result);
                //contentManager.ReloadShader("final");
                result = contentManager.ReloadShader("sunShader");
                System.Console.WriteLine(result);
            }

            if (e.Key == Key.C)
            {
                System.Console.WriteLine(lineShader.Reload());
            }

            //if (e.KeyCode == Keys.S)
            //    cam.setFocus(sun);
        }

        private void setAxialTiltDraw(bool value)
        {
            foreach (Planet p in planets)
                p.DrawAxisTilt = value;

            earth.DrawAxisTilt = value;
        }

        private void setOrbitDraw(bool value)
        {
            foreach (Planet p in planets)
                p.DrawOrbit = value;

            earth.DrawOrbit = value;
        }

        private void setPlanetSize(double value)
        {
            //foreach (Planet p in planets)
            //    p.setSize(value);
        }

        private void setHoursPerSecond(double value)
        {
            this.hoursPerSecond = value;
        }
    }
}
