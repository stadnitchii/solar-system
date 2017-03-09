//using System;
//using System.Collections.Generic;
//using OpenTK.Input;
//using Solar_System.OpenGL;
//using OpenTK;
//using OpenTK.Graphics.OpenGL;

//namespace Solar_System
//{
//    class PlanetSIzeScene : Scene
//    {
//        private GraphicsContext context;

//        private Camera cam;
//        private List<Planet> planets;
//        private List<PlanetRing> rings;
//        private SkyBox skybox;
//        private double hoursPerSecond;
//        private bool ZAxisLine;

//        private ShaderProgram defaultShader;
//        private ShaderProgram sunShader;
//        private ShaderProgram lineShader;
//        private ShaderProgram earthShader;

//        private int currentFocus = 0;

//        //this one is special
//        private Sun sun;

//        public PlanetSIzeScene(GraphicsContext graphicsContext, PlanetParameters planetParams, ContentManager contentManager, double hoursPerSecond)
//        {
//            this.context = graphicsContext;
//            this.hoursPerSecond = hoursPerSecond;
//            this.defaultShader = contentManager.getShader("default");
//            this.sunShader = contentManager.getShader("sunShader");
//            this.lineShader = contentManager.getShader("lineShader");
//            this.earthShader = contentManager.getShader("earth");

//            //sets the projection for the shaders, this way we dont have to do it each time we render the frame
//            this.setProjectionMatrix();

//            this.loadContent(planetParams, contentManager);
//        }

//        private void loadContent(PlanetParameters planetParams, ContentManager content)
//        {
//            this.planets = new List<Planet>();
//            this.rings = new List<PlanetRing>();

//            ModelVBOs planetVBOs = content.GetVao("sphere");

//            skybox = new SkyBox(context, content.GetVao("cube"), content.getTexture("skybox"), content.getShader("skybox"));

//            Planet mercury = new Planet("mercury", planetParams, null, planetVBOs, content.getTexture("mercury4096.jpg"), defaultShader, lineShader);
//            mercury.setPosition(0, 0, 0);
//            mercury.setSize(0);

//            double total = getSeparation(planetParams, "mercury", "venus");
//            Planet venus = new Planet("venus", planetParams, null, planetVBOs, content.getTexture("venus4096.jpg"), defaultShader, lineShader);
//            venus.setPosition(total, 0, 0);
//            venus.setSize(0);

//            total += getSeparation(planetParams, "venus", "earth");
//            Earth earth = new Earth(context, "earth", planetParams, null, planetVBOs, content.getTexture("earth4096.jpg"), content.getTexture("earth_Spec4096.png"),
//               content.getTexture("earth_Night4096.jpg"), content.getTexture("earth_Normal4096.jpg"), content.getTexture("earth_Clouds4096.jpg"), earthShader, lineShader);
//            earth.setPosition(total, 0, 0);
//            earth.setSize(0);

//            total += getSeparation(planetParams, "moon", "earth");
//            Planet moon = new Planet("moon", planetParams, null, planetVBOs, content.getTexture("moon4096.jpg"), defaultShader, lineShader);
//            moon.setPosition(total, 0, 0);
//            moon.setSize(0);

//            total += getSeparation(planetParams, "mars", "moon");
//            Planet mars = new Planet("mars", planetParams, null, planetVBOs, content.getTexture("mars4096.jpg"), defaultShader, lineShader);
//            mars.setPosition(total, 0, 0);
//            mars.setSize(0);

//            total += getSeparation(planetParams, "mars", "jupiter");
//            Planet jupiter = new Planet("jupiter", planetParams, null, planetVBOs, content.getTexture("jupiter4096.jpg"), defaultShader, lineShader);
//            jupiter.setPosition(total, 0, 0);
//            jupiter.setSize(0);

//            total += getSeparation(planetParams, "saturn", "jupiter");
//            Planet saturn = new Planet("saturn", planetParams, null, planetVBOs, content.getTexture("saturn2048.jpg"), defaultShader, lineShader);
//            saturn.setPosition(total, 0, 0);
//            saturn.setSize(0);

//            total += getSeparation(planetParams, "saturn", "uranus");
//            Planet uranus = new Planet("uranus", planetParams, null, planetVBOs, content.getTexture("uranus2048.png"), defaultShader, lineShader);
//            uranus.setPosition(total, 0, 0);
//            uranus.setSize(0);

//            total += getSeparation(planetParams, "uranus", "neptune");
//            Planet neptune = new Planet("neptune", planetParams, null, planetVBOs, content.getTexture("neptune1024.png"), defaultShader, lineShader);
//            neptune.setPosition(total, 0, 0);
//            neptune.setSize(0);

//            total += getSeparation(planetParams, "pluto", "neptune");
//            Planet pluto = new Planet("pluto", planetParams, null, planetVBOs, content.getTexture("pluto1024.jpg"), defaultShader, lineShader);
//            pluto.setPosition(total, 0, 0);
//            pluto.setSize(0);

//            total += getSeparation(planetParams, "pluto", "sun");
//            sun = new Sun(context, "sun", planetParams, null, planetVBOs, content.getTexture("sun4096g.jpg"), sunShader, lineShader);
//            sun.setPosition(total, 0, 0);
//            sun.setSize(0);

//            PlanetRing saturnRings = new PlanetRing(content.GetVao("saturnRings"), content.getTexture("saturnRings.png"), saturn);
//            PlanetRing uranusRings = new PlanetRing(content.GetVao("uranusRings"), content.getTexture("uranusRings.png"), uranus);

//            planets.Add(mercury);
//            planets.Add(venus);
//            planets.Add(earth);
//            planets.Add(moon);
//            planets.Add(mars);
//            planets.Add(jupiter);
//            planets.Add(saturn);
//            planets.Add(uranus);
//            planets.Add(neptune);
//            planets.Add(pluto);

//            //rings.Add(saturnRings);
//            //rings.Add(uranusRings);

//            cam = new Camera();
//            cam.setFocus(mercury);

//            foreach (Planet p in planets)
//            {
//                p.DrawOrbit = false;
//                p.DrawAxisTilt = false;
//                sun.DrawAxisTilt = false;
//            }
//        }

//        public void Draw()
//        {
//            Matrix4 view = cam.Transfrom;
//            GL.MatrixMode(MatrixMode.Modelview);
//            GL.LoadMatrix(ref view);

//            defaultShader.Use();
//            defaultShader.setUniform("lightPos", new Vector3(0, 0,1000));
//            defaultShader.setUniform("view", view);

//            earthShader.Use();
//            earthShader.setUniform("view", view);
//            earthShader.setUniform("lightPos", new Vector3(0, 0, 1000));
//            earthShader.setUniform("eyePos", cam.Position);

//            sunShader.Use();
//            sunShader.setUniform("view", view);
//            lineShader.Use();
//            lineShader.setUniform("view", view);

//            //textured items
//            foreach (Planet p in planets)
//                p.draw(view);

//            //transparent items
//            //foreach (PlanetRing r in rings)
//            //    r.draw(cam.Transfrom, defaultShader);

//            skybox.draw(cam.Transfrom);

//            sun.draw(view);
//        }

//        public void update(double delta)
//        {
//            foreach (Planet p in planets)
//                p.update(delta, hoursPerSecond);
//            cam.update();
//        }

//        public void setProjectionMatrix()
//        {
//            //set ortho
//            Matrix4 perspecitve = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(70), context.Dimentions.X / context.Dimentions.Y, .1f, 1000000f);
//            var derp = context.ProjectionMatrix;
//            if (defaultShader != null)
//            {
//                defaultShader.Use();
//                defaultShader.setUniform("proj", perspecitve);
//            }
//            if (sunShader != null)
//            {
//                sunShader.Use();
//                sunShader.setUniform("proj", perspecitve);
//            }
//            if (lineShader != null)
//            {
//                lineShader.Use();
//                lineShader.setUniform("proj", perspecitve);
//            }
//            if (earthShader != null)
//            {
//                earthShader.Use();
//                earthShader.setUniform("proj", perspecitve);
//            }
//        }

//        private double getSeparation(PlanetParameters p, string planet, string planet2)
//        {
//            return (p.RadiusRealistic[planet] * 10 + p.RadiusRealistic[planet2] * 10) * 1.2;
//        }

//        #region Input
//        public void keyDown(KeyboardKeyEventArgs e)
//        {
//            if (e.Key == Key.Right)
//                currentFocus++;
//            if (e.Key == Key.Left)
//                currentFocus--;
//            if (currentFocus < 0) currentFocus = 0;
//            if (currentFocus > 9) currentFocus = 9;

//            cam.setFocus(planets[currentFocus]);

//            if (e.Key == Key.S)
//                cam.setFocus(sun);
//        }

//        public void mouseDown(int x, int y)
//        {
//            cam.mouseDown(x, y);
//        }

//        public void mouseUp()
//        {
//            cam.mouseUp();
//        }

//        public void mouseWheel(int delta)
//        {
//            cam.mouseWheel(delta);
//        }

//        public void mouseMove(int x, int y)
//        {
//            cam.mouseMove(x, y);
//        }

//        public void setFocus(int index)
//        {
//            //throw new NotImplementedException();
//        }

//        public void setHoursPerSecond(double value)
//        {
//            //throw new NotImplementedException();
//        }
//        #endregion
//    }
//}
