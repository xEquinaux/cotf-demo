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
    public class Trap : SimpleEntity, IDisposable
    {
        public bool hidden;
        public bool activated;
        public static Trap NewTrap(int x, int y, int size, int type)
        {
            int num = 1000;
            for (int i = 0; i < Main.trap.Length; i++)
            {
                if (Main.trap[i] == null || !Main.trap[i].active)
                {
                    num = i;
                    break;
                }
                if (i == num)
                {
                    break;
                }
            }
            Main.trap[num] = new Trap();
            Main.trap[num].position = new Vector2(x, y);
            Main.trap[num].width = size;
            Main.trap[num].height = size;
            Main.trap[num].active = true;
            Main.trap[num].hidden = true;
            Main.trap[num].whoAmI = num;
            Main.trap[num].type = type;
            Main.trap[num].Initialize();
            return Main.trap[num];
        }
        private void Initialize()
        {
            switch (type)
            {
                case TrapID.Acid:
                default:
                    damage = 10;
                    texture = Main.Temporal;
                    break;
            }
        }
        public void Update(Player player)
        {
            if (active && !activated && Distance(player.Center, Center) <= 25f)
            {
                TrapEffects(player);
                activated = true;
            }
        }
        public void Draw(RewBatch rb)
        {
            if (active && activated) rb.Draw(Main.Temporal, hitbox, Color.White);
        }
        private void TrapEffects(Player player)
        {
            switch (type)
            {
                case TrapID.Acid:
                    player.Stats.currentLife -= 10;
                    break;
                default:
                    break;
            }
        }
        public void Dispose()
        {
            Main.trap[whoAmI].active = false;
            Main.trap[whoAmI] = null;
        }
    }
}
