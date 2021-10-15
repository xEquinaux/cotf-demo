using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePort
{
    public class Room
    {
        public bool active(bool active)
        {
            Active = active;
            return Active;
        }
        public bool active()
        {
            return Active;
        }
        private bool Active = false;
        private int width;
        private int height;
        private Point origin;
        public bool lit;
        public int floorNumber;
        public int whoAmI;
        public int X => origin.X;
        public int Y => origin.Y;
        public int Width => width;
        public int Height => height;
        public int Size => SquareBrush.Size;
        public Rectangle Hitbox => new Rectangle(X, Y, Width, Height);
        public static Room NewRoom(int x, int y, int width, int height, bool lit = false, int floorNum = -1)
        {
            int num = 100;
            for (int n = 0; n < Main.room.Length; n++)
            {
                if (Main.room[n] == null)
                {
                    num = n;
                    break;
                }
                if (n == num - 1)
                {
                    break;
                }
            }
            Main.room[num] = new Room();
            //  Make true for always lit
            Main.room[num].active(false);
            Main.room[num].origin = new Point(x, y);
            Main.room[num].width = width;
            Main.room[num].height = height;
            Main.room[num].floorNumber = floorNum;
            Main.room[num].lit = lit;
            return Main.room[num];
        }
        public static void Clear(int reInitIndex)
        {
            for (int i = 0; i < Main.room.Length; i++)
            {
                if (Main.room[i] != null)
                { 
                    Main.room[i].active(false);
                    Main.room[i] = null;
                }
            }
            Main.room = new Room[reInitIndex];
        }
        public void Update()
        {
            if (active()) return;
            foreach (Player player in Main.player)
            {
                if (player != null && player.active)
                {
                    if (Helper.Distance(Hitbox.Center.ToVector2(), player.Center) < Math.Max(Width, Height))
                    {
                        active(true);
                    }
                }
            }
        }

        public static Background[][,] room = new Background[101][,];
        private static bool[] _active = new bool[101];
        private static void PopulateArray()
        {
            int index = 0;
            bool flag = false;
            int width = 0;
            int height = 0;    
            for (int i = 1; i < Main.squareMulti.GetLength(0); i++)
            {                                                                                     
                for (int j = 1; j < Main.squareMulti.GetLength(1); j++)
                {
                    if (!SquareBrush.GetSafely(i, j).active() && SquareBrush.GetSafely(i - 1, j - 1).active() && !SquareBrush.GetSafely(i + 1, j).active()
                        && !SquareBrush.GetSafely(i, j + 1).active() && !SquareBrush.GetSafely(i + 1, j + 1).active())
                    {
                        while (flag);
                        flag = true;
                        int m = i;
                        int n = j;
                        while (!SquareBrush.GetSafely(i++, j).active() && !SquareBrush.GetSafely(i, j + 1).active() && !SquareBrush.GetSafely(i, j + 2).active())
                            width++;
                        i = m;
                        while (!SquareBrush.GetSafely(i, j++).active() && !SquareBrush.GetSafely(i + 1, j).active() && !SquareBrush.GetSafely(i + 2, j).active())
                            height++;
                        j = n;
                        if (width == 0 || height == 0 || Main.rand.NextFloat() < 0.75f)
                        {
                            flag = false;
                            continue;
                        }
                        room[index] = new Background[width, height];
                        for (int k = m; k < m + width; k++)
                        {
                            for (int l = n; l < n + height; l++)
                            {
                                var count = Main.ground.Where(t => t != null && t.active && t.hitbox.Contains(new Point(k * 50 + 25, l * 50 + 25)));
                                if (count.Count() > 0)
                                    room[index][k - m, l - n] = count.First();
                            }
                        }
                        width = 0;
                        height = 0;
                        index++;
                        flag = false;
                    }
                }
            }
        }
        public static void Initialize()
        {
        //  TODO troubleshoot
            Room.PopulateArray();
            for (int i = 0; i < Room.room.Count(); i++)
            { 
                if (Room.room[i] == null) continue;
                if (Main.rand.NextDouble() < 0.8f) continue;
                _active[i] = true;
            }
        }
        //public static void Update()
        //{
        //    for (int i = 0; i < Room.room.Count(); i++)
        //    { 
        //        if (Room.room[i] == null) continue;
        //        if (!_active[i]) continue;
        //        foreach (Background bg in Room.room[i])
        //        {
        //            if (bg != null && bg.active)
        //                bg.lit = true;
        //        }
        //    }
        //}
    }
}
