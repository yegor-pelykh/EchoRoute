using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace EchoRoute
{
    public class DeviceManager : IDisposable
    {
        private readonly MMDeviceEnumerator _enumerator;
        public event Action? DevicesChanged;
        public event Action? DefaultDeviceChanged;

        public DeviceManager()
        {
            _enumerator = new MMDeviceEnumerator();
            _enumerator.RegisterEndpointNotificationCallback(new NotificationClient(this));
        }

        public MMDevice GetDefaultRenderDevice()
        {
            return _enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        }

        public List<MMDevice> GetAllRenderDevices()
        {
            return _enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).ToList();
        }

        public void Dispose()
        {
            _enumerator.Dispose();
        }
        
        private class NotificationClient(DeviceManager manager) : IMMNotificationClient
        {
            public void OnDeviceStateChanged(string deviceId, DeviceState newState) => manager.DevicesChanged?.Invoke();
            public void OnDeviceAdded(string pwstrDeviceId) => manager.DevicesChanged?.Invoke();
            public void OnDeviceRemoved(string deviceId) => manager.DevicesChanged?.Invoke();
            public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
            {
                if (flow == DataFlow.Render && role == Role.Multimedia)
                    manager.DefaultDeviceChanged?.Invoke();
            }
            public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key) { }
        }
    }
}