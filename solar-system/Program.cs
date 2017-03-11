using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Collections.Generic;
using OpenGL;
using System.Diagnostics;
using OpenTK.Graphics;
using OpenTK.Input;

namespace SolarSystem
{
    class Program : GameWindow
    {
        /// <summary>
        /// Entry Point
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            using (GameWindow gw = new GameWindow())
            {
                //Console.WriteLine("extentions: ");
                //var extentions = GL.GetString(StringName.Extensions).Split(' ');
                //foreach(var extention in extentions)
                //    Console.WriteLine(extention);
                Console.WriteLine($"renderer: {GL.GetString(StringName.Renderer)}");
                Console.WriteLine($"vendor: {GL.GetString(StringName.Vendor)}");
                Console.WriteLine($"shading language version: {GL.GetString(StringName.ShadingLanguageVersion)}");
                Console.WriteLine($"version: {GL.GetString(StringName.Version)}");
            }

            new Program();
        }

        /// <summary>
        /// List of Scenes that will be rendered
        /// Each Scene will have its own plannets or potentially solary system
        /// </summary>
        private List<Scene> scenes;
        private Scene currentScene;

        public Program() : base(1600, 900, GraphicsMode.Default, "Solar System", GameWindowFlags.Default)
        {
            //settings
            this.VSync = VSyncMode.Off;

            //init and load content
            InitOpenGL();
            LoadContent();

            //run the window
            this.Run(60, 120);
        }

        #region Init/LoadContent
        private void InitOpenGL()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
        }

        private void LoadContent()
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();
            scenes = new List<Scene>();

            ContentManager contentManager = new ContentManager($@"{Environment.CurrentDirectory}\content\");
            contentManager.ShaderFolder = @"shaders\";
            contentManager.TextureFolder = @"textures\";
            contentManager.MeshFolder = @"models\";

            //loading the shaders
            ShaderProgram defaultShader = contentManager.LoadShader("default");
            defaultShader.Initialize = () =>
            {
                defaultShader.Bind();
                defaultShader.SetUniform("lightPos", new Vector3(0, 0, 0));
                defaultShader.SetUniform("ambientCoefficient", .01f);
                defaultShader.SetUniform("lightIntensities", Vector3.One);
            };
            defaultShader.Init();

            //s = defaultShader;
            ShaderProgram earthShader = contentManager.LoadShader("earth");
            earthShader.Initialize = () =>
            {
                earthShader.Bind();
                //earthShader.SetUniform("lightIntensities", Vector3.One);
                earthShader.SetUniform("texture", 0);
                earthShader.SetUniform("specularTexture", 1);
                earthShader.SetUniform("nightTexture", 2);
                //earthShader.SetUniform("normalTexture", 3);
                //earthShader.SetUniform("cloudsTexture", 4);
            };
            earthShader.Init();

            ShaderProgram sunShader = contentManager.LoadShader("sunShader");

            ShaderProgram lineShader = contentManager.LoadShader("lineShader");

            Console.WriteLine($"loaded shaders: {watch.ElapsedMilliseconds}");
            watch.Restart();

            //loading models
            PlanetParameters planetParams = PlanetParameters.readFromFile("content/planetInfo.txt");

            contentManager.LoadVao("saturnRings");
            contentManager.LoadVao("sphere");
            contentManager.LoadVao("uranusRings");

            Console.WriteLine($"loaded meshes: {watch.ElapsedMilliseconds}");
            watch.Restart();

            //loading the textures
            contentManager.LoadTexture("sun4096g.jpg", "sun");
            contentManager.LoadTexture("mercury4096.jpg", "mercury");
            contentManager.LoadTexture("venus4096.jpg", "venus");
            contentManager.LoadTexture("earth4096.jpg", "earth");
            contentManager.LoadTexture("earth_Spec4096.png", "earth_spec");
            contentManager.LoadTexture("earth_Night4096.jpg", "earth_night");
            contentManager.LoadTexture("earth_Normal4096.jpg", "earth_normal");
            contentManager.LoadTexture("earth_Clouds4096.jpg", "earth_clouds");
            contentManager.LoadTexture("moon4096.jpg", "moon");
            contentManager.LoadTexture("mars4096.jpg", "mars");
            contentManager.LoadTexture("jupiter4096.jpg", "jupiter");
            contentManager.LoadTexture("saturn2048.jpg", "saturn");
            contentManager.LoadTexture("uranus2048.png", "uranus");
            contentManager.LoadTexture("neptune1024.png", "neptune");
            contentManager.LoadTexture("pluto1024.jpg", "pluto");
            //contentManager.LoadTexture("saturnRings.png");
            //contentManager.LoadTexture("uranusRings.png");

            Console.WriteLine($"loaded textures: {watch.ElapsedMilliseconds}");
            watch.Restart();

            var solarSystemScene = new SolarSystemScene(this, planetParams, contentManager);
            //var planetSizeScene = new PlanetSIzeScene(context, planetParams, contentManager, 1);

            scenes.Add(solarSystemScene);
            // _scenes.Add(planetSizeScene);
            currentScene = scenes[0];

            Console.WriteLine($"created scenes: {watch.ElapsedMilliseconds}");
            watch.Stop();
        }
        #endregion


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            currentScene.Draw(this, e);

            this.SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            currentScene.Update(this, e);
        }

        #region Window Events

        protected override void OnResize(EventArgs e)
        {
            currentScene.WindowResized(this);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            currentScene.MouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            currentScene.MouseUp(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            currentScene.MouseWheel(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            currentScene.MouseMove(e);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            //if escape or alf+f4
            if(e.Key == OpenTK.Input.Key.Escape || (e.Key == Key.F4 && e.Modifiers == KeyModifiers.Alt))
                this.Exit();
            currentScene.KeyDown(e);
        }
        #endregion
    }
}
