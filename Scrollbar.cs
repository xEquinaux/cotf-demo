using FoundationR;
using FoundationR.Lib;
using FoundationR.Rew;
using FoundationR.Loader;
using FoundationR.Ext;
using FoundationR.Headers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;





namespace cotf_rewd
{
    public class ScrollBar : SimpleEntity
    {
        public static ScrollBar Instance;
        public ScrollBar()
        {
            Instance = this;
        }
        public static ScrollBar[] scroll = new ScrollBar[2];
        public float value;
        public Rectangle container;
        public static byte FLOOR = 0, INVENTORY = 1;
        public const int cursorSize = 12;
        public new Rectangle hitbox;
        public void Initialize()
        {
            scroll[0] = new ScrollBar()
            {
                X = (int)Inventory.Equipment.X + (int)Inventory.Equipment.Width - width,
                Y = (int)Inventory.Equipment.Y,
                height = (int)Inventory.Equipment.Height,
                container = Inventory.Equipment,
                whoAmI = 0
            };
            scroll[1] = new ScrollBar()
            {
                X = (int)Inventory.Items.X + (int)Inventory.Items.Width - width,
                Y = (int)Inventory.Items.Y,
                height = (int)Inventory.Items.Height,
                container = Inventory.Items, 
                whoAmI = 1   
            };
        }
        public static void Update()
        {
            var floor = scroll[FLOOR];
            var inv = scroll[INVENTORY];

            const int width = 12;
            floor.hitbox = new Rectangle(scroll[0].X - (int)Main.ScreenX, scroll[0].Y - (int)Main.ScreenY, width, scroll[0].height);
            inv.hitbox = new Rectangle(scroll[1].X - (int)Main.ScreenX, scroll[1].Y - (int)Main.ScreenY, width, scroll[1].height);

            Instance.Scroll(Inventory.Equipment, floor);
            Instance.Scroll(Inventory.Items, inv);
        }
        private void Scroll(Rectangle container, ScrollBar bar)
        {
            if (container.Contains(Main.WorldMouse.X, Main.WorldMouse.Y))
            {
                if (Main.LocalPlayer.KeyDown(Key.Down))
                {
                    if (bar.value * bar.hitbox.Height < bar.hitbox.Height - 12)
                    {
                        bar.value += 0.04f;
                    }
                }
                if (Main.LocalPlayer.KeyDown(Key.Up))
                {
                    if (bar.value > 0f)
                    {
                        bar.value -= 0.04f;
                    }
                    else bar.value = 0f;
                }
            }
        }
        public void Draw(RewBatch rb)
        {
            const int size = 12;

            //  System.Drawing
            //gfx.FillRectangles(Brushes.GhostWhite, new RectangleF[] { scroll[0].hitbox.GetRectangleF(), scroll[1].hitbox.GetRectangleF() });
            //gfx.FillRectangles(Brushes.Blue, new RectangleF[] 
            //{ 
            //    new RectangleF(scroll[0].hitbox.x, scroll[0].hitbox.y + scroll[0].hitbox.Height * scroll[0].value, width, width),
            //    new RectangleF(scroll[1].hitbox.x, scroll[1].hitbox.y + scroll[1].hitbox.Height * scroll[1].value, width, width)
            //});

            for (int i = 0; i < 2; i++)
            {
                rb.Draw(Main.MagicPixel, scroll[i].hitbox, Color.GhostWhite);
                rb.Draw(Main.MagicPixel, new Rectangle(scroll[i].hitbox.X, (int)(scroll[i].hitbox.Y + scroll[i].hitbox.Height * scroll[i].value), size, size), Color.Blue);
            }
        }
    }
}
