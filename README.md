
# iot-car
原帖：http://bbs.wfun.com/thread-962889-1-1.html
---
别忘了给你的UWP配置好权限，否则你是不能调用手机的蓝牙接口的：

配置文件就是Package.appxmanifest
关键在下面这一段，写到</Applications>的后面
Package.appxmanifest：
```
<Capabilities>
    <Capability Name="internetClient" />
    <Capability Name="internetClientServer" />
    <DeviceCapability Name="bluetooth" />
    <DeviceCapability Name="bluetooth.rfcomm">
      <Device Id="any">
        <Function Type="name:serialPort" />
      </Device>
    </DeviceCapability>
</Capabilities>
```

##电路接线图：
![image](http://bbs.wfun.com/data/attachment/forum/201612/04/000243eyfoyww6w1f92wl8.jpg)
![image](http://bbs.wfun.com/data/attachment/forum/201612/04/100457kownboj8z2xddxhj.jpg.thumb.jpg)

你可能有点疑惑，为啥要用 q,w,a,s,z,x 这6个字符，注意看下图，相信你能懂：

![image](http://bbs.wfun.com/data/attachment/forum/201612/03/235515fskn353drtswl3dd.jpg)

---
最后附上，我这整个项目用到的一些软件工具，大部分都在微软官方商店有下载：

##[ IDE ]

Arduino IDE 官方UWP版（Win32转制，叼的不要不要的）：
https://www.microsoft.com/zh-cn/store/p/arduino-ide/9nblggh4rsd8?tduid=(0f615a0c23892bb37dd5b8ba5828b9e1)(235166)(2858066)()()

Arduino IDE For Visual Studio 插件版：
http://www.visualmicro.com/page/Arduino-Visual-Studio-Downloads.aspx

以上二选一，我推荐后者，有代码提示。

##[ C#编写 ]

然后是C#编程，这就不说了 Visual Studio 必须的：

https://www.visualstudio.com/zh-hans/

##[ 蓝牙串口调试工具 ]

商店里面搜索"蓝牙串口"，有很多。

比如，蓝牙串口调试助手（UWP免费试用版足够调试了）。
调试工具的作用，就是确保你的蓝牙模块和手机之间可以正常通信，比如用调试工具给你的蓝牙串口设备发送字符串，看看蓝牙串口设备能不能响应。

![image](http://bbs.wfun.com/data/attachment/forum/201612/03/212404wxs0sxsmxpm10173.jpg)
![image](http://bbs.wfun.com/data/attachment/forum/201612/03/212407rgeeulvckvykeye4.jpg)
