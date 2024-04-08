using FoundationR;
using FoundationR.Lib;
using FoundationR.Rew;
using FoundationR.Loader;
using FoundationR.Ext;
using FoundationR.Headers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;





namespace cotf_rewd
{
    public class GUI : Entity
    {
        public const byte
            MainHand = 0,
            Shield = 1,
            Helm = 2,
            Torso = 3,
            Greaves = 4,
            Boots = 5,
            Cloak = 6,
            Pack = 7,
            Purse = 8,
            Necklace = 9,
            Ring1 = 10,
            Ring2 = 11,
            Gauntlets = 12,
            Belt = 13,
            Bracers = 14,
            MAX = 14,
            Offhand = 1;
        public GUI()
        {
        }
        public int x, y;
        public Rectangle drawBox;
        public int owner;
        public string text;
        public Action onClick;
        private int flag;
        public bool selected;
        public bool unlocked = false;
        public bool disabled;
        public Item Item
        {
            get; set;
        }
        public Item.Type Armory
        {
            get; private set;
        }
        private Player player
        {
            get { return Main.player[owner]; }
        }
        public bool Equipped
        {
            get { return Main.LocalPlayer.Armory[style].Item != null && Main.LocalPlayer.Armory[style].Item.active; }
        }
        public static GUI NewElement(int x, int y, int width, int height, string text, REW texture, int owner, int type, Action onClick)
        {
            int num = 128;
            for (int i = 0; i < num; i++)
            {
                if (Main.gui[i] == null || !Main.gui[i].active)
                {
                    num = i;
                    break;
                }
                if (i == num)
                {
                    return Main.gui[i];
                }
            }
            Main.gui[num] = new GUI();
            Main.gui[num].whoAmI = num;
            Main.gui[num].owner = owner;
            Main.gui[num].active = true;
            Main.gui[num].text = text;
            Main.gui[num].x = x;
            Main.gui[num].y = y;
            Main.gui[num].type = type;
            Main.gui[num].width = width;
            Main.gui[num].height = height;
            Main.gui[num].Name = text.Replace(' ', '_').Replace('/', '_');
            Main.gui[num].texture = texture;
            Main.gui[num].onClick = onClick;
            Main.gui[num].Initialize();
            return Main.gui[num];
        }
        public static GUI NewMenu(int x, int y, int width, int height, string text, REW texture, int owner, int whoAmI, int type, Action onClick)
        {
            Main.gui[whoAmI] = new GUI();
            Main.gui[whoAmI].whoAmI = whoAmI;
            Main.gui[whoAmI].owner = owner;
            Main.gui[whoAmI].text = text;
            Main.gui[whoAmI].x = x;
            Main.gui[whoAmI].y = y;
            Main.gui[whoAmI].width = width;
            Main.gui[whoAmI].height = height;
            Main.gui[whoAmI].Name = text.Replace(' ', '_').Replace('/', '_');
            Main.gui[whoAmI].texture = texture;
            Main.gui[whoAmI].onClick = onClick;
            Main.gui[whoAmI].type = type;
            Main.gui[whoAmI].InitMenu();
            Main.gui[whoAmI].Initialize();
            return Main.gui[whoAmI];
        }
        public static GUI ArmoryGUI(int width, int height, short styleID, Item.Type type, Item.Style style, string name, int owner, int whoAmI, Vector2 v2 = default(Vector2), REW texture = null, Action onClick = null)
        {
            Player player = Main.LocalPlayer;
            player.Armory[whoAmI] = new GUI();
            //player.Armory[whoAmI].position = v2;
            player.Armory[whoAmI].style = styleID;
            player.Armory[whoAmI].Armory = type;
            player.Armory[whoAmI].text = name;
            player.Armory[whoAmI].onClick = onClick;
            player.Armory[whoAmI].texture = Main.MagicPixel;
            player.Armory[whoAmI].width = width;
            player.Armory[whoAmI].height = height;
            player.Armory[whoAmI].Name = name.Replace(' ', '_');
            player.Armory[whoAmI].type = -1;
            player.Armory[whoAmI].Initialize();
            return player.Armory[whoAmI];
        }
        public void InitMenu()
        {
            switch (type)
            {
                case LightSpell:
                    text = ID.Light;
                    Name = text;
                    onClick = new Action(() => 
                    {
                        Light.range = 128f;
                        player.justCastLight = true;
                        //  In future applies to room light
                        //var bg = Main.ground.Where(t => t != null && t.hitbox.Contains(Main.LocalPlayer.Center.X, Main.LocalPlayer.Center.Y));
                        //if (bg.Count() > 0)
                        //{
                        //    bg.First().light = true;
                        //}
                    });
                    break;
                case PhaseDoor:
                    text = ID.PhaseDoor;
                    Name = text;
                    onClick = new Action(delegate 
                    {
                        if (!player.Phasing)
                            player.Phasing = true;
                    });
                    break;
                default:
                    break;
            }
        }

        public class ID
        {
            public const string
                DEBUG = "DEBUG",
                SwordSwipe = "Swipe",
                SwordSwing = "Swing",
                Light = "Light",
                PhaseDoor = "Phase Door";
        }
        public const byte
            DEBUG = 0,
            SwordSwipe = 1,
            SwordSwing = 2,
            Screenshot = 3,
            SkillMenu = 5,
            LightSpell = 7,
            PhaseDoor = 8;
        public const byte
            Nothing = 0,
            SmallPack = 15,
            MediumPack = 30,
            LargePack = 45,
            SmallChest = 20,
            MediumChest = 45,
            LargeChest = 65,
            EnchantAddSpace = 7,
            CursedLoseSpace = 7;

        public void Initialize()
        {
            if (type >= 0)
            {
                switch (type)
                {
                    case DEBUG:
                        text = "DEBUG";
                        unlocked = true;
                        onClick = delegate 
                        {
                            NPC.NewNPC(0, 0, NPCID.Necrosis, default(Color), 40);
                        };
                        break;
                    case SwordSwipe:
                        text = "Swipe";
                        Name = text;
                        unlocked = true;
                        break;
                    case SwordSwing:
                        text = "Swing";
                        Name = text;
                        unlocked = true;
                        break;
                    case Screenshot:
                        text = "Screenshot";
                        Name = text;
                        unlocked = true;
                        onClick = delegate { screenshot = !screenshot; };
                        break;
                    case SkillMenu:
                        unlocked = true;
                        break;
                    case LightSpell:
                        unlocked = true;
                        break;
                    case PhaseDoor:
                        unlocked = true;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                var box = Inventory.Armor;
                const int spacing = 10;
                const int size = 32;
                const int halfSize = 16;
                const int yStart = 120;
                int total = spacing + size;
                switch (style)
                {
                    case Ring1:
                        position = new Vector2(box.X + spacing, box.Y + yStart - total);
                        goto default;
                    case Ring2:
                        position = new Vector2(box.X + box.Width - total, box.Y + yStart - total);
                        goto default;
                    case MainHand:
                        position = new Vector2(box.X + spacing, box.Y + yStart);
                        goto default;
                    case Shield:
                        position = new Vector2(box.X + box.Width - total, box.Y + yStart);
                        goto default;
                    case Bracers:
                        position = new Vector2(box.X + spacing, box.Y + yStart + total);
                        goto default;
                    case Greaves:
                        position = new Vector2(box.X + spacing, box.Y + yStart + (total * 2));
                        goto default;
                    case Gauntlets:
                        position = new Vector2(box.X + box.Width - total, box.Y + yStart + total);
                        goto default;
                    case Helm:
                        position = new Vector2(box.X + box.Width / 2 - halfSize, box.Y + spacing);
                        goto default;
                    case Cloak:
                        position = new Vector2(box.X + box.Width / 2 + size - spacing, box.Y + spacing);
                        goto default;
                    case Necklace:
                        position = new Vector2(box.X + box.Width / 2 - (total * 2 + halfSize), box.Y + spacing);
                        goto default;
                    case Torso:
                        position = new Vector2(box.X + box.Width / 2 - (total + halfSize), box.Y + spacing);
                        goto default;
                    case Boots:
                        position = new Vector2(box.X + box.Width / 2 - halfSize, box.Y + box.Height - total);
                        goto default;
                    case Belt:
                        position = new Vector2(box.X + box.Width - total, box.Y + yStart + (total * 2));
                        goto default;
                    case Purse:
                        position = new Vector2(box.X + box.Width - total, box.Y + yStart + (total * 3));
                        goto default;
                    case Pack:
                        position = new Vector2(box.X + box.Width - total, box.Y + yStart + (total * 4));
                        goto default;
                    default:
                        hitbox = new Rectangle((int)position.X, (int)position.Y, width, height);
                        break;
                }
            }
        }

        public void UpdateArmory()
        {
            if (!Inventory.open || Item?.equipped == false)
                return;
            if (drawBox.Contains(Main.MousePosition) && player.LeftMouse())
            {
                Menu.NewMenu(position.X + width - Main.ScreenX, position.Y - Main.ScreenY, new string[] { "Unequip", "Close" }, this);
            }
        }
        public void Update()
        {
            if (!active)
                return;
            position = new Vector2(x - Main.ScreenX, y - Main.ScreenY);
            hitbox = new Rectangle(x, y, width, height);
            
            if (!unlocked)
                return;

            if (Game.mouseLeft && flag % 2 == 0 && whoAmI < SkillMenu)
            {
                flag++;
                foreach (GUI menu in Main.skill)
                {
                    if (menu.active && menu.unlocked)
                    {
                        if (menu.hitbox.Contains(Main.MousePosition.X, Main.MousePosition.Y) && selected)
                        {
                            MenuReplace(menu, this, menu.onClick, menu.Name, menu.texture, menu.text, menu.whoAmI);
                            return;
                        }
                    }
                }
                if (hitbox.Contains(Main.MousePosition.X, Main.MousePosition.Y) && Main.gui.ToList().Contains(this))
                {
                    foreach (GUI g in Main.gui.Where(t => t != null))
                        g.selected = false;
                    selected = true;
                }
                if (selected)
                    onClick?.Invoke();
            }
            if (!Game.mouseLeft && flag % 2 == 1)
                flag = 0;
        }
        private void MenuReplace(GUI menu, GUI g, Action click, string name, REW tex, string text, int type)
        {
            menu.Name = g.Name;
            menu.onClick = g.onClick;
            menu.texture = g.texture;
            menu.text = g.text;
            menu.type = g.type;
            g.Name = name;
            g.onClick = click;
            g.texture = tex;
            g.text = text;
            g.type = type;
        }
        public static bool screenshot = false;
        public void DrawArmory(RewBatch rb)
        {
            if (!Inventory.open)
                return;

            drawBox = new Rectangle((int)position.X - (int)Main.ScreenX, (int)position.Y - (int)Main.ScreenY, width, height);

            rb.Draw(texture, drawBox, Color.Silver);

            if (Item != null && Item?.equipped == true)
                rb.Draw(Item.texture, drawBox, Item.color);

            //  DEBUG
            //gfx.DrawRectangle(Item != null && Item?.equipped == true ? System.Drawing.Pens.Gold : Pens.Gray, drawBox.GetSDRectangle());

            if (drawBox.Contains(Main.WorldMouse))
                rb.DrawString(Game.Font[FontID.Arial], text, (int)(position.X - Main.ScreenX), (int)(position.Y - Main.ScreenY) + height, 100, 16, Color.White);
        }
        public void Draw(RewBatch sb)
        {
            if (!active)
                return;
            //  DEBUG
            sb.Draw(texture, new Rectangle((int)position.X, (int)position.Y, width, height), Color.White);
            //  DEBUG
            //gfx.DrawRectangle(selected && unlocked ? System.Drawing.Pens.Gold : Pens.Gray, new System.Drawing.Rectangle((int)position.X, (int)position.Y, hitbox.Width, hitbox.Height));
        }
        public void DrawMenuText(RewBatch sb)
        {
            if (!active)
                return;

            if (hitbox.Contains(Main.MousePosition.X, Main.MousePosition.Y))
                sb.DrawString(Game.Font[FontID.Arial], text, (int)(position.X + new Vector2(0, height).X), (int)(position.Y + new Vector2(0, height).Y), 100, 16, Color.White);
        }
        public static GUI GetElement(string name)
        {
            var gui = Main.gui.Where(t => t != null && t.text.Contains(name));
            if (gui.Count() > 0)
                return gui.First();
            else return null;
        }
    }
}
