using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using FoundationR;
using FoundationR.Lib;
using FoundationR.Rew;
using FoundationR.Loader;
using FoundationR.Ext;
using FoundationR.Headers;




namespace cotf_rewd
{
    public class Worldgen 
    {
        public static Worldgen Instance;
        public Worldgen()
        {
            Instance = this;
        }
        int[,] room0 = new int[,]
        {
            { 0, 0, 1, 1, 1, 0, 0 },
            { 0, 0, 1, 0, 1, 0, 0 },
            { 0, 1, 1, 0, 1, 1, 0 },
            { 0, 1, 0, 0, 0, 1, 0 },
            { 1, 1, 0, 0, 0, 1, 1 },
            { 1, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1 }
        };
        private const byte maxTorches = 4;
        public float Distance(Vector2 one, Vector2 two)
        {
            Vector2 v1 = one;
            Vector2 v2 = two;
            int a = (int)Math.Abs(v2.X - v1.X);
            int b = (int)Math.Abs(v2.Y - v1.Y);
            int c = (int)Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
            return c;
        }
        public SquareBrush[,] CastleGen(int size, int width, int height, int maxNodes = 5, float range = 300f, float nodeDistance = 800f)
        {
            //  Constructing values
            width += width % size;
            height += height % size;
            var brush = new SquareBrush[width / size, height / size];
            Vector2[] nodes = new Vector2[maxNodes];
            int numNodes = 0;

            //  Filling entire space with brushes
            for (int j = 0; j < height; j += size)
            {
                for (int i = 0; i < width; i += size)
                {
                    brush[i / size, j / size] = SquareBrush.NewBrush(i, j, size, size);
                }
            }

            //  Generating vector nodes
            int randX = 0,
                randY = 0;
            while (numNodes < maxNodes)
            {
                foreach (Vector2 node in nodes)
                {
                    do
                    {
                        randX = Main.rand.Next(size, width - size);
                        randY = Main.rand.Next(size, height - size);
                        nodes[numNodes] = new Vector2(randX, randY);
                    } while (nodes.All(t => Distance(t, nodes[numNodes]) < nodeDistance));
                    numNodes++;
                }
            }

            //  Carve out rooms
            int W = 0, H = 0;
            const int maxSize = 7;
            foreach (Vector2 node in nodes)
            {
                int rand = Main.rand.Next(2);
                switch (rand)
                {
                    case 0:
                        W = Main.rand.Next(4, maxSize) * size;
                        H = Main.rand.Next(4, maxSize) * size;
                        for (int i = (int)node.X - W / 2; i < node.X + W / 2; i++)
                        {
                            for (int j = (int)node.Y - H / 2; j < node.Y + H / 2; j++)
                            {
                                if (i > 0 && j > 0 && i < width && j < height)
                                {
                                    brush[i / size, j / size].active(false);
                                }
                            }
                        }
                        Room.NewRoom(
                            (int)(node.X - W / 2), 
                            (int)(node.Y - H / 2), 
                            W + size / 2 - 1, 
                            H + size / 2 - 1,  
                            Main.rand.NextFloat() < 0.67f, 
                            Level.floorNumber);
                        break;
                    case 1:
                        for (int i = 0; i < room0.GetLength(0); i++)
                        {
                            for (int j = 0; j < room0.GetLength(1); j++)
                            {
                                W = i * size + (int)node.X;
                                H = j * size + (int)node.Y;
                                W += W % size;
                                H += H % size;
                                if (W > 0 && H > 0 && W < width && H < height)
                                {
                                    brush[W / size, H / size].active(false);
                                }
                            }
                        }
                        Room.NewRoom(W, H, width, height, Main.rand.NextFloat() < 0.67f, Level.floorNumber);
                        break;
                    default:
                        break;
                }
            }

            //  Generating hallways
            for (int k = 1; k < nodes.Length; k++)
            {
                int X, Y;
                Vector2 start, 
                        end;
                
                //  Normal pass
                start = nodes[k - 1];
                end = nodes[k];

                #region Hallway carving
                if (start.X < end.X && start.Y < end.Y)
                {
                    X = (int)start.X + (int)start.X % size;
                    Y = (int)start.Y + (int)start.Y % size;

                    while (Y++ <= (start.Y + end.Y) / 2 + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (X++ <= end.X + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (Y++ <= end.Y + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                }
                else if (start.X > end.X && start.Y < end.Y)
                {
                    X = (int)start.X + (int)start.X % size;
                    Y = (int)start.Y + (int)start.Y % size;

                    while (Y++ <= (start.Y + end.Y) / 2 + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (X++ <= end.X + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (Y++ <= end.Y + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                }
                else if (start.X < end.X && start.Y > end.Y)
                {
                    X = (int)start.X + (int)start.X % size;
                    Y = (int)start.Y + (int)start.Y % size;

                    while (X++ <= (start.X + end.X) / 2 + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (Y++ <= end.Y + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (X++ <= end.X + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                }
                else if (start.X > end.X && start.Y > end.Y)
                {
                    X = (int)start.X + (int)start.X % size;
                    Y = (int)start.Y + (int)start.Y % size;

                    while (X++ <= (start.X + end.X) / 2 + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (Y++ <= end.Y + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (X++ <= end.X + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                }

                //  Reversed pass
                start = nodes[k];
                end = nodes[k - 1];

                if (start.X < end.X && start.Y < end.Y)
                {
                    X = (int)start.X + (int)start.X % size;
                    Y = (int)start.Y + (int)start.Y % size;

                    while (Y++ <= (start.Y + end.Y) / 2 + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (X++ <= end.X + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (Y++ <= end.Y + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                }
                else if (start.X > end.X && start.Y < end.Y)
                {
                    X = (int)start.X + (int)start.X % size;
                    Y = (int)start.Y + (int)start.Y % size;

                    while (Y++ <= (start.Y + end.Y) / 2 + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (X++ <= end.X + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (Y++ <= end.Y + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                }
                else if (start.X < end.X && start.Y > end.Y)
                {
                    X = (int)start.X + (int)start.X % size;
                    Y = (int)start.Y + (int)start.Y % size;

                    while (X++ <= (start.X + end.X) / 2 + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (Y++ <= end.Y + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (X++ <= end.X + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                }
                else if (start.X > end.X && start.Y > end.Y)
                {
                    X = (int)start.X + (int)start.X % size;
                    Y = (int)start.Y + (int)start.Y % size;

                    while (X++ <= (start.X + end.X) / 2 + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (Y++ <= end.Y + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (X++ <= end.X + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                }
                #endregion
            }

            //  Map boundaries
            int m = 0, n = 0;
            while (true)
            {
                for (int i = 0; i < width; i += size)
                {
                    brush[i / size, m / size] = new SquareBrush(i, m, size, size);
                }
                if (m < height - size)
                {
                    m = height - size;
                    continue;
                }
                break;
            }
            while (true)
            {
                for (int j = 0; j < height; j += size)
                {
                    brush[n / size, j / size] = new SquareBrush(n, j, size, size);
                }
                if (n < width - size)
                {
                    n = width - size;
                    continue;
                }
                break;
            }

            int num = 0;
            int numDown = 0, numUp = 0;
            int numTraps = 0;
            int numItems = 0;
            int numNPCs = 0;
            int numTorches = 0;
            int numFoliage = 0;
            const float mult = 1.5f;
            SquareBrush.InitializeArray(brush.Length);
            while (numDown == 0 || numUp == 0)
            {
                foreach (var b in brush)
                {
                    //  Adding background objects
                    if (!b.active()) Background.NewGround((int)b.Center.X - size / 2, (int)b.Center.Y - size / 2, size, size, 0, 128f);
                    
                    //  Adding tile objects
                    for (int k = 0; k < nodes.Length; k++)
                    {
                        if (!b.active() && Distance(nodes[k], b.Center) < range * mult)
                        {
                            Vector2 randv2 = Vector2.Zero;
                            do
                            {
                                randX = Main.rand.Next(size, width - size);
                                randY = Main.rand.Next(size, height - size);
                                randv2 = new Vector2(randX, randY);
                            } while (brush[randX / size, randY / size].active());
                            int rand = Main.rand.Next(13);
                            randv2.X -= randv2.X % size;
                            randv2.Y -= randv2.Y % size;
                            switch (rand)
                            {
                                case TileID.Empty:
                                    break;
                                case TileID.Item:
                                    if (numItems++ < 10)
                                        Item.NewItem((int)randv2.X, (int)randv2.Y, 24, 24, Item.Owner_World, Main.rand.Next(11));
                                    break;
                                case TileID.Torch:
                                    //  Unoptimized: causes large slowdown
                                    if (numTorches++ < maxTorches)
                                        Light.NewTorch(randv2, Main.rand.Next(80, 150));
                                    break;
                                case TileID.Monster:
                                    if (numNPCs++ < 12)
                                    {
                                        if (Main.rand.Next(4) == 0)
                                            NPCs.Wurm_Head.NewWurm((int)randv2.X, (int)randv2.Y, 6, 0);
                                        else NPC.NewNPC((int)randv2.X, (int)randv2.Y, NPCID.Kobold, Color.White);
                                    }
                                    break;
                                case TileID.Trap:
                                    if (numTraps++ < 10)
                                        Trap.NewTrap((int)randv2.X, (int)randv2.Y, size, TrapID.Acid);
                                    break;
                                case TileID.StairsDown:
                                    if (numDown < 1)
                                    {
                                        //  Place down stairs
                                        Vector2 vector2 = randv2;
                                        var up = Main.stair.Where(t => t != null && t.transition == Staircase.Transition.GoingUp);
                                        if (up.Count() > 0)
                                        {
                                            var stair = up.First();
                                            if (Distance(stair.Center, vector2) > range)
                                            {
                                                Staircase.NewStairs((int)vector2.X, (int)vector2.Y, size, 0, Staircase.Transition.GoingDown);
                                                numDown++;
                                            }
                                        }
                                        else
                                        {
                                            Staircase.NewStairs((int)vector2.X, (int)vector2.Y, size, 0, Staircase.Transition.GoingDown);
                                            numDown++;
                                        }
                                    }
                                    break;
                                case TileID.StairsUp:
                                    if (numUp < 1)
                                    {
                                        //  Place up stairs
                                        Vector2 vector2 = randv2;
                                        var down = Main.stair.Where(t => t != null && t.transition == Staircase.Transition.GoingDown);
                                        if (down.Count() > 0)
                                        {
                                            var stair = down.First();
                                            if (Distance(stair.Center, vector2) > range)
                                            {
                                                Staircase.NewStairs((int)vector2.X, (int)vector2.Y, size, 0, Staircase.Transition.GoingUp);
                                                numUp++;
                                            }
                                        }
                                        else 
                                        {
                                            Staircase.NewStairs((int)vector2.X, (int)vector2.Y, size, 0, Staircase.Transition.GoingUp);
                                            numUp++;
                                        }
                                    }
                                    break;
                                case TileID.Stone:
                                    if (numFoliage++ < 7)
                                    {
                                        Foliage.NewFoliage((int)randv2.X, (int)randv2.Y, 40, 35, FoliageID.StoneLarge);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    //  Inserting into main brush array
                    for (int i = num; i < Main.square.Count; i++)
                    {
                        if (Main.square[i] == null)
                        {
                            num = i;
                            Main.square[i] = b;
                            Main.square[i].active(b.active());
                            break;
                        }
                    }
                }
            }
            return brush;
        }
        public SquareBrush[,] DungeonGen(int size, int width, int height, int maxNodes = 4, float range = 300f)
        {
            //  Constructing values
            width += width % size;
            height += height % size;
            var brush = new SquareBrush[width / size, height / size];
            Vector2[] nodes = new Vector2[maxNodes];
            int randX = 0, randY = 0;
            int numNodes = 0;

            //  Filling entire space with brushes
            for (int j = 0; j < height; j += size)
            {
                for (int i = 0; i < width; i += size)
                {
                    brush[i / size, j / size] = SquareBrush.NewBrush(i, j, size, size, true, Level.floorNumber <= Level.litFloors);
                }
            }

            //  Generating vector nodes
            while (numNodes < maxNodes)
            {
                foreach (Vector2 node in nodes)
                {
                    randX = Main.rand.Next(size, width - size);
                    randY = Main.rand.Next(size, height - size);
                    nodes[numNodes] = new Vector2(randX, randY);
                    numNodes++;
                }
            }

            //  Making rooms from node vectors                   
            foreach (var b in brush)
            {
                foreach (Vector2 node in nodes)
                {
                    if (Distance(node, b.Center) < range)
                    {
                        b.active(false);
                    }
                }
            }

            //  Generating hallways
            for (int i = 1; i < nodes.Length; i++)
            {
                Vector2 start = nodes[i - 1];
                Vector2 end = nodes[i];
                while (Distance(start, end) > size / 2)
                {
                    var line = NPC.AngleToSpeed(NPC.AngleTo(start, end), size / 3);
                    start.X += line.X;
                    start.Y += line.Y;
                    foreach (var b in brush)
                    {
                        if (Distance(start, b.Center) < size * 1.34f)
                        {
                            b.active(false);
                        }
                    }
                }
            }
            //  Hallway generation reversal
            for (int i = nodes.Length - 1; i > 0; i--)
            {
                Vector2 start = nodes[i];
                Vector2 end = nodes[i - 1];
                while (Distance(start, end) > size / 2)
                {
                    var line = NPC.AngleToSpeed(NPC.AngleTo(start, end), size / 3);
                    start.X += line.X;
                    start.Y += line.Y;
                    foreach (var b in brush)
                    {
                        if (Distance(start, b.Center) < size * 1.34f)
                        {
                            b.active(false);
                        }
                    }
                }
            }

            //  Map boundaries
            int m = 0, n = 0;
            while (true)
            {
                for (int i = 0; i < width; i += size)
                {
                    brush[i / size, m / size] = new SquareBrush(i, m, size, size);
                }
                if (m < height - size)
                {
                    m = height - size;
                    continue;
                }
                break;
            }
            while (true)
            {
                for (int j = 0; j < height; j += size)
                {
                    brush[n / size, j / size] = new SquareBrush(n, j, size, size);
                }
                if (n < width - size)
                {
                    n = width - size;
                    continue;
                }
                break;
            }

            int num = 0;
            int numDown = 0, numUp = 0;
            int numTraps = 0;
            int numItems = 0;
            int numNpcs = 0;
            int numTorches = 0;
            int numFoliage = 0;
            SquareBrush.InitializeArray(brush.Length);
            while (numDown == 0 || numUp == 0)
            {
                foreach (var b in brush)
                {
                    if (!b.active()) Background.NewGround((int)b.Center.X - size / 2, (int)b.Center.Y - size / 2, size, size, 0, 128f);
                    //  Adding tile objects
                    Vector2 randv2 = Vector2.Zero;
                    do
                    {
                        randX = Main.rand.Next(size, width - size);
                        randY = Main.rand.Next(size, height - size);
                        randv2 = new Vector2(randX, randY);
                    } while (brush[randX / size, randY / size].active());
                    for (int k = 0; k < nodes.Length; k++)
                    {
                        if (!b.active() && NPC.Distance(nodes[k], b.Center) < range)
                        {
                            int rand = Main.rand.Next(13);
                            switch (rand)
                            {
                                case TileID.Empty:
                                    break;
                                case TileID.Item:
                                    if (numItems++ < 10)
                                    {
                                        Item.NewItem((int)randv2.X, (int)randv2.Y, 24, 24, Item.Owner_World, Main.rand.Next(11));
                                    }
                                    break;
                                case TileID.Torch:
                                    //  Unoptimized: causes large slowdown
                                    if (numTorches++ < maxTorches)
                                        Light.NewTorch(randv2, Main.rand.Next(80, 200));
                                    break;
                                case TileID.Monster:
                                    if (numNpcs++ < 12)
                                        NPC.NewNPC((int)randv2.X, (int)randv2.Y, NPCID.Kobold, Color.White);
                                    break;
                                case TileID.Trap:
                                    if (numTraps++ < 10)
                                        Trap.NewTrap((int)randv2.X, (int)randv2.Y, size, TrapID.Acid);
                                    break;
                                case TileID.StairsDown:
                                    if (numDown < 1)
                                    {
                                        //  Place down stairs
                                        Vector2 vector2 = randv2;
                                        var up = Main.stair.Where(t => t != null && t.transition == Staircase.Transition.GoingUp);
                                        if (up.Count() > 0)
                                        {
                                            var stair = up.First();
                                            if (Distance(stair.Center, vector2) > range)
                                            {
                                                Staircase.NewStairs((int)vector2.X, (int)vector2.Y, size, 0, Staircase.Transition.GoingDown);
                                                numDown++;
                                            }
                                        }
                                        else
                                        {
                                            Staircase.NewStairs((int)vector2.X, (int)vector2.Y, size, 0, Staircase.Transition.GoingDown);
                                            numDown++;
                                        }
                                    }
                                    break;
                                case TileID.StairsUp:
                                    if (numUp < 1)
                                    {
                                        //  Place up stairs
                                        Vector2 vector2 = randv2;
                                        var down = Main.stair.Where(t => t != null && t.transition == Staircase.Transition.GoingDown);
                                        if (down.Count() > 0)
                                        {
                                            var stair = down.First();
                                            if (Distance(stair.Center, vector2) > range)
                                            {
                                                Staircase.NewStairs((int)vector2.X, (int)vector2.Y, size, 0, Staircase.Transition.GoingUp);
                                                numUp++;
                                            }
                                        }
                                        else
                                        {
                                            Staircase.NewStairs((int)vector2.X, (int)vector2.Y, size, 0, Staircase.Transition.GoingUp);
                                            numUp++;
                                        }
                                    }
                                    break;
                                case TileID.Stone:
                                    if (numFoliage++ < 7)
                                    {
                                        Foliage.NewFoliage((int)randv2.X - size / 2, (int)randv2.Y - size / 2, 40, 35, FoliageID.StoneLarge);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    //  Inserting into main brush array
                    for (int i = num; i < Main.square.Count; i++)
                    {
                        if (Main.square[i] == null)
                        {
                            num = i;
                            Main.square[i] = b;
                            Main.square[i].active(b.active());
                            break;
                        }
                    }
                }
            }
            return brush;
        }
    }
}
