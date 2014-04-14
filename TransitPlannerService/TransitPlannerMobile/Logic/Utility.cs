using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace TransitPlannerMobile.Logic
{
    public abstract class Utility
    {
        /// <summary>
        /// From: http://stackoverflow.com/a/4735854
        /// </summary>
        /// <param name="myMethod">Action to perform</param>
        /// <param name="delayInMilliseconds">Delay in milliseconds</param>
        public static void DelayedCall(Action myMethod, int delayInMilliseconds)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (s, e) => Thread.Sleep(delayInMilliseconds);
            worker.RunWorkerCompleted += (s, e) => myMethod.Invoke();
            worker.RunWorkerAsync();
        }
    }
}
