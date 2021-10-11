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
        public static Background[][,] room = new Background[101][,];
        private static bool[] active = new bool[101];
        private static void PopulateArray()
        {
            int index = 0;
            for (int i = 1; i < Main.squareMulti.GetLength(0); i++)
            {
                for (int j = 1; j < Main.squareMulti.GetLength(1); j++)
                {
                    if (!SquareBrush.GetSafely(i, j).active() && SquareBrush.GetSafely(i - 1, j - 1).active() && !SquareBrush.GetSafely(i + 1, j).active()
                        && !SquareBrush.GetSafely(i, j + 1).active() && !SquareBrush.GetSafely(i + 1, j + 1).active())
                    {
                        int width = 0;
                        int height = 0;
                        while (!SquareBrush.GetSafely(i++, j).active() && !SquareBrush.GetSafely(i, j + 1).active() && !SquareBrush.GetSafely(i, j + 2).active())
                            width++;
                        while (!SquareBrush.GetSafely(i, j++).active() && !SquareBrush.GetSafely(i + 1, j).active() && !SquareBrush.GetSafely(i + 2, j).active())
                            height++;
                        room[index] = new Background[width, height];
                        for (int k = i; k < i + width; k++)
                        {
                            for (int l = j; l < j + height; l++)
                            {
                                room[index][k, l] = Main.ground.First(t => t != null && t.active && t.hitbox.Contains(new Point(k * 50 + 25, l * 50 + 25)));
                            }
                        }
                        index++;
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
                if (Main.rand.NextBool()) continue;
                active[i] = true;
            }
        }
        public static void Update()
        {
            for (int i = 0; i < Room.room.Count(); i++)
            { 
                if (Room.room[i] == null) continue;
                if (!active[i]) continue;
                foreach (Background bg in Room.room[i])
                    bg.light = true;
            }
        }
    }
}
