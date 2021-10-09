﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePort
{
    public class Inventory : Main
    {
        public static int Width
        {
            get { return Main.ScreenWidth; }
        }
        public static int Height
        {
            get { return Main.ScreenHeight; }
        }
        public static Rectangle Armor
        {
            get { return new Rectangle(Width / 10 - (int)Main.ScreenX, 30 - (int)Main.ScreenY, Width / 3, Height - 60); }
        }
        public static Rectangle Equipment
        {
            get { return new Rectangle(Width / 2 + 30 - (int)Main.ScreenX, 30 - (int)Main.ScreenY, Width / 3, Height / 3 + 30); }
        }
        public static Rectangle Items
        {
            get { return new Rectangle(Width / 2 + 30 - (int)Main.ScreenX, Height / 2 + 30 - (int)Main.ScreenY, Width / 3, Height / 3 + 15); }
        }
        public new Player player;
        public static bool open = false;
        internal static bool listUpdate;
        byte flag;
        const byte MaxItems = 72;
        public Inventory(Player player)
        {
            this.player = player;
            
            //  DEBUG
            for (int i = 0; i < 40; i++)
            {
                itemList.Add(Item.NewItem(0, 0, 24, 24, 0, Main.rand.Next(ItemID.Purse + 1)));
                //itemList.Add(Item.NewItem(0, 0, 24, 24, 0, ItemID.LargePack, Item.Type.Pack_Backpack, Item.Style.Storage));
                //itemList.Add(Item.NewItem(0, 0, 24, 24, 0, ItemID.LargeChest, Item.Type.Pack_Chest, Item.Style.Storage));
                //Item.NewItem(Main.rand.Next(0, Main.LevelWidth), Main.rand.Next(0, Main.LevelHeight), 24, 24, Item.Owner_World, Item.RedRust, Item.Type.Sword_OneHand, Item.Style.Weapon_OneHand);
                //Item.NewItem(Main.rand.Next(0, Main.LevelWidth), Main.rand.Next(0, Main.LevelHeight), 24, 24, Item.Owner_World, Item.SmallPack, Item.Type.Pack_Backpack, Item.Style.Storage);
                //Item.NewItem(Main.rand.Next(0, Main.LevelWidth), Main.rand.Next(0, Main.LevelHeight), 24, 24, Item.Owner_World, Item.MediumPack, Item.Type.Pack_Backpack, Item.Style.Storage);
            }
        }
        public static IList<Item> itemList = new List<Item>();
        public static IList<Item> itemProximate = new List<Item>();
        public new void Update()
        {
            //  Item inventory display from OWNER_WORLD
            if (KeyDown(Keys.O) && flag % 2 == 0)
            {
                foreach (Item i in Main.item)
                {
                    if (i != null && i.active && i.owner == Item.Owner_World && Entity.Proximity(player.Center, i.Center, 64f))
                    {
                        itemProximate.Add(i);
                    } 
                }
                open = !open;
                flag = 1;
            }
            if (KeyUp(Keys.O) && flag % 2 == 1)
            {
                flag = 0;
            }

            if (!open)
            {
                itemProximate.Clear();
                return;
            }

            if (!Inventory.listUpdate)
            {
                foreach (Item i in itemProximate)
                {
                    if (i == null) continue;
                    var floor = ScrollBar.scroll[ScrollBar.FLOOR];
                    i.position = new Vector2(i.X, i.Y - (int)(floor.value * floor.container.Height));
                }
                foreach (Item i in itemList)
                {
                    if (i == null) continue;
                    var inv = ScrollBar.scroll[ScrollBar.INVENTORY];
                    i.position = new Vector2(i.X, i.Y - (int)(inv.value * inv.container.Height));
                }
            }
        }
        public new void PreDraw(SpriteBatch sb)
        {
            if (!open)
                return;
            sb.Draw(Background.BGs[BackgroundID.Temp], new Rectangle(0 - (int)Main.ScreenX, 0 - (int)Main.ScreenY, Width, Height), Color.White);
            var array = new Rectangle[] { Items, Equipment, Armor };
            for (int i = 0; i < 3; i++)
            {
                sb.Draw(Main.MagicPixel, array[i], Color.White);
            }
        }
        public void Draw(SpriteBatch sb)
        {
            if (!open)
                return;
            if (!listUpdate)
            {
                DrawInventory(itemList, Items, sb);
                DrawInventory(itemProximate, Equipment, sb);
            }
            listUpdate = false;
        }
        public float[] value
        {
            get {
                return new float[]
                { 
                    ScrollBar.scroll[ScrollBar.FLOOR].value, ScrollBar.scroll[ScrollBar.INVENTORY].value
                };
            }
        }
        private void DrawInventory(IList<Item> list, Rectangle region, SpriteBatch sb)
        {
            const int offset = 6;
            int c = (int)region.X + offset, r = (int)region.Y + offset;
            foreach (Item i in list)
            {
                if (i == null) continue;
                int n = r - (int)(value[(int)i.placement] * region.Height);
                i.DrawInventory(c, n, sb, n < region.Height + region.Y - 24 && n > region.Y);
                c += 32;
                if (c > region.Width + region.X - 24)
                {
                    r += 32;
                    c = (int)region.X + offset;
                }
            }
        }
        public new bool KeyDown(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key);
        }
        public new bool KeyUp(Keys key)
        {
            return Keyboard.GetState().IsKeyUp(key);
        }
        public bool LeftMouse()
        {
            return player.LeftMouse();
        }
        public bool RightMouse()
        {
            return player.RightMouse();
        }
    }
}