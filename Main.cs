using FoundationR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;





namespace cotf_rewd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class Main
    {
        public static Main Instance;
        public static rand rand;
        public static SquareBrush[,] squareMulti = new SquareBrush[128,128];
        public static List<SquareBrush> square = new List<SquareBrush>();
        public static Background[] ground = new Background[2001];
        public static Dust[] dust = new Dust[1001];
        public static NPC[] npc = new NPC[501];
        public static Projectile[] projectile = new Projectile[1001];
        public static GUI[] gui = new GUI[129];
        public static GUI[,] skill = new GUI[10, 4];
        //public static TriangleBrush[] triangle = new TriangleBrush[256];
        public static Item[] item = new Item[501];
        public static NPCs.Wurm_Head[] wurm = new NPCs.Wurm_Head[101];
        public static IList<Foreground> Fg = new List<Foreground>();
        public static Player[] player = new Player[256];
        public static Worldgen worldgen;
        public static Trap[] trap = new Trap[101];
        public static Light[] light = new Light[1001];
        public static Staircase[] stair = new Staircase[51];
        public static Foliage[] foliage = new Foliage[201];
        public static Room[] room = new Room[RoomMax];

        public static REW MagicPixel;
        public static REW Temporal;
        public static REW[] NPCTexture = new REW[2];
        public static REW[] ProjTexture = new REW[1];

        public const int RoomMax = 101;
        public const int NPCMax = 501;
        public const int GroundMax = 2001;
        public static int TimeScale = 1;
        public static int mainFrameRate => 1000 / 60;

        public static bool Logo = true;
        public Main()
        {
            rand = new rand();
            Initialize();
        }
        public static void UpdateBrushes()
        {
            var list = new List<SquareBrush>();
            for (int i = 0; i < squareMulti.GetLength(0); i++)
            {
                for (int j = 0; j < squareMulti.GetLength(1); j++)
                {
                    if (squareMulti[i, j] != null)
                        list.Add(squareMulti[i, j]);
                }
            }
            square = list;
        }

        /*
        public static int rightWorld
        {
           get { return width; }
        }
        public static int bottomWorld
        {
           get { return height; }
        }
        public static int maxTilesX
        {
           get { return width; }
        }
        public static int maxTilesY
        {
           get { return height; }
        }*/
        public static bool IsZoomed;               
        public bool once = true;
        public static int ScreenWidth;
        public static int ScreenHeight;
        
        public static int LevelWidth
        {
            get; private set;
        }
        public static int LevelHeight
        {
            get; private set;
        }
        public static float ScreenX
        {
            get;
            private set;
        }
        public static float ScreenY
        {
            get;
            private set;
        }
        public static int MapX, MapY;
        public static System.Drawing.Point WorldMouse;
        public static System.Drawing.Point MousePosition => Game.MousePosition;
        public static MouseDevice MouseDevice => Mouse.PrimaryDevice;
        public static MouseDevice OldMouse;

        public Vector2 ScreenPosition
        {
            get { return new Vector2(ScreenX, ScreenY); }
        }
        public static System.Drawing.Color[] types = new System.Drawing.Color[]
        {
            System.Drawing.Color.Black,
            System.Drawing.Color.White,
            System.Drawing.Color.Red,
            System.Drawing.Color.Brown,
            System.Drawing.Color.Green
        };
        public static Player LocalPlayer
        {
            get { return player[0]; }
        }
        public Rectangle WindowBox
        {
            get { return new Rectangle(0, 0, ScreenWidth, ScreenHeight); }
        }
        public static bool InventoryOpen
        {
            get { return Inventory.open; }
        }

        protected virtual void Initialize()
        {
            worldgen = new Worldgen();
        }
        public void LogoDisplay(RewBatch rb)
        {
            rb.DrawString(Game.Font[FontID.Arial], "alpha", 10, 30, 100, 24, Color.White);
            // TODO: remove after DEMO
            rb.DrawString(Game.Font[FontID.LucidaConsole], 
                "Controls\n" +
                "Left click...activates selected skill\n" +
                "Right click..activates equipped torch\n" +
                "Enter........use on stairwells\n" +
                "O............opens inventory\n" +
                "M............opens map\n" +
                "Esc..........closes game", 
                10, 75, 300, 112, Color.White);
        }

        internal void MainDraw(RewBatch rb)
        {
            Logo = false;
            ScrollBar scroll = new ScrollBar();
            OldMouse = MouseDevice;
            if (once)
            {
                player[0] = new Player();
                player[0].Initialize();
                //player[0].position = new Vector2(ScreenWidth / 2 - Player.plrWidth / 2, ScreenHeight / 2 - Player.plrHeight / 2);
                
                int num = skill.Length + 6;
                const byte buffer = 8;
                for (int i = 0; i < 5; i++)
                {
                    gui[i] = GUI.NewElement((i + 1) * 42 + buffer, 20, 36, 36, "Element " + i, i == 0 ? Main.NPCTexture[NPCID.Necrosis + 1] : Main.MagicPixel, LocalPlayer.whoAmI, i, i == 0 ? new Action(() => { NPC.NewNPC(0, 0, -1, Color.White, 500); }) : null);
                }
                gui[5] = GUI.NewElement(buffer, 20, 36, 36, "Menu", Main.MagicPixel, LocalPlayer.whoAmI, GUI.SkillMenu, null);
                
                for (int i = skill.GetLength(0) - 1; i >= 0; i--)
                {
                    for (int j = skill.GetLength(1) - 1; j >= 0; j--)
                    {
                        skill[i, j] = GUI.NewMenu((i + 1) * 42, (j + 2) * 42, 36, 36, string.Concat("Element ", i, "/", j), Main.MagicPixel, LocalPlayer.whoAmI, num, num, null);
                        num--;
                    }
                }
                LocalPlayer.inventory = new Inventory(LocalPlayer);
                Dialog.NewDialog(0, 0, new string[] { }, Item.NewItem(0, 0, 1, 1, 256, 0, Item.Type.Sword_OneHand, Item.Style.Weapon_OneHand));
                scroll.Initialize();
                Light.Instance = new Light();

                //  DEBUG starting player position
                var next = Main.stair.Where(t => t != null && t.transition == Staircase.Transition.GoingUp);
                if (next?.Count() > 0) Main.LocalPlayer.position = next.First().position;

                //  DEBUG Generating wurm npc
                //NPCs.Wurm_Head.head = NPCs.Wurm_Head.NewWurm((int)LocalPlayer.X, (int)LocalPlayer.Y, 6, 0);

                once = false;
            }
            else
            {
                WorldMouse = new System.Drawing.Point((int)(MousePosition.X - ScreenX), (int)(MousePosition.Y - ScreenY));
                if (LocalPlayer.IsMoving())
                {
                    if (!LocalPlayer.colLeft && !LocalPlayer.colRight)
                        ScreenX = ScreenWidth / 2 - player[0].position.X - Player.plrWidth / 2;
                    if (!LocalPlayer.colUp && !LocalPlayer.colDown)
                        ScreenY = ScreenHeight / 2 - player[0].position.Y - Player.plrHeight / 2;
                }
                if (!IsZoomed)
                {
                    foreach (Background g in ground.Where(t => t != null))
                        g.PreDraw(rb);
                }
                PreDraw(rb);
                if (!InventoryOpen)
                {
                    foreach (Foliage stuff in Main.foliage)
                        stuff?.Draw(rb);
                    foreach (Trap traps in trap.Where(t => t != null))
                        traps.Draw(rb);
                    foreach (Staircase s in Main.stair)
                        s?.Draw(rb);   
                    foreach (NPC n in npc.Where(t => t != null))
                        n.PreDraw(rb);
                    foreach (Dust d in dust.Where(t => t != null))
                        d.Draw(rb);
                    foreach (Item i in item.Where(t => t != null))
                        i.Draw(rb);
                    foreach (Foreground fg in Fg)
                        fg?.Draw(rb);
                    foreach (NPCs.Wurm_Head w in Main.wurm)
                        w?.Draw(rb);
                    try
                    { 
                        foreach (SquareBrush sq in square)
                            sq?.PreDraw(rb);
                    }
                    catch { }
                    if (!IsZoomed)
                    {
                        //foreach (Light l in Light.light)
                        //    l?.Draw(rb);
                        foreach (Light l in Main.light)
                        {
                            if (l?.active == true)
                                l?.Draw(rb);
                        }
                        foreach (GUI g in gui.Where(t => t != null))
                            g.Draw(rb);
                        foreach (GUI g in gui.Where(t => t != null))
                            g.DrawMenuText(rb);
                    }
                }
                foreach (Player p in player.Where(t => t != null))
                {
                    p.PreDraw(rb);
                    if (InventoryOpen)
                    {
                        p.inventory.PreDraw(rb);
                        p.inventory.Draw(rb);
                    }
                }
                if (InventoryOpen)
                {
                    foreach (GUI g in Main.LocalPlayer.Armory)
                        g?.DrawArmory(rb);
                    ScrollBar.scroll[0].Draw(rb);
                    ScrollBar.scroll[1].Draw(rb);
                    foreach (Item n in Inventory.itemList)
                        n?.Draw(rb);
                    Menu.Select?.Draw(rb);
                    Dialog.Instance?.Draw(rb);
                }
                else
                {
                    foreach (Projectile pr in projectile.Where(t => t != null))
                        pr.PreDraw(rb, pr.animated);
                }
            }
        }

        public void Updater()
        {
            bool open = InventoryOpen;
            foreach (Player p in player.Where(t => t != null))
            {
                p.Update();
                p.inventory.Update();
                p.collide = false;
                p.colUp = false;
                p.colDown = false;
                p.colRight = false;
                p.colLeft = false;

                foreach (Staircase s in Main.stair)
                    s?.Update(p);
            }
            foreach (Trap traps in trap.Where(t => t != null))
                traps.Update(Main.LocalPlayer);
            foreach (Item i in item.Where(t => t != null))
                i.Update();
            foreach (Item m in Inventory.itemList)
                m?.Update();
            foreach (Item n in Inventory.itemProximate)
                n?.Update();
            if (!open)
            {
                foreach (SquareBrush sq in square)
                {
                    sq.Collision(LocalPlayer);
                    sq.Update(100f);
                }
                foreach (Foliage stuff in foliage.Where(t => t != null))
                { 
                    stuff.Update();
                    stuff.collide = false;
                    stuff.colUp = false;
                    stuff.colDown = false;
                    stuff.colRight = false;
                    stuff.colLeft = false;
                    stuff.Collision(LocalPlayer);
                }
                foreach (Light l in Light.light)
                    l?.Update();
                foreach (Light l in Main.light)
                {
                    if (l?.active == true) l?.Update();
                }
                foreach (Background g in ground.Where(t => t != null))
                    g.Update();
                for (int i = 0; i < Main.TimeScale; i++)
                {
                    foreach (NPC n in npc.Where(t => t != null))
                    {
                        n.Update();
                        n.AI();
                        n.collide = false;
                        n.colUp = false;
                        n.colDown = false;
                        n.colRight = false;
                        n.colLeft = false;
                        n.PlayerNPCCollision();
                    }
                    foreach (Projectile pr in projectile.Where(t => t != null))
                    {
                        pr.Update();
                        pr.AI();
                    }
                }
                foreach (NPCs.Wurm_Head w in Main.wurm)
                    w?.AI();
                foreach (Dust d in dust.Where(t => t != null))
                    d.Update();
                foreach (GUI g in gui.Where(t => t != null))
                    g.Update();
                PostUpdate();
            }
            else
            {
                IsZoomed = false;
                foreach (GUI g in Main.LocalPlayer.Armory)
                    g?.UpdateArmory();
                ScrollBar.Update();
                Menu.Select?.Update();
                Dialog.Instance?.Update();
            }
        }

        public static void GenerateLevel(int size = 50, int width = 2000, int height = 1600, int maxNodes = 7, float range = 200f)
        {
            LevelWidth = width;
            LevelHeight = height;

            int rand = Main.rand.Next(2);
            if (rand == 0)
                Main.squareMulti = worldgen.CastleGen(size, width, height, maxNodes);
            else if (rand == 1)
                Main.squareMulti = worldgen.DungeonGen(size, width, height, maxNodes, 250f);

            Light.Create(0, 0, Main.LevelWidth, Main.LevelHeight, null);
            
            //  Worldgen generates better foliage
            //Foliage.GenerateFoliage(12);

            //  Old player placement
            //Main.stair.Where(t => t.transition == Staircase.Transition.GoingUp).First().position;
            Main.UpdateBrushes();
            
            //  Getting room data from level generation instead
            //Selecting rooms to diversify
            //Room.Initialize();
        }
        public bool KeyUp(Key key)
        {
            return Game.KeyUp(key);
        }
        public bool KeyDown(Key key)
        {
            return Game.KeyDown(key);
        }
        float ticks = 0;
        protected virtual void PreDraw(RewBatch rb)
        {
            //ticks += Keyboard.IsKeyDown(Key.NumPad0) ? 0.05f : 0.017f;
            //float cos = ScreenWidth / 2 + ScreenWidth / 2 * (float)Math.Cos(ticks);
            //float sin = ScreenHeight / 2 + ScreenHeight / 2 * (float)Math.Sin(ticks);
            //gfx.DrawLine(Pens.White, ScreenWidth / 2, ScreenHeight / 2, (int)cos, (int)sin);
        }
        protected virtual void Update()
        {

        }
        protected void PostUpdate()
        {
            foreach (Room room in Main.room.Where(t => t != null))
            {
                room.Update();
            }
            //  Getting room data from level generation instead
            //Room.Update();

            //if (KeyDown(Key.NumPad0) && npc[0] == null)
            //{
            //    NPC.NewNPC(0, 0, -1, System.Drawing.Color.White, 500);
            //    Projectile.NewProjectile(300, 0, -1, System.Drawing.Color.White, 300);
            //}
            //if (rand.NextDouble() > 0.90f)
            //{
            //    Dust.NewDust(rand.Next(0, LevelWidth), rand.Next(0, LevelHeight), 16, 16, rand.Next(3), System.Drawing.Color.Green, 150);
            //}
        }

        private void ExitLogoScene()
        {
            //if (!begin && !e.Handled && !e.IsRepeat && e.Key.Equals(Key.Enter))
            //{
            //    begin = true;
            //    Button_Click(this, null);
            //}
        }
        public static bool SavePlayer()
        {
            using (FileStream stream = new FileStream("player", FileMode.Create, FileAccess.Write))
            using (BinaryWriter bw = new BinaryWriter(stream))
            {
                Player player = LocalPlayer;
                if (player == null) return false;
                bw.Write(player.Name);

                //  Map ID
                bw.Write(Level.floorNumber);

                //  Position
                bw.Write(player.position.X);
                bw.Write(player.position.Y);
                
                //  Stats
                bw.Write(player.Stats.totalLife);
                bw.Write(player.Stats.currentLife);
                bw.Write(player.Stats.totalMana);
                bw.Write(player.Stats.mana);
                bw.Write(player.Stats.iFrames);
                //  TODO: Add more complete save of stats

                //  Traits
                bw.Write(player.Traits.bookSmarts);
                bw.Write(player.Traits.courage);
                bw.Write(player.Traits.streetSmarts);
                bw.Write(player.Traits.wellBeing);

                //  Attributes
                //  TODO: Complete attributes then save                
            }
            return true;
        }
        public static bool LoadPlayer()
        {
            Player player = LocalPlayer;
            if (File.Exists("player"))
            {
                using (FileStream stream = new FileStream("player", FileMode.OpenOrCreate, FileAccess.Read))
                using (BinaryReader br = new BinaryReader(stream))
                {
                    player.Name = br.ReadString();

                    Level.floorNumber = br.ReadInt32();
                    if (!LoadLevel(Level.floorNumber))
                        GenerateLevel();

                    //  Reading previous player coordinates
                    float x = br.ReadSingle();
                    float y = br.ReadSingle();

                    player.position = new Vector2(x, y);

                    //  DEBUG reset player position
                    //player.position = Main.stair.First(t => t.transition == Staircase.Transition.GoingUp).position;

                    player.Stats = new Stats();
                    player.Stats.totalLife = br.ReadInt32();
                    player.Stats.currentLife = br.ReadInt32();
                    player.Stats.totalMana = br.ReadInt32();
                    player.Stats.mana = br.ReadInt32();
                    player.Stats.iFrames = br.ReadInt32();

                    player.Traits = new Traits();
                    player.Traits.bookSmarts = br.ReadSingle();
                    player.Traits.courage = br.ReadSingle();
                    player.Traits.streetSmarts = br.ReadSingle();
                    player.Traits.wellBeing = br.ReadSingle();
                }
                //  DEBUG set player position
                //  MOVED to player init
                //if (Level.floorNumber == 0 || player.PlayerInWall())
                //{
                //    Background bg = null;
                //    do
                //    {
                //        bg = Main.ground[(int)Main.rand.Next(0, Main.ground.Length)];
                //    } while (bg == null || !bg.active);
                //    Main.LocalPlayer.position = bg.position;
                //}
                return true;
            }
            if (!LoadLevel(Level.floorNumber))
                GenerateLevel();                              
            return false;
        }
        public static bool LoadLevel(int floor)
        {
            if (File.Exists(Level.Name + Level.floorNumber))
            {
                LevelWidth = 2000;
                LevelHeight = 1600;
                Level.LoadFloor(floor);
                Light.light.Clear();
                Light.Create(0, 0, Main.LevelWidth, Main.LevelHeight, null);
                return true;
            }
            return false;
        }
    }
}
