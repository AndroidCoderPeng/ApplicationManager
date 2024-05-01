﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;
using ApplicationManager.Utils;
using Prism.Commands;
using Prism.Mvvm;

namespace ApplicationManager.ViewModels
{
    /// <summary>
    /// 所有的View必须在Views包下面
    /// 所有的ViewModel必须在ViewModels包下面
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        #region VM

        private ObservableCollection<string> _deviceItems;

        public ObservableCollection<string> DeviceItems
        {
            get => _deviceItems;
            set
            {
                _deviceItems = value;
                RaisePropertyChanged();
            }
        }

        private string _androidId;

        public string AndroidId
        {
            get => _androidId;
            set
            {
                _androidId = value;
                RaisePropertyChanged();
            }
        }

        private string _deviceModel;

        public string DeviceModel
        {
            get => _deviceModel;
            set
            {
                _deviceModel = value;
                RaisePropertyChanged();
            }
        }

        private string _deviceBrand;

        public string DeviceBrand
        {
            get => _deviceBrand;
            set
            {
                _deviceBrand = value;
                RaisePropertyChanged();
            }
        }

        private string _deviceName;

        public string DeviceName
        {
            get => _deviceName;
            set
            {
                _deviceName = value;
                RaisePropertyChanged();
            }
        }

        private string _deviceCpu;

        public string DeviceCpu
        {
            get => _deviceCpu;
            set
            {
                _deviceCpu = value;
                RaisePropertyChanged();
            }
        }

        private string _deviceAbi;

        public string DeviceAbi
        {
            get => _deviceAbi;
            set
            {
                _deviceAbi = value;
                RaisePropertyChanged();
            }
        }

        private string _androidVersion;

        public string AndroidVersion
        {
            get => _androidVersion;
            set
            {
                _androidVersion = value;
                RaisePropertyChanged();
            }
        }

        private string _deviceSize;

        public string DeviceSize
        {
            get => _deviceSize;
            set
            {
                _deviceSize = value;
                RaisePropertyChanged();
            }
        }

        private string _deviceDensity;

        public string DeviceDensity
        {
            get => _deviceDensity;
            set
            {
                _deviceDensity = value;
                RaisePropertyChanged();
            }
        }

        private string _memory;

        public string Memory
        {
            get => _memory;
            set
            {
                _memory = value;
                RaisePropertyChanged();
            }
        }

        private string _batteryState;

        public string BatteryState
        {
            get => _batteryState;
            set
            {
                _batteryState = value;
                RaisePropertyChanged();
            }
        }

        private string _battery;

        public string Battery
        {
            get => _battery;
            set
            {
                _battery = value;
                RaisePropertyChanged();
            }
        }

        private string _batteryTemperature;

        public string BatteryTemperature
        {
            get => _batteryTemperature;
            set
            {
                _batteryTemperature = value;
                RaisePropertyChanged();
            }
        }

        private string _applicationName;

        public string ApplicationName
        {
            get => _applicationName;
            set
            {
                _applicationName = value;
                RaisePropertyChanged();
            }
        }

        private string _packageName;

        public string PackageName
        {
            get => _packageName;
            set
            {
                _packageName = value;
                RaisePropertyChanged();
            }
        }

        private string _applicationVersion;

        public string ApplicationVersion
        {
            get => _applicationVersion;
            set
            {
                _applicationVersion = value;
                RaisePropertyChanged();
            }
        }

        private string _fileSize;

        public string FileSize
        {
            get => _fileSize;
            set
            {
                _fileSize = value;
                RaisePropertyChanged();
            }
        }

        private string _deviceState;

        public string DeviceState
        {
            get => _deviceState;
            set
            {
                _deviceState = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region DelegateCommand

        public DelegateCommand RefreshDeviceCommand { set; get; }
        public DelegateCommand<string> DeviceSelectedCommand { set; get; }
        public DelegateCommand RebootDeviceCommand { set; get; }
        public DelegateCommand DisconnectDeviceCommand { set; get; }
        public DelegateCommand OutputImageCommand { set; get; }
        public DelegateCommand ImportImageCommand { set; get; }
        public DelegateCommand ScreenshotCommand { set; get; }
        public DelegateCommand SelectFileCommand { set; get; }
        public DelegateCommand UninstallCommand { set; get; }
        public DelegateCommand InstallCommand { set; get; }

        #endregion

        private string _selectDeviceAddress = string.Empty;
        
        private readonly DispatcherTimer _refreshDeviceTimer = new DispatcherTimer
        {
            Interval = new TimeSpan(0, 0, 3)
        };

        public MainWindowViewModel()
        {
            //异步尝试获取设备列表，可能会为空，因为开发者模式可能没开
            Task.Run(delegate { DeviceItems = GetDevices(); });
            
            _refreshDeviceTimer.Tick += delegate
            {
                if (string.IsNullOrEmpty(_selectDeviceAddress))
                {
                    return;
                }
                
                GetDeviceDetail();
            };
            _refreshDeviceTimer.Start();

            RefreshDeviceCommand = new DelegateCommand(delegate { DeviceItems = GetDevices(); });

            DeviceSelectedCommand = new DelegateCommand<string>(address =>
            {
                _selectDeviceAddress = address;
                //异步线程处理
                Task.Run(GetDeviceDetail);
            });
        }

        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<string> GetDevices()
        {
            var result = new ObservableCollection<string>();
            var creator = new CommandCreator();
            var command = creator.Append("devices").Build();
            CommandManager.Get.ExecuteCommand(command, delegate(string value)
            {
                if (value.Equals("List of devices attached"))
                {
                    //TODO
                    Console.WriteLine(@"无设备");
                    return;
                }

                // 259dc884        device
                // 192.168.3.11:40773      device
                //解析返回值，序列化成 ObservableCollection
                var strings = value.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (var i = 1; i < strings.Length; i++)
                {
                    var newLine = Regex.Replace(strings[i], @"\s", "*");
                    var split = newLine.Split(new[] { "*" }, StringSplitOptions.RemoveEmptyEntries);
                    result.Add(split[0]);
                }
            });
            return result;
        }

        /// <summary>
        /// 获取设备详情
        /// </summary>
        private void GetDeviceDetail()
        {
            var creator = new CommandCreator();
            //adb shell settings get secure android_id 查看android id
            var androidIdCommand = creator.Init()
                .Append("shell").Append("settings").Append("get").Append("secure").Append("android_id").Build();
            CommandManager.Get.ExecuteCommand(androidIdCommand, delegate(string value) { AndroidId = value; });

            //adb shell getprop ro.product.model 获取设备型号
            var modelCommand = creator.Init().Append("shell").Append("getprop").Append("ro.product.model").Build();
            CommandManager.Get.ExecuteCommand(modelCommand, delegate(string value) { DeviceModel = value; });

            //adb shell getprop ro.product.brand 获取设备品牌
            var brandCommand = creator.Init().Append("shell").Append("getprop").Append("ro.product.brand").Build();
            CommandManager.Get.ExecuteCommand(brandCommand, delegate(string value) { DeviceBrand = value; });

            //adb shell getprop ro.product.name 获取设备名称
            var nameCommand = creator.Init().Append("shell").Append("getprop").Append("ro.product.name").Build();
            CommandManager.Get.ExecuteCommand(nameCommand, delegate(string value) { DeviceName = value; });

            //adb shell getprop ro.product.board 获取处理器型号
            var boardCommand = creator.Init().Append("shell").Append("getprop").Append("ro.product.board").Build();
            CommandManager.Get.ExecuteCommand(boardCommand, delegate(string value) { DeviceCpu = value; });

            //adb shell getprop ro.product.cpu.abilist 获取CPU支持的abi架构列表
            var abiCommand = creator.Init().Append("shell").Append("getprop").Append("ro.product.cpu.abilist").Build();
            CommandManager.Get.ExecuteCommand(abiCommand, delegate(string value) { DeviceAbi = value; });

            //adb shell getprop ro.build.version.release 获取设备Android系统版本
            var versionCommand = creator.Init()
                .Append("shell").Append("getprop").Append("ro.build.version.release").Build();
            CommandManager.Get.ExecuteCommand(versionCommand, delegate(string value) { AndroidVersion = value; });

            //adb shell wm size 获取设备屏幕分辨率
            var sizeCommand = creator.Init().Append("shell").Append("wm").Append("size").Build();
            CommandManager.Get.ExecuteCommand(sizeCommand, delegate(string value)
            {
                //Physical size: 1240x2772
                DeviceSize = value.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
            });

            //adb shell wm density 获取设备屏幕密度
            var densityCommand = creator.Init().Append("shell").Append("wm").Append("density").Build();
            CommandManager.Get.ExecuteCommand(densityCommand, delegate(string value)
            {
                //Physical density: 560
                DeviceDensity = value.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
            });

            //adb shell cat /proc/meminfo 获取手机内存信息
            var meminfoCommand = creator.Init().Append("shell").Append("cat").Append("/proc/meminfo").Build();
            CommandManager.Get.ExecuteCommand(meminfoCommand, delegate(string value) { });

            //adb shell dumpsys battery 监控电池信息
            var batteryCommand = creator.Init().Append("shell").Append("dumpsys").Append("battery").Build();
            CommandManager.Get.ExecuteCommand(batteryCommand, delegate(string value)
            {
                var strings = value.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                var dictionary = strings.Select(
                    temp => temp.Split(new[] { ":" }, StringSplitOptions.None)
                ).ToDictionary(
                    split => split[0].Trim(), split => split[1].Trim()
                );

                foreach (var kvp in dictionary)
                {
                    switch (kvp.Key)
                    {
                        case "status":
                            // 2:正充电；3：没插充电器；4：不充电； 5：电池充满
                            switch (kvp.Value)
                            {
                                case "2":
                                    BatteryState = "正在充电";
                                    break;

                                case "5":
                                    BatteryState = "充电完成";
                                    break;

                                default:
                                    BatteryState = "未充电";
                                    break;
                            }

                            break;
                        case "level":
                            Battery = kvp.Value;
                            break;

                        case "temperature":
                            var temperature = int.Parse(kvp.Value) * 0.1;
                            BatteryTemperature = $"{temperature}℃";
                            break;
                    }
                }
            });
        }
    }
}