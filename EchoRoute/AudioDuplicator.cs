using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace EchoRoute
{
    public class AudioDuplicator : IDisposable
    {
        private WasapiLoopbackCapture? _capture;
        private WasapiOut? _extraOut;
        private BufferedWaveProvider? _bufferExtra;
        private MMDevice? _extraDevice;
        private float _extraVolume = 1.0f;

        private const int BufferMilliseconds = 50;

        public float ExtraVolume
        {
            get => _extraVolume;
            set
            {
                _extraVolume = Math.Clamp(value, 0f, 1f);
                if (_extraOut != null)
                {
                    try
                    {
                        _extraOut.Volume = _extraVolume;
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }

        public void Start(MMDevice defaultDevice, MMDevice extraDevice, float? initialVolume = null)
        {
            Stop();

            if (extraDevice.ID == defaultDevice.ID)
                return;

            _extraDevice = extraDevice;

            _capture = new WasapiLoopbackCapture();
            _bufferExtra = new BufferedWaveProvider(_capture.WaveFormat);

            _capture.DataAvailable += (_, e) =>
            {
                _bufferExtra.AddSamples(e.Buffer, 0, e.BytesRecorded);
            };

            _extraOut = new WasapiOut(_extraDevice, AudioClientShareMode.Shared, false, BufferMilliseconds);
            _extraOut.Init(_bufferExtra);

            if (initialVolume.HasValue)
                ExtraVolume = initialVolume.Value;
            else
                _extraOut.Volume = _extraVolume;

            _extraOut.Play();

            _capture.StartRecording();
        }

        public void Stop()
        {
            _capture?.StopRecording();
            _extraOut?.Stop();

            _capture?.Dispose();
            _extraOut?.Dispose();

            _capture = null;
            _extraOut = null;
            _bufferExtra = null;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}