using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace CodeOwls.SeeShell.Common.Triggers
{
    public class IntervalTrigger : ITrigger
    {
        private Timer _timer;

        public IntervalTrigger()
        {
            _timer = new Timer();
            _timer.AutoReset = true;

            Interval = TimeSpan.FromSeconds(1.0d);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }
        
        public event EventHandler Trigger;

        public string Name { get; set; }

        public void RaiseTrigger()
        {
            EventHandler handler = Trigger;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public TimeSpan Interval
        {
            get { return TimeSpan.FromMilliseconds( _timer.Interval ); }
            set
            {
                _timer.Interval = value.TotalMilliseconds;
            }
        }

        public bool IsEnabled
        {
            get { return _timer.Enabled; }
            set { _timer.Enabled = value; }
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            RaiseTrigger();
        }

        public void Dispose()
        {
            var timer = Interlocked.Exchange(ref _timer, null);
            if( null == timer )
            {
                return;
            }

            timer.Stop();
            timer.Dispose();
        }
    }
}
