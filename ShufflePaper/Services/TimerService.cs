using System;
using System.Windows.Threading;

namespace ShufflePaper
{
    public class TimerService
    {
        private readonly DispatcherTimer _timer = new();
        private bool _isRunning;

        public bool IsRunning => _isRunning;

        public TimeSpan Interval
        {
            get => _timer.Interval;
            set => _timer.Interval = value;
        }

        public event EventHandler? Tick;

        public TimerService()
        {
            _timer.Tick += (s, e) => Tick?.Invoke(this, EventArgs.Empty);
        }

        public void Start()
        {
            if (_isRunning) return;
            _timer.Start();
            _isRunning = true;
        }

        public void Stop()
        {
            if (!_isRunning) return;
            _timer.Stop();
            _isRunning = false;
        }

        public void Toggle()
        {
            if (_isRunning)
                Stop();
            else
                Start();
        }
    }
}
