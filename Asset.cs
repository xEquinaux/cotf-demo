using FoundationR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace cotf_rewd
{
    internal class Asset
    {
        public static REW ConvertFromFile(string path)
        {
            return REW.Extract(new Bitmap(path), 32);
        }
        public static void ConvertFromFile(string path, out REW image)
        {
            image = REW.Extract(new Bitmap(path), 32);
        }
        public static REW LoadFromFile(string path)
        {
            REW rew;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                (rew = new REW()).ReadData(new BinaryReader(fs));
            }
            return rew;
        }
        public static void LoadFromFile(string path, out REW image)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                (image = new REW()).ReadData(new BinaryReader(fs));
            }
        }
    }
}
