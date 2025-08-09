using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace EchoRoute
{
    public class AudioDuplicator : IDisposable
    {
        private WasapiLoopbackCapture? _capture;
        private WasapiOut? _defaultOut;
        private WasapiOut? _extraOut;
        private BufferedWaveProvider? _bufferDefault;
        private BufferedWaveProvider? _bufferExtra;
        private MMDevice? _defaultDevice;
        private MMDevice? _extraDevice;
        private Thread? _copyThread;
        private bool _running;

        public void Start(MMDevice defaultDevice, MMDevice extraDevice)
        {
            Stop();

            if (extraDevice.ID == defaultDevice.ID)
                return;

            _defaultDevice = defaultDevice;
            _extraDevice = extraDevice;

            _capture = new WasapiLoopbackCapture();
            _bufferDefault = new BufferedWaveProvider(_capture.WaveFormat);
            _bufferExtra = new BufferedWaveProvider(_capture.WaveFormat);

            _capture.DataAvailable += (_, e) =>
            {
                _bufferDefault.AddSamples(e.Buffer, 0, e.BytesRecorded);
                _bufferExtra.AddSamples(e.Buffer, 0, e.BytesRecorded);
            };

            _defaultOut = new WasapiOut(_defaultDevice, AudioClientShareMode.Shared, false, 100);
            _extraOut = new WasapiOut(_extraDevice, AudioClientShareMode.Shared, false, 100);

            _defaultOut.Init(_bufferDefault);
            _extraOut.Init(_bufferExtra);

            _running = true;
            _capture.StartRecording();
            _defaultOut.Play();
            _extraOut.Play();

            _copyThread = new Thread(BufferCleaner) { IsBackground = true };
            _copyThread.Start();
        }

        public void Stop()
        {
            _running = false;
            _capture?.StopRecording();
            _defaultOut?.Stop();
            _extraOut?.Stop();

            _capture?.Dispose();
            _defaultOut?.Dispose();
            _extraOut?.Dispose();

            _capture = null;
            _defaultOut = null;
            _extraOut = null;
            _bufferDefault = null;
            _bufferExtra = null;
            _copyThread = null;
        }

        private void BufferCleaner()
        {
            while (_running)
            {
                if (_bufferDefault != null)
                    _bufferDefault.DiscardOnBufferOverflow = true;
                if (_bufferExtra != null)
                    _bufferExtra.DiscardOnBufferOverflow = true;

                Thread.Sleep(100);
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}