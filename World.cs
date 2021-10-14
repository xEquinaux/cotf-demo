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
    public class SquareBrush : Entity, IDisposable
    {
        public int X;
        public int Y;
        public int i => X / width;
        public int j => Y / height;
        private bool Active = true;
        public bool Square = true;
        public const int Size = 50;
        public new Vector2 Center
        {
            get { return new Vector2(X + width / 2, Y + height / 2); }
        }
        public Rectangle Hitbox => new Rectangle(X, Y, width, height);
        public bool door(bool door)
        {
            return false;
        }
        public new bool active(bool active)
        {
            Active = active;
            return active;
        }
        public new bool active()
        {
            return Active;
        }
        public SquareBrush(int x, int y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            this.width = width;
            this.height = height;
            this.position = new Vector2(X, Y);
        }
        public static SquareBrush GetSafely(int x, int y)
        {
            return Main.squareMulti[Math.Max(0, Math.Min(Main.squareMulti.GetLength(0) - 1, x)), Math.Max(0, Math.Min(Main.squareMulti.GetLength(1) - 1, y))];
        }
        public static SquareBrush NewBrush(int x, int y, int width, int height, bool active = true, bool discovered = false)
        {
            int num = Main.square.Count - 1;
            for (int i = 0; i < Main.square.Count; i++)
            {
                if (Main.square[i] == null)
                {
                    num = i;
                    break;
                }
                if (i == num)
                {
                    break;
                }
            }
            int m = Math.Min(x / width, Main.squareMulti.GetLength(0) - 1);
            int n = Math.Min(y / height, Main.squareMulti.GetLength(1) - 1);
            Main.squareMulti[m, n] = new SquareBrush(x, y, width, height);
            Main.squareMulti[m, n].position = new Vector2(x, y);
            Main.squareMulti[m, n].active(active);
            Main.squareMulti[m, n].discovered = discovered;
            Main.squareMulti[m, n].whoAmI = Math.Max(num, 0);
            if (!discovered)
                Main.squareMulti[m, n].alpha = 0f;
            return Main.squareMulti[m, n];
        }
        public void Update(float range)
        {
            
            if (Distance(Center, Main.LocalPlayer.Center) < Math.Max(range, Light.range * Light.AddLight))
            {
                discovered = true;
            }
        }
        public void Collision(Player player, int buffer = 4)
        {
            if (!Active) return;

            if (Hitbox.Intersects(new Rectangle((int)player.position.X, (int)player.position.Y, Player.plrWidth, Player.plrHeight)))
                player.collide = true;
            //  Directions
            if (Hitbox.Intersects(new Rectangle((int)player.position.X, (int)player.position.Y - buffer, Player.plrWidth, 2)))
                player.colUp = true;
            if (Hitbox.Intersects(new Rectangle((int)player.position.X, (int)player.position.Y + Player.plrHeight + buffer, Player.plrWidth, 2)))
                player.colDown = true;
            if (Hitbox.Intersects(new Rectangle((int)player.position.X + Player.plrWidth + buffer, (int)player.position.Y, 2, Player.plrHeight)))
                player.colRight = true;
            if (Hitbox.Intersects(new Rectangle((int)player.position.X - buffer, (int)player.position.Y, 2, Player.plrHeight)))
                player.colLeft = true;
        }
        public void NPCCollision(NPC npc, int buffer = 4)
        {
            if (!Active) return;

            if (Hitbox.Intersects(new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height)))
                npc.collide = true;
            //  Directions
            if (Hitbox.Intersects(new Rectangle((int)npc.position.X, (int)npc.position.Y - buffer, npc.width, 2)))
                npc.colUp = true;
            if (Hitbox.Intersects(new Rectangle((int)npc.position.X, (int)npc.position.Y + npc.height + buffer, npc.width, 2)))
                npc.colDown = true;
            if (Hitbox.Intersects(new Rectangle((int)npc.position.X + npc.width + buffer, (int)npc.position.Y, 2, npc.height)))
                npc.colRight = true;
            if (Hitbox.Intersects(new Rectangle((int)npc.position.X - buffer, (int)npc.position.Y, 2, npc.height)))
                npc.colLeft = true;
        }
        public void PreDraw(SpriteBatch sb)
        {        
            if (!Active || !discovered)
                return;
            if (alpha < 1f)
            {
                alpha += 0.1f;
            }
            else alpha = 1f;
            color = Color.Gray;
            sb.Draw(Main.MagicPixel, Hitbox, DynamicTorch(Light.range) * alpha);

            //  DEBUG: lighting color mask
            //if (NPC.Distance(Center, Main.player[0].Center) > Light.range)
            //    return;
            //for (int n = 0; n < 50; n += 10)
            //for (int i = Hitbox.X + n; i < Hitbox.X + Hitbox.Width; i += 10)
            //{ 
            //    for (int j = Hitbox.Y + n; j < Hitbox.Y + Hitbox.Height; j += 10)
            //    { 
            //sb.Draw(Main.MagicPixel, Hitbox, Color.Orange * alpha * ((Distance(Center, Main.player[0].Center) * -1f + Light.range) / Light.range));
            //    }
            //}
        }
        public void Dispose()
        {
            Main.square[Math.Max(whoAmI, 0)]?.active(false);
            Main.square[Math.Max(whoAmI, 0)] = null;
        }
        public static void ClearBrushes()
        {
            for (int i = 0; i < Main.square.Count; i++)
            {
                Main.square[i]?.Dispose();
            }
        }
        public static void InitializeArray(int length, bool loading = false)
        {
            if (!loading)
                length = (int)Math.Sqrt(length);
            Main.squareMulti = new SquareBrush[length,length];
        }
    }
}
