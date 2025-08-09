using NAudio.CoreAudioApi;

namespace EchoRoute
{
    public class TrayApp : ApplicationContext
    {
        private const string TrayIconFile = "TrayIcon.ico";

        private readonly NotifyIcon _trayIcon;
        private readonly ToolStripMenuItem _devicesMenu;
        private readonly DeviceManager _deviceManager;
        private readonly AudioDuplicator _duplicator;
        private MMDevice? _selectedExtraDevice;

        public TrayApp()
        {
            _deviceManager = new DeviceManager();
            _duplicator = new AudioDuplicator();

            _devicesMenu = new ToolStripMenuItem(Strings.ExtraOutputDevice);

            var trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add(_devicesMenu);
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add(new ToolStripMenuItem(Strings.Exit, null, OnExit));

            _trayIcon = new NotifyIcon()
            {
                Icon = new Icon(TrayIconFile),
                ContextMenuStrip = trayMenu,
                Visible = true,
                Text = Strings.AppName
            };

            _trayIcon.DoubleClick += (_, _) => trayMenu.Show(Cursor.Position);

            _deviceManager.DevicesChanged += UpdateDevicesMenu;
            _deviceManager.DefaultDeviceChanged += UpdateDevicesMenu;

            UpdateDevicesMenu();
        }

        private void UpdateDevicesMenu()
        {
            _devicesMenu.DropDownItems.Clear();

            var defaultDevice = _deviceManager.GetDefaultRenderDevice();
            var allDevices = _deviceManager.GetAllRenderDevices();

            var offItem = new ToolStripMenuItem(Strings.Off)
            {
                Checked = _selectedExtraDevice == null
            };
            offItem.Click += (_, _) => SelectExtraDevice(null, defaultDevice);
            _devicesMenu.DropDownItems.Add(offItem);

            foreach (var dev in allDevices)
            {
                if (dev.ID == defaultDevice.ID)
                    continue;

                var item = new ToolStripMenuItem($"{dev.FriendlyName}")
                {
                    Checked = _selectedExtraDevice != null && dev.ID == _selectedExtraDevice.ID
                };
                item.Click += (_, _) => SelectExtraDevice(dev, defaultDevice);
                _devicesMenu.DropDownItems.Add(item);
            }

            if (_selectedExtraDevice != null && _selectedExtraDevice.ID == defaultDevice.ID)
            {
                SelectExtraDevice(null, defaultDevice);
            }
        }

        private void SelectExtraDevice(MMDevice? device, MMDevice defaultDevice)
        {
            _selectedExtraDevice = device;

            _duplicator.Stop();

            if (device != null && device.ID != defaultDevice.ID)
            {
                _duplicator.Start(defaultDevice, device);
                _trayIcon.Text = string.Format(Strings.DuplicationOn, device.FriendlyName);
            }
            else
            {
                _trayIcon.Text = Strings.DuplicationOff;
            }

            UpdateDevicesMenu();
        }

        private void OnExit(object? sender, EventArgs e)
        {
            _duplicator.Dispose();
            _deviceManager.Dispose();

            _trayIcon.Visible = false;

            Application.Exit();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _duplicator.Dispose();
                _deviceManager.Dispose();
                _trayIcon.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}