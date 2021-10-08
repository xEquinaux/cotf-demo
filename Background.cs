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
                sb.Draw(texture, hitbox, Color.White * alpha);
        }
        public void Dispose()
        {
            Main.ground[whoAmI].active = false;
            Main.ground[whoAmI] = null;
        }
    }
}
