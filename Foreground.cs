﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePort
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
        public void Draw(SpriteBatch sb)
        {
            if (!active)
                return;
            if (!light)
                sb.Draw(Main.MagicPixel, hitbox, Color.Black);
        }
    }
}