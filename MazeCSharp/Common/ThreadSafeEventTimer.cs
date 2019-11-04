using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Common
{
    public class ThreadSafeEventTimer : Timer
    {
        #region Member Variables..
        #endregion Member Variables..

        #region Properties..
        public double ElapsedMilliseconds { get; private set; }
        #endregion Properties..

        #region Constructors..
        private ThreadSafeEventTimer(int interval)
            : base(interval)
        {
            ElapsedMilliseconds = 0;
            this.Elapsed += Timer_Elapsed;
        }
        #endregion Constructors..

        #region Methods..
        #region Events..
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ElapsedMilliseconds += ((ThreadSafeEventTimer)sender).Interval;
        }
        #endregion Events..

        public static ThreadSafeEventTimer StartNew()
        {
            ThreadSafeEventTimer Timer = new ThreadSafeEventTimer(1000) { AutoReset = true };
            Timer.Start();

            return Timer;
        }
        #endregion Methods..
    }
}
