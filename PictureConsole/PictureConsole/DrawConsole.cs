using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PictureConsole
{
    public class DrawConsole
    {
        [DllImport("kernel32.dll", EntryPoint = "GetConsoleWindow", SetLastError = true)]
        public static extern IntPtr GetConsoleHandle();

        static int[] cColors = { 0x000000, 0x000080, 0x008000, 0x008080, 0x800000, 0x800080, 0x808000, 0xC0C0C0, 0x808080, 0x0000FF, 0x00FF00, 0x00FFFF, 0xFF0000, 0xFF00FF, 0xFFFF00, 0xFFFFFF };
        
        //ConsoleWriteImage(bmpSrc);

        //ConsoleWriteImage(new Bitmap("C:\\Users\\сервер\\Desktop\\ConsoleApp1\\1.png"));


        //CieLab.ComputeColors();
        //Bitmap image = new Bitmap("C:\\Users\\сервер\\Desktop\\ConsoleApp1\\1.png", true);
        //CieLab.DrawImage(image);

        //var handler = GetConsoleHandle();
        //using (var graphics = Graphics.FromHwnd(handler))
        //using (var image = Image.FromFile("C:\\Users\\сервер\\Desktop\\ConsoleApp1\\1.png"))
        //graphics.DrawImage(image, 50, 50, 250, 200);

        public static void ConsoleWritePixel(Color cValue)
        {
            Color[] cTable = cColors.Select(x => Color.FromArgb(x)).ToArray();
            char[] rList = new char[] { (char)9617, (char)9618, (char)9619, (char)9608 }; // 1/4, 2/4, 3/4, 4/4
            int[] bestHit = new int[] { 0, 0, 4, int.MaxValue }; //ForeColor, BackColor, Symbol, Score

            for (int rChar = rList.Length; rChar > 0; rChar--)
            {
                for (int cFore = 0; cFore < cTable.Length; cFore++)
                {
                    for (int cBack = 0; cBack < cTable.Length; cBack++)
                    {
                        int R = (cTable[cFore].R * rChar + cTable[cBack].R * (rList.Length - rChar)) / rList.Length;
                        int G = (cTable[cFore].G * rChar + cTable[cBack].G * (rList.Length - rChar)) / rList.Length;
                        int B = (cTable[cFore].B * rChar + cTable[cBack].B * (rList.Length - rChar)) / rList.Length;
                        int iScore = (cValue.R - R) * (cValue.R - R) + (cValue.G - G) * (cValue.G - G) + (cValue.B - B) * (cValue.B - B);
                        if (!(rChar > 1 && rChar < 4 && iScore > 50000)) // rule out too weird combinations
                        {
                            if (iScore < bestHit[3])
                            {
                                bestHit[3] = iScore; //Score
                                bestHit[0] = cFore;  //ForeColor
                                bestHit[1] = cBack;  //BackColor
                                bestHit[2] = rChar;  //Symbol
                            }
                        }
                    }
                }
            }
            Console.ForegroundColor = (ConsoleColor)bestHit[0];
            Console.BackgroundColor = (ConsoleColor)bestHit[1];
            Console.Write(rList[bestHit[2] - 1]);
        }
        public static void ConsoleWriteImage(Bitmap source)
        {
            int sMax = 39;
            decimal percent = Math.Min(decimal.Divide(sMax, source.Width), decimal.Divide(sMax, source.Height));
            Size dSize = new Size((int)(source.Width * percent), (int)(source.Height * percent));
            Bitmap bmpMax = new Bitmap(source, dSize.Width * 2, dSize.Height);
            for (int i = 0; i < dSize.Height; i++)
            {
                for (int j = 0; j < dSize.Width; j++)
                {
                    ConsoleWritePixel(bmpMax.GetPixel(j * 2, i));
                    ConsoleWritePixel(bmpMax.GetPixel(j * 2 + 1, i));
                }
                System.Console.WriteLine();
            }
            Console.ResetColor();
        }
        public static void ConsoleWriteImageBlur(Bitmap bmpSrc)
        {
            int sMax = 39;
            decimal percent = Math.Min(decimal.Divide(sMax, bmpSrc.Width), decimal.Divide(sMax, bmpSrc.Height));
            Size resSize = new Size((int)(bmpSrc.Width * percent), (int)(bmpSrc.Height * percent));
            Func<System.Drawing.Color, int> ToConsoleColor = c =>
            {
                int index = (c.R > 128 | c.G > 128 | c.B > 128) ? 8 : 0;
                index |= (c.R > 64) ? 4 : 0;
                index |= (c.G > 64) ? 2 : 0;
                index |= (c.B > 64) ? 1 : 0;
                return index;
            };
            Bitmap bmpMin = new Bitmap(bmpSrc, resSize);
            for (int i = 0; i < resSize.Height; i++)
            {
                for (int j = 0; j < resSize.Width; j++)
                {
                    Console.ForegroundColor = (ConsoleColor)ToConsoleColor(bmpMin.GetPixel(j, i));
                    Console.Write("██");
                }
                System.Console.WriteLine();
            }
        }

        public static Bitmap BmpURL(string url)
        {
            PictureBox picbox = new PictureBox();
            picbox.Load(url);
            return (Bitmap)picbox.Image;          
        }
        public class CieLab
        {
            public double L { get; set; }
            public double A { get; set; }
            public double B { get; set; }
            public static double DeltaE(CieLab l1, CieLab l2)
            {
                return Math.Pow(l1.L - l2.L, 2) + Math.Pow(l1.A - l2.A, 2) + Math.Pow(l1.B - l2.B, 2);
            }

            static List<ConsolePixel> pixels;
            public static void ComputeColors()
            {
                pixels = new List<ConsolePixel>();

                char[] chars = { '█', '▓', '▒', '░' };

                int[] rs = { 0, 0, 0, 0, 128, 128, 128, 192, 128, 0, 0, 0, 255, 255, 255, 255 };
                int[] gs = { 0, 0, 128, 128, 0, 0, 128, 192, 128, 0, 255, 255, 0, 0, 255, 255 };
                int[] bs = { 0, 128, 0, 128, 0, 128, 0, 192, 128, 255, 0, 255, 0, 255, 0, 255 };

                for (int i = 0; i < 16; i++)
                    for (int j = i + 1; j < 16; j++)
                    {
                        var l1 = RGBtoLab(rs[i], gs[i], bs[i]);
                        var l2 = RGBtoLab(rs[j], gs[j], bs[j]);

                        for (int k = 0; k < 4; k++)
                        {
                            var l = CieLab.Combine(l1, l2, (4 - k) / 4.0);

                            pixels.Add(new ConsolePixel
                            {
                                Char = chars[k],
                                Forecolor = (ConsoleColor)i,
                                Backcolor = (ConsoleColor)j,
                                Lab = l
                            });
                        }
                    }
            }
            public static CieLab Combine(CieLab l1, CieLab l2, double amount)
            {
                var l = l1.L * amount + l2.L * (1 - amount);
                var a = l1.A * amount + l2.A * (1 - amount);
                var b = l1.B * amount + l2.B * (1 - amount);

                return new CieLab { L = l, A = a, B = b };
            }
            public static CieLab RGBtoLab(int red, int green, int blue)
            {
                var rLinear = red / 255.0;
                var gLinear = green / 255.0;
                var bLinear = blue / 255.0;

                double r = rLinear > 0.04045 ? Math.Pow((rLinear + 0.055) / (1 + 0.055), 2.2) : (rLinear / 12.92);
                double g = gLinear > 0.04045 ? Math.Pow((gLinear + 0.055) / (1 + 0.055), 2.2) : (gLinear / 12.92);
                double b = bLinear > 0.04045 ? Math.Pow((bLinear + 0.055) / (1 + 0.055), 2.2) : (bLinear / 12.92);

                var x = r * 0.4124 + g * 0.3576 + b * 0.1805;
                var y = r * 0.2126 + g * 0.7152 + b * 0.0722;
                var z = r * 0.0193 + g * 0.1192 + b * 0.9505;

                Func<double, double> Fxyz = t => ((t > 0.008856) ? Math.Pow(t, (1.0 / 3.0)) : (7.787 * t + 16.0 / 116.0));

                return new CieLab
                {
                    L = 116.0 * Fxyz(y / 1.0) - 16,
                    A = 500.0 * (Fxyz(x / 0.9505) - Fxyz(y / 1.0)),
                    B = 200.0 * (Fxyz(y / 1.0) - Fxyz(z / 1.0890))
                };
            }
            public static void DrawImage(Bitmap source)
            {
                int width = Console.WindowWidth - 1;
                int height = (int)(width * source.Height / 2.0 / source.Width);

                using (var bmp = new Bitmap(source, width, height))
                {
                    var unit = GraphicsUnit.Pixel;
                    using (var src = bmp.Clone(bmp.GetBounds(ref unit), PixelFormat.Format24bppRgb))
                    {
                        var bits = src.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, src.PixelFormat);
                        byte[] data = new byte[bits.Stride * bits.Height];

                        Marshal.Copy(bits.Scan0, data, 0, data.Length);

                        for (int j = 0; j < height; j++)
                        {
                            StringBuilder builder = new StringBuilder();
                            var fore = ConsoleColor.White;
                            var back = ConsoleColor.Black;

                            for (int i = 0; i < width; i++)
                            {
                                int idx = j * bits.Stride + i * 3;
                                var pixel = DrawPixel(data[idx + 2], data[idx + 1], data[idx + 0]);


                                if (pixel.Forecolor != fore || pixel.Backcolor != back)
                                {
                                    Console.ForegroundColor = fore;
                                    Console.BackgroundColor = back;
                                    Console.Write(builder);

                                    builder.Clear();
                                }

                                fore = pixel.Forecolor;
                                back = pixel.Backcolor;
                                builder.Append(pixel.Char);
                            }

                            Console.ForegroundColor = fore;
                            Console.BackgroundColor = back;
                            Console.WriteLine(builder);
                        }

                        Console.ResetColor();
                    }
                }
            }
            private static ConsolePixel DrawPixel(int r, int g, int b)
            {
                var l = RGBtoLab(r, g, b);

                double diff = double.MaxValue;
                var pixel = pixels[0];

                foreach (var item in pixels)
                {
                    var delta = CieLab.DeltaE(l, item.Lab);
                    if (delta < diff)
                    {
                        diff = delta;
                        pixel = item;
                    }
                }

                return pixel;
            }

        }
        public class ConsolePixel
        {
            public char Char { get; set; }

            public ConsoleColor Forecolor { get; set; }
            public ConsoleColor Backcolor { get; set; }
            public CieLab Lab { get; set; }
        }
    }
}
