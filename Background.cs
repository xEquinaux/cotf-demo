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
    public class Foliage : SimpleEntity
    {
        public bool cut = false;
        public Background tile;
        public bool modifiesMovement = false;
        public int soundType;
        public int lifeLeft = 100;
        public Foliage(bool active = true)
        {
            this.active = active;
        }
        public static Foliage NewFoliage(float x, float y, int width, int height, int type, bool cuttable = false, Background tile = null)
        {
            int num = Main.foliage.Length;
            for (int i = 0; i < num; i++)
            {
                if (Main.foliage[i] == null || !Main.foliage[i].active)
                {
                    num = i;
                    break;
                }
                if (i == num - 1)
                {
                    break;
                }
            }
            Main.foliage[num] = new Foliage();
            Main.foliage[num].position = new Vector2(x, y);
            Main.foliage[num].X = (int)x + Main.rand.Next(50 - width);
            Main.foliage[num].Y = (int)y + Main.rand.Next(50 - height);
            Main.foliage[num].width = width;
            Main.foliage[num].height = height;
            Main.foliage[num].type = type;
            Main.foliage[num].cut = cuttable;
            Main.foliage[num].tile = tile;
            Main.foliage[num].whoAmI = num;
            Main.foliage[num].range  = 200f;
            return Main.foliage[num];
        }
        public static int ChooseType()
        {
            return Main.rand.Next(FoliageID.Length);
        }
        public void Collision(Player player, int buffer = 4)
        {
            if (!active || type != FoliageID.StoneLarge) return;

            if (hitbox.Intersects(new Rectangle((int)player.position.X, (int)player.position.Y, Player.plrWidth, Player.plrHeight)))
                player.collide = true;
            //  Directions
            if (hitbox.Intersects(new Rectangle((int)player.position.X, (int)player.position.Y - buffer, Player.plrWidth, 2)))
                player.colUp = true;
            if (hitbox.Intersects(new Rectangle((int)player.position.X, (int)player.position.Y + Player.plrHeight + buffer, Player.plrWidth, 2)))
                player.colDown = true;
            if (hitbox.Intersects(new Rectangle((int)player.position.X + Player.plrWidth + buffer, (int)player.position.Y, 2, Player.plrHeight)))
                player.colRight = true;
            if (hitbox.Intersects(new Rectangle((int)player.position.X - buffer, (int)player.position.Y, 2, Player.plrHeight)))
                player.colLeft = true;
        }
        public void NPCCollision(NPC npc, int buffer = 4)
        {
            if (!active || type != FoliageID.StoneLarge) return;

            if (hitbox.Intersects(new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height)))
                npc.collide = true;
            //  Directions
            if (hitbox.Intersects(new Rectangle((int)npc.position.X, (int)npc.position.Y - buffer, npc.width, 2)))
                npc.colUp = true;
            if (hitbox.Intersects(new Rectangle((int)npc.position.X, (int)npc.position.Y + npc.height + buffer, npc.width, 2)))
                npc.colDown = true;
            if (hitbox.Intersects(new Rectangle((int)npc.position.X + npc.width + buffer, (int)npc.position.Y, 2, npc.height)))
                npc.colRight = true;
            if (hitbox.Intersects(new Rectangle((int)npc.position.X - buffer, (int)npc.position.Y, 2, npc.height)))
                npc.colLeft = true;
        }
        public void Update()
        {
            if (!discovered && Distance(Main.LocalPlayer.Center, Center) <= Math.Max(range, Light.range * Light.AddLight))
                discovered = true;

            if (!discovered)
            { 
                alpha = 0f;
                return; 
            }
            else if (alpha < 1f)
            {
                alpha += 0.05f;
            }

            //  TODO: Foliage collision needs adjusting
            //foreach (SquareBrush brush in Main.square.Where(t => t != null && t.active() && Proximity(t.Center, range * 2f)))
            //{
            //    Collision(this, brush);
            //}
            //CollideResult();
            //if (velocity != Vector2.Zero)
            //{ 
            //    VelocityMech.BasicSlowClamp(this, 0.3f, 0.1f, 3f);
            //}
        }
        public static void GenerateFoliage(int maxFoliage = 10)
        {
            for (int i = 0; i < maxFoliage; i++)
            {
                Background bg = null;
                do
                {
                    bg = Main.ground[(int)Main.rand.Next(0, Main.ground.Length)];
                } while (bg == null || !bg.active);
                NewFoliage(bg.X, bg.Y, 40, 35, FoliageID.StoneLarge, false, bg);
            }
        }
        public void Draw(SpriteBatch sb)
        {
            if (!discovered) return;
            //  TODO get foliage sprites in gray tone and color them in Draw method
            Color color = default(Color);
            switch (type)
            {
                case FoliageID.Dirt:
                    color = Color.Brown;
                    sb.Draw(Main.MagicPixel, new Rectangle(X, Y, width, height), color * alpha);
                    break;
                case FoliageID.Puddle:
                    color = Color.DeepSkyBlue;
                    sb.Draw(Main.MagicPixel, new Rectangle(X, Y, width, height), color * alpha);
                    break;
                case FoliageID.StoneLarge:
                    color = Color.Gray;
                    sb.Draw(Main.Temporal, new Rectangle(X, Y, width, height), color * alpha);
                    break;
            }
        }
    }

    public class Background : SimpleEntity, IDisposable
    {
        public bool light;
        public new Rectangle hitbox;
        public Foreground foreground;
        public static Texture2D[] BGs = new Texture2D[3];
        private bool onScreen;
        public Background(int x, int y, int width, int height, int style, float fogRange)
        {
            this.width = width;
            this.height = height;
            position = new Vector2(x, y);
            hitbox = new Rectangle(x, y, width, height);
            this.style = style;
            active = true;
            //Main.Fg.Add(foreground = new Foreground(X, y, width, height, this));
            range = fogRange;
            Initialize();
        }
        public static Background NewGround(int x, int y, int width, int height, int style, float range)
        {
            int num = Main.ground.Length - 1;
            for (int i = 0; i < Main.ground.Length; i++)
            {
                if (Main.ground[i] == null || !Main.ground[i].active)
                {
                    num = i;
                    break;
                }
                if (i == num)
                {
                    break;
                }
            }
            Main.ground[num] = new Background(x, y, width, height, style, range);
            Main.ground[num].whoAmI = num; 
            return Main.ground[num];
        }
        private void Initialize()
        {
            switch (style)
            {
                case 0:
                    texture = Background.BGs[BackgroundID.SmallTiles];
                    break;
            }
        }
        public void Update()
        {
            onScreen =
                position.X >= Main.LocalPlayer.position.X - (Main.ScreenWidth + width * 5) / 2 &&
                position.X <= Main.LocalPlayer.position.X + (Main.ScreenWidth + width * 10) / 2 &&
                position.Y >= Main.LocalPlayer.position.Y - (Main.ScreenHeight + width * 5) / 2 &&
                position.Y <= Main.LocalPlayer.position.Y + (Main.ScreenHeight + width * 10) / 2;
            if (!onScreen) return;

            if (!discovered && Distance(Main.LocalPlayer.Center, Center) <= Math.Max(range, Light.range * Light.AddLight))
                discovered = true;

            if (!hitbox.Contains(Main.LocalPlayer.Center))
            {
                //foreground.light = light;
                RoomItemUpdate(false);
            }
            else
            {
                //foreground.light = true;
                RoomItemUpdate(light);
            }
        }
        private void RoomItemUpdate(bool active)
        {
            foreach (Item item in Main.item.Where(t => t != null))
            {
                if (!Inventory.itemList.Contains(item) && hitbox.Contains(item.Center))
                {
                    if (Main.LocalPlayer.Distance(item.Center, Main.LocalPlayer.Center) < range)
                    {
                        item.active = true;
                    }
                }
            }
        }
        public void PreDraw(SpriteBatch sb)
        {
            if (!active || !discovered || texture == null)
            {
                alpha = 0f;
                return;
            }
            if (alpha < 1f)
            {
                alpha += 0.1f;
            }
            else alpha = 1f;
            if ((light || discovered) && onScreen)
            {
                color = Color.Gray;
                sb.Draw(texture, hitbox, DynamicTorch(120f) * alpha);
            }
        }
        public void Dispose()
        {
            Main.ground[whoAmI].active = false;
            Main.ground[whoAmI] = null;
        }
    }
}
