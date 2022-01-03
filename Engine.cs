using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Reflection;
using System.Runtime;

namespace Paleon
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Engine : Game
    {
        public string Title { get; private set; }

        public static Engine Instance { get; private set; }
        public static GraphicsDeviceManager Graphics { get; private set; }

        public static CommandLine CommandLine { get; private set; }

        public static int Width { get; private set; }
        public static int Height { get; private set; }
        public static int HalfWidth { get; private set; }
        public static int HalfHeight { get; private set; }

        public static float GameDeltaTime { get; private set; }
        public static float DeltaTime { get; private set; }
        public static TimeSpan ElapsedTime { get; private set; }

        public static Color ClearColor;
        public static bool ExitOnEscapeKeypress;

        private Scene scene;
        private Scene nextScene;

        public static int GameSpeed { get; set; } = 1;

#if !CONSOLE
        private static string AssemblyDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
#endif

        public static string ContentDirectory
        {
#if PS4
            get { return Path.Combine("/app0/", Instance.Content.RootDirectory); }
#elif NSWITCH
            get { return Path.Combine("rom:/", Instance.Content.RootDirectory); }
#elif XBOXONE
            get { return Instance.Content.RootDirectory; }
#else
            get { return Path.Combine(AssemblyDirectory, Instance.Content.RootDirectory); }
#endif
        }

        public Engine(int width, int height, string windowTitle, bool fullscreen)
        {
            Instance = this;

            Title = Window.Title = windowTitle;
            Width = width;
            Height = height;
            HalfWidth = Width / 2;
            HalfHeight = Height / 2;
            ClearColor = Color.Black;

            Graphics = new GraphicsDeviceManager(this);

#if PS4 || XBOXONE
            Width = 1920;
            Height = 1080;
#elif NSWITCH
            Width = 1280;
            Height = 720;
#else
            if (fullscreen)
            {
                Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                HalfWidth = Width / 2;
                HalfHeight = Height / 2;
                Graphics.IsFullScreen = true;
            }
            else
            {
                Width = width;
                Height = height;
                Graphics.IsFullScreen = false;
            }
#endif
            Graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            Graphics.PreferredBackBufferWidth = Width;
            Graphics.PreferredBackBufferHeight = Height;
            Graphics.ApplyChanges();

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnClientSizeChanged;

            Content.RootDirectory = @"Content";

            IsMouseVisible = false;
            ExitOnEscapeKeypress = true;

            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            MInput.Initialize();
            CommandLine = new CommandLine();

            Scene = new GameplayScene();
        }


#if !CONSOLE
        protected virtual void OnClientSizeChanged(object sender, EventArgs e)
        {
            if (Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0)
            {
                Graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                Graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;

                Width = Window.ClientBounds.Width;
                Height = Window.ClientBounds.Height;
            }
        }
#endif

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            RenderManager.Initialize(GraphicsDevice);

            ResourceManager.Initialize(Content);

            TextureBank.Initialize();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            GameDeltaTime = DeltaTime * GameSpeed;
            ElapsedTime = gameTime.ElapsedGameTime;

            //Update input
            MInput.Update();

            if (ExitOnEscapeKeypress && MInput.Keyboard.Pressed(Keys.Escape))
            {
                Exit();
                return;
            }

            //Debug Console
            if (CommandLine.Open)
                CommandLine.UpdateOpen();
            else if (CommandLine.Enabled)
                CommandLine.UpdateClosed();

            if (scene != null)
            {
                scene.UpdateLists();
                scene.Begin();
                scene.Update();
            }

            if (scene != nextScene)
            {
                var lastScene = scene;
                scene = nextScene;
                OnSceneTransition(lastScene, nextScene);
                if (scene != null)
                {
                    scene.UpdateLists();
                    scene.Begin();
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(ClearColor);

            if (scene != null)
                scene.Render();

            base.Draw(gameTime);

            if (CommandLine.Open)
                CommandLine.Render();
        }

        public static Scene Scene
        {
            get { return Instance.scene; }
            set { Instance.nextScene = value; }
        }

        protected virtual void OnSceneTransition(Scene from, Scene to)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static void SetWindowed(int width, int height)
        {
            if (width > 0 && height > 0)
            {
                Graphics.PreferredBackBufferWidth = width;
                Graphics.PreferredBackBufferHeight = height;
                Graphics.IsFullScreen = false;
                Graphics.ApplyChanges();
                Console.WriteLine("WINDOW-" + width + "x" + height);
            }
        }

        public static void SetFullscreen()
        {
            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Graphics.IsFullScreen = true;
            Graphics.ApplyChanges();
            Console.WriteLine("FULLSCREEN");
        }
    }
}
