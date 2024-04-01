using FoundationR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;





namespace cotf_rewd
{
    public class Menu : SimpleEntity, IDisposable
    {
        public static Menu Select
        {
            get; private set;
        }
        internal string[] options;
        private const int textHeight = 12;
        internal static Item Current;
        internal static string Selected;
        private Player player
        {
            get { return Main.LocalPlayer; }
        }
        public new Rectangle hitbox;
        public GUI gui;
        public static Menu NewMenu(float x, float y, string[] options)
        {
            Select?.Dispose();
            Select = new Menu();
            Select.position = new Vector2(x, y);
            Select.active = true;
            Select.options = options;
            Select.Initialize();
            return Select;
        }
        public static Menu NewMenu(float x, float y, string[] options, GUI gui)
        {
            Select?.Dispose();
            Select = new Menu();
            Select.position = new Vector2(x, y);
            Select.active = true;
            Select.options = options;
            Select.gui = gui;
            Select.Initialize();
            return Select;
        }
        private void Initialize()
        {
            //X = (int)position.X;
            //Y = (int)position.Y;

            const int textWidth = 10;
            for (int i = 1; i < options.Length; i++)
            {
                width = options[i - 1].Length * textWidth;
                if (options[i - 1].Length > options[i].Length)
                    width = options[i].Length * textWidth;
                hitbox = new Rectangle(X, Y, width, (i + 1) * textHeight);
            }
        }
        public void Update()
        {
            if (SingleLeftClick())
            {
                if (Selected != null)
                {
                    switch (Selected)
                    {
                        case "Drop":
                            if (Inventory.itemList.Contains(Current))
                            {
                                Inventory.itemList.Remove(Current);
                                Inventory.itemProximate.Add(Current);
                                Inventory.listUpdate = true;
                                Item.NewItem((int)player.Center.X, (int)player.Center.Y, Current.width, Current.height, Item.Owner_World, Current.style, Current.itemType, Current.itemStyle);
                            }
                            goto default;
                        case "Pickup":
                            if (player.ArmorySlotUsed(GUI.Pack))
                            {
                                if (player.PackHasSpace(player.Armory[GUI.Pack].Item))
                                {
                                    if (Inventory.itemProximate.Contains(Current))
                                    {
                                        Current.owner = player.whoAmI;
                                        Inventory.itemList.Add(Current);
                                        Inventory.itemProximate.Remove(Current);
                                        Inventory.listUpdate = true;
                                    }
                                }
                            }
                            goto default;
                        case "Close":
                            goto default;
                        case "Equip":
                            var item = player.Armory[Current.type].Item;
                            var main = player.Armory[GUI.MainHand];
                            var offh = player.Armory[GUI.Offhand];
                            if (Current.itemStyle == Item.Style.Weapon_TwoHand)
                            {
                                if (offh != null && offh.Item?.equipped == true)
                                {
                                    EquipItem(Current);
                                    Inventory.itemList.Add(offh.Item);
                                    offh.Item.Unequip();
                                    MoveItem(Current);
                                    goto default;
                                }
                            }
                            if (Current != null && Current.itemStyle == Item.Style.OffHand && main.Item != null && main.Item.itemStyle == Item.Style.Weapon_TwoHand && main.Item.equipped)
                            {
                                EquipItem(Current);
                                Inventory.itemList.Add(main.Item);
                                main.Item.Unequip();
                                MoveItem(Current);
                                goto default;
                            }
                            if (item == null || !item.active)
                            {
                                EquipItem(Current);
                            }
                            else
                            {
                                Inventory.itemList.Add(item);
                                EquipItem(Current);
                            }
                            MoveItem(Current);
                            goto default;
                        case "Combine":
                            var item2 = player.Armory[GUI.Purse].Item?.purse;
                            if (item2 != null && Current.purse != null)
                            {
                                item2.Combine(Current.purse);
                                Current.Kill();
                                Inventory.itemList.Remove(Current);
                            }
                            goto default;
                        case "Unequip":
                            gui.Item?.Unequip();
                            if (gui.Item.type != GUI.Pack)
                            {
                                Inventory.itemList.Add(Current);
                                Inventory.listUpdate = true;
                            }
                            else
                            {
                                int count = Inventory.itemList.Count;
                                if (count > 0)
                                {
                                    for (int i = 0; i < count; i++)
                                    {
                                        var n = Inventory.itemList[i];
                                        Inventory.itemProximate.Add(n);
                                        Item.NewItem((int)player.Center.X - n.width / 2, (int)player.Center.Y - n.height / 2, n.width, n.height, Item.Owner_World, n.style, n.itemType, n.itemStyle);
                                    }
                                }
                                else
                                {
                                    var n = gui.Item;
                                    Item.NewItem((int)player.Center.X - n.width / 2, (int)player.Center.Y - n.height / 2, n.width, n.height, Item.Owner_World, n.style, n.itemType, n.itemStyle);
                                }
                                Inventory.itemProximate.Add(gui.Item);
                                Inventory.itemList.Clear();
                                Inventory.listUpdate = true;
                            }
                            goto default;
                        default:
                            CloseMenu();
                            return;
                    }
                }
            }
        }
        public void EquipItem(Item current)
        {
            Current.owner = player.whoAmI;
            Current.equipped = true;
            player.Armory[current.type].Item = current;
        }
        private void MoveItem(Item current)
        {
            Dialog.Instance.active = false;
            Inventory.listUpdate = true;
            if (Inventory.itemList.Contains(current))
                Inventory.itemList.Remove(current);
            if (Inventory.itemProximate.Contains(current))
                Inventory.itemProximate.Remove(current);
        }
        private void CloseMenu()
        {
            Selected = null;
            if (Current != null) Current.menu = false;
            active = false;
        }
        public void Draw(RewBatch rb)
        {
            if (!active) return;

            rb.Draw(Main.MagicPixel, hitbox);//, Color.Black);
            for (int i = 0; i < options.Length; i++)
            {
                Rectangle select = new Rectangle((int)position.X, (int)position.Y + i * textHeight, hitbox.Width, textHeight);
                if (select.Contains(Main.WorldMouse))
                {
                    rb.Draw(Main.MagicPixel, select);//, Color.Blue);
                    Selected = options[i];
                    break;
                }
            }
            for (int j = 0; j < options.Length; j++)
            {
                rb.DrawString(Game.Font[FontID.Arial], options[j], position + new Vector2(0, j * 12), Color.White);
            }
        }
        int flag = 0;
        public bool SingleLeftClick()
        {
            if (Main.LocalPlayer.LeftMouse())
            {
                if (flag % 2 == 0)
                {
                    flag = 1;
                    return true;
                }
            }
            if (Main.MouseDevice.LeftButton == System.Windows.Input.MouseButtonState.Released)
            {
                if (flag % 2 == 1)
                {
                    flag = 0;
                }
            }
            return false;
        }
        public void Dispose()
        {
        }
    }
}
