using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace BingedIt.Services
{
    internal class ConnectivityService : IDisposable
    {
        const int TIME_OUT_DURATION = 3000;
        const int TIMER_INTERVAL = 5000;
        static readonly Uri _uri = new Uri("http://www.gstatic.com/generate_204");
        static readonly HttpClient _client = new HttpClient();

        private bool _disposed;
        private Timer _timer;

        public ConnectivityService()
        {
            _timer = new(CheckForConnection);
        }

        private void CheckForConnection(object? _)
        {
            CancellationTokenSource? tokenSource = null;
            HttpRequestMessage? request = null;
            try
            {
                request = new HttpRequestMessage(HttpMethod.Get, _uri);
                request.Headers.ConnectionClose = true;
                tokenSource = new(TIME_OUT_DURATION);
                var response = _client.Send(request, tokenSource.Token);
                if (response.IsSuccessStatusCode)
                {
                    StopTimer();
                    Application.Current.Dispatcher.Invoke(OnlineCallback!.Invoke);
                    OnlineCallback = null;
                }
            }
            catch (TaskCanceledException) { }
            catch (HttpRequestException) { }
            finally
            {
                request?.Dispose();
                tokenSource?.Dispose();
            }
        }

        private event Action? OnlineCallback;

        public void NotifyWhenOnline(Action callback)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ConnectivityService));
            if (OnlineCallback is null) StartTimer();
            OnlineCallback += callback;
        }

        public void RemoveNotify(Action callback)
        {
            OnlineCallback -= callback;
            if (OnlineCallback is null) StopTimer();
        }

        void StartTimer() => _timer.Change(TIMER_INTERVAL, TIMER_INTERVAL);
        void StopTimer() => _timer.Change(Timeout.Infinite, Timeout.Infinite);

        public void Dispose()
        {
            if (!_disposed)
            {
                _timer.Dispose();
                OnlineCallback = null;
                _disposed = true;
            }
        }
    }
}
