# iot-car
[ 菜鸟也玩IoT ] 教你借助Win10手机制作Arduino蓝牙遥控小车 
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
