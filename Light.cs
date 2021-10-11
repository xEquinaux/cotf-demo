using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePort
{
    public class Light
    {
        public static Light Instance;
        public bool active;
        public bool lit;
        public int x, y, width = 8, height = 8;
        //  Old value: 8
        public static int Size = 10;
        public int size;
        public Vector2 position;
        public Rectangle hitbox => new Rectangle(x, y, width, height);
        public Vector2 Center => new Vector2(x + width / 2, y + height / 2);
        public static IList<SimpleEntity> entity = new List<SimpleEntity>();
        public static IList<Light> light = new List<Light>();
        public static float range = 100f;
        public Background bg;
        public static bool updating;
        public bool onScreen;
        public const float AddLight = 1.2f;
        public static Texture2D fow;
        public static Color TorchLight = Color.Orange;
        public void Draw(SpriteBatch sb)
        {
            if (!lit /*&& bg?.light == false*/ && onScreen)
                sb.Draw(fow, new Rectangle((int)position.X - 10, (int)position.Y - 10, Size * 3, Size * 3), Color.Black * Math.Min(Main.LocalPlayer.Distance(Center) / (range * 2f), 1f));
            updating = false;

            //  Comment out for fog of war
            if (!Main.LocalPlayer.justCastLight)
                lit = false;
        }
        public void Update()
        {
            if (updating) return;

            onScreen =
                position.X >= Main.LocalPlayer.position.X - (Main.ScreenWidth + Size * 5) / 2 &&
                position.X <= Main.LocalPlayer.position.X + (Main.ScreenWidth + Size * 10) / 2 &&
                position.Y >= Main.LocalPlayer.position.Y - (Main.ScreenHeight + Size * 5) / 2 &&
                position.Y <= Main.LocalPlayer.position.Y + (Main.ScreenHeight + Size * 10) / 2;
            if (!onScreen) return;
            
            foreach (var ent in entity)
            {
                var center = ent.position + new Vector2(ent.width / 2, ent.height / 2);
                if (ent.Distance(Center, center) < range)
                {
                    if (ent.active)
                    {
                        if (ent.owner != Item.Owner_World)
                        {
                            if (!lit)
                            {
                                //  Player using torch fx -- looks ugly around world torches
                                var armory = Main.LocalPlayer.Armory[GUI.Offhand];
                                bool equipped = armory.Item != null && armory.Item.equipped && armory.Item.inUse;
                                lit = equipped && armory.Item.itemType == Item.Type.Torch;
                                //lit = true;
                            }
                        }
                        else
                        {
                            if (!ent.discovered)
                            {
                                foreach (Background ground in Main.ground.Where(t => t != null && !t.discovered && t.Distance(ent.Center, t.Center) < range))
                                {
                                    ground.discovered = true;
                                }
                                foreach (SquareBrush sq in Main.square.Where(t => t != null && !t.discovered && NPC.Distance(ent.Center, t.Center) < range))
                                {
                                    sq.discovered = true;
                                }
                                ent.discovered = true;
                            }
                            lit = true;
                        }
                    }
                }
            }

            //  Static player light
            //position = new Vector2(x - Main.ScreenX, y - Main.ScreenY);
        }
        public static SimpleEntity NewTorch(Vector2 position, float range, int owner = Item.Owner_World)
        {
            //  Unoptimized: causes large slowdown
            SimpleEntity ent;
            entity.Add(ent = new SimpleEntity()
            {
                owner = owner,
                active = true,
                position = position,
                range = range
            });
            return ent;
        }
        public static Light NewLight(int x, int y, int size, bool active, bool lit = false)
        {
            int num = 1000;
            for (int i = 0; i < Main.light.Length; i++)
            {
                if (Main.light[i] == null)
                {
                    num = i;
                    break;
                }
                if (i == num)
                {
                    break;
                }
            }
            Main.light[num] = new Light();
            Main.light[num].active = active;
            Main.light[num].position = new Vector2(x, y);
            Main.light[num].width = size;
            Main.light[num].height = size;
            Main.light[num].size = size;
            return Main.light[num];
        }
        public static void Create(int x, int y, int width, int height, Background bg)
        {
            updating = true;
            for (int i = x; i < x + width; i += Size)
            {
                for (int j = y; j < y + height; j += Size)
                {
                    light.Add(new Light()
                    {
                        position = new Vector2(i, j),
                        x = i,
                        y = j,
                        active = true,
                        lit = false,
                        bg = bg
                    });
                }
            }
        }
    }
}
