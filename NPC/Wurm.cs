using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePort.NPCs
{
    public class Wurm_Head : NPC
    {
        public const byte
            Normal = 0;
        public static Wurm_Head head;
        public Wurm_Body[] body;
        public Wurm_Tail tail;
        const int maxIFrames = 15;
        private int length;
        public bool IsHit()
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj == null || !proj.active) continue;
                if (!proj.Friendly(proj.type)) continue;
                if (proj.hitbox.Intersects(hitbox) && stats.iFrames <= 0)
                {
                    stats.currentLife -= proj.damage;
                    stats.iFrames = maxIFrames;
                    return true;
                }
            }
            return false;
        }
        public static Wurm_Head NewWurm(int x, int y, int length, int type)
        {
            var wurm = new Wurm_Head();
            //  Body
            wurm.body = new Wurm_Body[length];
            for (int i = 0; i < length; i++)
            {
                wurm.body[i] = new Wurm_Body(wurm);
                wurm.body[i].X = x;
                wurm.body[i].Y = y;
                wurm.body[i].width = 32;
                wurm.body[i].height = 32;
                wurm.body[i].type = type;
                wurm.body[i].active = true;
            }
            wurm.length = length;

            //  Tail
            wurm.tail = new Wurm_Tail(wurm);
            wurm.tail.type = type;
            wurm.tail.width = 32;
            wurm.tail.height = 32;
            wurm.tail.X = x;
            wurm.tail.Y = y;
            wurm.tail.active = true;

            //  Head
            wurm.type = type;
            wurm.width = 32;
            wurm.height = 32;
            wurm.position = new Vector2(x, y);
            wurm.active = true;
            wurm.stats = new Stats()
            {
                totalLife = 100,
                currentLife = 100,
                damage = 10,
                defense = 3,
                iFrames = maxIFrames
            };
            wurm.traits = new Traits()
            {
                bookSmarts = 0.01f,
                streetSmarts = 0.1f,
                courage = 0.25f,
                wellBeing = 0.02f
            };
            head = wurm;
            return wurm;
        }
        public override void Kill()
        {
            Player.UpdateDowned(this);
            this.active = false;
            foreach (var b in body)
                b.active = false;
            tail.active = false;
        }
        public override void AI()
        {
            if (!active) return;

            hitbox = new Rectangle((int)position.X, (int)position.Y, width, height);

            float range = 24f;
            float maxSpeed = 2f;
            float distance;

            if (IsHit() && stats.currentLife <= 0)
            {
                Kill();
            }

            if (Main.LocalPlayer.Movement())
            {
                if (stats.iFrames > 0)
                    stats.iFrames--;

                for (int i = 1; i < length; i++)
                {
                    var previous = body[i - 1];
                    if ((distance = previous.Distance(previous.Center, body[i].Center)) > range)
                    {
                        previous.velocity = Helper.AngleToSpeed(Helper.AngleTo(previous.Center, body[i].Center), Math.Min(distance, maxSpeed));
                    }
                    else previous.velocity = Vector2.Zero;
                    previous.position += previous.velocity;
                }
                int neck = length - 1;
                if ((distance = body[neck].Distance(head.Center, body[neck].Center)) > range)
                {
                    body[neck].velocity = Helper.AngleToSpeed(Helper.AngleTo(body[neck].Center, head.Center), Math.Min(distance, maxSpeed));
                }
                else body[neck].velocity = Vector2.Zero;
                body[neck].position += body[neck].velocity;

                if ((distance = tail.Distance(tail.Center, body[0].Center)) > range)
                {
                    tail.velocity = Helper.AngleToSpeed(Helper.AngleTo(tail.Center, body[0].Center), Math.Min(distance, maxSpeed));
                }
                else tail.velocity = Vector2.Zero;
                tail.position += tail.velocity;

                if ((distance = Main.LocalPlayer.Distance(head.Center, Main.LocalPlayer.Center)) > range)
                {
                    head.velocity = Helper.AngleToSpeed(Helper.AngleTo(head.Center, Main.LocalPlayer.Center), Math.Min(distance, maxSpeed));
                }
                else head.velocity = Vector2.Zero;
                head.position += head.velocity;
            }
        }
        public void Draw(SpriteBatch sb)
        {
            if (!active) return;

            //  Head
            sb.Draw(Main.MagicPixel, hitbox, new Rectangle(0, 0, width, height), IFrames(Color.Blue), Helper.AngleTo(Center, Main.LocalPlayer.Center), new Vector2(width / 2, height / 2), SpriteEffects.None, 0f);
            //  Tail + 1
            sb.Draw(Main.MagicPixel, body[0].hitbox, new Rectangle(0, 0, width, height), Color.Blue, Helper.AngleTo(body[0].Center, body[1].Center), new Vector2(width / 2, height / 2), SpriteEffects.None, 0f);
            //  Body
            for (int i = 1; i < length; i++)
            {
                var previous = body[i - 1];
                var segment = body[i]; 
                sb.Draw(Main.MagicPixel, segment.hitbox, new Rectangle(0, 0, width, height), Color.Blue, Helper.AngleTo(previous.Center, segment.Center), new Vector2(width / 2, height / 2), SpriteEffects.None, 0f);
            }
            //  Tail
            sb.Draw(Main.MagicPixel, tail.hitbox, new Rectangle(0, 0, width, height), Color.Blue, Helper.AngleTo(tail.Center, body[0].Center), new Vector2(width / 2, height / 2), SpriteEffects.None, 0f);
        }

        private Color IFrames(Color color)
        {
            return stats.iFrames % 2 == 0 && stats.iFrames > 0 ? Color.Red : color;
        }
    }

    public class Wurm_Body : SimpleEntity
    {
        public Wurm_Head head;
        public Wurm_Body(Wurm_Head head)
        {
            this.head = head;
        }
    }
    public class Wurm_Tail: SimpleEntity
    {
        public Wurm_Head head;
        public Wurm_Tail(Wurm_Head head)
        {
            this.head = head;
        }
    }
}
