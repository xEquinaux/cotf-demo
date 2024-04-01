using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Numerics;
using System.Threading;                      
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using FoundationR;
using Microsoft.Win32;
using System.Windows.Controls;

namespace cotf_rewd
{
    internal class Program
    {
        static int StartX => 0;
        static int StartY => 0;
        internal static int Width => 800;
        internal static int Height => 600;
        static int BitsPerPixel => 32;
        static string Title = "Castle of the Flame";
        [STAThread]
        static void Main(string[] args)
        {
            Game m = null;
            Thread t = new Thread(() => { (m = new Game()).Run(SurfaceType.WindowHandle_Loop, new FoundationR.Surface(StartX, StartY, Width, Height, Title, BitsPerPixel)); });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            while (Console.ReadLine() != "exit");
            t.Abort();
            Environment.Exit(0);
        }
    }
    public class Game : Foundation
    {
        public static Game Instance;
        public static REW[] Textures = new REW[10];
        public static Matrix matrix;
        public static Main Main => Main.Instance;
        public static Point MousePosition;
        public const int ScrollSpeed = 10;
        public const int FontSize = 12;
        public static string[] Font = new string[4];
        public static string RootDirectory = ".\\Content";
        bool flag;
        bool init;

        internal Game()
        {
            Instance = this;
            Main.Instance = new Main();
        }
        
        public override void RegisterHooks()
        {
            Foundation.UpdateEvent += Update;
            Foundation.ResizeEvent += Resize;
            Foundation.InputEvent += Input;
            Foundation.DrawEvent += Draw;
            Foundation.InitializeEvent += Initialize;
            Foundation.LoadResourcesEvent += LoadResources;
            Foundation.MainMenuEvent += MainMenu;
            Foundation.PreDrawEvent += PreDraw;
            Foundation.CameraEvent += Camera;
        }

        protected void Input(InputArgs e)
        {
            MousePosition = e.mouse;
        }

        protected void Camera(CameraArgs e)
        {
        }

        protected void PreDraw(PreDrawArgs e)
        {
        }

        protected void MainMenu(DrawingArgs e)
        {
        }

        protected void LoadResources()
        {
            Font[FontID.Arial] = "Arial";
            Font[FontID.Consolas] = "Consolas";
            Font[FontID.LucidaConsole] = "LucidaConsole";
            Asset.ConvertFromFile($"{RootDirectory}\\MagicPixel.png", out Main.MagicPixel);
            Asset.ConvertFromFile($"{RootDirectory}\\temp.png", out Main.Temporal);
            Asset.ConvertFromFile($"{RootDirectory}\\Alpha Tiles Scratches.png", out cotf_rewd.Background.BGs[BackgroundID.Tiles]);
            Asset.ConvertFromFile($"{RootDirectory}\\temp_bg.png", out cotf_rewd.Background.BGs[BackgroundID.Temp]);
            Asset.ConvertFromFile($"{RootDirectory}\\background2.png", out cotf_rewd.Background.BGs[BackgroundID.SmallTiles]);
            Asset.ConvertFromFile($"{RootDirectory}\\Necrosis.png", out Main.NPCTexture[NPCID.Necrosis + 1]);
            Asset.ConvertFromFile($"{RootDirectory}\\temp.png", out Main.NPCTexture[NPCID.Kobold + 1]);
            Asset.ConvertFromFile($"{RootDirectory}\\Orb.png", out Main.ProjTexture[ProjectileID.Orb]);
            Asset.ConvertFromFile($"{RootDirectory}\\fow.png", out Light.fow);
        }

        public static Color FromAlpha(float r, float g, float b, float a)
        {
            return Color.FromArgb((int)(255 * a), (int)(255 * r), (int)(255 * g), (int)(255 * b));
        }

        protected void Initialize(InitializeArgs e)
        {
            matrix = new Matrix();

            Dialog.CoinColor = new Color[]
            {
                FromAlpha(0.722f, 0.451f, 0.20f, 1f),
                FromAlpha(0.804f, 0.498f, 0.196f, 1f),
                Color.Silver,
                Color.Gold,
                FromAlpha(0.898f, 0.894f, 0.886f, 1f)
            };
        }

        protected void Draw(DrawingArgs e)
        {
            // TODO: Add your drawing code here
            if (Main.Logo) Main?.LogoDisplay(e.rewBatch);
            if (flag)
            {
                Main.MainDraw(e.rewBatch);
            }
        }

        protected void Update(UpdateArgs e)
        {
            if (KeyDown(Key.Escape))
            {
                Main.SavePlayer();
                Level.Save(Level.floorNumber);
                Environment.Exit(1);
            }

            // TODO: Add your update logic here
            if (!init && Main.LocalPlayer != null)
            {
                Main.LoadPlayer();
                init = true;
            }
            Main.ScreenWidth = Program.Width;
            Main.ScreenHeight = Program.Height;

            Main.rand?.Seed();
            Main.Updater();
            if (Main.LocalPlayer != null)
            {
                var offset = new Vector3(Main.ScreenWidth / 2, Main.ScreenHeight / 2, 0);
                var camera = new Vector3(-Main.LocalPlayer.position.X - Player.plrWidth / 2, -Main.LocalPlayer.position.Y - Player.plrHeight / 2, 0);
                //matrix = Matrix.CreateTranslation(camera + offset + (Main.IsZoomed ? new Vector3(Main.ScreenWidth * 0.5f - Main.MapX * ScrollSpeed, Main.ScreenHeight * 0.5f - Main.MapY * ScrollSpeed, 0) : Vector3.Zero)) * Matrix.CreateScale(Main.IsZoomed ? 0.5f : 1f);
            }
            //else matrix = Matrix.CreateTranslation(0, 0, 0);
            if (KeyDown(Key.Space))
                flag = true;
        }

        protected new bool Resize()
        {
            return false;
        }

        public static new bool KeyDown(Key k)
        {
            return Keyboard.PrimaryDevice.IsKeyDown(k);
        }
        public static new bool KeyUp(Key k)
        {
            return Keyboard.PrimaryDevice.IsKeyUp(k);
        }
    }
}
