using FoundationR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using FoundationR;
using FoundationR.Lib;
using FoundationR.Rew;
using FoundationR.Loader;
using FoundationR.Ext;
using FoundationR.Headers;




namespace cotf_rewd
{
    public class Dust : Entity
    {
        public Dust(float x, float y)
        {
            this.position = new Vector2(x, y);
        }
        public Dust(float x, float y, int width, int height, Color color)
        {
            this.position = new Vector2(x, y);
            this.color = color;
            this.width = width;
            this.height = height;
        }
        private float moveSpeed = 1f, maxSpeed = 4f, stopSpeed = 0.05f;
        public float x
        {
            get { return position.X; }
        }
        public float y
        {
            get { return position.Y; }
        }
        public new int width = 16, height = 16;
        public int z = -1;
        public static Dust NewDust(int x, int y, int width, int height, int type, Color color, int maxLife)
        {
            int index = 1000;
            for (int i = 0; i < Main.dust.Length; i++)
            {
                if (Main.dust[i] == null || !Main.dust[i].active)
                {
                    index = i;
                    break;
                }
                if (i == index)
                {
                    return Main.dust[index];
                }
            }
            Main.dust[index] = new Dust(x, y, width, height, color);
            Main.dust[index].type = type;
            Main.dust[index].active = true;
            Main.dust[index].maxLife = maxLife;
            Main.dust[index].whoAmI = index;
            return Main.dust[index];
        }
        int ticks;
        public void Update()
        {
            hitbox = new Rectangle((int)position.X, (int)position.Y, width, height);
            Effect();
            if (ticks++ % Main.mainFrameRate * 15 == 0)
                timeLeft++;
            if (timeLeft > maxLife)
            {
                Kill();
            }

            ProjectileCollision();
        }
        private void ProjectileCollision()
        {
            position.X += velocity.X;
            position.Y += velocity.Y;

            //  Stopping movement
            if (velocity.X > 0f)
                velocity.X -= stopSpeed;
            if (velocity.X < 0f)
                velocity.X += stopSpeed;
            if (velocity.Y > 0f)
                velocity.Y -= stopSpeed;
            if (velocity.Y < 0f)
                velocity.Y += stopSpeed;

            //  Clamp
            if (velocity.X > maxSpeed)
                velocity.X = maxSpeed;
            if (velocity.X < -maxSpeed)
                velocity.X = -maxSpeed;
            if (velocity.Y > maxSpeed)
                velocity.Y = maxSpeed;
            if (velocity.Y < -maxSpeed)
                velocity.Y = -maxSpeed;

            //  Movement speed set
            if (velocity.X < moveSpeed / 4 && velocity.X > -moveSpeed / 4)
                velocity.X = 0f;
            if (velocity.Y < moveSpeed / 4 && velocity.Y > -moveSpeed / 4)
                velocity.Y = 0f;

            foreach (Projectile proj in Main.projectile.Where(t => t != null && t.active))
            {
                if (proj.whoAmI == Main.LocalPlayer.whoAmI)
                {
                    if (hitbox.IntersectsWith(proj.hitbox))
                    {
                        var speed = NPC.AngleToSpeed(NPC.AngleTo(Main.LocalPlayer.Center, Center), moveSpeed);
                        velocity.X += speed.X;
                        velocity.Y += speed.Y;
                    }
                }
            }
        }
        public void Draw(RewBatch rb)
        {
            //  Room darkness & Culling
            for (int i = 0; i < Main.ground.Length; i++)
            {
                if (Main.ground[i] != null)
                {
                    if (!Main.ground[i].hitbox.Contains((int)Main.LocalPlayer.Center.X, (int)Main.LocalPlayer.Center.Y))
                    {
                        return;
                    }
                    if (!Main.ground[i].light)
                    {
                        return;
                    }
                }
            }

            //  Typical draw
            if (active)
            {
                rb.Draw(texture, new Rectangle((int)x - width / 2, (int)y - width / 2, width, height), color);
            }
        }
        private void Effect()
        {
            switch (type)
            {
                case 0:
                    color = Color.Green;
                    break;
                case 1:
                    color = Color.Red;
                    break;
                case 2:
                    color = Color.Yellow;
                    break;
            }
        }
        public void Kill(bool explode = false)
        {
            Main.dust[whoAmI].active = false;
            if (active && explode && Main.rand.NextDouble() >= 0.50f)
            {
                Projectile.NewProjectile((int)position.X, (int)position.Y, Main.rand.Next(-2, 2), Main.rand.Next(-2, 2), -1, 0, Color.Purple, 600);
            }
        }
        public class Waypoint
        {
            public const int
                Green = 0,
                Red = 1,
                Yellow = 2;
        }
    }
}
