using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Arduino_bluetooth
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private RfcommDeviceService service;       
        private DeviceInformationCollection services;
        private StreamSocket socket;
        private DataWriter writer;
        private DataReader reader;
        private string deviceName;

        public MainPage()
        {
            this.InitializeComponent();
            var action = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() => {           
                
            }));
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
           bluetooth_list(); //进入APP立即搜索设备
           base.OnNavigatedTo(e);


        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            
            if (services == null || services.Count == 0)
            {
                bluetooth_list(); //先搜索设备
            }else if(service == null)
            {                
                bluetooth_connect(); //如果有搜索到设备，则连接选中的设备
            }

        }         
        /// <summary>
        /// 执行蓝牙设备搜索。
        /// </summary>
        public async void bluetooth_list()
        {
            services = await DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));
            //获取周围所有的蓝牙串口设备
            foreach (DeviceInformation v in services)
            {
                resultsListView.Items.Add(v.Name);
                //把找到的设备名称添加到前端XAML界面的ListView里面去
            }           

            if (services == null || services.Count == 0)
            {
                bluetooth_check();
            }
            else
            {
                btn_button_Click.Content = "继续连接";
                OnConnectionEstablished(true, false); //让连接按钮可点击
                btn_click_info.Background = new SolidColorBrush(Windows.UI.Colors.Blue);
            }
            

        }

        private void resultsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            foreach(string Item in e.AddedItems)
            {
                deviceName = Item;
               
            }
        }

        /// <summary>
        /// 开始连接蓝牙串口设备
        /// </summary>
        private async void bluetooth_connect()
        {

            if (deviceName == null)
            {
                await new MessageDialog("请您先从搜索到的蓝牙设备列表选择一个设备").ShowAsync();
                return;
            }
            OnConnectionEstablished(false, false); //点击连接按钮后立即禁用连接按钮，防止误操作
            services = await DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort)); //这里再扫描一次设备，是确保设备没有掉线。
            if (services.Count > 0)
            {
                foreach (DeviceInformation info in services)
                {
                    if (deviceName==info.Name)
                    {                      

                        service = await RfcommDeviceService.FromIdAsync(info.Id);

                        //调试代码
                        //Debug.WriteLine(service.DeviceAccessInformation.CurrentStatus.ToString());
                        //确保设备有权访问再连接，否则会遭遇闪退！
                        if (service.DeviceAccessInformation.CurrentStatus.ToString() != null && service.DeviceAccessInformation.CurrentStatus.ToString() == "Allowed")
                        {
                            socket = new StreamSocket();
                            await socket.ConnectAsync(service.ConnectionHostName, service.ConnectionServiceName);
                            writer = new DataWriter(socket.OutputStream);
                            reader = new DataReader(socket.InputStream);

                            OnConnectionEstablished(false, true); //连接成功后继续禁用连接按钮，但开启控制按钮。
                            btn_button_Click.Content = "连接成功！";                            
                            btn_click_info.Background = new SolidColorBrush(Windows.UI.Colors.LawnGreen);
                        }
                        else
                        {
                            bluetooth_check();
                            return;
                        }
                        
                    }
                    
                }


            }
            else
            {
                bluetooth_check();
                return;
            }

        }
        /// <summary>
        /// 断开蓝牙连接
        /// </summary>
        public void bluetooth_close()
        {
            if (writer != null)
            {
                writer.DetachStream();
                writer.Dispose();
                writer = null;
            }
            if (socket != null)
            {
                socket.Dispose();
                socket = null;
            }
            if (service != null)
            {
                service.Dispose();
                service = null;
            }
            //断开后让连接按钮可点击
            OnConnectionEstablished(true, false);            
            btn_button_Click.Content = "继续连接";
            btn_click_info.Background = new SolidColorBrush(Windows.UI.Colors.Blue);
        }
        /// <summary>
        /// 连接按钮状态，因为在已经连接成功的情况下重复点击连接会出错闪退
        /// </summary>
        /// <param name="off">不可点击</param>
        /// <param name="on">可点击</param>
        private void OnConnectionEstablished(bool off, bool on)
        {
            var action = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() => {
                btn_button_Click.IsEnabled = off;
                btn_clickLeftTOP.IsEnabled = on;
                btn_clickLeftBOTTOM.IsEnabled = on;
                btn_clickRightTOP.IsEnabled = on;
                btn_clickRightBOTTOM.IsEnabled = on;
            }));
        }
        /// <summary>
        /// 这个是调试用的，前端界面里面暂时没有用到
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async  void click_info(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:bluetooth"));
        }
        

        private async void bluetooth_check()
        {
            var message = new MessageDialog("搜索设备失败，请确保您的蓝牙串口设备已经开启连接");
            message.Commands.Add(new UICommand("确认"));
            message.Commands.Add(new UICommand("取消"));
            var result = await message.ShowAsync();
            if (result.Label == "确认")
            {
                await Launcher.LaunchUriAsync(new Uri("ms-settings:bluetooth"));
            }
        }


        private void button_close(object sender, RoutedEventArgs e)
        {
            bluetooth_close();
        }
        

        /// <summary>
        /// 左侧前进按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LeftTOP_Holding(object sender, HoldingRoutedEventArgs e)        {
            
            //如果按钮按下，则给串口设备发送一个字符"q"
            if(e.HoldingState == Windows.UI.Input.HoldingState.Started){
                writer.WriteString("q");
                await writer.StoreAsync();
            }else{ //如果按钮没有按下，比如释放掉，则给串口设备发送一个字符"z"
                writer.WriteString("z");
                await writer.StoreAsync();
            }
            
        }
        /// <summary>
        /// 左侧后退按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LeftBOTTOM_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                writer.WriteString("a");
                await writer.StoreAsync();
            }
            else
            {
                writer.WriteString("z");
                await writer.StoreAsync();
            }
        }
        /// <summary>
        /// 右侧前进按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RightTOP_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                writer.WriteString("w");
                await writer.StoreAsync();
            }
            else
            {
                writer.WriteString("x");
                await writer.StoreAsync();
            }
        }
        /// <summary>
        /// 右侧后退按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RightBOTTOM_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                writer.WriteString("s");
                await writer.StoreAsync();
            }
            else
            {
                writer.WriteString("x");
                await writer.StoreAsync();
            }
        }
        
    }
}
