using FoundationR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;





namespace cotf_rewd
{
    public class Player : SimpleEntity
    {
        public bool Phasing
        {
            get; set;
        }
        private bool canJump;
        private bool clamp;
        public const int plrWidth = 32, plrHeight = 45;
        const int buffer = 4;
        public bool controlLeft, controlUp, controlRight, controlDown;
        public bool isAttacking;
        //  Initial values
        bool canLeft = true, canRight = true;
        float moveSpeed = 0.15f;
        float maxSpeed = 3f;
        float boosted = 4f;
        float stopSpeed;
        float jumpSpeed;
        float fallSpeed = 0.917f;
        public static Color torchLight = Color.White;
        public const int MaxHealth = 100, MaxMana = 20;
        public int ScaledMaxMana
        {
            get { return (int)(MaxMana * Math.Max(Stats.level * Stats.manaMultiplier, 1)); }
        }
        public int ScaledMaxLife
        {
            get { return (int)(MaxHealth * Math.Max(Stats.level * Stats.lifeMultiplier, 1)); }
        }
        List<SquareBrush> collision = new List<SquareBrush>();
        int flag, flag2, flag3, flag4;
        public bool justCastLight;
        public int restTimer;

        public Inventory inventory;
        public Item mouseItem = new Item();

        public int iFrames = 60;
        public int iFrameCounter;
        public int maxIFrames = 60;
        public float knockBackResist = 1f;
        public void Initialize()
        {
            texture = Main.Temporal;
            active = true;
        }
        public GUI[] Armory = new GUI[GUI.MAX];
        private bool init = false;
        public bool invulnDraw;
        public Stats Stats
        {
            get; internal set;
        }                  
        public Attributes Attributes
        {
            get; internal set;
        }
        public Traits Traits
        {
            get; internal set;
        }
        internal IList<NPC> downedNPC = new List<NPC>();
        internal bool downedUpdate;
        public new Rectangle hitbox;
        public KeyboardDevice keyboard;
        public List<NPC> npcCollide = new List<NPC>();
        public void PreDraw(RewBatch rb)
        {
            if (Inventory.open || !init)
                return;

            if (iFrameCounter > 0)
            {
                if (iFrameCounter % 5 == 0)
                {
                    invulnDraw = !invulnDraw;
                }
            }
            else invulnDraw = false;

            rb.Draw(texture, hitbox);//, Color.Blue * (invulnDraw ? 0f : alpha));
            
            rb.Draw(Main.MagicPixel, new Rectangle((int)position.X, (int)position.Y + plrHeight + 10, Stats.currentLife, 10), Color.Green);// * alpha);
            rb.Draw(Main.MagicPixel, new Rectangle((int)position.X, (int)position.Y + plrHeight + 22, Stats.mana, 10), Color.Blue);// * alpha);
            rb.DrawString(Game.Font[FontID.Arial], string.Format("{0} / {1}\n{2} / {3}", Stats.currentLife, Stats.totalLife, Stats.mana, MaxMana), new Vector2(position.X, position.Y + plrHeight + 10), Color.White);// * alpha);

            width = plrWidth;
            height = plrHeight;
        }
        public bool ArmorySlotUsed(byte index)
        {
            return Armory[index].Item != null && Armory[index].Item.equipped;
        }
        private bool phase = true;
        //  TODO: fix phase door
        private bool PhaseDoorEffect(float spellRange)
        {
            if (Phasing)
            {
                if (phase)
                {
                    if (alpha > 0.1f)
                    {
                        alpha -= 0.02f;
                    }
                    else
                    {
                        alpha = 0.1f;
                        foreach (SquareBrush b in Main.square.Where(t => NPC.Distance(t.Center, Center) < spellRange))
                        {
                            if (!b.active())
                            {
                                phase = false;
                                position = b.Center - new Vector2(b.width / 2, b.height / 2);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (alpha < 1f)
                    {
                        alpha += 0.02f;
                    }
                    else
                    {
                        alpha = 1f;
                        Phasing = false;
                        phase = true;
                    }
                }
            }
            return Phasing;
        }
        public bool PackHasSpace(Item item)
        {
            int space = 0;
            if (item.Cursed)
                space = -7;
            if (item.Enchanted)
                space = 7;
            switch (item.Name)
            {
                case "Small Backpack":
                    if (Inventory.itemList.Count < GUI.SmallPack + space)
                        return true;
                    break;
                case "Medium Backpack":
                    if (Inventory.itemList.Count < GUI.MediumPack + space)
                        return true;
                    break;
                case "Large Backpack":
                    if (Inventory.itemList.Count < GUI.LargePack + space)
                        return true;
                    break;
                case "Small Chest":
                    if (Inventory.itemList.Count < GUI.SmallChest + space)
                        return true;
                    break;
                case "Medium Chest":
                    if (Inventory.itemList.Count < GUI.MediumChest + space)
                        return true;
                    break;
                case "Large Chest":
                    if (Inventory.itemList.Count < GUI.LargeChest + space)
                        return true;
                    break;
            }
            return false;
        }
        public void Update()
        {
            if (!init)
            {
                Name = "Player001";
                //  DEBUG adding player and singular light source
                Light.entity.Add(this);
                //Light.entity.Add(new SimpleEntity()
                //{
                //    active = true,
                //    position = new Vector2(Main.ScreenWidth / 2, Main.ScreenHeight / 2)
                //});

                //  DEBUG set player position
                if (Level.floorNumber == 0 || PlayerInWall())
                {
                    Background bg = null;
                    do
                    {
                        bg = Main.ground[(int)Main.rand.Next(0, Main.ground.Length)];
                    } while (bg == null || !bg.active);
                    Main.LocalPlayer.position = bg.position;
                }

                Light.updating = true;
                if (Stats == null)
                {
                    Stats = new Stats()
                    {
                        iFrames = 60,
                        totalLife = MaxHealth,
                        currentLife = MaxHealth,
                        level = 1,
                        manaMultiplier = 0.5f,
                        lifeMultiplier = 0.25f,
                        mana = MaxMana,
                        restoreMana = 1,
                        healAmount = 2
                    };
                }
                if (Attributes == null)
                    Attributes = new Attributes();
                if (Traits == null)
                    Traits = new Traits();
                for (int i = 0; i < Armory.Length; i++)
                {
                    switch (i)
                    {
                        case GUI.MainHand:
                            Armory[GUI.MainHand] = GUI.ArmoryGUI(32, 32, GUI.MainHand, Item.Type.Sword_OneHand, Item.Style.Weapon_OneHand, "Main hand", 0, GUI.MainHand);
                            break;
                        case GUI.Shield:
                            Armory[GUI.Offhand] = GUI.ArmoryGUI(32, 32, GUI.Offhand, Item.Type.Shield, Item.Style.OffHand, "Off hand", 0, GUI.Offhand);
                            break;
                        case GUI.Helm:
                            Armory[GUI.Helm] = GUI.ArmoryGUI(32, 32, GUI.Helm, Item.Type.Helm, Item.Style.Helm, "Helm", 0, GUI.Helm);
                            break;
                        case GUI.Cloak:
                            Armory[GUI.Cloak] = GUI.ArmoryGUI(32, 32, GUI.Cloak, Item.Type.Cloak, Item.Style.Cloak, "Cloak", 0, GUI.Cloak);
                            break;
                        case GUI.Torso:
                            Armory[GUI.Torso] = GUI.ArmoryGUI(32, 32, GUI.Torso, Item.Type.Torso, Item.Style.Torso, "Torso", 0, GUI.Torso);
                            break;
                        case GUI.Boots:
                            Armory[GUI.Boots] = GUI.ArmoryGUI(32, 32, GUI.Boots, Item.Type.Boots, Item.Style.Boot, "Boots", 0, GUI.Boots);
                            break;
                        case GUI.Bracers:
                            Armory[GUI.Bracers] = GUI.ArmoryGUI(32, 32, GUI.Bracers, Item.Type.Bracers, Item.Style.Bracers, "Bracers", 0, GUI.Bracers);
                            break;
                        case GUI.Gauntlets:
                            Armory[GUI.Gauntlets] = GUI.ArmoryGUI(32, 32, GUI.Gauntlets, Item.Type.Gauntlets, Item.Style.Gloves, "Gauntlets", 0, GUI.Gauntlets);
                            break;
                        case GUI.Greaves:
                            Armory[GUI.Greaves] = GUI.ArmoryGUI(32, 32, GUI.Greaves, Item.Type.Greaves, Item.Style.Greaves, "Greaves", 0, GUI.Greaves);
                            break;
                        case GUI.Necklace:
                            Armory[GUI.Necklace] = GUI.ArmoryGUI(32, 32, GUI.Necklace, Item.Type.Necklace, Item.Style.Necklace, "Necklace", 0, GUI.Necklace);
                            break;
                        case GUI.Pack:
                            Armory[GUI.Pack] = GUI.ArmoryGUI(32, 32, GUI.Pack, Item.Type.Pack_Backpack, Item.Style.Storage, "Pack", 0, GUI.Pack);
                            break;
                        case GUI.Purse:
                            Armory[GUI.Purse] = GUI.ArmoryGUI(32, 32, GUI.Purse, Item.Type.Purse, Item.Style.Purse, "Purse", 0, GUI.Purse);
                            break;
                        case GUI.Ring1:
                            Armory[GUI.Ring1] = GUI.ArmoryGUI(32, 32, GUI.Ring1, Item.Type.Ring, Item.Style.Ring, "Ring One", 0, GUI.Ring1);
                            break;
                        case GUI.Ring2:
                            Armory[GUI.Ring2] = GUI.ArmoryGUI(32, 32, GUI.Ring2, Item.Type.Ring, Item.Style.Ring, "Ring Two", 0, GUI.Ring2);
                            break;
                        default:
                            break;
                    }
                }
                init = true;
            }

            if (downedUpdate)
            {
                foreach (NPC npc in downedNPC)
                {
                    Traits += npc.traits;
                }
                downedNPC.Clear();
                downedUpdate = false;
            }

            //keyboard = Keyboard.GetState();

            if (PhaseDoorEffect(300f))
            {
                foreach (GUI menu in Main.skill)
                {
                    menu.active = false;
                }
                return;
            }

            ToggleZoom();
            ScrollMap();

            if (Inventory.open || Main.IsZoomed)
                return;

            //  Player mana and health regen
            const int interval = 180;
            if (Movement() || Main.TimeScale > 1)
            {
                for (int i = 0; i < Main.TimeScale; i++)
                {
                    restTimer++;
                    if (restTimer % (int)(interval * 0.75f) == 0)
                    {
                        if (Stats.mana < ScaledMaxMana)
                            Stats.mana = Math.Min(ScaledMaxMana, Stats.mana + Stats.restoreMana);
                    }
                    if (restTimer % interval == 0)
                    {
                        if (Stats.currentLife < ScaledMaxLife)
                            Stats.currentLife = Math.Min(ScaledMaxLife, Stats.healAmount + Stats.currentLife);
                    }
                }
            }

            //  Player hurt
            //  Basic hitbox damage
            //if (IsMoving())
            //{
            //    if (iFrameCounter > 0)
            //        iFrameCounter--;
            //    foreach (NPC npc in Main.npc.Where(t => t != null))
            //    {
            //        if (!npc.active || !npc.discovered)
            //            continue;
            //        if (iFrameCounter == 0 && npc.hitbox.Intersects(this.hitbox))
            //        {
            //            iFrameCounter = Stats.iFrames;
            //            PlayerHurt(npc);
            //            break;
            //        }
            //    }
            //}
            if (IsMovingNoCollide())
            {
                if (iFrameCounter > 0)
                    iFrameCounter--;
                foreach (NPC npc in npcCollide)
                {
                    if (npc == null || !npc.active || !npc.discovered)
                        continue;
                    if (iFrameCounter == 0)
                    {
                        iFrameCounter = Stats.iFrames;
                        PlayerHurt(npc);
                    }
                }
            }
            npcCollide.Clear();

            //  Armory status
            foreach (GUI gui in Armory.Where(t => t != null))
            {
                switch (gui.Item?.itemType)
                {
                    case Item.Type.Sword_OneHand:
                        itemType = Item.Type.Sword_OneHand;
                        switch (gui.Item.type)
                        {
                            
                            default:
                                break;
                        }
                        break;
                    case Item.Type.Sword_TwoHand:
                        itemType = Item.Type.Sword_TwoHand;
                        break;
                    default:
                        break;
                }
            }

            hitbox = new Rectangle((int)position.X, (int)position.Y, plrWidth, plrHeight);

            //  Initializing
            stopSpeed = moveSpeed * 2f;
            jumpSpeed = maxSpeed * 2f * (!clamp ? boosted : 1f);

            #region depracated sliding walls fix
            //  Collision
            /*
            if (colLeft)
            {
                if (controlRight)
                    position.X += moveSpeed * 2;
                if (controlUp)
                    velocity.Y -= moveSpeed;
                else if (controlDown)
                    velocity.Y += moveSpeed;

            }
            if (colRight)
            {
                if (controlLeft)
                    position.X -= moveSpeed * 2;
                if (controlUp)
                    velocity.Y -= moveSpeed;
                else if (controlDown)
                    velocity.Y += moveSpeed;
            }
            if (colDown)
            {
                if (controlUp)
                    position.Y -= moveSpeed * 2;
                if (controlLeft)
                    velocity.X -= moveSpeed;
                else if (controlRight)
                    velocity.X += moveSpeed;
            }
            if (colUp)
            {
                if (controlDown)
                    position.Y += moveSpeed * 2;
                if (controlLeft)
                    velocity.X -= moveSpeed;
                else if (controlRight)
                    velocity.X += moveSpeed;
            }             */
            #endregion

            //  Movement mechanic
            if (!IsMoving())
            {
                //  Stopping movement
                if (velocity.X > 0f && !controlRight)
                    velocity.X -= stopSpeed;
                if (velocity.X < 0f && !controlLeft)
                    velocity.X += stopSpeed;
                if (velocity.Y > 0f && !controlDown)
                    velocity.Y -= stopSpeed;
                if (velocity.Y < 0f && !controlUp)
                    velocity.Y += stopSpeed;
            }         

            //  Clamp
            if (velocity.X > maxSpeed)
                velocity.X = maxSpeed;
            if (velocity.X < -maxSpeed)
                velocity.X = -maxSpeed;
            if (velocity.Y > maxSpeed)
                velocity.Y = maxSpeed;
            if (velocity.Y < -maxSpeed)
                velocity.Y = -maxSpeed;

            //  Border
            //canLeft = position.X >= 1;
            //canRight = position.X < width - 1;

            //  Movement speed set
            if (velocity.X < moveSpeed && velocity.X > -moveSpeed)
                velocity.X = 0f;
            if (velocity.Y < moveSpeed && velocity.Y > -moveSpeed)
                velocity.Y = 0f;

            //  Positioning
            #region wall sticking
            /*
            if (IsMoving())
            {
                if (!colLeft && !colRight)
                    position.X += velocity.X;
                if (!colUp && !colDown)
                    position.Y += velocity.Y;
            } */
            #endregion
            #region fixed
            //  Collision reaction
            if (IsMoving())
            {
                if (!colLeft && velocity.X < 0f)
                    position.X += velocity.X;
                if (!colRight && velocity.X > 0f)
                    position.X += velocity.X;
                if (!colUp && velocity.Y < 0f)
                    position.Y += velocity.Y;
                if (!colDown & velocity.Y > 0f)
                    position.Y += velocity.Y;
            }

            //  Correcting sliding wall momentum
            //  With it enabled moving into corridors is difficult
            //if (colLeft || colRight)
            //    velocity.X = 0f;
            //if (colUp || colDown)
            //    velocity.Y = 0f;
            #endregion

            //  Controls
            if (controlRight = !Game.KeyDown(Key.A) && Game.KeyDown(Key.D))
            {
                // move right
                Main.TimeScale = 1;
                velocity.X += moveSpeed;
            }
            if (controlLeft = Game.KeyDown(Key.A) && !Game.KeyDown(Key.D))
            {
                // move left
                Main.TimeScale = 1;
                velocity.X -= moveSpeed;
            }
            if (controlDown = !Game.KeyDown(Key.W) && Game.KeyDown(Key.S))
            {
                // move down
                Main.TimeScale = 1;
                velocity.Y += moveSpeed;
            }
            if (controlUp = Game.KeyDown(Key.W) && !Game.KeyDown(Key.S))
            {
                // move up
                Main.TimeScale = 1;
                velocity.Y -= moveSpeed;
            }

            //AuxMovement();

            //  GUI Hotbar
            if (Game.KeyDown(Key.D1) || Game.KeyDown(Key.D2) || Game.KeyDown(Key.D3) || Game.KeyDown(Key.D4) || Game.KeyDown(Key.D5))
            {
                foreach (GUI g in Main.gui.Where(t => t != null))
                    g.selected = false;
            }
            if (Game.KeyDown(Key.D1))
                Main.gui[0].selected = true;
            else if (Game.KeyDown(Key.D2))
                Main.gui[1].selected = true;
            else if (Game.KeyDown(Key.D3))
                Main.gui[2].selected = true;
            else if (Game.KeyDown(Key.D4))
                Main.gui[3].selected = true;
            else if (Game.KeyDown(Key.D5))
                Main.gui[4].selected = true;

            //  Time scaling
            if (Stats.mana < ScaledMaxMana || Stats.currentLife < ScaledMaxLife)
            {
                if (Game.KeyDown(Key.R))
                    Main.TimeScale = 15;
            }
            else Main.TimeScale = 1;
            foreach (NPC npc in Main.npc.Where(t => t != null && t.active))
            {
                if (NPC.Distance(Center, npc.Center) < range)
                    Main.TimeScale = 1;
            }

            //  Menu
            OpenMenu();

            //  Equipped torch toggle which might be applicable to other toggled items
            if (flag4 % 2 == 0)
            {
                if (RightMouse())
                {
                    var item = this.Armory[GUI.Offhand].Item;
                    if (item != null && item.itemType == Item.Type.Torch)
                    {
                        item.inUse = !item.inUse;
                    }
                    flag4++;
                }
            }
            if (Main.MouseDevice.RightButton == MouseButtonState.Released && flag4 % 2 == 1)
                flag4 = 0;

            //  TODO: Torch light
            var torch = this.Armory[GUI.Offhand].Item;
            if (torch != null && torch.itemType == Item.Type.Torch && torch.inUse)
            {
                torchLight = Color.Orange;
                lit = true;
            }
            else 
            { 
                torchLight = Color.Black;
                lit = false;
            }

            //  GUI
            if (isAttacking)
                return;
            if (flag % 2 == 0)
            {
                foreach (GUI gui in Main.gui.Where(t => t != null))
                {
                    if (gui.hitbox.Contains(Main.MousePosition.X, Main.MousePosition.Y))
                    {
                        return;
                    }
                }
                if (LeftMouse())
                {
                    Item item = Armory[GUI.MainHand].Item;
                    if (GUI.GetElement(GUI.ID.SwordSwing).selected)
                    {
                        Projectile.NewProjectile(0, 0, Projectile.SwordSwing, item == null ? 0 : item.style, Color.White, 10, whoAmI);
                        ManageMana(0);
                    }
                    if (GUI.GetElement(GUI.ID.SwordSwipe).selected)
                    {
                        Projectile.NewProjectile(0, 0, Projectile.SwordSwipe, item == null ? 0 : item.style, Color.White, 60, whoAmI);
                        ManageMana(1);
                    }
                }
                flag++;
            }
            if (!Game.mouseLeft && flag % 2 == 1)
                flag = 0;
        }

        public static void UpdateDowned(NPC npc)
        {
            Main.LocalPlayer.downedNPC.Add(npc);
            Main.LocalPlayer.downedUpdate = true;
        }
        public void ManageMana(int cost)
        {
            Stats.mana -= cost;
            if (Stats.mana < 0)
            {
                Stats.currentLife -= (int)((Math.Abs(Stats.mana) + cost) * (Main.rand.Next(3) * 0.1f));
            }
        }
        public void PlayerHurt(NPC npc)
        {
            Stats.currentLife -= npc.stats.damage;
            //  TODO: Causes blackhole effect
            //velocity += Helper.AngleToSpeed(Helper.AngleTo(Center, npc.Center), npc.stat.knockback);
            if (Stats.currentLife <= 0)
            {
                Kill();
            }
        }
        public void Kill()
        {
            //  TODO: Make kill method into a wound status instead of death effect
            //  Each enemy only adds a certain status
            //  The player MIGHT only die under certain status effects, or after a threshold of status' is reached
            Stats.currentLife = 100;
            Stats.mana = 20;
            Background bg = null;
            do
            {
                bg = Main.ground[(int)Main.rand.Next(0, Main.ground.Length)];
            } while (bg == null || !bg.active);
            Main.LocalPlayer.position = bg.position;
        }

        public void ToggleZoom()
        {
            if (Game.KeyDown(Key.M) && flag3 % 2 == 0)
            {
                flag3++;
                Main.IsZoomed = !Main.IsZoomed;
            }
            if (Game.KeyUp(Key.M))
            {
                flag3 = 0;
            }
        }
        public bool PlayerInWall()
        {
            foreach (SquareBrush brush in Main.square.Where(t => t != null && t.active()))
            {
                if (brush.Hitbox.Contains((int)Center.X, (int)Center.Y))
                    return true;
            }
            return false;
        }
        public void ScrollMap()
        {
            if (Main.IsZoomed)
            {
                if (Game.KeyDown(Key.Down) && Main.MapY * Game.ScrollSpeed < Main.LevelHeight / 2)
                    Main.MapY++;
                if (Game.KeyDown(Key.Up) && Main.MapY * Game.ScrollSpeed > -Main.LevelHeight / 2)
                    Main.MapY--;
                if (Game.KeyDown(Key.Right) && Main.MapX * Game.ScrollSpeed < Main.LevelHeight / 2)
                    Main.MapX++;
                if (Game.KeyDown(Key.Left) && Main.MapX * Game.ScrollSpeed > -Main.LevelHeight / 2)
                    Main.MapX--;
            }
            else
            {
                Main.MapX = 0;
                Main.MapY = 0;
            }
        }
        public void OpenMenu()
        {
            if (Game.mouseLeft && flag2 % 2 == 0)
            {
                flag2++;
                if (Main.gui[GUI.SkillMenu].hitbox.Contains(Main.MousePosition.X, Main.MousePosition.Y))
                {
                    foreach (GUI menu in Main.skill)
                    {
                        menu.active = !menu.active;
                    }
                }
            }
            if (!Game.mouseLeft && flag2 % 2 == 1)
                flag2 = 0;
        }
        private int fuel = 3;
        private int maxFuel = 3;
        private void AuxMovement()
        {
            //clamp = fuel <= 0;
            Point mouse = Main.WorldMouse;
            if (Game.mouseLeft)
            {
                foreach (Dust dust in Main.dust.Where(t => t != null && t.active))
                {
                    if (dust.type == Dust.Waypoint.Green)
                    {
                        if (dust.hitbox.Contains(mouse.X, mouse.Y))
                        {
                            var speed = AngleToSpeed(AngleTo(position, mouse), 5f);
                            velocity.X += speed.X;
                            velocity.Y += speed.Y;
                            break;
                        }
                    }
                }
            }
            else if (fuel < maxFuel)
                fuel++;
        }
        public static Vector2 AngleToSpeed(float angle, float amount)
        {
            float cos = (float)(amount * Math.Cos(angle));
            float sine = (float)(amount * Math.Sin(angle));
            return new Vector2(cos, sine);
        }
        public static float AngleTo(Vector2 from, Point to)
        {
            return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        }
        public static float AngleTo(Vector2 from, Vector2 to)
        {
            return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        }
        public bool KeyUp(Key key)
        {
            return Game.KeyUp(key);
        }
        public bool KeyDown(Key key)
        {
            return Game.KeyDown(key);
        }
        
        public bool IsMovingNoCollide()
        {
            return (Game.KeyDown(Key.Space) || Game.KeyDown(Key.W) || Game.KeyDown(Key.A) || Game.KeyDown(Key.S) || Game.KeyDown(Key.D)) && (velocity.X > 0f || velocity.X < 0f || velocity.Y > 0f || velocity.Y < 0f);
        }
        public bool IsMoving()
        {
            return !Game.KeyDown(Key.Space) && (Game.KeyDown(Key.W) || Game.KeyDown(Key.A) || Game.KeyDown(Key.S) || Game.KeyDown(Key.D)) && (velocity.X > 0f || velocity.X < 0f || velocity.Y > 0f || velocity.Y < 0f) && (!colDown || !colLeft || !colRight || !colUp);
        }
        public bool Movement()
        {
            return Game.KeyDown(Key.W) || Game.KeyDown(Key.A) || Game.KeyDown(Key.S) || Game.KeyDown(Key.D) || Game.KeyDown(Key.Space) || Main.TimeScale != 1;
        }
        public bool LeftMouse()
        {
            return Game.mouseLeft;
        }
        public bool RightMouse()
        {
            return Game.mouseRight;
        }
    }
}
