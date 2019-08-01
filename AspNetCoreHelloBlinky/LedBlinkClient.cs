using System;
using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCoreHelloBlinky
{
    public class LedBlinkClient : IDisposable
    {
        private const int LedPin = 17;
        private const int LightTimeInMilliseconds = 1000;
        private const int DimTimeInMilliseconds = 200;

        private bool disposedValue = false;
        private object _locker = new object();
        private Task _blinkTask;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _token;

        public void StartBlinking()
        {
            if (_blinkTask != null)
            {
                return;
            }

            lock (_locker)
            {
                if (_blinkTask != null)
                {
                    return;
                }

                _tokenSource = new CancellationTokenSource();
                _token = _tokenSource.Token;

                _blinkTask = new Task(() =>
                {
                    using (var controller = new GpioController())
                    {
                        controller.OpenPin(LedPin, PinMode.Output);

                        IsBlinking = true;

                        while (true)
                        {
                            if (_token.IsCancellationRequested)
                            {
                                break;
                            }

                            controller.Write(LedPin, PinValue.High);
                            Thread.Sleep(LightTimeInMilliseconds);
                            controller.Write(LedPin, PinValue.Low);
                            Thread.Sleep(DimTimeInMilliseconds);
                        }

                        IsBlinking = false;
                    }
                });
                _blinkTask.Start();
            }
        }

        public void StopBlinking()
        {
            if (_blinkTask == null)
            {
                return;
            }

            lock (_locker)
            {
                if (_blinkTask == null)
                {
                    return;
                }

                _tokenSource.Cancel();
                _blinkTask.Wait();
                IsBlinking = false;

                _tokenSource.Dispose();
                _blinkTask.Dispose();

                _tokenSource = null;
                _blinkTask = null;
            }
        }

        public bool IsBlinking { get; private set; } = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    StopBlinking();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
