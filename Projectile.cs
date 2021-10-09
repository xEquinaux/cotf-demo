using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePort
{
    public class Projectile : Entity
    {
        public const int Necrosis = -1;
        public const byte
            SwordSwipe = 0,
            SwordSwing = 1;
        public bool animated = false;
        private float timer;
        public float startAngle = 0f;
        public Vector2 strikePoint;
        public Stats stat;
        public bool friendly = true;
        public bool hostile = false;
        private float EndAngle(float start)
        {
            return Math.Abs(start) + Draw.radian * 90f;
        }
        private float endAngle;
        private float currentAngle;
        public int damage;
        public float knockBack;
        public int owner;
        public bool IsAttacking
        {
            get { return Main.LocalPlayer.isAttacking; }
            set { Main.LocalPlayer.isAttacking = value; }
        }
        public bool Friendly(int type)
        {
            switch (type)
            {
                case Necrosis:
                    return false;
                case SwordSwipe:
                    return true;
                case SwordSwing:
                    return true;
                default:
                    return false;
            }
        }
        public bool Hostile(int type)
        {
            switch (type)
            {
                case Necrosis:
                    return true;
                case SwordSwipe:
                    return false;
                case SwordSwing:
                    return false;
                default:
                    return true;
            }
        }
        private void Initialize()
        {
            Vector2 ang = Vector2.Zero;
            switch (type)
            {
                case Necrosis:
                    width = 38;
                    height = 44;
                    style = 0;
                    Name = "Orb";
                    texture = Main.ProjTexture[ProjectileID.Orb];
                    break;
                case SwordSwipe:
                    if (IsAttacking)
                    {
                        Kill();
                        return;
                    }
                    switch (style)
                    {
                        case ItemID.RedRust:
                            width = 24;
                            height = 36;
                            break;
                        case ItemID.SilverLongsword:
                            width = 24;
                            height = 36 + 16;
                            break;
                        case ItemID.Zweihander:
                            width = 28;
                            height = 36 + 18;
                            break;
                        default:
                            width = 24;
                            height = 24;
                            texture = Main.MagicPixel;
                            break;
                    }
                    IsAttacking = true;
                    break;
                case SwordSwing:
                    if (IsAttacking)
                    {
                        Kill();
                        return;
                    }
                    startAngle = NPC.AngleTo(Main.LocalPlayer.Center, Main.WorldMouse) - Draw.radian * 135f;
                    currentAngle = startAngle;
                    endAngle = EndAngle(startAngle);
                    ang = NPC.AngleToSpeed(startAngle, 32);
                    position = new Vector2(Main.LocalPlayer.Center.X + ang.X, Main.LocalPlayer.Center.Y + ang.Y);
                    goto case SwordSwipe;
            }
        }
        private string ProjTexture(string name)
        {
            return "Textures\\" + name + ".png";
        }
        public static Projectile NewProjectile(int x, int y, float speedX, float speedY, int type, int style, Color color, int maxTime, int owner = 255, bool animated = false)
        {
            int index = 1001;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i] == null || !Main.projectile[i].active)
                {
                    index = i;
                    break;
                }
                if (i == index)
                {
                    return Main.projectile[index];
                }
            }
            Main.projectile[index] = new Projectile();
            Main.projectile[index].position = new Vector2(x, y);
            Main.projectile[index].velocity = new Vector2(speedX, speedY);
            Main.projectile[index].type = type;
            Main.projectile[index].style = style;
            Main.projectile[index].active = true;
            Main.projectile[index].maxLife = maxTime;
            Main.projectile[index].color = color;
            Main.projectile[index].whoAmI = index;
            Main.projectile[index].animated = animated;
            Main.projectile[index].Initialize();
            return Main.projectile[index];
        }
        public static Projectile NewProjectile(int x, int y, int type, int style, Color color, int maxTime, int owner = 255, bool animated = false)
        {
            int index = 1001;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i] == null || !Main.projectile[i].active)
                {
                    index = i;
                    break;
                }
                if (i == index)
                {
                    return Main.projectile[index];
                }
            }
            Main.projectile[index] = new Projectile();
            Main.projectile[index].position = new Vector2(x, y);
            Main.projectile[index].type = type;
            Main.projectile[index].style = style;
            Main.projectile[index].active = true;
            Main.projectile[index].maxLife = maxTime;
            Main.projectile[index].color = color;
            Main.projectile[index].whoAmI = index;
            Main.projectile[index].animated = animated;
            Main.projectile[index].Initialize();
            return Main.projectile[index];
        }
        float moveSpeed = 0.15f;
        float maxSpeed = 4f;
        float stopSpeed
        {
            get { return moveSpeed; }
        }
        float jumpSpeed
        {
            get { return maxSpeed; }
        }
        public void AI()
        {
            if (!active)
                return;

            Vector2 plr = Vector2.Zero;
            Vector2 ang = Vector2.Zero;
            switch (type)
            {
                case Necrosis:
                    if (velocity.X > maxSpeed)
                        velocity.X = maxSpeed;
                    if (velocity.X < -maxSpeed)
                        velocity.X = -maxSpeed;
                    if (velocity.Y > maxSpeed)
                        velocity.Y = maxSpeed;
                    if (velocity.Y < -maxSpeed)
                        velocity.Y = -maxSpeed;

                    position.X += velocity.X;
                    position.Y += velocity.Y;
                    break;
                case SwordSwipe:
                    plr = Main.LocalPlayer.position;
                    ang = NPC.AngleToSpeed(timer, 128f);
                    position = new Vector2(plr.X + ang.X, plr.Y + ang.Y);
                    hitbox = new Rectangle((int)position.X, (int)position.Y, width, height);
                    if (!Main.LocalPlayer.Movement())
                        return;
                    timer += 0.25f;
                    break;
                case SwordSwing:
                    plr = Main.LocalPlayer.position;
                    ang = NPC.AngleToSpeed(currentAngle, 32f);
                    position = new Vector2(plr.X + ang.X, plr.Y + ang.Y);
                    hitbox = new Rectangle((int)position.X, (int)position.Y, 32, 32);
                    break;
                default:
                    break;
            }
        }
        public static Vector2 AngleToSpeed(float angle, float amount)
        {
            float cos = (float)(amount * Math.Cos(angle));
            float sine = (float)(amount * Math.Sin(angle));
            return new Vector2(cos, sine);
        }
        public void Update()
        {
            var armory = Main.LocalPlayer.Armory;
            var item = armory[GUI.MainHand].Item;
            if (item != null)
            {
                switch (item.style)
                {
                    case ItemID.RedRust:
                        color = Color.Red;
                        texture = Main.MagicPixel;
                        goto default;
                    case ItemID.SilverLongsword:
                        color = Color.Silver;
                        texture = Main.MagicPixel;
                        goto default;
                    case ItemID.Zweihander:
                        color = Color.Yellow;
                        texture = Main.MagicPixel;
                        goto default;
                    default:
                        damage = item.damage;
                        break;
                }
            }

            switch (type)
            {
                case Necrosis:
                    goto default;
                case SwordSwipe:
                case SwordSwing:
                    if (!Main.LocalPlayer.Movement())
                        break;
                    currentAngle += 0.2f;
                    if (timeLeft++ > maxLife)
                    {
                        Kill();
                        IsAttacking = false;
                    }
                    break;
                default:
                    if (timeLeft++ > maxLife)
                        Kill();
                    break;
            }
            Foliage hit = default;
            if ((hit = GetHitFoliage()) != null)
                hit.velocity = Helper.AngleToSpeed(Helper.AngleTo(hit.Center, Center), 5f);
        }
        public Foliage GetHitFoliage()
        {
            foreach (Foliage fol in Main.foliage.Where(t => t != null && t.active))
            {
                if (fol.hitbox.Contains(strikePoint))
                {
                    return fol;
                }
            }
            return default(Foliage);
        }
        public void Kill()
        {
            Main.projectile[whoAmI].active = false;
        }
        int ticks;
        int frameY;
        int totalFrames = 7;
        public void PreDraw(SpriteBatch sb, bool animated = false)
        {
            if (texture == null || !active)
                return;
            if (animated)
            {
                int frameHeight = height;
                if (ticks++ % 5 == 0)
                    frameY++;
                if (frameY == totalFrames)
                    frameY = 0;
                sb.Draw(texture, hitbox, new Rectangle(0, frameY * frameHeight, width, height), Color.White);
            }
            else
            {
                if (owner == Main.LocalPlayer.whoAmI)
                {
                    float angle = OrientAngle(Main.player[owner].Center);
                    sb.Draw(texture, Main.player[owner].Center + Helper.AngleToSpeed(angle, Player.plrWidth), new Rectangle(0, 0, width, height), color, angle + 90f, new Vector2(0, height / 2), 1f, SpriteEffects.None, 0);
                    
                    strikePoint = Main.player[owner].Center + Helper.AngleToSpeed(angle + 27f * Draw.radian, height);
                    //  DEBUG drawing
                    //sb.Draw(Main.MagicPixel, new Rectangle((int)strikePoint.X - 5, (int)strikePoint.Y - 5, 10, 10), Color.Red);
                }
            }
        }
        private float OrientAngle(Vector2 other)
        {
            return Helper.AngleTo(Center, other) + 180f;
        }
    }
}
