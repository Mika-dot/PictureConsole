# PictureConsole
Outputting images to the console.

The first way to display the image
```c#
ConsoleWriteImage(bmp);
```

the second way to display the image
```c#
ConsoleWriteImage(new Bitmap("C:\\1.png"));
```

Getting a picture from an ip camera
```c#
Bitmap image = BmpURL("C:\\1.png", true);
```

the third way to display the image
```c#
CieLab.ComputeColors();
Bitmap image = new Bitmap("C:\\1.png", true);
CieLab.DrawImage(image);
```

>cool withdrawal method

the fourth way of displaying the image cool
```c#
var handler = GetConsoleHandle();

using (var graphics = Graphics.FromHwnd(handler))

using (var image = Image.FromFile("C:\\1.png"))

graphics.DrawImage(image, 50, 50, 250, 200);
```

***better with that***

```c#
[DllImport("kernel32.dll", EntryPoint = "GetConsoleWindow", SetLastError = true)]

private static extern IntPtr GetConsoleHandle();
```
