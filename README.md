# PictureConsole
Outputting images to the console.

The first way to display the image
ConsoleWriteImage(bmp);


the second way to display the image
ConsoleWriteImage(new Bitmap("C:\\1.png"));



the third way to display the image
CieLab.ComputeColors();
Bitmap image = new Bitmap("C:\\1.png", true);
CieLab.DrawImage(image);


>cool withdrawal method
the fourth way of displaying the image cool
var handler = GetConsoleHandle();
using (var graphics = Graphics.FromHwnd(handler))
using (var image = Image.FromFile("C:\\1.png"))
graphics.DrawImage(image, 50, 50, 250, 200);

***better with that***

[DllImport("kernel32.dll", EntryPoint = "GetConsoleWindow", SetLastError = true)]
private static extern IntPtr GetConsoleHandle();
