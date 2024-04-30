using System;
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
        public DelegateCommand RebootDeviceCommand { set; get; }
        public DelegateCommand DisconnectDeviceCommand { set; get; }
        public DelegateCommand OutputImageCommand { set; get; }
        public DelegateCommand ImportImageCommand { set; get; }
        public DelegateCommand ScreenshotCommand { set; get; }
        public DelegateCommand SelectFileCommand { set; get; }
        public DelegateCommand UninstallCommand { set; get; }
        public DelegateCommand InstallCommand { set; get; }

        #endregion

        public MainWindowViewModel()
        {
            RefreshDeviceCommand = new DelegateCommand(delegate
            {
                CommandManager.Get.ExecuteCommand("adb", " devices",
                    delegate(string value) { Console.WriteLine(value); });
            });
        }
    }
}