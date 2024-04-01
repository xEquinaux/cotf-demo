using FoundationR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;





namespace cotf_rewd
{
    public class NPC : Entity, IDisposable
    {
        public float knockBack = 1f;
        public int iFrameCounter;
        public NPC()
        {

        }
        public static NPC NewNPC(int x, int y, int type, Color color, int maxLife = 0)
        {
            int index = 500;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i] == null || !Main.npc[i].active)
                {
                    index = i;
                    break;
                }
                if (i == index)
                {
                    return Main.npc[index];
                }
            }
            Main.npc[index] = new NPC();
            if (maxLife > 0) Main.npc[index].maxLife = maxLife;
            Main.npc[index].position = new Vector2(x, y);
            Main.npc[index].type = type;
            Main.npc[index].active = true;
            Main.npc[index].color = color;
            Main.npc[index].whoAmI = index;
            Main.npc[index].stats = new Stats();
            Main.npc[index].Initialize();
            return Main.npc[index];
        }
        public static void Clear(int preInitIndex)
        {
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i] != null)
                { 
                    Main.npc[i].active = false;
                    Main.npc[i] = null;
                }
            }
            Main.npc = new NPC[preInitIndex];
        }
        private void Initialize()
        {
            switch (type)
            {
                case NPCID.Necrosis:
                    width = 176;
                    height = 192;
                    Name = "Necrosis";
                    texture = Main.NPCTexture[NPCID.Necrosis + 1];
                    knockBack = 3f;
                    break;
                case NPCID.Kobold:
                    stats.currentLife = Main.rand.Next(20, 35);
                    stats.totalLife = stats.currentLife;
                    stats.damage = 8;
                    stats.defense = 2;
                    stats.knockback = 1.2f;
                    stats.totalLife = maxLife;
                    stats.kbResistance = 1f;
                    stats.iFrames = 60;
                    maxSpeed = 1.5f;
                    moveSpeed = 0.20f;
                    width = 24;
                    height = 32;
                    Name = "Kobold";
                    texture = Main.Temporal;
                    knockBack = 3f;
                    range = 400f;
                    break;
            }
        }
        public bool PlayerNPCCollision(int buffer = 4)
        {
            if (!active) return false;
            bool flag = false;
            foreach (Player player in Main.player.Where(t => t != null && t.active))
            {
                if (player.hitbox.IntersectsWith(new Rectangle((int)position.X, (int)position.Y, width, height)))
                { 
                    collide = true;
                    player.collide = true;
                }
                //  Directions
                if (player.hitbox.IntersectsWith(new Rectangle((int)position.X, (int)position.Y - buffer, width, 2)))
                { 
                    colUp = true;
                    player.colDown = true;
                }
                if (player.hitbox.IntersectsWith(new Rectangle((int)position.X, (int)position.Y + height + buffer, width, 2)))
                { 
                    colDown = true;
                    player.colUp = true;
                }
                if (player.hitbox.IntersectsWith(new Rectangle((int)position.X + width + buffer, (int)position.Y, 2, height)))
                { 
                    colRight = true;
                    player.colLeft = true;
                }
                if (player.hitbox.IntersectsWith(new Rectangle((int)position.X - buffer, (int)position.Y, 2, height)))
                { 
                    colLeft = true;
                    player.colRight = true;
                }
                if (flag = collide || colUp || colDown || colLeft || colRight)
                {
                    player.npcCollide.Add(this);
                }
            }
            return flag;
        }
        public void Update()
        {   
            //  General all purpose discover based on range
            if (!discovered)
            {
                if (Distance(Main.LocalPlayer.Center, Center) < range)
                {
                    discovered = true;
                }
            }
            if (iFrameCounter > 0)
                iFrameCounter--;

            //NPC projectile behavior
            foreach (Projectile proj in Main.projectile.Where(t => t != null))
            {
                if (proj.owner == Main.LocalPlayer.whoAmI)
                {
                    if (iFrameCounter == 0 && this.hitbox.Contains((int)proj.strikePoint.X, (int)proj.strikePoint.Y))
                    {
                        iFrameCounter = stats.iFrames;
                        NPCHurt(Main.LocalPlayer, proj);
                        break;
                    }
                }
            }

            switch (type)
            {
                case NPCID.Necrosis:
                    goto default;
                case NPCID.Kobold:
                    foreach (SquareBrush sq in Main.square.Where(t => t != null))
                    {
                        sq.NPCCollision(this);
                    }
                    foreach (Foliage stuff in Main.foliage.Where(t => t != null))
                    {
                        stuff.NPCCollision(this);
                    }
                    goto default;
                default:
                    hitbox = new Rectangle((int)position.X, (int)position.Y, width, height);
                    break;
            }
        }
        private int ticks;
        private int proj;
        private Dust target;
        public void NPCHurt(Player player, Projectile proj)
        {
            stats.currentLife -= proj.damage;
            velocity += Helper.AngleToSpeed(Helper.AngleTo(player.Center, this.Center), proj.knockBack);

            switch (player.itemType)
            {
                case Item.Type.Sword_OneHand:
                    stats.hitStun = 30;  //45
                    break;
                case Item.Type.Sword_TwoHand:
                    stats.hitStun = 60;  //80
                    break;
                //case ItemID.RedRust:
                //    stat.hitStun = 15;
                //    break;
                //case ItemID.SilverLongsword:
                //    stat.hitStun = 30;
                //    break;
                //case ItemID.Zweihander:
                //    stat.hitStun = 60;
                //    break;
                //case ItemID.Dirk:
                //    stat.hitStun = 10;
                //    break;
            }

            if (stats.currentLife <= 0)
            {
                NPCLoot();
                Kill();
            }
        }
        public void NPCLoot()
        {
            int x = (int)(position.X -= position.X % SquareBrush.Size);
            int y = (int)(position.Y -= position.Y % SquareBrush.Size);

            switch (type)
            {
                case NPCID.Kobold:
                    if (Main.rand.NextBool())
                        Item.NewItem(x, y, 32, 32, Item.Owner_World, Main.rand.Next(ItemID.Purse + 1));
                    else
                    { 
                        int rand = Main.rand.Next(5);
                        switch (rand)
                        {
                            case 0:
                                Item.NewItem(x, y, 32, 32, Item.Owner_World, ItemID.CoinIron);
                                break;
                            case 1:
                                Item.NewItem(x, y, 32, 32, Item.Owner_World, ItemID.CoinCopper);
                                break;
                            case 2:
                                Item.NewItem(x, y, 32, 32, Item.Owner_World, ItemID.CoinSilver);
                                break;
                            case 3:
                                Item.NewItem(x, y, 32, 32, Item.Owner_World, ItemID.CoinGold);
                                break;
                            case 4:
                                Item.NewItem(x, y, 32, 32, Item.Owner_World, ItemID.CoinPlatinum);
                                break;
                        }
                    }
                    break;
            }
        }
        public virtual void Kill()
        {
            active = false;
            discovered = false;
            Main.npc[whoAmI] = null;
        }
        public Player Target
        {
            get; private set;
        }
        float moveSpeed = 0.15f;
        float maxSpeed = 3f;
        private Projectile[] orb = new Projectile[5];
        float stopSpeed
        {
            get { return moveSpeed * 2f; }
        }
        float jumpSpeed
        {
            get { return maxSpeed; }
        }
        public Dust pickRand()
        {
            var list = Main.dust.Where(t => t != null).ToArray();
            return list.Count() > 0 ? list[Main.rand.Next(list.Length - 1)] : new Dust(0, 0);
        }
        public Player pickRandPlayer()
        {
            var list = Main.player.Where(t => t != null).ToArray();
            return list[Main.rand.Next(list.Length - 1)];
        }
        private bool launch;
        private bool redPhase;
        private int timer, timer2;
        private int redOrbsCollected;
        private Dust[] targets = new Dust[1001];
        private bool moving;
        public bool hidden = false;
        public virtual void AI()
        {
            if (!discovered)
                return;

            switch (type)
            {
                case NPCID.Necrosis:
                    #region DEBUG npc
                    if (!Main.LocalPlayer.Movement())
                        return;
                    if (velocity.X > maxSpeed)
                        velocity.X = maxSpeed;
                    if (velocity.X < -maxSpeed)
                        velocity.X = -maxSpeed;
                    if (velocity.Y > maxSpeed)
                        velocity.Y = maxSpeed;
                    if (velocity.Y < -maxSpeed)
                        velocity.Y = -maxSpeed;
                    if (target == null || !target.active)
                    {
                        target = pickRand();
                        ticks = 0;
                        proj = 0;
                        launch = false;
                    }
                    if (target.active)
                    {
                        if (target.type != Dust.Waypoint.Yellow && scale < 1f)
                            scale += 0.1f;

                        position.X += velocity.X;
                        position.Y += velocity.Y;
                        switch (target.type)
                        {
                            case Dust.Waypoint.Green:
                                if (!this.hitbox.Contains((int)target.Center.X, (int)target.Center.Y))
                                {
                                    var speed = AngleToSpeed((float)Math.Atan2(position.Y - target.position.Y, position.X - target.position.X) + 180f * Draw.radian, moveSpeed);
                                    velocity.X += speed.X;
                                    velocity.Y += speed.Y;
                                }
                                else
                                {
                                    target.Kill();
                                }
                                break;
                            case Dust.Waypoint.Yellow:
                                if (!this.hitbox.Contains((int)target.Center.X, (int)target.Center.Y))
                                {
                                    var speed = AngleToSpeed((float)Math.Atan2(position.Y - target.position.Y, position.X - target.position.X) + 180f * Draw.radian, moveSpeed);
                                    velocity.X += speed.X;
                                    velocity.Y += speed.Y;
                                }
                                else
                                {
                                    if (scale > 0.1f)
                                        scale -= 0.1f;
                                    else
                                    {
                                        target.Kill();
                                        while ((target = pickRand()).type != Dust.Waypoint.Yellow)
                                        {
                                            position = target.position;
                                            break;
                                        }
                                    }
                                }
                                break;
                            case Dust.Waypoint.Red:
                                //  player kill red orbs before collected

                                if (velocity.X > stopSpeed)
                                    velocity.X = stopSpeed;
                                if (velocity.X < -stopSpeed)
                                    velocity.X = -stopSpeed;
                                if (velocity.Y > stopSpeed)
                                    velocity.Y = stopSpeed;
                                if (velocity.Y < -stopSpeed)
                                    velocity.Y = -stopSpeed;

                                if (!launch)
                                {
                                    redOrbsCollected++;
                                    Vector2 speed = AngleToSpeed(AngleTo(position, Main.LocalPlayer.position), 4f);
                                    orb[proj] = Projectile.NewProjectile((int)position.X - 16, (int)position.Y - 16, speed.X, speed.Y, -1, 0, Color.White, 300, whoAmI, true);
                                    launch = true;
                                }
                                if (Distance(position, orb[proj].position) > 300)
                                {
                                    orb[proj].Kill();
                                }
                                break;
                        }
                    }
                    if (ticks++ >= 600)
                        ticks = 1;
                    if (redOrbsCollected >= 4)
                    {
                        if (!redPhase && (scale -= 0.1f) <= 0.3f)
                        {
                            targets = Main.dust.Where(t => t != null && t.active && Distance(t.position, position) < 300f && Main.rand.NextDouble() >= 0.34f).ToArray();
                            redPhase = true;
                        }
                        maxSpeed = 4f;
                    }
                    if (redPhase)
                    {
                        if (timer++ > 600)
                        {
                            timer = 0;
                            redPhase = false;
                        }
                    }
                    #endregion
                    break;
                case NPCID.Kobold:
                    if (!Main.LocalPlayer.Movement() || Distance(Center, Main.LocalPlayer.Center) > range)
                        return;

                    if (Target == null || !Target.active)
                    {
                        Target = pickRandPlayer();
                        ticks = 0;
                        proj = 0;
                        launch = false;
                    }
                    
                    VelocityMech.BasicSlowClamp(this, moveSpeed, stopSpeed, maxSpeed);
                    if (stats.hitStun-- > 0)
                        return;

                    if (collide)
                    {
                        return;
                    }
                    if (ticks++ > 15)
                    {
                        ticks = 0;
                        if (!(hidden = LineIntersect(Center, Main.LocalPlayer.Center)))
                        { 
                            velocity = AngleToSpeed(NPC.AngleTo(Center, Target.Center), maxSpeed * 3f);
                        }
                    }

                    //  Collision reaction
                    if (!colLeft && velocity.X < 0f)
                        position.X += velocity.X;
                    if (!colRight && velocity.X > 0f)
                        position.X += velocity.X;
                    if (!colUp && velocity.Y < 0f)
                        position.Y += velocity.Y;
                    if (!colDown & velocity.Y > 0f)
                        position.Y += velocity.Y;
                    break;
            }
        }
        public static float AngleTo(Vector2 from, Point to)
        {
            return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        }
        public static float AngleTo(Vector2 from, Vector2 to)
        {
            return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        }
        public static Vector2 AngleToSpeed(float angle, float amount)
        {
            float cos = (float)(amount * Math.Cos(angle));
            float sine = (float)(amount * Math.Sin(angle));
            return new Vector2(cos, sine);
        }
        public static bool LineIntersect(Vector2 from, Vector2 to)
        {
            for (float i = 0f; i < Distance(from, to); i++)
            {
                float angle = NPC.AngleTo(from, to);
                float cos = from.X + (float)(i * Math.Cos(angle));
                float sin = from.Y + (float)(i * Math.Sin(angle));
                Vector2 line = new Vector2(cos, sin);
                SquareBrush brush = SquareBrush.GetSafely((int)line.X / 50, (int)line.Y / 50);
                if (brush == null) return false;
                if (brush.active() && brush.Hitbox.Contains((int)line.X, (int)line.Y))
                {
                    return true;
                }
            }
            return false;
        }
        public void PreDraw(RewBatch rb)
        {
            if (!discovered || !active || hidden || texture == null)
                return;

            if (iFrameCounter > 0)
                color = Color.Red;
            else color = Color.Gray;

            rb.Draw(texture, hitbox);//, DynamicTorch(120f));
        }
        public void Dispose()
        {
            Main.npc[whoAmI].active = false;
            Main.npc[whoAmI] = null;
        }
    }
}
