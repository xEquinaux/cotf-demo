using FoundationR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;





namespace cotf_rewd
{
    public class Foreground : SimpleEntity
    {
        public bool light;
        public Background bg;
        public Foreground(int x, int y, int width, int height, Background bg)
        {
            this.width = width;
            this.height = height;
            position = new Vector2(x, y);
            active = true;
            this.bg = bg;
        }
        public void Draw(RewBatch rb)
        {
            if (!active)
                return;
            if (!light)
                rb.Draw(Main.MagicPixel, hitbox);//, Color.Black);
        }
    }
}
