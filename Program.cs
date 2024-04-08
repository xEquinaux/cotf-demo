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
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static FoundationR.Foundation;

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
        public static bool mouseLeft;
        public static bool mouseRight;
        bool flag;
        bool init;
        bool exit = false;
        int ticks = 1;
        int frame;
        REW cans;
        static IList<Keys> keyboard = new List<Keys>();
        static Form form;

        internal Game()
        {
            Instance = this;
            Main.Instance = new Main();
        }
        
        public override void RegisterHooks(Form form)
        {
            Foundation.UpdateEvent += Update;
            Foundation.ResizeEvent += Resize;
            Foundation.InputEvent += Input;
            Foundation.DrawEvent += Draw;
            Foundation.InitializeEvent += Initialize;
            Foundation.LoadResourcesEvent += LoadResources;
            Foundation.MainMenuEvent += MainMenu;
            Foundation.PreDrawEvent += PreDraw;
            Foundation.ViewportEvent += Viewport;
            Foundation.ExitEvent += ExitApp;
            Game.form = form;
            form.KeyDown += Form_KeyDown;
            form.MouseClick += Form_MouseClick;
        }

        private void Form_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                mouseLeft = true;
            if (e.Button == MouseButtons.Right)
                mouseRight = true;
        }

        public override void ClearInput()
        {
            mouseLeft = false;
            mouseRight = false;
            keyboard.Clear();
        }

        private void Form_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!e.Handled)
            { 
                keyboard.Add(e.KeyCode);
            }
        }

        protected bool ExitApp(ExitArgs e)
        {
            return exit;
        }

        protected void Input(InputArgs e)
        {
            try
            {
                form.Invoke(new Action(() => 
                { 
                    Point mouse = mouse = form.PointToClient(System.Windows.Forms.Cursor.Position);
                    int x = mouse.X;
                    int y = mouse.Y;
                    MousePosition = new Point(x + 8, y + 31);
                }));
            }
            catch
            {
                return;
            }
        }

        protected void Viewport(ViewportArgs e)
        {
            //var offset = new Vector2(Main.ScreenWidth / 2, Main.ScreenHeight / 2);
            //var camera = new Vector2(-Main.LocalPlayer.position.X - Player.plrWidth / 2, -Main.LocalPlayer.position.Y - Player.plrHeight / 2);
            //matrix = Matrix.CreateTranslation(camera + offset + (Main.IsZoomed ? new Vector3(Main.ScreenWidth * 0.5f - Main.MapX * ScrollSpeed, Main.ScreenHeight * 0.5f - Main.MapY * ScrollSpeed, 0) : Vector3.Zero)) * Matrix.CreateScale(Main.IsZoomed ? 0.5f : 1f);

            if (!Main.Instance.once)
            {
                var off = new Vector2(Main.ScreenWidth / 2 - Main.LocalPlayer.width / 2, Main.ScreenHeight / 2 - Main.LocalPlayer.height / 2);
                var old = Main.LocalPlayer.position + off;
                e.viewport.position = old;
            }
        }

        protected void PreDraw(PreDrawArgs e)
        {
        }

        protected void MainMenu(DrawingArgs e)
        {
            Main.Logo = false;
            flag = true;
        }

        protected void LoadResources()
        {
            Font[FontID.Arial] = "Arial";
            Font[FontID.Consolas] = "Consolas";
            Font[FontID.LucidaConsole] = "LucidaConsole";
            //Asset.ConvertFromFile($"{RootDirectory}\\MagicPixel.png", out Main.MagicPixel);
            Main.MagicPixel = REW.Create(50, 50, Color.White, Ext.GetFormat(4));
            Asset.LoadFromFile($"{RootDirectory}\\temp.rew", out Main.Temporal);
            Asset.LoadFromFile($"{RootDirectory}\\Alpha Tiles Scratches.rew", out cotf_rewd.Background.BGs[BackgroundID.Tiles]);
            Asset.LoadFromFile($"{RootDirectory}\\temp_bg.rew", out cotf_rewd.Background.BGs[BackgroundID.Temp]);
            Asset.LoadFromFile($"{RootDirectory}\\background2.rew", out cotf_rewd.Background.BGs[BackgroundID.SmallTiles]);
            Asset.LoadFromFile($"{RootDirectory}\\Necrosis.rew", out Main.NPCTexture[NPCID.Necrosis + 1]);
            Asset.LoadFromFile($"{RootDirectory}\\temp.rew", out Main.NPCTexture[NPCID.Kobold + 1]);
            Asset.LoadFromFile($"{RootDirectory}\\Orb.rew", 7, out Main.ProjTexture[ProjectileID.Orb]);
            Asset.LoadFromFile($"{RootDirectory}\\fow.rew", out Light.fow);
        }

        public static Color FromAlpha(float r, float g, float b, float a)
        {
            return Color.FromArgb((int)(255 * a), (int)(255 * r), (int)(255 * g), (int)(255 * b));
        }

        protected void Initialize(InitializeArgs e)
        {
            Dialog.CoinColor = new Color[]
            {
                FromAlpha(0.722f, 0.451f, 0.20f, 1f),
                FromAlpha(0.804f, 0.498f, 0.196f, 1f),
                Color.Silver,
                Color.Gold,
                FromAlpha(0.898f, 0.894f, 0.886f, 1f)
            };
        }

        Stopwatch GameTime = new Stopwatch();
        protected void Draw(DrawingArgs e)
        {
            // TODO: Add your drawing code here
            if (Main.Logo) Main?.LogoDisplay(e.rewBatch);
            if (flag)
            {
                Main.MainDraw(e.rewBatch);
                //e.rewBatch.Draw(Main.ProjTexture[ProjectileID.Orb].Animate(frame, 44, 38), MousePosition.X, MousePosition.Y, 38, 44);
                e.rewBatch.Draw(Light.fow, MousePosition.X, MousePosition.Y);
                e.rewBatch.DrawString("Arial", (GameTime.Elapsed.Milliseconds / 1000M * 60M).ToString(), RewBatch.Viewport.X + 50, RewBatch.Viewport.Y + 50, 200, 60, Color.White);
            }
            GameTime.Restart();
        }

        protected void Update(UpdateArgs e)
        {
            if (KeyDown(Keys.Escape))
            {
                Main.SavePlayer();
                Level.Save(Level.floorNumber);
                exit = true;
                return;
            }

            // TODO: Add your update logic here
            if (!init)
            {
                Main.LoadPlayer();
                init = true;
            }
            
            Main.rand?.Seed();
            Main.Updater();
            if (Main.LocalPlayer != null)
            {
                var offset = new Vector3(Main.ScreenWidth / 2, Main.ScreenHeight / 2, 0);
                var camera = new Vector3(-Main.LocalPlayer.position.X - Player.plrWidth / 2, -Main.LocalPlayer.position.Y - Player.plrHeight / 2, 0);
                //matrix = Matrix.CreateTranslation(camera + offset + (Main.IsZoomed ? new Vector3(Main.ScreenWidth * 0.5f - Main.MapX * ScrollSpeed, Main.ScreenHeight * 0.5f - Main.MapY * ScrollSpeed, 0) : Vector3.Zero)) * Matrix.CreateScale(Main.IsZoomed ? 0.5f : 1f);
            }
            //else matrix = Matrix.CreateTranslation(0, 0, 0);
            if (KeyDown(Keys.Space))        
                flag = true;
        }

        protected bool Resize(ResizeArgs e)
        {
            return false;
        }

        public static bool KeyDown(Keys k)
        {
            return keyboard.Contains(k);
        }
        public static bool KeyUp(Keys k)
        {
            return !keyboard.Contains(k);
        }
    }
}
