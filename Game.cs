using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePort
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static Game Instance;
        private bool flag;
        public static Matrix matrix;
        public const int ScrollSpeed = 10;
        bool init;
        public static Main Main
        {
            get { return Main.Instance; }
        }

        public static SpriteFont[] Font = new SpriteFont[1];
        public Game()
        {
            Main.Instance = new Main();
            Instance = this;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            matrix = new Matrix();
            _graphics.GraphicsDevice.Viewport = new Viewport(0, 0, 800, 600);
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;

            _graphics.ApplyChanges();

            Dialog.CoinColor = new Color[] 
            {
                Color.FromNonPremultiplied(new Vector4(0.722f, 0.451f, 0.20f, 1f)),
                Color.FromNonPremultiplied(new Vector4(0.804f, 0.498f, 0.196f, 1f)),
                Color.Silver,                                                          
                Color.Gold,
                Color.FromNonPremultiplied(new Vector4(0.898f, 0.894f, 0.886f, 1f))
            };

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Font[FontID.Arial] = this.Content.Load<SpriteFont>("Arial");
            Main.MagicPixel = this.Content.Load<Texture2D>("MagicPixel");
            Main.Temporal = this.Content.Load<Texture2D>("temp");
            Background.BGs[BackgroundID.Tiles] = this.Content.Load<Texture2D>("Alpha Tiles Scratches");
            Background.BGs[BackgroundID.Temp] = this.Content.Load<Texture2D>("temp_bg");
            Background.BGs[BackgroundID.SmallTiles] = this.Content.Load<Texture2D>("background2");
            Main.NPCTexture[NPCID.Necrosis + 1] = this.Content.Load<Texture2D>("Necrosis");
            Main.NPCTexture[NPCID.Kobold + 1] = this.Content.Load<Texture2D>("temp");
            Main.ProjTexture[ProjectileID.Orb] = this.Content.Load<Texture2D>("Orb");
            Light.fow = this.Content.Load<Texture2D>("fow");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Main.SavePlayer();
                Level.Save(Level.floorNumber);
                Exit();
            }

            // TODO: Add your update logic here
            if (!init && Main.LocalPlayer != null)
            {
                Main.LoadPlayer();
                init = true;
            }
            Main.ScreenWidth = _graphics.PreferredBackBufferWidth;
            Main.ScreenHeight = _graphics.PreferredBackBufferHeight;

            Main.rand?.Seed();
            Main.Updater();
            if (Main.LocalPlayer != null)
            {
                var offset = new Vector3(Main.ScreenWidth / 2, Main.ScreenHeight / 2, 0);
                var camera = new Vector3(-Main.LocalPlayer.position.X - Player.plrWidth / 2, -Main.LocalPlayer.position.Y - Player.plrHeight / 2, 0);
                matrix = Matrix.CreateTranslation(camera + offset + (Main.IsZoomed ? new Vector3(Main.ScreenWidth * 0.5f - Main.MapX * ScrollSpeed, Main.ScreenHeight * 0.5f - Main.MapY * ScrollSpeed, 0) : Vector3.Zero)) * Matrix.CreateScale(Main.IsZoomed ? 0.5f : 1f);
            }
            else matrix = Matrix.CreateTranslation(0, 0, 0);
            if (Main.KeyDown(Keys.Space))
                flag = true;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, matrix);
            if (Main.Logo) Main?.LogoDisplay(_spriteBatch);
            if (flag)
            {
                Main.MainDraw(_spriteBatch);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
