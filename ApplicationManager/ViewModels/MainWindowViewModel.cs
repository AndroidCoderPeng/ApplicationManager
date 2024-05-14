using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using ApkNet.ApkReader;
using ApplicationManager.DataService;
using ApplicationManager.Models;
using ApplicationManager.Utils;
using ApplicationManager.Views;
using HandyControl.Controls;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using MessageBox = System.Windows.MessageBox;
using ZipFile = System.IO.Compression.ZipFile;

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

        private string _filePath;

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
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

        private ObservableCollection<PermissionModel> _permissionItems;

        public ObservableCollection<PermissionModel> PermissionItems
        {
            get => _permissionItems;
            set
            {
                _permissionItems = value;
                RaisePropertyChanged();
            }
        }

        private bool _isTaskBusy;

        public bool IsTaskBusy
        {
            get => _isTaskBusy;
            set
            {
                _isTaskBusy = value;
                RaisePropertyChanged();
            }
        }

        private string _deviceState = "设备已断开";

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
        public DelegateCommand ScreenshotCommand { set; get; }
        public DelegateCommand<DragEventArgs> DropFileCommand { set; get; }
        public DelegateCommand SelectFileCommand { set; get; }
        public DelegateCommand UninstallCommand { set; get; }
        public DelegateCommand<MainWindow> InstallCommand { set; get; }

        #endregion

        private readonly List<PermissionModel> _permissions;
        private string _selectedDevice = string.Empty;
        private string _selectedPackage = string.Empty;

        /// <summary>
        /// DispatcherTimer与窗体为同一个线程，故如果频繁的执行DispatcherTimer的话，会造成主线程的卡顿。
        /// 用System.Timers.Timer来初始化一个异步的时钟，初始化一个时钟的事件，在时钟的事件中采用BeginInvoke来进行异步委托。
        /// 这样就能防止timer控件的同步事件不停的刷新时，界面的卡顿
        /// </summary>
        private readonly Timer _refreshDeviceTimer = new Timer(3000);

        private readonly Timer _refreshDeviceDetailTimer = new Timer(3000);

        public MainWindowViewModel(IApplicationDataService dataService)
        {
            //获取Android权限中英文对照列表
            var filePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\Permissions.json";
            _permissions = dataService.GetAndroidPermissions(filePath);

            //定时刷新设备列表，可能会为空，因为开发者模式可能没开
            _refreshDeviceTimer.Elapsed += delegate { DeviceItems = GetDevices(); };
            _refreshDeviceTimer.Enabled = true;

            _refreshDeviceDetailTimer.Elapsed += delegate
            {
                if (string.IsNullOrEmpty(_selectedDevice))
                {
                    return;
                }

                GetDeviceDetail();
            };
            _refreshDeviceDetailTimer.Enabled = true;

            RefreshDeviceCommand = new DelegateCommand(delegate { DeviceItems = GetDevices(); });

            DeviceSelectedCommand = new DelegateCommand<string>(address =>
            {
                _selectedDevice = address;
                //异步线程处理
                Task.Run(GetDeviceDetail);

                //另起线程获取第三方应用列表
                GetDeviceApplication();

                if (!_refreshDeviceDetailTimer.Enabled)
                {
                    _refreshDeviceDetailTimer.Enabled = true;
                }
            });

            RefreshApplicationCommand = new DelegateCommand(delegate
            {
                if (string.IsNullOrEmpty(_selectedDevice))
                {
                    ShowAlertMessageDialog("未选中任何设备", "请先选择设备");
                    return;
                }

                GetDeviceApplication();
            });

            PackageSelectedCommand = new DelegateCommand<string>(package => { _selectedPackage = package; });

            RebootDeviceCommand = new DelegateCommand(delegate
            {
                var result = MessageBox.Show("确定重启该设备？", "重启设备", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.OK)
                {
                    var creator = new CommandCreator();
                    //adb reboot 重启设备
                    var rebootCommand = creator.Init(_selectedDevice).Append("reboot").Append(_selectedDevice).Build();
                    CommandManager.Get.ExecuteCommand(rebootCommand, ResetValue);
                }
            });

            ScreenshotCommand = new DelegateCommand(TakeScreenshot);

            DropFileCommand = new DelegateCommand<DragEventArgs>(delegate(DragEventArgs e)
            {
                var data = e.Data.GetData(DataFormats.FileDrop);
                if (data == null)
                {
                    ShowAlertMessageDialog("操作失败", "文件路径未知");
                    return;
                }

                var result = ((Array)data).GetValue(0).ToString();
                if (string.IsNullOrEmpty(result) || !result.EndsWith(".apk"))
                {
                    ShowAlertMessageDialog("操作失败", "文件路径未知");
                    return;
                }

                FilePath = result;
                //解压缩获取apk文件基本信息。不能用adb获取APK信息，因为选择的APK不一定是安装了的
                GetApplicationInfo(_filePath);
            });

            SelectFileCommand = new DelegateCommand(delegate
            {
                var fileDialog = new OpenFileDialog
                {
                    // 设置默认格式
                    DefaultExt = ".apk",
                    Filter = "安装包文件(*.apk)|*.apk"
                };
                var result = fileDialog.ShowDialog();
                if (result != true) return;
                FilePath = fileDialog.FileName;

                //解压缩获取apk文件基本信息。不能用adb获取APK信息，因为选择的APK不一定是安装了的
                GetApplicationInfo(_filePath);
            });

            UninstallCommand = new DelegateCommand(delegate
            {
                if (string.IsNullOrEmpty(_selectedPackage))
                {
                    ShowAlertMessageDialog("操作失败", "请先选择需要卸载的应用");
                    return;
                }

                var result = MessageBox.Show("确定卸载该应用？", "卸载应用", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.OK)
                {
                    var creator = new CommandCreator();
                    //adb -s <设备序列号> uninstall 卸载应用（应用包名）
                    var uninstallCommand = creator.Init(_selectedDevice)
                        .Append("uninstall").Append(_selectedPackage).Build();
                    CommandManager.Get.ExecuteCommand(uninstallCommand, delegate(string value)
                    {
                        Growl.Success(value);
                        ApplicationPackages.Remove(_selectedPackage);
                    });
                }
            });

            InstallCommand = new DelegateCommand<MainWindow>(InstallApplication);
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
                    DeviceState = "设备已断开";
                }
                else
                {
                    DeviceState = "设备已连接";
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
                }
            });
            return result;
        }

        /// <summary>
        /// 重置已绑定的值
        /// </summary>
        private void ResetValue(string value)
        {
            _refreshDeviceDetailTimer.Enabled = false;
            DeviceItems.Clear();

            AndroidId = string.Empty;
            DeviceModel = string.Empty;
            DeviceBrand = string.Empty;
            DeviceName = string.Empty;
            DeviceCpu = string.Empty;
            DeviceAbi = string.Empty;
            AndroidVersion = string.Empty;
            DeviceSize = string.Empty;
            DeviceDensity = string.Empty;
            MemoryRatio = string.Empty;
            MemoryProgress = 0;
            BatteryState = string.Empty;
            BatteryProgress = 0;
            BatteryTemperature = string.Empty;

            ApplicationPackages.Clear();
        }

        /// <summary>
        /// 获取设备详情
        /// </summary>
        private void GetDeviceDetail()
        {
            var creator = new CommandCreator();
            //adb shell settings get secure android_id 查看android id
            var androidIdCommand = creator.Init(_selectedDevice)
                .Append("shell").Append("settings").Append("get").Append("secure").Append("android_id").Build();
            CommandManager.Get.ExecuteCommand(androidIdCommand, delegate(string value) { AndroidId = value; });

            //adb shell getprop ro.product.model 获取设备型号
            var modelCommand = creator.Init(_selectedDevice)
                .Append("shell").Append("getprop").Append("ro.product.model").Build();
            CommandManager.Get.ExecuteCommand(modelCommand, delegate(string value) { DeviceModel = value; });

            //adb shell getprop ro.product.brand 获取设备品牌
            var brandCommand = creator.Init(_selectedDevice)
                .Append("shell").Append("getprop").Append("ro.product.brand").Build();
            CommandManager.Get.ExecuteCommand(brandCommand, delegate(string value) { DeviceBrand = value; });

            //adb shell getprop ro.product.name 获取设备名称
            var nameCommand = creator.Init(_selectedDevice)
                .Append("shell").Append("getprop").Append("ro.product.name").Build();
            CommandManager.Get.ExecuteCommand(nameCommand, delegate(string value) { DeviceName = value; });

            //adb shell getprop ro.product.board 获取处理器型号
            var boardCommand = creator.Init(_selectedDevice)
                .Append("shell").Append("getprop").Append("ro.product.board").Build();
            CommandManager.Get.ExecuteCommand(boardCommand, delegate(string value) { DeviceCpu = value; });

            //adb shell getprop ro.product.cpu.abilist 获取CPU支持的abi架构列表
            var abiCommand = creator.Init(_selectedDevice)
                .Append("shell").Append("getprop").Append("ro.product.cpu.abilist").Build();
            CommandManager.Get.ExecuteCommand(abiCommand, delegate(string value) { DeviceAbi = value; });

            //adb shell getprop ro.build.version.release 获取设备Android系统版本
            var versionCommand = creator.Init(_selectedDevice)
                .Append("shell").Append("getprop").Append("ro.build.version.release").Build();
            CommandManager.Get.ExecuteCommand(versionCommand, delegate(string value) { AndroidVersion = value; });

            //adb shell wm size 获取设备屏幕分辨率
            var sizeCommand = creator.Init(_selectedDevice).Append("shell").Append("wm").Append("size").Build();
            CommandManager.Get.ExecuteCommand(sizeCommand, delegate(string value)
            {
                //Physical size: 1240x2772
                DeviceSize = value.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
            });

            //adb shell wm density 获取设备屏幕密度
            var densityCommand = creator.Init(_selectedDevice).Append("shell").Append("wm").Append("density").Build();
            CommandManager.Get.ExecuteCommand(densityCommand, delegate(string value)
            {
                //Physical density: 560
                DeviceDensity = $"{value.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim()}dpi";
            });

            //adb shell cat /proc/meminfo 获取手机内存信息
            var meminfoCommand = creator.Init(_selectedDevice)
                .Append("shell").Append("cat").Append("/proc/meminfo").Build();
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
                            //进一取整
                            total = Math.Ceiling(kvp.Value.FormatMemoryValue());
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
            var batteryCommand = creator.Init(_selectedDevice)
                .Append("shell").Append("dumpsys").Append("battery").Build();
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
                var result = new List<string>();
                var creator = new CommandCreator();
                //adb shell pm list package -3 列出第三方的应用
                var packageCommand = creator.Init(_selectedDevice)
                    .Append("shell").Append("pm").Append("list").Append("package").Append("-3").Build();
                CommandManager.Get.ExecuteCommand(packageCommand, delegate(string value)
                {
                    var strings = value.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    var packages = strings.Select(
                        temp => temp.Split(new[] { ":" }, StringSplitOptions.None)[1]
                    );
                    result.AddRange(packages);
                });
                //排序
                result.Sort();
                return result;
            });
            foreach (var s in await task)
            {
                ApplicationPackages.Add(s);
            }
        }

        /// <summary>
        /// 异步截屏
        /// </summary>
        private async void TakeScreenshot()
        {
            var creator = new CommandCreator();
            //adb shell screencap -p /sdcard/screen.png 截取屏幕截图并保存到指定位置
            var screenCapCommand = creator.Init(_selectedDevice)
                .Append("shell").Append("screencap").Append("-p")
                .Append($"/sdcard/{DateTime.Now:yyyyMMddHHmmss}.png").Build();
            var result = "";
            var task = Task.Run(delegate
            {
                CommandManager.Get.ExecuteCommand(screenCapCommand, delegate(string value) { result = value; });
                return result;
            });

            await task;
            Growl.Success("屏幕抓取成功");
        }

        /// <summary>
        /// 异步安装APK
        /// </summary>
        private async void InstallApplication(MainWindow window)
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                ShowAlertMessageDialog("操作失败", "安装包路径错误，请重新选择");
                return;
            }

            var creator = new CommandCreator();
            //adb -s <设备序列号> install  -r 覆盖安装应用（apk）
            var installCommand = creator.Init(_selectedDevice).Append("install").Append("-r").Append(_filePath).Build();
            LoadingDialogHub.Get.ShowLoadingDialog(window, "软件安装中，请稍后......");
            var result = "";
            var task = Task.Run(delegate
            {
                CommandManager.Get.ExecuteCommand(installCommand, delegate(string value) { result = value; });
                return result;
            });
            var s = await task;
            LoadingDialogHub.Get.DismissLoadingDialog();
            Growl.Success(s);
            if (_applicationPackages != null)
            {
                if (_packageName != "解析失败")
                {
                    ApplicationPackages.Add(_packageName);
                }
            }
        }

        /// <summary>
        /// 异步解析APK获取APK基本信息
        /// </summary>
        private async void GetApplicationInfo(string apkPath)
        {
            if (_isTaskBusy)
            {
                ShowAlertMessageDialog("操作正忙", "正在解析安装包中，请稍后......");
                return;
            }

            //获取apk文件基本信息
            var file = new FileInfo(apkPath);
            ApplicationName = file.Name;
            var size = (double)file.Length / 1024 / 1024;
            FileSize = $"{Math.Round(size, 1)}M";

            var destinationDirectory = $@"{AppDomain.CurrentDomain.BaseDirectory}\Temp";
            var directory = new DirectoryInfo(destinationDirectory);
            if (directory.Exists)
            {
                directory.DeleteDirectoryFiles();
            }

            var task = Task.Run(delegate
            {
                IsTaskBusy = true;
                try
                {
                    ZipFile.ExtractToDirectory(apkPath, destinationDirectory);
                    byte[] manifestData = null;
                    byte[] resourcesData = null;
                    using (var zip = new ZipInputStream(File.OpenRead(apkPath)))
                    {
                        using (var filestream = new FileStream(apkPath, FileMode.Open, FileAccess.Read))
                        {
                            var zipFile = new ICSharpCode.SharpZipLib.Zip.ZipFile(filestream);
                            ZipEntry zipEntry;
                            while ((zipEntry = zip.GetNextEntry()) != null)
                            {
                                switch (zipEntry.Name.ToLower())
                                {
                                    case "androidmanifest.xml":
                                        manifestData = new byte[50 * 1024];
                                        using (var stream = zipFile.GetInputStream(zipEntry))
                                        {
                                            var read = stream.Read(manifestData, 0, manifestData.Length);
                                        }

                                        break;

                                    case "resources.arsc":
                                        using (var stream = zipFile.GetInputStream(zipEntry))
                                        {
                                            using (var s = new BinaryReader(stream))
                                            {
                                                resourcesData = s.ReadBytes((int)zipEntry.Size);
                                            }
                                        }

                                        break;
                                }
                            }
                        }
                    }

                    var apkReader = new ApkReader();
                    return apkReader.extractInfo(manifestData, resourcesData);
                }
                catch (IOException)
                {
                    return null;
                }
            });

            var info = await task;
            PermissionItems = new ObservableCollection<PermissionModel>();
            if (info != null)
            {
                PackageName = info.packageName;
                ApplicationVersion = info.versionName;

                foreach (var permission in info.Permissions)
                {
                    foreach (var model in _permissions.Where(model => model.Permission.Equals(permission)))
                    {
                        PermissionItems.Add(model);
                    }
                }
            }
            else
            {
                PackageName = "解析失败";
                ApplicationVersion = "解析失败";
                PermissionItems.Add(new PermissionModel
                {
                    Description = "解析失败"
                });
            }

            //删除生成的Temp文件夹下面的文件
            directory.DeleteDirectoryFiles();
            IsTaskBusy = false;
        }

        /// <summary>
        /// 普通提示框
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        private void ShowAlertMessageDialog(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }
}