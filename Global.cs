using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePort
{
    public class FontID
    {
        public const byte
            Arial = 0;
    }
    public class BackgroundID
    {
        public const byte
            Tiles = 0,
            Temp = 1,
            SmallTiles = 2;
    }
    public class NPCID
    {
        public const int
            Necrosis = -1,
            Kobold = 0;
    }
    public class ProjectileID
    {
        public const byte
            Orb = 0;
    }
    public class Stats
    {
        public int damage;
        public float knockback;
        public int defense;
        public bool detectItems;
        public bool detectMonsters;
        public bool detectTraps;
        public int bonusLife;
        public int bonusDefense;
        public bool electrified;
        public bool poisoned;
        public bool dowse;
        public bool onFire;
        public bool freeze;
        public int healAmount;
        public int energy;
        public int weight;
        public int mana;
        public int bonusMana;
        public int restoreMana;
        public int recoveryTime;
        public bool fastRecover;
        public bool fullRecover;
        public int totalLife;
        public int currentLife;
        public int totalMana;
        public short constitution;
        public float kbResistance;
        public int iFrames;
        public int level;
        public float lifeMultiplier;
        public float manaMultiplier;
    }
    public class Attributes
    {
        public int constitution;
        public int vitality;
        public int fortitude;
        public int clarity;
    }
    public class Affix
    {
        public float castRate;
        public float dodgeChance;
        public float damageReduction;
        public float blockRate;
        public float blockChance;
        public float hitRecovery;
        public float elementalDefense;
    }
    public class Traits
    {
        public float wellBeing;
        public float courage;
        public float streetSmarts;
        public float bookSmarts;
        public static Traits operator +(Traits a, Traits b)
        {
            var t = new Traits();
            t.bookSmarts = a.bookSmarts + b.bookSmarts;
            t.courage = a.courage + b.courage;
            t.streetSmarts = a.streetSmarts + b.streetSmarts;
            t.wellBeing = a.wellBeing + b.wellBeing;
            return t;
        }
    }
    public struct PointData
    {
        public bool active;
        public int x;
        public int y;
        public System.Drawing.Color type;
        public PointData(bool active, int x, int y, System.Drawing.Color type)
        {
            this.active = active;
            this.x = x;
            this.y = y;
            this.type = type;
        }
    }
    public class Draw
    {
        public const float radian = 0.017f;
        public static float Circumference(float distance)
        {
            return radian * (45f / distance);
        }
        public float radians(float distance)
        {
            return radian * (45f / distance);
        }
    }
    public class WorldGen
    {
        public static rand genRand
        {
            get { return Main.rand; }
        }
    }
    public struct Tile
    {
        public static Texture2D texture;
        public void type(int x, int y, ushort type)
        {
            //bmp.SetPixel(x, y, Main.types[type]);
        }
        public byte liquid;
        public bool active()
        {
            return Main.rand.NextBool();
        }
        public bool active(bool active)
        {
            return active;
        }
    }
    public class TileID
    {
        public const byte
            Empty = 0,
            Ash = 1,
            Ore = 2,
            Stone = 3,
            Plant = 4,
            Furniture = 5,
            Pillar = 6,
            StairsUp = 7,
            StairsDown = 8,
            Item = 9,
            Monster = 10,
            Torch = 11,
            Trap = 12;
    }
    public class CoinID
    {
        public const byte
            Iron = 0,
            Copper = 1,
            Silver = 2,
            Gold = 3,
            Platinum = 4;
    }
    public class TrapID
    {
        public const byte
            None = 0,
            Trapdoor = 1,
            Cavein = 2,
            Teleporter = 3,
            Flame = 4,
            Acid = 5,
            Dart = 6,
            Mute = 7;
    }
    public class ItemID
    {
        public const short
            RedRust = 0,
            SmallPack = 1,
            MediumPack = 2,
            LargePack = 3,
            SmallChest = 4,
            MediumChest = 5,
            LargeChest = 6,
            SilverLongsword = 7,
            Dirk = 8,
            Zweihander = 9,
            Torch = 10,
            Purse = 11,
            CoinIron = 12,
            CoinCopper = 13,
            CoinSilver = 14,
            CoinGold = 15,
            CoinPlatinum = 16;
    }
    public struct NewRectangle
    {
        public int x;
        public int y;
        public int Width;
        public int Height;
        public NewRectangle(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.Width = width;
            this.Height = height;
        }
        public Vector2 GetVector2()
        {
            return new Vector2(x, y);
        }
        public static Rectangle Empty
        {
            get { return new Rectangle(); }
        }
        private bool Intersects(NewRectangle other)
        {
            return other.x >= x && other.x <= x + Width && other.y >= y && other.y <= y + Height;
        }
        public bool Intersect(NewRectangle other)
        {
            int width = Width / (Width / 4);
            int height = Height / (Height / 4);
            for (int i = x; i < x + width; i += width)
            {
                for (int j = y; j < y + height; j += height)
                {
                    if (i >= other.x && i <= other.x + other.Width && j >= other.y && j <= other.y + other.Height)
                        return true;
                }
            }
            return false;
        }
        public System.Drawing.Rectangle GetSDRectangle()
        {
            return new System.Drawing.Rectangle(x, y, Width, Height);
        }
        public bool TopCollide(float X, float Y)
        {
            return X >= this.x && X <= this.x + Width && Y >= this.y;
        }
        public bool Contains(float X, float Y, int width, int height)
        {

            return
                X + width >= this.x &&
                X <= this.x + Width &&
                Y + height >= this.y &&
                Y <= this.y + Height;
        }
        public bool Contains(float X, float Y)
        {
            return
                X >= this.x &&
                X <= this.x + Width &&
                Y >= this.y &&
                Y <= this.y + Height;
        }
        public bool Contains(Vector2 v2)
        {
            return
                v2.X >= this.x &&
                v2.X <= this.x + Width &&
                v2.Y >= this.y &&
                v2.Y <= this.y + Height;
        }
        public bool RightCollide(float X, float Y)
        {
            return X >= this.x && Y >= this.y && Y <= this.y + Height;
        }
        public bool LeftCollide(float X, float Y)
        {
            return X <= this.x + Width && Y >= this.y && Y <= this.y + Height;
        }
        public bool Contains(int X, int Y)
        {
            for (int m = x; m < x + Width; m++)
            {
                for (int n = y; n < y + Height; n++)
                {
                    if (X >= x && X <= x + Width)
                    {
                        if (Y >= y && Y <= y + Height)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
    public class VelocityMech
    {
        public static void BasicSlowClamp(NPC npc, float moveSpeed, float stopSpeed, float maxSpeed, bool moving = false)
        {
            //  Stopping movement
            if (npc.velocity.X > 0f && !moving)
                npc.velocity.X -= stopSpeed;
            if (npc.velocity.X < 0f && !moving)
                npc.velocity.X += stopSpeed;
            if (npc.velocity.Y > 0f && !moving)
                npc.velocity.Y -= stopSpeed;
            if (npc.velocity.Y < 0f && !moving)
                npc.velocity.Y += stopSpeed;

            //  Clamp
            if (npc.velocity.X > maxSpeed)
                npc.velocity.X = maxSpeed;
            if (npc.velocity.X < -maxSpeed)
                npc.velocity.X = -maxSpeed;
            if (npc.velocity.Y > maxSpeed)
                npc.velocity.Y = maxSpeed;
            if (npc.velocity.Y < -maxSpeed)
                npc.velocity.Y = -maxSpeed;

            //  Movement zeroing
            if (npc.velocity.X < moveSpeed && npc.velocity.X > -moveSpeed)
                npc.velocity.X = 0f;
            if (npc.velocity.Y < moveSpeed && npc.velocity.Y > -moveSpeed)
                npc.velocity.Y = 0f;
        }
    }
    public class Helper
    {
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

    }
}
