using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using ApplicationManager.Utils;
using HandyControl.Controls;
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

        private string _memoryRatio;

        public string MemoryRatio
        {
            get => _memoryRatio;
            set
            {
                _memoryRatio = value;
                RaisePropertyChanged();
            }
        }

        private double _memoryProgress;

        public double MemoryProgress
        {
            get => _memoryProgress;
            set
            {
                _memoryProgress = value;
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

        private double _batteryProgress;

        public double BatteryProgress
        {
            get => _batteryProgress;
            set
            {
                _batteryProgress = value;
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

        private ObservableCollection<string> _applicationPackages;

        public ObservableCollection<string> ApplicationPackages
        {
            get => _applicationPackages;
            set
            {
                _applicationPackages = value;
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
        public DelegateCommand RefreshApplicationCommand { set; get; }
        public DelegateCommand<string> PackageSelectedCommand { set; get; }
        public DelegateCommand RebootDeviceCommand { set; get; }
        public DelegateCommand DisconnectDeviceCommand { set; get; }
        public DelegateCommand OutputImageCommand { set; get; }
        public DelegateCommand ImportImageCommand { set; get; }
        public DelegateCommand ScreenshotCommand { set; get; }
        public DelegateCommand SelectFileCommand { set; get; }
        public DelegateCommand UninstallCommand { set; get; }
        public DelegateCommand InstallCommand { set; get; }

        #endregion

        private string _selectedDeviceAddress = string.Empty;
        private string _selectedPackage = string.Empty;

        /// <summary>
        /// DispatcherTimer与窗体为同一个线程，故如果频繁的执行DispatcherTimer的话，会造成主线程的卡顿。
        /// 用System.Timers.Timer来初始化一个异步的时钟，初始化一个时钟的事件，在时钟的事件中采用BeginInvoke来进行异步委托。
        /// 这样就能防止timer控件的同步事件不停的刷新时，界面的卡顿
        /// </summary>
        private readonly Timer _refreshDeviceTimer = new Timer(3000);

        public MainWindowViewModel()
        {
            //异步尝试获取设备列表，可能会为空，因为开发者模式可能没开
            Task.Run(delegate { DeviceItems = GetDevices(); });

            _refreshDeviceTimer.Elapsed += delegate
            {
                if (string.IsNullOrEmpty(_selectedDeviceAddress))
                {
                    //TODO 提示用户
                    return;
                }

                GetDeviceDetail();
            };
            _refreshDeviceTimer.Enabled = true;

            RefreshDeviceCommand = new DelegateCommand(delegate { DeviceItems = GetDevices(); });

            DeviceSelectedCommand = new DelegateCommand<string>(address =>
            {
                _selectedDeviceAddress = address;
                //异步线程处理
                Task.Run(GetDeviceDetail);

                //另起线程获取第三方应用列表
                GetDeviceApplication();
            });

            RefreshApplicationCommand = new DelegateCommand(delegate
            {
                if (string.IsNullOrEmpty(_selectedDeviceAddress))
                {
                    //TODO 提示用户
                    return;
                }

                GetDeviceApplication();
            });

            PackageSelectedCommand = new DelegateCommand<string>(package => { _selectedPackage = package; });

            UninstallCommand = new DelegateCommand(delegate
            {
                if (string.IsNullOrEmpty(_selectedPackage))
                {
                    //TODO 提示用户
                    return;
                }

                var creator = new CommandCreator();
                //adb uninstall 卸载应用（应用包名）
                var uninstallCommand = creator.Init().Append("uninstall").Append(_selectedPackage).Build();
                CommandManager.Get.ExecuteCommand(uninstallCommand, delegate(string value)
                {
                    Growl.Success(value);
                    ApplicationPackages.Remove(_selectedPackage);
                });
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
                DeviceDensity = $"{value.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim()}dpi";
            });

            //adb shell cat /proc/meminfo 获取手机内存信息
            var meminfoCommand = creator.Init().Append("shell").Append("cat").Append("/proc/meminfo").Build();
            CommandManager.Get.ExecuteCommand(meminfoCommand, delegate(string value)
            {
                var dictionary = value.ToDictionary();
                var available = 0.0;
                var total = 0.0;
                foreach (var kvp in dictionary)
                {
                    switch (kvp.Key)
                    {
                        case "MemAvailable":
                            available = kvp.Value.FormatMemoryValue();
                            break;

                        case "MemTotal":
                            total = kvp.Value.FormatMemoryValue();
                            break;
                    }
                }

                if (total == 0)
                {
                    MemoryRatio = "内存获取失败";
                    MemoryProgress = 0;
                }
                else
                {
                    MemoryRatio = $"{available}G/{total}G";
                    MemoryProgress = Math.Round((total - available) / total, 2) * 100;
                }
            });

            //adb shell dumpsys battery 监控电池信息
            var batteryCommand = creator.Init().Append("shell").Append("dumpsys").Append("battery").Build();
            CommandManager.Get.ExecuteCommand(batteryCommand, delegate(string value)
            {
                var dictionary = value.ToDictionary();
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
                            BatteryProgress = double.Parse(kvp.Value);
                            break;

                        case "temperature":
                            var temperature = int.Parse(kvp.Value) * 0.1;
                            BatteryTemperature = $"{temperature}℃";
                            break;
                    }
                }
            });
        }

        /// <summary>
        /// 获取设备第三方应用列表
        /// </summary>
        private async void GetDeviceApplication()
        {
            ApplicationPackages = new ObservableCollection<string>();
            var task = Task.Run(delegate
            {
                var result = new ObservableCollection<string>();
                var creator = new CommandCreator();
                //adb shell pm list package -3 列出第三方的应用
                var packageCommand = creator.Init()
                    .Append("shell").Append("pm").Append("list").Append("package").Append("-3").Build();
                CommandManager.Get.ExecuteCommand(packageCommand, delegate(string value)
                {
                    var strings = value.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    var packages = strings.Select(
                        temp => temp.Split(new[] { ":" }, StringSplitOptions.None)[1]
                    );
                    foreach (var package in packages)
                    {
                        result.Add(package);
                    }
                });
                return result;
            });
            ApplicationPackages = await task;
        }
    }
}