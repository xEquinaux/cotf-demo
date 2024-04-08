using FoundationR;
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
    public class Dialog : SimpleEntity, IDisposable
    {
        public static Dialog Instance;
        internal Item Selected
        {
            get; private set;
        }
        internal string[] contents;
        public Rectangle Hitbox
        {
            get; private set;
        }
        public REW CoinIcon => Main.Temporal;
        private string[] coinName => new string[] { "0", "0", "0", "0" };
        public static Color[] CoinColor;
        public static Dialog NewDialog(int x, int y, string[] contents, Item item)
        {
            Instance?.Dispose();
            Instance = new Dialog()
            {
                position = new Vector2(x, y),
                contents = contents
            };
            Instance.Selected = item;
            Instance.Initialize();
            Instance.active = true;
            return Instance;
        }
        private void Initialize()
        {
            int width = 0;
            const int textHeight = 12;
            const int textWidth = 12;
            if (Selected.itemStyle == Item.Style.Purse)
            {
                width = 32 * 5;
                height = 32;
                Hitbox = new Rectangle((int)position.X, (int)position.Y, width, height);
                texture = Main.Temporal;
            }
            else if (contents != null)
            {
                for (int i = 0; i < contents.Length; i++)
                {
                    width = contents[i].Length * textWidth;
                    if (contents[i].Length > contents[i].Length)
                        width = contents[i].Length * textWidth;
                    Hitbox = new Rectangle((int)position.X, (int)position.Y, width, (i + 1) * textHeight);
                }
            }
        }

        public void Update()
        {
            foreach (Item i in Inventory.itemList)
            {
                if (i == null) continue;
                var box = new Rectangle(i.hitbox.X, i.hitbox.Y, i.hitbox.Width + 4, i.hitbox.Height + 4);
                if (i.visible && (active = box.Contains(Main.WorldMouse)))
                {
                    Dialog.NewDialog((int)i.position.X, (int)i.position.Y, i.Description, i);
                    return;
                }
            }
            foreach (Item i in Inventory.itemProximate)
            {
                if (i == null) continue;
                var box = new Rectangle(i.hitbox.X, i.hitbox.Y, i.hitbox.Width + 4, i.hitbox.Height + 4);
                if (i.visible && (active = box.Contains(Main.WorldMouse)))
                {
                    Dialog.NewDialog((int)i.position.X, (int)i.position.Y, i.Description, i);
                    return;
                }
            }
            foreach (GUI g in Main.LocalPlayer.Armory.Where(t => t != null))
            {
                if (g.Item == null) continue;
                if (active = g.hitbox.Contains(Main.MousePosition))
                {
                    Dialog.NewDialog(g.drawBox.X, g.drawBox.Y, g.Item.Description, g.Item);
                    return;
                }
            }
        }
        public void Draw(RewBatch rb)
        {
            if (!active || Menu.Select?.active == true) return;

            var purse = Main.LocalPlayer.Armory[GUI.Purse];
            var item = purse.Item?.purse;
            if (purse.Item != null && item != null && purse.Item.equipped && (Item.Style)Selected?.itemStyle == Item.Style.Purse)
            {
                rb.Draw(Main.MagicPixel, Hitbox.X, Hitbox.Y, Color.Black);
                for (int i = 0; i < CoinColor.Length; i++)
                {
                    rb.Draw(texture, new Rectangle(Hitbox.X + 32 * i, Hitbox.Y, 32, 32), CoinColor[i]);
                    rb.DrawString(Game.Font[FontID.Arial], new string[] { "0", item.copper.ToString(), item.silver.ToString(), item.gold.ToString(), item.platinum.ToString() }[i], Hitbox.X + 32 * i, Hitbox.Y, 16, 64, Color.Red);//, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                }
                return;
            }

            if (contents != null)
            {
                rb.Draw(Main.MagicPixel, Hitbox.X, Hitbox.Y, Color.Black);
                for (int j = 0; j < contents.Length; j++)
                {
                    rb.DrawString(Game.Font[FontID.Arial], contents[j], position + new Vector2(0, j * 12), Color.Silver);
                }
            }
        }

        public void Dispose() 
        { 
        }
    }
}
