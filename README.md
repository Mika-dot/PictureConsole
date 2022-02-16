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
<p align="center">
  <img src="https://github.com/Mika-dot/PictureConsole/blob/main/img/ConsoleWriteImage.PNG?raw=true" alt="ConsoleWriteImage"/>
</p>

Getting a picture from an ip camera
```c#
Bitmap image = BmpURL("http://185.10.80.33:8082/cgi-bin/guestimage.html");
```

the third way to display the image
```c#
CieLab.ComputeColors();
Bitmap image = new Bitmap("C:\\1.png", true);
CieLab.DrawImage(image);
```
<p align="center">
  <img src="https://github.com/Mika-dot/PictureConsole/blob/main/img/ComputeColors.PNG?raw=true" alt="ComputeColors"/>
</p>

>cool withdrawal method

the fourth way of displaying the image cool
```c#
var handler = GetConsoleHandle();

using (var graphics = Graphics.FromHwnd(handler))

using (var image = Image.FromFile("C:\\1.png"))

graphics.DrawImage(image, 50, 50, 250, 200);
```

<p align="center">
  <img src="https://github.com/Mika-dot/PictureConsole/blob/main/img/GetConsoleHandle.PNG?raw=true" alt="GetConsoleHandle"/>
</p>

***better with that***

```c#
[DllImport("kernel32.dll", EntryPoint = "GetConsoleWindow", SetLastError = true)]

private static extern IntPtr GetConsoleHandle();
```
