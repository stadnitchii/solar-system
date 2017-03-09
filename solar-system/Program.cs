using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Collections.Generic;
using OpenGL;
using System.Diagnostics;
using OpenTK.Graphics;

namespace SolarSystem
{
    public class GraphicsContext
    {
        public Vector2 Dimentions { get; set; }
        public Matrix4 ProjectionMatrix { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            new Program();
        }

        static void GetCapabilities()
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
        }

        /// <summary>
        /// The window that we will render stuff to
        /// </summary>
        private readonly GameWindow _window;

        /// <summary>
        /// List of Scenes that will be rendered
        /// Each Scene will have its own plannets or potentially solary system
        /// </summary>
        private List<Scene> _scenes;
        private Scene _currentScene;

        public Program()
        {
            GetCapabilities();
            _window = new GameWindow(1600, 900, GraphicsMode.Default, "Solar System", GameWindowFlags.Default);

            //settings
            _window.VSync = VSyncMode.Off;

            //window events
            _window.Resize += Window_Resize;
            _window.MouseDown += Window_MouseDown;
            _window.MouseUp += Window_MouseUp;
            _window.MouseMove += Window_MouseMove;
            _window.MouseWheel += Window_MouseWheel;
            _window.KeyDown += Window_KeyDown;

            _window.RenderFrame += Window_RenderFrame;
            _window.UpdateFrame += Window_UpdateFrame;

            //init and load content
            InitOpenGL();
            LoadContent();

            //run the window
            _window.Run(60, 120);
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
            _scenes = new List<Scene>();

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

            var solarSystemScene = new SolarSystemScene(_window, planetParams, contentManager);
            //var planetSizeScene = new PlanetSIzeScene(context, planetParams, contentManager, 1);

            _scenes.Add(solarSystemScene);
           // _scenes.Add(planetSizeScene);
            _currentScene = _scenes[0];

            Console.WriteLine("created scenes: {0}", watch.ElapsedMilliseconds);
            watch.Restart();
        }
        #endregion

        private void Window_RenderFrame(object sender, FrameEventArgs e)
        {
            GL.ClearColor(0f, 0f, 0f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _currentScene.Draw(_window);

            _window.SwapBuffers();
        }

        private void Window_UpdateFrame(object sender, FrameEventArgs e)
        {
            _currentScene.Update(_window, e.Time);
        }

        #region Window Events
        private void Window_Resize(object sender, EventArgs e)
        {
            _currentScene.WindowResized(_window.Width, _window.Height);
        }

        private void Window_MouseDown(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            _currentScene.MouseDown(e.X, e.Y);
        }

        private void Window_MouseUp(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            _currentScene.MouseUp();
        }

        private void Window_MouseWheel(object sender, OpenTK.Input.MouseWheelEventArgs e)
        {
            _currentScene.MouseWheel(e.Delta);
        }

        private void Window_MouseMove(object sender, OpenTK.Input.MouseMoveEventArgs e)
        {
            _currentScene.MouseMove(e.X, e.Y);
        }

        private void Window_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (e.Key == OpenTK.Input.Key.Escape)
                _window.Exit();
            _currentScene.KeyDown(e);
        }
        #endregion
    }
}
