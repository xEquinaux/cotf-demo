using FoundationR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;





namespace cotf_rewd
{
    public class Entity
    {
        public int style;
        public bool active;
        public int type;
        public int maxLife;
        public int statLife;
        public int timeLeft;
        public Vector2 position;
        public Vector2 velocity;
        public int whoAmI;
        public float scale = 1f;
        public float range;
        public Vector2 Center
        {
            get { return new Vector2(position.X + width / 2, position.Y + height / 2); }
        }
        public static bool Proximity(Vector2 vec1, Vector2 vec2, float distance)
        {
            return vec1.X < vec2.X + distance && vec1.X > vec2.X - distance && vec1.Y < vec2.Y + distance && vec1.Y > vec2.Y - distance;
        }
        public Rectangle hitbox;
        public Color color;
        public int width;
        public int height;
        public REW texture;
        public string Name;
        public bool discovered;
        public float alpha = 1f;
        public Stats stats;
        public Traits traits;
        public bool collide, colUp, colDown, colRight, colLeft;
        public bool lit = true;
        public static float Distance(Vector2 one, Vector2 two)
        {
            Vector2 v1 = one;
            Vector2 v2 = two;
            int a = (int)Math.Abs(v2.X - v1.X);
            int b = (int)Math.Abs(v2.Y - v1.Y);
            int c = (int)Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
            return c;
        }
        public bool Collision(Entity ent, int buffer = 4)
        {
            if (!active) return false;

            if (hitbox.IntersectsWith(new Rectangle((int)ent.position.X, (int)ent.position.Y, ent.width, ent.height)))
                ent.collide = true;
            //  Directions
            if (hitbox.IntersectsWith(new Rectangle((int)ent.position.X, (int)ent.position.Y - buffer, ent.width, 2)))
                ent.colUp = true;
            if (hitbox.IntersectsWith(new Rectangle((int)ent.position.X, (int)ent.position.Y + ent.height + buffer, ent.width, 2)))
                ent.colDown = true;
            if (hitbox.IntersectsWith(new Rectangle((int)ent.position.X + ent.width + buffer, (int)ent.position.Y, 2, ent.height)))
                ent.colRight = true;
            if (hitbox.IntersectsWith(new Rectangle((int)ent.position.X - buffer, (int)ent.position.Y, 2, ent.height)))
                ent.colLeft = true;

            return collide || colUp || colDown || colLeft || colRight;
        }
        public bool Collision(SimpleEntity ent, int buffer = 4)
        {
            if (!active) return false;
            bool flag = false;
            if (hitbox.IntersectsWith(new Rectangle((int)ent.position.X, (int)ent.position.Y, ent.width, ent.height)))
                ent.collide = true;
            //  Directions
            if (hitbox.IntersectsWith(new Rectangle((int)ent.position.X, (int)ent.position.Y - buffer, ent.width, 2)))
                ent.colUp = true;
            if (hitbox.IntersectsWith(new Rectangle((int)ent.position.X, (int)ent.position.Y + ent.height + buffer, ent.width, 2)))
                ent.colDown = true;
            if (hitbox.IntersectsWith(new Rectangle((int)ent.position.X + ent.width + buffer, (int)ent.position.Y, 2, ent.height)))
                ent.colRight = true;
            if (hitbox.IntersectsWith(new Rectangle((int)ent.position.X - buffer, (int)ent.position.Y, 2, ent.height)))
                ent.colLeft = true;
            flag = collide || colUp || colDown || colLeft || colRight;
            return flag;
        }
        public Color ColorShift(Color newColor, float distance)
        {
            return Color.FromArgb(
                (byte)Math.Min(color.R + (byte)(newColor.R * distance), 255), 
                (byte)Math.Min(color.G + (byte)(newColor.G * distance), 255), 
                (byte)Math.Min(color.B + (byte)(newColor.B * distance), 255), 
                color.A);
        }
        public float RangeNormal(Vector2 to, Vector2 from, float range = 100f)
        {
            return Math.Max((Distance(from, to) * -1f + range) / range, 0);
        }
        public Color DynamicTorch(float range = 100f)
        {
            var list = Light.entity;
            float num = RangeNormal(Center, list.OrderBy(t => t.Distance(this.Center, t.Center)).First(t => t != null && t.active && t.lit).Center, range);
            return ColorShift(Light.TorchLight, num);
        }
    }
    public class SimpleEntity
    {
        public int X
        {
            get { return (int)position.X; }
            set { position.X = value; }
        }
        public int Y
        {
            get { return (int)position.Y; }
            set { position.Y = value; }
        }
        public int style;
        public Item.Style itemStyle;
        public Item.Type itemType;
        public bool active;
        public int type;
        public int maxLife;
        public int statLife;                                                               
        public int timeLeft;
        public Vector2 position;
        public Vector2 velocity;
        public int whoAmI, owner;
        public float scale = 1f;
        public float knockBack;
        public int damage;
        public float range;
        public bool discovered;
        public float alpha = 1f;
        public Stats stats;
        public bool lit = true;
        public Vector2 Center
        {
            get { return new Vector2(position.X + width / 2, position.Y + height / 2); }
        }
        public bool Proximity(Vector2 vec, float distance)
        {
            return vec.X < Center.X + distance && vec.X > Center.X - distance && vec.Y < Center.Y + distance && vec.Y > Center.Y - distance;
        }
        public Rectangle hitbox
        {
            get { return new Rectangle((int)position.X, (int)position.Y, width, height); }
        }
        public Color color;
        public int width;
        public int height;
        public REW texture;
        public string text;
        public string Name;
        public bool collide, colUp, colDown, colRight, colLeft;
        public float Distance(Vector2 other)
        {
            Vector2 v1 = other;
            Vector2 v2 = Center;
            int a = (int)Math.Abs(v2.X - v1.X);
            int b = (int)Math.Abs(v2.Y - v1.Y);
            int c = (int)Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
            return c;
        }
        public float Distance(Vector2 one, Vector2 two)
        {
            Vector2 v1 = one;
            Vector2 v2 = two;
            int a = (int)Math.Abs(v2.X - v1.X);
            int b = (int)Math.Abs(v2.Y - v1.Y);
            int c = (int)Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
            return c;
        }
        public void CollideResult()
        {
            //  Collision reaction
            if (!colLeft && velocity.X < 0f)
                position.X += velocity.X;
            if (!colRight && velocity.X > 0f)
                position.X += velocity.X;
            if (!colUp && velocity.Y < 0f)
                position.Y += velocity.Y;
            if (!colDown & velocity.Y > 0f)
                position.Y += velocity.Y;
        }
        public bool Collision(SimpleEntity ent, int buffer = 4)
        {
            if (!active) return false;

            if (hitbox.IntersectsWith(new Rectangle((int)ent.position.X, (int)ent.position.Y, ent.width, ent.height)))
                ent.collide = true;
            //  Directions
            if (hitbox.IntersectsWith(new Rectangle((int)ent.position.X, (int)ent.position.Y - buffer, ent.width, 2)))
                ent.colUp = true;
            if (hitbox.IntersectsWith(new Rectangle((int)ent.position.X, (int)ent.position.Y + ent.height + buffer, ent.width, 2)))
                ent.colDown = true;
            if (hitbox.IntersectsWith(new Rectangle((int)ent.position.X + ent.width + buffer, (int)ent.position.Y, 2, ent.height)))
                ent.colRight = true;
            if (hitbox.IntersectsWith(new Rectangle((int)ent.position.X - buffer, (int)ent.position.Y, 2, ent.height)))
                ent.colLeft = true;

            return collide || colUp || colDown || colLeft || colRight;
        }
        public bool Collision(Entity ent, int buffer = 4)
        {
            if (!active) return false;

            if (hitbox.IntersectsWith(new Rectangle((int)ent.position.X, (int)ent.position.Y, ent.width, ent.height)))
                ent.collide = true;
            //  Directions
            if (hitbox.IntersectsWith(new Rectangle((int)ent.position.X, (int)ent.position.Y - buffer, ent.width, 2)))
                ent.colUp = true;
            if (hitbox.IntersectsWith(new Rectangle((int)ent.position.X, (int)ent.position.Y + ent.height + buffer, ent.width, 2)))
                ent.colDown = true;
            if (hitbox.IntersectsWith(new Rectangle((int)ent.position.X + ent.width + buffer, (int)ent.position.Y, 2, ent.height)))
                ent.colRight = true;
            if (hitbox.IntersectsWith(new Rectangle((int)ent.position.X - buffer, (int)ent.position.Y, 2, ent.height)))
                ent.colLeft = true;
            
            return collide || colUp || colDown || colLeft || colRight;
        }
        public static bool Collision(SimpleEntity whoAmI, Entity collider, int buffer = 4)
        {
            if (!whoAmI.active) return false;

            if (collider.hitbox.IntersectsWith(new Rectangle((int)whoAmI.position.X, (int)whoAmI.position.Y, whoAmI.width, whoAmI.height)))
                whoAmI.collide = true;
            //  Directions
            if (collider.hitbox.IntersectsWith(new Rectangle((int)whoAmI.position.X, (int)whoAmI.position.Y - buffer, whoAmI.width, 2)))
                whoAmI.colUp = true;
            if (collider.hitbox.IntersectsWith(new Rectangle((int)whoAmI.position.X, (int)whoAmI.position.Y + whoAmI.height + buffer, whoAmI.width, 2)))
                whoAmI.colDown = true;
            if (collider.hitbox.IntersectsWith(new Rectangle((int)whoAmI.position.X + whoAmI.width + buffer, (int)whoAmI.position.Y, 2, whoAmI.height)))
                whoAmI.colRight = true;
            if (collider.hitbox.IntersectsWith(new Rectangle((int)whoAmI.position.X - buffer, (int)whoAmI.position.Y, 2, whoAmI.height)))
                whoAmI.colLeft = true;
            
            return whoAmI.collide || whoAmI.colUp || whoAmI.colDown || whoAmI.colLeft || whoAmI.colRight;
        }
        public Color ColorShift(Color newColor, float distance)
        {
            return Color.FromArgb(
                (byte)Math.Min(color.R + (byte)(newColor.R * distance), 255), 
                (byte)Math.Min(color.G + (byte)(newColor.G * distance), 255), 
                (byte)Math.Min(color.B + (byte)(newColor.B * distance), 255), 
                color.A);
        }
        public float RangeNormal(Vector2 to, Vector2 from, float range = 100f)
        {
            return Math.Max((Distance(from, to) * -1f + range) / range, 0);
        }
        public Color DynamicTorch(float range = 100f)
        {
            var list = Light.entity;
            float num = RangeNormal(Center, list.OrderBy(t => t.Distance(this.Center, t.Center)).First(t => t != null && t.active && t.lit).Center, range);
            return ColorShift(Light.TorchLight, num);
        }
    }
}
