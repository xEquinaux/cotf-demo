using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePort
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
        public Texture2D texture;
        public string Name;
        public bool discovered;
        public float alpha = 1f;
        public Stats stats;
        public float Distance(Vector2 one, Vector2 two)
        {
            Vector2 v1 = one;
            Vector2 v2 = two;
            int a = (int)Math.Abs(v2.X - v1.X);
            int b = (int)Math.Abs(v2.Y - v1.Y);
            int c = (int)Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
            return c;
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
        public Texture2D texture;
        public string text;
        public string Name;
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
    }
}
