using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePort
{
    public class Item : SimpleEntity, IDisposable
    {
        public Item()
        {
        }
        public enum Style : byte
        {
            Empty = 0,
            Weapon_OneHand = 1,
            Weapon_TwoHand = 2,
            Cloak = 3,
            Helm = 4,
            Greaves = 5,
            Bracers = 6,
            Torso = 7,
            Ring = 8,
            Necklace = 9,
            Consumable = 10,
            Storage = 11,
            Purse = 12,
            Coin = 13,
            Boot = 14,
            Belt = 15,
            OffHand = 16,
            Gloves = 17
        }
        public const byte Owner_World = 255;
        public enum Type : byte
        {
            Fist = 0,
            Sword_OneHand = 1,
            Sword_TwoHand = 2,
            Shield = 3,
            Helm = 4,
            Torso = 5,
            Greaves = 6,
            Boots = 7,
            Cloak = 8,
            Pack_Chest = 9,
            Pack_Backpack = 10,
            Purse = 11,
            Necklace = 12,
            Ring = 13,
            Gauntlets = 14,
            Potion = 15,
            Scroll = 16,
            Polearm = 17,
            Mace_OneHand = 18,
            Mace_TwoHand = 19,
            Spear = 20,
            Dagger_Dirk = 21,
            Coin_Iron = 22,
            Coin_Copper = 23,
            Coin_Silver = 24,
            Coin_Gold = 25,
            Coin_Platinum = 26,
            Flail = 27,
            Axe_OneHand = 28,
            Axe_TwoHand = 29,
            Mallet = 30,
            Hammer = 31,
            Bracers = 32,
            Belt = 33,
            Torch = 34
        }
        

        public Player player
        {
            get { return Main.player[owner]; }
        }
        public bool MouseItem
        {
            get { return owner != Owner_World && player.mouseItem != null && player.mouseItem == this; }
        }
        public bool visible;
        public bool menu;
        public Location placement;
        public Vector2 saved;
        private Menu MENU;
        public bool equipped;
        public bool inUse = true;
        public const int DrawSize = 28;
        public Purse purse;
        public bool Enchanted { get; set; }
        public bool Cursed { get; set; }
        public enum Location
        {
            Floor = 0,
            Inventory = 1
        }
        public string[] Description
        {
            get; private set;
        }

        public static Item NewItem(int x, int y, int width, int height, int owner, int styleID, Type type, Style style)
        {
            int num = Main.item.Length - 1;
            for (int i = 0; i < Main.item.Length; i++)
            {
                if (Main.item[i] == null || !Main.item[i].active)
                {
                    num = i;
                    break;
                }
                if (i == num)
                {
                    break;
                }
            }
            Main.item[num] = new Item();
            Main.item[num].active = true;
            Main.item[num].X = x;
            Main.item[num].Y = y;
            Main.item[num].position = new Vector2(x, y);
            Main.item[num].saved = new Vector2(x, y);
            Main.item[num].width = width;
            Main.item[num].height = height;
            Main.item[num].style = styleID;
            Main.item[num].itemType = type;
            Main.item[num].itemStyle = style;
            Main.item[num].owner = owner;
            Main.item[num].whoAmI = num;
            Main.item[num].Initialize();
            return Main.item[num];
        }
        public static Item NewItem(int x, int y, int width, int height, int owner, int styleID)
        {
            int num = Main.item.Length - 1;
            for (int i = 0; i < Main.item.Length; i++)
            {
                if (Main.item[i] == null || !Main.item[i].active)
                {
                    num = i;
                    break;
                }
                if (i == num)
                {
                    break;
                }
            }
            Main.item[num] = new Item();
            Main.item[num].active = true;
            Main.item[num].X = x;
            Main.item[num].Y = y;
            Main.item[num].position = new Vector2(x, y);
            Main.item[num].saved = new Vector2(x, y);
            Main.item[num].width = width;
            Main.item[num].height = height;
            Main.item[num].style = styleID;
            Main.item[num].itemType = GetType(styleID);
            Main.item[num].itemStyle = GetStyle(styleID);
            Main.item[num].owner = owner;
            Main.item[num].whoAmI = num;
            Main.item[num].Initialize();
            return Main.item[num];
        }
        private static Type GetType(int styleID)
        {
            switch (styleID)
            {
                case ItemID.SilverLongsword:
                case ItemID.RedRust:
                case ItemID.Dirk:
                    return Type.Sword_OneHand;
                case ItemID.Zweihander:
                    return Type.Sword_TwoHand;
                case ItemID.SmallPack:
                case ItemID.MediumPack:
                case ItemID.LargePack:
                    return Type.Pack_Backpack;
                case ItemID.SmallChest:
                case ItemID.MediumChest:
                case ItemID.LargeChest:
                    return Type.Pack_Chest;
                case ItemID.Torch:
                    return Type.Torch;
                case ItemID.Purse:
                    return Type.Purse;
                default:
                    return Type.Fist;
            }
        }
        private static Style GetStyle(int styleID)
        {
            switch (styleID)
            {
                case ItemID.RedRust:
                case ItemID.SilverLongsword:
                case ItemID.Dirk:
                    return Style.Weapon_OneHand;
                case ItemID.Zweihander:
                    return Style.Weapon_TwoHand;
                case ItemID.SmallPack:
                case ItemID.MediumPack:
                case ItemID.LargePack:
                case ItemID.SmallChest:
                case ItemID.MediumChest:
                case ItemID.LargeChest:
                    return Style.Storage;
                case ItemID.Torch:
                    return Style.OffHand;
                case ItemID.Purse:
                    return Style.Purse;
                default:
                    return Style.Empty;
            }
        }
        
        internal void Initialize()
        {
            switch (itemType)
            {
                case Type.Torch:
                    type = GUI.Offhand;
                    Name = "Torch";
                    damage = 0;
                    texture = Main.Temporal;
                    color = Color.Orange;
                    goto default;
                case Type.Sword_OneHand:
                    type = GUI.MainHand;
                    
                    // <--
                    switch (style)
                    {
                        case ItemID.RedRust:
                            Name = "Red Rust";
                            damage = 2;
                            texture = Main.Temporal;
                            knockBack = 0f;
                            color = Color.Red;
                            break;
                        case ItemID.SilverLongsword:
                            Name = "Silver Longsword";
                            damage = 5;
                            texture = Main.Temporal;
                            knockBack = 1f;
                            color = Color.Silver;
                            break;
                        case ItemID.Dirk:
                            Name = "Dirk";
                            damage = 3;
                            texture = Main.Temporal;
                            knockBack = 0.5f;
                            color = Color.White;
                            break;
                        default:
                            break;
                    }
                    // -->
                    goto default;
                case Type.Sword_TwoHand:
                    switch (style)
                    {
                        case ItemID.Zweihander:
                            Name = "Zweihander";
                            damage = 10;
                            texture = Main.Temporal;
                            knockBack = 2f;
                            color = Color.Yellow;
                            break;
                    }
                    goto default;
                case Type.Pack_Backpack:
                    type = GUI.Pack;
                    // <--
                    switch (style)
                    {
                        case ItemID.SmallPack:
                            Name = "Small Backpack";
                            damage = -1;
                            goto default;
                        case ItemID.MediumPack:
                            Name = "Medium Backpack";
                            damage = -1;
                            goto default;
                        case ItemID.LargePack:
                            Name = "Large Backpack";
                            damage = -1;
                            goto default;
                        default:
                            texture = Main.Temporal;
                            color = Color.LightBlue;
                            break;
                    }
                    // -->
                    goto default;
                case Type.Pack_Chest:
                    type = GUI.Pack;
                    // <--
                    switch (style)
                    {
                        case ItemID.SmallChest:
                            Name = "Small Chest";
                            damage = -1;
                            goto default;
                        case ItemID.MediumChest:
                            Name = "Medium Chest";
                            damage = -1;
                            goto default;
                        case ItemID.LargeChest:
                            Name = "Large Chest";
                            damage = -1;
                            goto default;
                        default:
                            color = Color.Brown;
                            texture = Main.Temporal;
                            break;
                    }
                    // -->
                    goto default;
                case Type.Purse:
                    purse = new Purse(true);
                    type = GUI.Purse;
                    Name = "Purse";
                    damage = 0;
                    texture = Main.Temporal;
                    color = Color.Purple;
                    goto default;
                case Type.Coin_Iron:
                    color = Dialog.CoinColor[0];
                    Name = "Iron coin";
                    texture = Main.Temporal;
                    break;
                case Type.Coin_Copper:
                    color = Dialog.CoinColor[1];
                    Name = "Copper coin";
                    texture = Main.Temporal;
                    break;
                case Type.Coin_Silver:
                    color = Dialog.CoinColor[2];
                    Name = "Silver coin";
                    texture = Main.Temporal;
                    break;
                case Type.Coin_Gold:
                    color = Dialog.CoinColor[3];
                    Name = "Gold coin";
                    texture = Main.Temporal;
                    break;
                case Type.Coin_Platinum:
                    color = Dialog.CoinColor[4];
                    Name = "Platinum coin";
                    texture = Main.Temporal;
                    break;
                default:
                    Description = new string[]
                    {
                        "Name: " + Name,
                        damage > 0 ? "Damage: " + damage : "Damage: N/a"
                    };
                    break;
            }
        }

        public void Update()
        {
            if (!active)
                return;

            if (owner == Owner_World)
            {
                position = new Vector2(X, Y);
                if (!discovered)
                {
                    discovered = Main.LocalPlayer.Distance(Center, Main.LocalPlayer.Center) < Light.range;
                    return;
                }
            }

            if (!Inventory.open)
            {
                position = saved;
                if (MENU != null) MENU.active = false;
                menu = false;
            }
            

            if (!Inventory.listUpdate)
            {
                if (Inventory.itemProximate.Contains(this))
                    placement = Location.Floor;
                if (Inventory.itemList.Contains(this))
                    placement = Location.Inventory;
            }

            //  In World [depracated]
            //PlayerAcquire();

            if (menu) 
            {
                MENU.active = true;
            }
        }
        public void Draw(SpriteBatch sb)
        {
            if (!active || !discovered || Inventory.itemList.Contains(this) || Inventory.itemProximate.Contains(this))
                return;
            if (owner == Owner_World && texture != null) sb.Draw(texture, new Vector2(X, Y), color);
        }
        public void DrawInventory(int x, int y, SpriteBatch sb, bool onScreen = true)
        {
            //  Draw Inventory Items
            if (!active)
                return;
            position = new Vector2(x, y);
            if (visible = onScreen)
            {
                if (texture != null) sb.Draw(texture, new Rectangle(X, Y, DrawSize, DrawSize), color);
            }
            PostMenuPositionAdjust();
        }
        private void PostMenuPositionAdjust()
        {
            //  Inventory Menu Interaction
            if (visible && (Inventory.itemList.Contains(this) || Inventory.itemProximate.Contains(this)) && hitbox.Contains(Main.WorldMouse))
            {
                if (Main.LocalPlayer.LeftMouse())
                {
                    Menu.Current = this;
                    MENU = Menu.NewMenu(position.X + width, position.Y, new string[] { "Pickup", "Equip", "Drop", "Combine", "Close" });

                    foreach (var m in Inventory.itemProximate.Where(t => t != null))
                        m.menu = false;
                    foreach (var n in Inventory.itemList.Where(t => t != null))
                        n.menu = false;
                    menu = true;
                }
            }
        }
        public void Unequip()
        {
            equipped = false;
        }
        public void Kill()
        {
            Main.item[whoAmI].active = false;
        }
        public void Dispose()
        {
            Kill();
            Main.item[whoAmI] = null;
        }
    }
    public class Purse
    {
        public Item item;
        public short
            copper,
            silver,
            gold,
            platinum;
        public Purse(bool world)
        {
            if (world)
            {
                //  TODO: change random values based on floor level
                copper = (short)Main.rand.Next(50);
                silver = (short)Main.rand.Next(20);
                gold = (short)Main.rand.Next(10);
                platinum = (short)Main.rand.Next(2);
            }
        }
        public void Combine(Purse other)
        {
            this.copper += other.copper;
            this.silver += other.silver;
            this.gold += other.gold;
            this.platinum += other.platinum;
        }
    }
}
