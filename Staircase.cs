using FoundationR;
using FoundationR.Lib;
using FoundationR.Rew;
using FoundationR.Loader;
using FoundationR.Ext;
using FoundationR.Headers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Forms;





namespace cotf_rewd
{
    public class Staircase : IDisposable
    {
        public bool active;
        public Vector2 position;
        public int width;
        public int height;
        public bool proximity;
        public Vector2 Center
        {
            get { return new Vector2(position.X + width / 2, position.Y + height / 2); }
        }
        public int X
        {
            get { return (int)position.X; }
        }
        public int Y
        {
            get { return (int)position.Y; }
        }
        public Rectangle hitbox;
        public int currentFloor;
        public Transition transition;
        public int whoAmI;
        public static bool goingDown;
        private static int flag = 0;
        public const Keys useStairs = Keys.Enter;
        public bool discovered;
        public const float range = 300f;
        public enum Transition
        {
            GoingUp = 0,
            GoingDown = 1
        }
        public static Staircase NewStairs(int x, int y, int size = 50, int currentFloor = 0, Transition transition = Transition.GoingDown)
        {
            int num = 50;
            for (int i = 0; i < Main.stair.Length; i++)
            {
                if (Main.stair[i] == null || !Main.stair[i].active)
                {
                    num = i;
                    break;
                }
                if (i == num)
                {
                    break;
                }
            }
            Main.stair[num] = new Staircase();
            Main.stair[num].position = new Vector2(x, y);
            Main.stair[num].active = true;
            Main.stair[num].whoAmI = num;
            Main.stair[num].width = size;
            Main.stair[num].height = size;
            Main.stair[num].transition = transition;
            Main.stair[num].currentFloor = currentFloor;
            Main.stair[num].hitbox = new Rectangle(x, y, size, size);
            return Main.stair[num];
        }
        public void Update(Player player)
        {
            if (!active) return;

            if (!discovered)
            {
                discovered = player.Distance(Center, player.Center) < range;
                return;
            }
            
            if (player.KeyUp(useStairs) && !player.KeyDown(useStairs))
                flag = 0;
            
            //  DEBUG Worldgen testing fast travel
            //if (player.KeyDown(Keys.Down))
            //    player.position = Main.stair.Where(t => t.transition == Transition.GoingDown).First().position;
            //if (player.KeyDown(Keys.Up))
            //    player.position = Main.stair.Where(t => t.transition == Transition.GoingUp).First().position;

            if ((proximity = player.hitbox.IntersectsWith(hitbox)) && player.KeyDown(useStairs) && flag++ == 0)
            {
                switch (transition)
                {
                    case Transition.GoingDown:
                        goingDown = true;
                        Level.Save(Level.floorNumber);
                        ClearPreviousLevel();
                        Level.floorNumber++;
                        Light.entity.Add(player);
                        if (File.Exists(Level.Name + Level.floorNumber))
                        {
                            Level.LoadFloor(Level.floorNumber);
                            Light.light.Clear();
                            Light.Create(0, 0, Main.LevelWidth, Main.LevelHeight, null);
                        }
                        else Main.GenerateLevel();

                        var next = Main.stair.Where(t => t != null && t.transition == Transition.GoingUp);
                        if (next?.Count() > 0) player.position = next.First().position;
                        break;
                    case Transition.GoingUp:
                        goingDown = false;
                        if (Level.floorNumber > 0)
                        {
                            Level.Save(Level.floorNumber);
                            ClearPreviousLevel();
                            Level.floorNumber--;
                            Light.entity.Add(player);
                            if (File.Exists(Level.Name + Level.floorNumber))
                            {
                                Level.LoadFloor(Level.floorNumber);
                                Light.light.Clear();
                                Light.Create(0, 0, Main.LevelWidth, Main.LevelHeight, null);
                            }
                            else Main.GenerateLevel();
                            player.position = Main.stair.Where(t => t.transition == Transition.GoingDown).First().position;
                        }
                        break;
                }
                
            }
        }
        private void ClearPreviousLevel()
        {
            Room.Clear(Main.RoomMax);
            NPC.Clear(Main.NPCMax);
            Background.Clear(Main.GroundMax);
            Light.entity.Clear();
            Light.light.Clear();
            foreach (var i in Main.item)
                i?.Dispose();
            foreach (var t in Main.trap)
                t?.Dispose();
            foreach (var s in Main.stair)
                s?.Dispose();
            SquareBrush.ClearBrushes();
        }
        public void Draw(RewBatch rb)
        {
            if (!active || !discovered) return;

            //  DEBUG
            rb.Draw(Main.MagicPixel, hitbox, Color.White);
            if (proximity)
            {
                rb.DrawString(Game.Font[FontID.Arial], transition == Transition.GoingDown ? "Down" : "Up", position + new Vector2(0, height), Color.White);
            }
        }
        public void Dispose()
        {
            Main.stair[whoAmI].active = false;
            Main.stair[whoAmI] = null;
        }
    }
    public class Level
    {
        private static FileStream stream;
        private static BinaryReader read;
        private static BinaryWriter write;
        public static int floorNumber;
        public const int litFloors = 3;
        public static string Name => "Dungeon";

        private static void Initialize(int floorIndex)
        {
            floorNumber = floorIndex;
            //stream = new FileStream(Name + floorNumber, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //read = new BinaryReader(stream);
            //write = new BinaryWriter(stream);
        }
        public static void Save(int floorIndex)
        {
            return;
            Initialize(floorIndex);

            write.Write(Name);
            write.Write(floorNumber);

            write.Write(Light.entity.Where(t => t.owner == Item.Owner_World && t.active == true).Count());
            write.Write(Main.square.Where(t => t != null && t.active()).Count());
            write.Write(Main.ground.Where(t => t != null && t.active == true).Count());
            write.Write(Main.item.Where(t => t != null).Count());
            write.Write(Main.trap.Where(t => t != null).Count());
            write.Write(Main.stair.Where(t => t != null).Count());
            write.Write(Main.npc.Where(t => t != null).Count());
            write.Write(Main.foliage.Where(t => t != null && t.active).Count());
            write.Write(Main.room.Where(t => t != null).Count());

            foreach (SimpleEntity ent in Light.entity.Where(t => t.owner == Item.Owner_World && t.active == true))
            {
                write.Write(ent.owner);
                write.Write(ent.range);
                write.Write(ent.X);
                write.Write(ent.Y);
            }
            foreach (SquareBrush sq in Main.square.Where(t => t != null && t.active()))
            {
                write.Write(sq.active());
                write.Write(sq.discovered);
                write.Write(sq.X);
                write.Write(sq.Y);
            }
            foreach (Background bg in Main.ground.Where(t => t != null && t.active == true))
            {
                write.Write(bg.active);
                write.Write(bg.discovered);
                write.Write(bg.style);
                write.Write(bg.range);
                write.Write(bg.X);
                write.Write(bg.Y);
                write.Write(bg.width);
                write.Write(bg.height);
            }
            foreach (Item item in Main.item.Where(t => t != null))
            {
                write.Write(item.active);
                write.Write(item.discovered);
                write.Write(item.style);
                write.Write(item.X);
                write.Write(item.Y);
                write.Write(item.width);
                write.Write(item.height);
            }
            foreach (Trap trap in Main.trap.Where(t => t != null))
            {
                write.Write(trap.active);
                write.Write(trap.activated);
                write.Write(trap.discovered);
                write.Write(trap.type);
                write.Write(trap.X);
                write.Write(trap.Y);
                write.Write(trap.width);
                write.Write(trap.height);
            }
            foreach (Staircase stair in Main.stair.Where(t => t != null))
            {
                write.Write(stair.active);
                write.Write(stair.discovered);
                write.Write(stair.transition.ToString());
                write.Write(stair.X);
                write.Write(stair.Y);
                write.Write(stair.width);
                write.Write(stair.height);
            }
            foreach (NPC n in Main.npc.Where(t => t != null))
            {
                write.Write(n.active);
                write.Write(n.discovered);
                write.Write((int)n.position.X);
                write.Write((int)n.position.Y);
                write.Write(n.statLife);
                write.Write(n.maxLife);
                write.Write(n.type);
            }
            foreach (Foliage fol in Main.foliage.Where(t => t != null && t.active))
            {
                write.Write(fol.discovered);
                write.Write((int)fol.position.X);
                write.Write((int)fol.position.Y);
                write.Write(fol.width);
                write.Write(fol.height);
                write.Write(fol.type);
            }
            foreach (Room room in Main.room.Where(t => t != null))
            {
                write.Write(room.X);
                write.Write(room.Y);
                write.Write(room.Width);
                write.Write(room.Height);
                write.Write(room.lit);
            }
            write.Flush();
            stream.Flush();
            stream.Close();
        }
        public static void LoadFloor(int floorIndex)
        {
            Initialize(floorIndex);

            Main.GenerateLevel();
            return;
            //  Iterate through unused values
            string text = read.ReadString();

            //  Floor index
            Level.floorNumber = read.ReadInt32();

            //  Get array lengths
            int torchLength = read.ReadInt32();
            int brushLength = read.ReadInt32();
            int bgLength = read.ReadInt32();
            int itemLength = read.ReadInt32();
            int trapLength = read.ReadInt32();
            int stairLength = read.ReadInt32();
            int npcLength = read.ReadInt32();
            int foliageLength = read.ReadInt32();
            int roomCount = read.ReadInt32();

            if (bgLength < 1)
            {
                Main.GenerateLevel();
                return;
            }

            for (int n = 0; n < torchLength; n++)
            {
                int owner = read.ReadInt32();
                float range = read.ReadSingle();
                int x = read.ReadInt32();
                int y = read.ReadInt32();
                Light.entity.Add(new SimpleEntity()
                {
                    active = true,
                    owner = owner,
                    range = range,
                    position = new Vector2(x, y)
                });
            }
            SquareBrush.InitializeArray(brushLength, true);
            for (int n = 0; n < brushLength; n++)
            {
                bool active = read.ReadBoolean();
                bool discovered = read.ReadBoolean();
                int x = read.ReadInt32();
                int y = read.ReadInt32();
                int width = 50;
                int height = 50;
                SquareBrush.NewBrush(x, y, width, height, active, discovered);
            }
            Main.UpdateBrushes();
            Main.ground = new Background[bgLength];
            for (int n = 0; n < bgLength; n++)
            {
                bool active = read.ReadBoolean();
                bool discovered = read.ReadBoolean();
                int style = read.ReadInt32();
                float range = read.ReadSingle();
                int x = read.ReadInt32();
                int y = read.ReadInt32();
                int width = read.ReadInt32();
                int height = read.ReadInt32();
                if (x > 3000 || y > 3000)
                {
                    throw new Exception("Maps coordinates outside nonimal range.");
                }
                
                var bg = Background.NewGround(x, y, width, height, style, range);
                bg.active = active;
                bg.discovered = discovered;
            }
            Main.item = new Item[itemLength];
            for (int n = 0; n < itemLength; n++)
            {
                bool active = read.ReadBoolean();
                bool discovered = read.ReadBoolean();
                int style = read.ReadInt32();
                int x = read.ReadInt32();
                int y = read.ReadInt32();
                int width = read.ReadInt32();
                int height = read.ReadInt32();

                var item = Item.NewItem(x, y, width, height, Item.Owner_World, style);
                item.active = active;
                item.discovered = discovered;
            }
            Main.trap = new Trap[trapLength];
            for (int n = 0; n < trapLength; n++)
            {
                bool active = read.ReadBoolean();
                bool activated = read.ReadBoolean();
                bool discovered = read.ReadBoolean();
                int type = read.ReadInt32();
                int x = read.ReadInt32();
                int y = read.ReadInt32();
                int width = read.ReadInt32();
                int height = read.ReadInt32();

                var trap = Trap.NewTrap(x, y, width, type);
                trap.active = active;
                trap.activated = activated;
                trap.discovered = discovered;
            }
            Main.stair = new Staircase[stairLength];
            for (int n = 0; n < stairLength; n++)
            {
                bool active = read.ReadBoolean();
                bool discovered = read.ReadBoolean();
                string text2 = read.ReadString();
                Enum.TryParse<Staircase.Transition>(text2, out Staircase.Transition transition);
                int x = read.ReadInt32();
                int y = read.ReadInt32();
                int width = read.ReadInt32();
                int height = read.ReadInt32();

                var stair = Staircase.NewStairs(x, y, 50, floorIndex, transition == null ? default : (Staircase.Transition)transition);
                stair.active = active;
                stair.discovered = discovered;
            }
            Main.npc = new NPC[npcLength];
            for (int n = 0; n < npcLength; n++)
            {
                bool active = read.ReadBoolean();
                bool discovered = read.ReadBoolean();
                int x = read.ReadInt32();
                int y = read.ReadInt32();
                int statLife = read.ReadInt32();
                int maxLife = read.ReadInt32();
                int type = read.ReadInt32();
                NPC npc = NPC.NewNPC(x, y, type, Color.White);
                npc.discovered = discovered;
                npc.active = active;
                npc.statLife = statLife;
            }
            Main.foliage = new Foliage[foliageLength];
            for (int n = 0; n < foliageLength; n++)
            {
                bool discovered = read.ReadBoolean();
                int x = read.ReadInt32();
                int y = read.ReadInt32();
                int width = read.ReadInt32();
                int height = read.ReadInt32();
                int type = read.ReadInt32();
                Foliage.NewFoliage(x, y, width, height, type);
            }
            Main.room = new Room[roomCount];
            for (int n = 0; n < roomCount; n++)
            { 
                int x = read.ReadInt32();
                int y = read.ReadInt32();
                int width = read.ReadInt32();
                int height = read.ReadInt32();
                bool lit = read.ReadBoolean();
                Room.NewRoom(x, y, width, height, lit, Level.floorNumber);
            }
            stream.Flush();
            stream.Close();
        }
    }
}
